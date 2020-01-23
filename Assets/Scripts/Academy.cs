using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Serialization;
using UnityEditor;
using MLAgents.CommunicatorObjects;
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
        int m_Port = 5000;
        public bool IsCommunicatorOn
        { get { return Communicator != null; } }

        Memory m_Memory;

        bool m_FirstAcademyReset;

        public event System.Action AgentUpdateObservations;
        public event System.Action AgentUpdateMovement;
        public event System.Action AgentUpdateFitness;

        public List<Brain> brains;
        public Dictionary<string, Brain> m_Brains;

        [HideInInspector]
        public int m_EpisodeCount;
        [HideInInspector]
        public int m_StepCount;

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

            m_Memory = new Memory(m_Port);
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
                m_Port = ReadArgs();
                Communicator = new RpcCommunicator(port: m_Port);
            }
            catch
            {
                Communicator = new RpcCommunicator(port: m_Port);
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
                }

                m_Memory.WriteAgentsObservations(brains);
            }

            using (TimerStack.Instance.Scoped("CommunicateWithPython"))
            {
                Communicator?.CommunicateWithPython();
            }
        }

        void OnStepCommandReceived()
        {
            m_Memory.ReadAgentsActions(brains);

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

            m_Memory.WriteAgentsFitness(brains);
        }

        void OnResetCommandReceived(ResetParameters customResetParameters)
        {
            m_CustomResetParameters = customResetParameters;
            m_StepCount = 0;
            m_EpisodeCount++;

            AcademyReset();
            PlantsSpawner.Instance.OnReset();

            m_FirstAcademyReset = true;
        }

        static void OnQuitCommandReceived()
        {
#if UNITY_EDITOR

            EditorApplication.isPlaying = false;
#endif
            Application.Quit();
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
               
        void OnApplicationQuit()
        {
            //Communicator.Dispose();
        }
    }
}
