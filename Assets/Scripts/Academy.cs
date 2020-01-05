using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Serialization;
using UnityEditor;
using MLAgents.CommunicatorObjects;
using System.IO.MemoryMappedFiles;
using System;
using System.IO;

namespace MLAgents
{
    [System.Serializable]
    public class EngineConfiguration
    {
        [Tooltip("Width of the environment window in pixels.")]
        public int width;

        [Tooltip("Height of the environment window in pixels.")]
        public int height;

        [Tooltip("Rendering quality of environment. (Higher is better quality.)")]
        [Range(0, 5)]
        public int qualityLevel;

        [Tooltip("Speed at which environment is run. (Higher is faster.)")]
        [Range(1f, 100f)]
        public float timeScale;

        [Tooltip("Frames per second (FPS) engine attempts to maintain.")]
        public int targetFrameRate;

        public EngineConfiguration(
            int width, int height, int qualityLevel,
            float timeScale, int targetFrameRate)
        {
            this.width = width;
            this.height = height;
            this.qualityLevel = qualityLevel;
            this.timeScale = timeScale;
            this.targetFrameRate = targetFrameRate;
        }

        public EngineConfiguration(EngineConfigurationProto engineConfiguration)
        {
            this.width = engineConfiguration.Width;
            this.height = engineConfiguration.Height;
            this.qualityLevel = engineConfiguration.QualityLevel;
            this.timeScale = engineConfiguration.TimeScale;
            this.targetFrameRate = engineConfiguration.TargetFrameRate;
        }
    }


    public class Academy : MonoBehaviour
    {
        public static Academy Instance;


        [SerializeField]
        public ResetParameters m_ResetParameters = new ResetParameters();
        [HideInInspector]
        public ResetParameters m_CustomResetParameters;

        [SerializeField]
        [Tooltip("The engine-level settings which correspond to rendering quality and engine speed during Training.")]
        EngineConfiguration m_EngineConfiguration = new EngineConfiguration(80, 80, 1, 100.0f, -1);

        public RpcCommunicator Communicator;
        public bool IsCommunicatorOn
        { get { return Communicator != null; } }

        bool m_FirstAcademyReset;

        public event System.Action AgentUpdateObservations;
        public event System.Action AgentUpdateMovement;
        public event System.Action AgentUpdateFitness;

        public List<Brain> brains;
        public Dictionary<string, Brain> m_Brains;

        public int m_EpisodeCount;
        public int m_StepCount;

        MemoryMappedFile m_UnityOutputMemory;
        MemoryMappedFile m_UnityInputMemory;

        void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(this);

            m_Brains = new Dictionary<string, Brain>();
            foreach (Brain brain in brains)
                m_Brains.Add(brain.brainName, brain);

            InitializeCommunicator();
            ConfigureEngine();
            AcademyInitialization();

            m_UnityOutputMemory = MemoryMappedFile.CreateNew("unity_output", 200000);
            m_UnityInputMemory = MemoryMappedFile.CreateNew("unity_input", 200000);
        }

        // Used to read Python-provided environment parameters
        static int ReadArgs()
        {
            var args = System.Environment.GetCommandLineArgs();
            var inputPort = "";
            for (var i = 0; i < args.Length; i++)
            {
                if (args[i] == "--port")
                {
                    inputPort = args[i + 1];
                }
            }

            return int.Parse(inputPort);
        }

        void InitializeCommunicator()
        {
            // Try to launch the communicator by using the arguments passed at launch
            try
            {
                Communicator = new RpcCommunicator(port: ReadArgs());
            }
            catch
            {
                Communicator = new RpcCommunicator(port: 5004);
            }

            if (Communicator != null)
            {
                try
                {
                    var unityInitializationInput = Communicator.Initialize(name: gameObject.name, resetParameters: m_ResetParameters);

                    UnityEngine.Random.InitState(unityInitializationInput.seed);
                    m_EngineConfiguration = unityInitializationInput.engine_configuration;
                }
                catch
                {
                    Communicator = null;
                }

                if (Communicator != null)
                {
                    Communicator.QuitCommandReceived += OnQuitCommandReceived;
                    Communicator.ResetCommandReceived += OnResetCommandReceived;
                    Communicator.StepCommandReceived += OnStepCommandReceived;
                    Communicator.EpisodeCompletedCommandReceived += OnEpisodeCompletedCommandReceived;
                }
            }
        }


