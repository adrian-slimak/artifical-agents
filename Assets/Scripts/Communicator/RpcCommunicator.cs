using Grpc.Core;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using MLAgents.CommunicatorObjects;
using Google.Protobuf;
using System.IO.MemoryMappedFiles;

namespace MLAgents
{
    /// Responsible for communication with External using gRPC.
    public class RpcCommunicator
    {
        public event QuitCommandHandler QuitCommandReceived;
        public event ResetCommandHandler ResetCommandReceived;
        public event InputReceivedHandler RLInputReceived;

        bool m_IsOpen;

        UnityRLOutputProto m_CurrentUnityRlOutput =
            new UnityRLOutputProto();

        /// The Unity to External client.
        UnityToExternalProto.UnityToExternalProtoClient m_Client;

        MemoryMappedFile m_UnityOutputMemory;
        MemoryMappedFile m_UnityInputMemory;
        /// The communicator parameters sent at construction
        CommunicatorInitParameters m_CommunicatorInitParameters;
        public RpcCommunicator(CommunicatorInitParameters communicatorInitParameters)
        {
            m_CommunicatorInitParameters = communicatorInitParameters;
            m_UnityOutputMemory = MemoryMappedFile.CreateNew("unity_output", 200000);
            m_UnityInputMemory = MemoryMappedFile.CreateNew("unity_input", 200000);
        }

        #region Initialization

        /// <summary>
        /// Sends the initialization parameters through the Communicator.
        /// Is used by the academy to send initialization parameters to the communicator.
        /// </summary>
        /// <returns>The External Initialization Parameters received.</returns>
        /// <param name="initParameters">The Unity Initialization Parameters to be sent.</param>
        public UnityInitializationParameters Initialize(CommunicatorInitParameters initParameters)
        {
            UnityInitializationOutputProto initialization_output = GetUnityInitializationOutput();
            initialization_output.Name = initParameters.name;
            initialization_output.EnvironmentParameters = new EnvironmentParametersProto();

            UnityInputProto resetInput;
            UnityInputProto initializationInput;
            try
            {
                initializationInput = Initialize(
                    new UnityOutputProto
                    {
                        InitializationOutput = initialization_output
                    },
                    out resetInput);
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

            UpdateEnvironmentWithInput(resetInput);

            return new UnityInitializationParameters
            {
                seed = initializationInput.InitializationInput.Seed
            };
        }

        void UpdateEnvironmentWithInput(UnityInputProto rlInput)
        {
            //SendInputReceivedEvent();
            SendCommandEvent(rlInput.Command);
        }

        UnityInputProto Initialize(UnityOutputProto unityOutput, out UnityInputProto unityInput)
        {
            m_IsOpen = true;
            var channel = new Channel(
                "localhost:" + m_CommunicatorInitParameters.port,
                ChannelCredentials.Insecure);

            m_Client = new UnityToExternalProto.UnityToExternalProtoClient(channel);

            UnityMessageProto result = m_Client.Exchange(WrapMessage(unityOutput, 200));
            UnityMessageProto msg = m_Client.Exchange(WrapMessage(null, 200));
            unityInput = msg.UnityInput;

            //Debug.Log(result);
            //Debug.Log(msg);

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

        void SendCommandEvent(CommandProto command)
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

        void SendInputReceivedEvent()
        {
            RLInputReceived?.Invoke(new UnityInputParameters { });
        }

        #endregion

        #region Sending and retreiving data

        public void DecideBatch(List<Brain> brains)
        {
            var message = new UnityOutputProto
            {
                InitializationOutput = GetUnityInitializationOutput()
            };

            int byteObservationsArraySize = 0;
            int byteActionsArraySize = 0;
            foreach (Brain brain in brains)
            {
                byteObservationsArraySize += brain.mmf_size_observations;
                byteActionsArraySize += brain.mmf_size_actions;
            }

            using (TimerStack.Instance.Scoped("MemoryWrite"))
            {
                using (MemoryMappedViewAccessor viewAccessor = m_UnityOutputMemory.CreateViewAccessor())
                {

                    var byteArray = new byte[byteObservationsArraySize];
                    foreach(Brain brain in brains)
                        Buffer.BlockCopy(brain.stackedObservations, 0, byteArray, brain.mmf_offset_observations, brain.mmf_size_observations);

                    viewAccessor.WriteArray(0, byteArray, 0, byteArray.Length);
                }
            }

            UnityInputProto input;
            using (TimerStack.Instance.Scoped("UnityPythonCommunication"))
            {
                input = Exchange(message);
            }

            using (TimerStack.Instance.Scoped("MemoryRead"))
            {
                using (MemoryMappedViewAccessor viewAccessor = m_UnityInputMemory.CreateViewAccessor())
                {
                    byte[] byteArray = new byte[byteActionsArraySize];
                    viewAccessor.ReadArray(0, byteArray, 0, byteArray.Length);
                    foreach (Brain brain in brains)
                        Buffer.BlockCopy(byteArray, brain.mmf_offset_actions, brain.stackedActions, 0, brain.mmf_size_actions);
                }
            }

            var rlInput = input?.InitializationInput;

            if (rlInput == null)
                return;

            UpdateEnvironmentWithInput(input);
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
                UnityMessageProto outputMessage = new UnityMessageProto
                {
                    Header = new HeaderProto { Status = 200 },
                    UnityOutput = unityOutput
                };

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

        UnityInitializationOutputProto GetUnityInitializationOutput()
        {
            UnityInitializationOutputProto output = null;

            if (output == null)
            {
                output = new UnityInitializationOutputProto();
            }

            foreach(Brain brain in Academy.m_Brains.Values)
            {
                var brainParametersProto = new BrainParametersProto
                {
                    BrainName = brain.brainName,
                    AgentsCount = brain.agentsCount,
                    ObservationsVectorSize = brain.observationsVectorSize,
                    ActionsVectorSize = brain.actionsVectorSize,
                    MmfOffsetObservations = brain.mmf_offset_observations,
                    MmfOffsetActions = brain.mmf_offset_actions,
                    MmfSizeObservations = brain.mmf_size_observations,
                    MmfSizeActions = brain.mmf_size_actions
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
