using Grpc.Core;
using UPC.CommunicatorObjects;
using System;
using UnityEditor;
using UnityEngine;

namespace UPC
{
    /// Responsible for communication with External using gRPC.
    public class RpcCommunicator
    {
        bool m_IsOpen;
        int m_Port;

        public event QuitCommandHandler QuitCommandReceived;
        public event ResetCommandHandler ResetCommandReceived;
        public event StepCommandHandler StepCommandReceived;
        public event EpisodeCompletedCommandHandler EpisodeCompletedCommandReceived;

        UnityToExternalProto.UnityToExternalProtoClient m_Client;

        public RpcCommunicator(int port)
        {
            m_Port = port;
        }

        #region Initialization

        public UnityInitializationParameters Initialize(string name, ResetParameters resetParameters)
        {
            UnityOutputProto unity_output = new UnityOutputProto {
                InitializationOutput = GetUnityInitializationOutput(name, resetParameters)
            };

            UnityInputProto unity_input;
            try
            {
                unity_input = Initialize(unity_output);
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

            var initializationParameters = new UnityInitializationParameters { seed = unity_input.InitializationInput.Seed };
            if (unity_input.InitializationInput.EngineConfiguration != null)
                initializationParameters.engine_configuration = new EngineConfiguration(unity_input.InitializationInput.EngineConfiguration);

            return initializationParameters;
        }

        UnityInputProto Initialize(UnityOutputProto unityOutput)
        {
            m_IsOpen = true;
            var channel = new Channel("localhost:" + m_Port, ChannelCredentials.Insecure);
            m_Client = new UnityToExternalProto.UnityToExternalProtoClient(channel);
            UnityMessageProto initializationMessage = m_Client.Exchange(WrapMessage(unityOutput, 200));

#if UNITY_EDITOR
            EditorApplication.playModeStateChanged += HandleOnPlayModeChanged;
#endif
            return initializationMessage.UnityInput;
        }

        public void Reset(UnityInputProto unity_input)
        {
            ResetParameters customResetParameters = new ResetParameters();
            if (unity_input.InitializationInput?.CustomResetParameters != null)
                foreach (string key in unity_input.InitializationInput.CustomResetParameters.Keys)
                    customResetParameters[key] = unity_input.InitializationInput.CustomResetParameters[key];

            ResetCommandReceived?.Invoke(customResetParameters);

            var reset_output = new UnityOutputProto
            {
                InitializationOutput = GetUnityResetOutput()
            };

            m_Client.Exchange(WrapMessage(reset_output, 200));
        }

        public void EpisodeCompleted()
        {
            EpisodeCompletedCommandReceived?.Invoke();
            var unity = m_Client.Exchange(WrapMessage(null, 200));
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

        #region Sending and retreiving data

        public void CommunicateWithPython()
        {
            var unityOutput = new UnityOutputProto();
            UnityInputProto unityInput;


            //using (TimerStack.Instance.Scoped("UnityPythonCommunication"))
            {
                unityInput = Exchange(unityOutput);
            }

            if (unityInput == null)
            {
                return;
            }

            CommandProto command = unityInput.Command;
            switch (command)
            {
                case CommandProto.Quit:
                    QuitCommandReceived?.Invoke();
                    break;

                case CommandProto.Reset:
                    Reset(unityInput);
                    break;

                case CommandProto.Step:
                    StepCommandReceived?.Invoke();
                    break;

                case CommandProto.EpisodeCompleted:
                    EpisodeCompleted();
                    break;
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
                var outputMessage = WrapMessage(unityOutput, 200);
                var inputMessage = m_Client.Exchange(outputMessage);

                if (inputMessage.Header.Status == 200)
                {
                    return inputMessage.UnityInput;
                }

                m_IsOpen = false;
                // Not sure if the quit command is actually sent when a
                // non 200 message is received.  Notify that we are indeed
                // quitting.
                QuitCommandReceived?.Invoke();
                return inputMessage.UnityInput;
            }
            catch
            {
                m_IsOpen = false;
                QuitCommandReceived?.Invoke();
                return null;
            }
        }

        static UnityMessageProto WrapMessage(UnityOutputProto content, int status)
        {
            return new UnityMessageProto
            {
                Header = new HeaderProto { Status = status },
                UnityOutput = content
            };
        }

        UnityInitializationOutputProto GetUnityResetOutput()
        {
            var output = new UnityInitializationOutputProto();

            foreach (Brain brain in Academy.Instance.m_Brains.Values)
            {
                var brainParametersProto = new BrainParametersProto
                {
                    BrainName = brain.brainName,
                    AgentsCount = brain.agentsCount,
                    ObservationsVectorSize = brain.observationsVectorSize,
                    ActionsVectorSize = brain.actionsVectorSize,
                    MmfOffsetObservations = brain.mmf_offset_observations,
                    MmfOffsetActions = brain.mmf_offset_actions,
                    MmfOffsetFitness = brain.mmf_offset_fitness,
                    MmfSizeObservations = brain.mmf_size_observations,
                    MmfSizeActions = brain.mmf_size_actions,
                    MmfSizeFitness = brain.mmf_size_fitness
                };

                output.BrainParameters.Add(brainParametersProto);
            }

            return output;
        }

        UnityInitializationOutputProto GetUnityInitializationOutput(string name, ResetParameters resetParameters)
        {
            var output = new UnityInitializationOutputProto();
            output.Name = name;
 

            foreach (string key in resetParameters.Keys)
            {
                output.DefaultResetParameters[key] = resetParameters[key];
            }

            return output;
        }



        #endregion

#if UNITY_EDITOR
        void HandleOnPlayModeChanged(PlayModeStateChange state)
        {
            // This method is run whenever the playmode state is changed.
            if (state == PlayModeStateChange.ExitingPlayMode)
            {
                Dispose();
            }
        }
#endif
    }
}
