using Grpc.Core;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using MLAgents.CommunicatorObjects;

namespace MLAgents
{
    /// Responsible for communication with External using gRPC.
    public class RpcCommunicator
    {
        public event QuitCommandHandler QuitCommandReceived;
        public event ResetCommandHandler ResetCommandReceived;
        public event RLInputReceivedHandler RLInputReceived;

        bool m_IsOpen;

        UnityRLOutputProto m_CurrentUnityRlOutput =
            new UnityRLOutputProto();

        /// The Unity to External client.
        UnityToExternalProto.UnityToExternalProtoClient m_Client;

        /// The communicator parameters sent at construction
        CommunicatorInitParameters m_CommunicatorInitParameters;
        public RpcCommunicator(CommunicatorInitParameters communicatorInitParameters)
        {
            m_CommunicatorInitParameters = communicatorInitParameters;
        }

        #region Initialization

        /// <summary>
        /// Sends the initialization parameters through the Communicator.
        /// Is used by the academy to send initialization parameters to the communicator.
        /// </summary>
        /// <returns>The External Initialization Parameters received.</returns>
        /// <param name="initParameters">The Unity Initialization Parameters to be sent.</param>
        public UnityRLInitParameters Initialize(CommunicatorInitParameters initParameters)
        {
            var academyParameters = new UnityRLInitializationOutputProto
            {
                Name = initParameters.name,
                Version = initParameters.version
            };

            academyParameters.EnvironmentParameters = new EnvironmentParametersProto();

            UnityInputProto input;
            UnityInputProto initializationInput;
            try
            {
                initializationInput = Initialize(
                    new UnityOutputProto
                    {
                        RlInitializationOutput = academyParameters
                    },
                    out input);
            }
            catch
            {
                var exceptionMessage = "The Communicator was unable to connect. Please make sure the External " +
                    "process is ready to accept communication with Unity.";

                // Check for common error condition and add details to the exception message.
                var httpProxy = Environment.GetEnvironmentVariable("HTTP_PROXY");
                var httpsProxy = Environment.GetEnvironmentVariable("HTTPS_PROXY");
                if (httpProxy != null || httpsProxy != null)
                {
                    exceptionMessage += " Try removing HTTP_PROXY and HTTPS_PROXY from the" +
                        "environment variables and try again.";
                }
                throw new Exception(exceptionMessage);
            }

            UpdateEnvironmentWithInput(input.RlInput);

            return new UnityRLInitParameters
            {
                seed = initializationInput.RlInitializationInput.Seed
            };
        }

        void UpdateEnvironmentWithInput(UnityRLInputProto rlInput)
        {
            SendRLInputReceivedEvent(rlInput.IsTraining);
            SendCommandEvent(rlInput.Command, rlInput.EnvironmentParameters);
        }

        UnityInputProto Initialize(UnityOutputProto unityOutput,
            out UnityInputProto unityInput)
        {
            m_IsOpen = true;
            var channel = new Channel(
                "localhost:" + m_CommunicatorInitParameters.port,
                ChannelCredentials.Insecure);

            m_Client = new UnityToExternalProto.UnityToExternalProtoClient(channel);
            var result = m_Client.Exchange(WrapMessage(unityOutput, 200));
            unityInput = m_Client.Exchange(WrapMessage(null, 200)).UnityInput;

            EditorApplication.playModeStateChanged += HandleOnPlayModeChanged;

            return result.UnityInput;
        }

        #endregion

        #region Destruction

        /// <summary>
        /// Close the communicator gracefully on both sides of the communication.
        /// </summary>
        public void Dispose()
        {
            if (!m_IsOpen)
                return;

            try
            {
                m_Client.Exchange(WrapMessage(null, 400));
                m_IsOpen = false;
            }
            catch
            {
                // ignored
            }
        }

        #endregion

        #region Sending Events

        void SendCommandEvent(CommandProto command, EnvironmentParametersProto environmentParametersProto)
        {
            switch (command)
            {
                case CommandProto.Quit:
                    {
                        QuitCommandReceived?.Invoke();
                        return;
                    }
                case CommandProto.Reset:
                    {
                        ResetCommandReceived?.Invoke();
                        return;
                    }
                default:
                    {
                        return;
                    }
            }
        }

        void SendRLInputReceivedEvent(bool isTraining)
        {
            RLInputReceived?.Invoke(new UnityRLInputParameters { isTraining = isTraining });
        }

        #endregion

        #region Sending and retreiving data