        void ConfigureEngine()
        {
            Screen.SetResolution(m_EngineConfiguration.width, m_EngineConfiguration.height, false);
            QualitySettings.SetQualityLevel(m_EngineConfiguration.qualityLevel, true);
            Time.timeScale = m_EngineConfiguration.timeScale;
            Time.captureFramerate = 60;
            Application.targetFrameRate = m_EngineConfiguration.targetFrameRate;
        }

        public virtual void AcademyInitialization()
        { }

        public virtual void AcademyStep()
        { }

        public virtual void AcademyReset()
        {
            AgentUpdateObservations = null;
            AgentUpdateMovement = null;
            AgentUpdateFitness = null;
        }



        void EnvironmentStep()
        {
            if (m_FirstAcademyReset)
            {
                using (TimerStack.Instance.Scoped("AgentUpdateObservations"))
                {
                    AgentUpdateObservations?.Invoke();

                    MemoryWrite(brains);
                }
            }

            using (TimerStack.Instance.Scoped("CommunicateWithPython"))
            {
                Communicator?.CommunicateWithPython();
            }
        }

        void OnStepCommandReceived()
        {
            MemoryRead(brains);

            using (TimerStack.Instance.Scoped("AgentUpdateMovement"))
            {
                AgentUpdateMovement?.Invoke();
            }

            m_StepCount += 1;
        }

        public void OnEpisodeCompletedCommandReceived()
        {
            m_FirstAcademyReset = false;
            AgentUpdateFitness?.Invoke();
            using (StreamWriter sw = new StreamWriter("C:/Users/adek1/Desktop/fitness.txt"))
            {
                foreach(Brain brain in brains)
                {
                    sw.Write(brain.brainName+" ");
                    foreach (float value in brain.agentsFitness)
                        sw.Write(value.ToString(System.Globalization.CultureInfo.InvariantCulture) +" ");
                    sw.WriteLine();
                }
            }
        }

        void OnResetCommandReceived(ResetParameters customResetParameters)
        {
            m_CustomResetParameters = customResetParameters;
            m_StepCount = 0;
            m_EpisodeCount++;

            AcademyReset();

            m_FirstAcademyReset = true;
        }

        public float? GetResetParameter(string key)
        {
            if (m_CustomResetParameters.ContainsKey(key))
                return m_CustomResetParameters[key];

            if (m_ResetParameters.ContainsKey(key))
                return m_ResetParameters[key];

            Debug.LogWarning($"Reset parameter '{key}' not found.");

            return null;
        }

        void FixedUpdate()
        {
            EnvironmentStep();
        }

        static void OnQuitCommandReceived()
        {
#if UNITY_EDITOR

            EditorApplication.isPlaying = false;
#endif
            Application.Quit();
        }

        void MemoryWrite(List<Brain> brains)
        {
            int byteObservationsArraySize = 0;
            foreach (Brain brain in brains)
            {
                byteObservationsArraySize += brain.mmf_size_observations;
            }

            using (TimerStack.Instance.Scoped("MemoryWrite"))
            {
                using (MemoryMappedViewAccessor viewAccessor = m_UnityOutputMemory.CreateViewAccessor())
                {

                    var byteArray = new byte[byteObservationsArraySize];
                    foreach (Brain brain in brains)
                        Buffer.BlockCopy(brain.stackedObservations, 0, byteArray, brain.mmf_offset_observations, brain.mmf_size_observations);

                    viewAccessor.WriteArray(0, byteArray, 0, byteArray.Length);
                }
            }

            //float sum = 0f;
            //foreach (float f in brains[0].stackedObservations)
            //    sum += f;
            //Debug.Log($"{m_StepCount}   Current observations: {sum}");
        }

        void MemoryRead(List<Brain> brains)
        {
            int byteActionsArraySize = 0;
            foreach (Brain brain in brains)
            {
                byteActionsArraySize += brain.mmf_size_actions;
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

            //float sum = 0f;
            //foreach (float f in brains[0].stackedActions)
            //    sum += f;
            //Debug.Log($"{m_StepCount}   Acting for: {sum}");
        }

        void OnApplicationQuit()
        {
            //Communicator.Dispose();
        }
    }
}
