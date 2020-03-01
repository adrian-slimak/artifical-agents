using UPC.CommunicatorObjects;
using System;
using System.IO.Pipes;
using Google.Protobuf;
using System.Collections.Generic;
using UnityEngine;

namespace UPC
{
    /// Responsible for communication with Python process using Named Pipes.
    public class NPCommunicator
    {
        int m_WorkerID;
        string m_PipeName;

        public event QuitCommandHandler QuitCommandReceived;
        public event ResetCommandHandler ResetCommandReceived;
        public event StepCommandHandler StepCommandReceived;
        public event EpisodeCompletedCommandHandler EpisodeCompletedCommandReceived;

        NamedPipeClientStream m_Pipe;
        bool m_IsOpen = false;

        public NPCommunicator(int workerID)
        {
            m_WorkerID = workerID;
            m_PipeName = $"named_pipe_{workerID}";

            CreatePipe();
        }

        void CreatePipe()
        {
            try
            {
                m_Pipe = new NamedPipeClientStream(".", m_PipeName, PipeDirection.InOut);
                m_Pipe.Connect();
                m_Pipe.ReadMode = PipeTransmissionMode.Message;
                m_IsOpen = true;
            }
            catch
            {
                throw new Exception($"Cannot connect to Named Pipe \"\\\\.\\pipe\\{m_PipeName}\"");
            }
        }

        public UnityInitializationParameters Initialize(string academyName, ResetParameters resetParameters)
        {
            UnityOutputProto unity_output = new UnityOutputProto
            {
                InitializationOutput = GetUnityInitializationOutput(academyName, resetParameters)
            };

            UnityInputProto unity_input = Exchange(unity_output);

            var initializationParameters = new UnityInitializationParameters { seed = unity_input.InitializationInput.Seed };

            if (unity_input.InitializationInput?.EngineConfiguration != null)
                initializationParameters.engine_configuration = new EngineConfiguration(unity_input.InitializationInput.EngineConfiguration);

            if (unity_input.InitializationInput?.CustomResetParameters != null)
                initializationParameters.custom_reset_parameters = new Dictionary<string, float>(unity_input.InitializationInput.CustomResetParameters);

            return initializationParameters;
        }

        public void Reset(UnityInputProto unity_input)
        {
            Dictionary<string, float> customResetParameters = null;
            if (unity_input.InitializationInput?.CustomResetParameters != null)
                customResetParameters = new Dictionary<string, float>(unity_input.InitializationInput.CustomResetParameters);


            ResetCommandReceived?.Invoke(customResetParameters);

            var reset_output = new UnityOutputProto
            {
                InitializationOutput = GetUnityResetOutput()
            };

            Send(reset_output);
        }


        /// <summary>
        /// Close the communicator gracefully on both sides of the communication.
        /// </summary>
        public void Dispose()
        {
            if (!m_IsOpen)
                return;

            try
            {
                Send(null, 400);
                m_IsOpen = false;
            }
            catch
            {
                // ignored
            }
        }

        public void CommunicateWithPython()
        {
            UnityInputProto unityInput;

            using (TimerStack.Instance.Scoped("UnityPythonSend"))
            {
                Send(null);
            }
            using (TimerStack.Instance.Scoped("UnityPythonReceive"))
            {
                unityInput = Receive();
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

                case CommandProto.EpisodeComplete:
                    EpisodeCompletedCommandReceived?.Invoke();
                    break;
            }
        }

        void Send(UnityOutputProto unityOutput, int status=200)
        {
            if (!m_IsOpen)
                return;

            UnityMessageProto message = WrapMessage(unityOutput, status);

            byte[] encodedMessage = message.ToByteArray();
            m_Pipe.Write(encodedMessage, 0, encodedMessage.Length);
        }

        byte[] buffer = new byte[10000];
        UnityInputProto Receive()
        {
            if (!m_IsOpen)
                return null;

            try
            {
                int dataLength = m_Pipe.Read(buffer, 0, buffer.Length);
                UnityMessageProto unityMessage = UnityMessageProto.Parser.ParseFrom(buffer, 0, dataLength);

                if (unityMessage.Status == 200)
                {
                    return unityMessage.UnityInput;
                }

                m_IsOpen = false;
                QuitCommandReceived?.Invoke();
                return unityMessage.UnityInput;
            }
            catch
            {
                m_IsOpen = false;
                QuitCommandReceived?.Invoke();
                return null;
            }
        }

        UnityInputProto Exchange(UnityOutputProto unityOutput)
        {
            UnityInputProto unityInput;

            using (TimerStack.Instance.Scoped("UnityPython"))
            {
                Send(unityOutput);

                unityInput = Receive();
            }

            return unityInput;
        }

        static UnityMessageProto WrapMessage(UnityOutputProto content, int status)
        {
            return new UnityMessageProto
            {
                Status = status,
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
                };

                output.BrainParameters.Add(brainParametersProto);
            }

            return output;
        }

        UnityInitializationOutputProto GetUnityInitializationOutput(string name, ResetParameters resetParameters)
        {
            var output = new UnityInitializationOutputProto();
            output.Name = name;


            //foreach (string key in resetParameters.Keys)
            //{
            //    output.DefaultResetParameters[key] = resetParameters[key];
            //}

            return output;
        }
    }
}