        public void DecideBatch(List<Agent> agents, float[] stackedObservations, float[] stackedActions)
        {
            if (!m_CurrentUnityRlOutput.AgentInfos.ContainsKey("prey"))
            {
                m_CurrentUnityRlOutput.AgentInfos.Add("prey",
                    new UnityRLOutputProto.Types.ListAgentInfoProto()
                );
            }

            using (TimerStack.Instance.Scoped("AgentInfo.ToProto"))
            {

                var agentInfoProto = new AgentInfoProto
                {
                    Reward = 0,
                    MaxStepReached = false,
                    Done = false,
                    Id = 0,
                };

                ObservationProto obsProto = null;

                var floatDataProto = new ObservationProto.Types.FloatData
                {
                    Data = { stackedObservations },
                };

                obsProto = new ObservationProto
                {
                    FloatData = floatDataProto,
                    CompressionType = CompressionTypeProto.None,
                };

                obsProto.Shape.AddRange(new int[] { stackedObservations.Length });

                agentInfoProto.Observations.Add(obsProto);

                m_CurrentUnityRlOutput.AgentInfos["prey"].Value.Add(agentInfoProto);
            }


            using (TimerStack.Instance.Scoped("UnityPythonCommunication"))
            {
                SendBatchedMessageHelper(stackedActions);
            }
        }

        /// <summary>
        /// Helper method that sends the current UnityRLOutput, receives the next UnityInput and
        /// Applies the appropriate AgentAction to the agents.
        /// </summary>
        void SendBatchedMessageHelper(float[] stackedActions)
        {
            var message = new UnityOutputProto
            {
                RlOutput = m_CurrentUnityRlOutput,
            };
            var tempUnityRlInitializationOutput = GetTempUnityRlInitializationOutput();
            if (tempUnityRlInitializationOutput != null)
            {
                message.RlInitializationOutput = tempUnityRlInitializationOutput;
            }

            var input = Exchange(message);

            foreach (var k in m_CurrentUnityRlOutput.AgentInfos.Keys)
            {
                m_CurrentUnityRlOutput.AgentInfos[k].Value.Clear();
            }

            var rlInput = input?.RlInput;

            if (rlInput?.AgentActions == null)
            {
                return;
            }

            UpdateEnvironmentWithInput(rlInput);

            foreach (var brainName in rlInput.AgentActions.Keys)
            {
                int floatsWritten = 0;
                foreach (var ap in rlInput.AgentActions[brainName].Value)
                {
                    foreach(float f in ap.VectorActions)
                    {
                        stackedActions[floatsWritten] = f;
                        floatsWritten++;
                    }
                }
            }
        }

        /// <summary>
        /// Send a UnityOutput and receives a UnityInput.
        /// </summary>
        /// <returns>The next UnityInput.</returns>
        /// <param name="unityOutput">The UnityOutput to be sent.</param>
        UnityInputProto Exchange(UnityOutputProto unityOutput)
        {
            if (!m_IsOpen)
            {
                return null;
            }
            try
            {
                var message = m_Client.Exchange(WrapMessage(unityOutput, 200));
                if (message.Header.Status == 200)
                {
                    return message.UnityInput;
                }

                m_IsOpen = false;
                // Not sure if the quit command is actually sent when a
                // non 200 message is received.  Notify that we are indeed
                // quitting.
                QuitCommandReceived?.Invoke();
                return message.UnityInput;
            }
            catch
            {
                m_IsOpen = false;
                QuitCommandReceived?.Invoke();
                return null;
            }
        }

        /// <summary>
        /// Wraps the UnityOuptut into a message with the appropriate status.
        /// </summary>
        /// <returns>The UnityMessage corresponding.</returns>
        /// <param name="content">The UnityOutput to be wrapped.</param>
        /// <param name="status">The status of the message.</param>
        static UnityMessageProto WrapMessage(UnityOutputProto content, int status)
        {
            return new UnityMessageProto
            {
                Header = new HeaderProto { Status = status },
                UnityOutput = content
            };
        }

        UnityRLInitializationOutputProto GetTempUnityRlInitializationOutput()
        {
            UnityRLInitializationOutputProto output = null;

            if (m_CurrentUnityRlOutput.AgentInfos.ContainsKey("prey"))
            {
                if (output == null)
                {
                    output = new UnityRLInitializationOutputProto();
                }

                var brainParametersProto = new BrainParametersProto
                {
                    VectorActionSize = { new[] { 32 } },
                    VectorActionSpaceType = SpaceTypeProto.Continuous,
                    BrainName = "prey",
                    IsTraining = true
                };
                output.BrainParameters.Add(brainParametersProto);
            }


            return output;
        }

        #endregion

        /// <summary>
        /// When the editor exits, the communicator must be closed
        /// </summary>
        /// <param name="state">State.</param>
        void HandleOnPlayModeChanged(PlayModeStateChange state)
        {
            // This method is run whenever the playmode state is changed.
            if (state == PlayModeStateChange.ExitingPlayMode)
            {
                Dispose();
            }
        }
    }
}
