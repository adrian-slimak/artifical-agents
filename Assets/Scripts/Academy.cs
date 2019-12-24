using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Serialization;
using UnityEditor;
using MLAgents.CommunicatorObjects;

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
        [FormerlySerializedAs("trainingConfiguration")]
        [SerializeField]
        [Tooltip("The engine-level settings which correspond to rendering quality and engine speed during Training.")]
        EngineConfiguration m_EngineConfiguration = new EngineConfiguration(80, 80, 1, 100.0f, -1);
        EngineConfiguration m_ExternalConfiguration = null;

        public RpcCommunicator Communicator;
        public bool IsCommunicatorOn
        { get { return Communicator != null; } }

        bool m_FirstAcademyReset;

        public event System.Action AgentUpdateObservations;
        public event System.Action AgentUpdateMovement;

        public List<Brain> brains;
        public static Dictionary<string, Brain> m_Brains;

        public int m_EpisodeCount;
        public int m_StepCount;

        void Awake()
        {
            m_Brains = new Dictionary<string, Brain>();
            foreach (Brain brain in brains)
                m_Brains.Add(brain.brainName, brain);

            InitializeCommunicator();
            ConfigureEngine();
            AcademyInitialization();

            Communicator.InitializeReset();
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
                Communicator = new RpcCommunicator(port:ReadArgs());
            }
            catch
            {
                Communicator = new RpcCommunicator(port:5004);
            }

            if (Communicator != null)
            {
                try
                {
                    var unityInitializationInput = Communicator.Initialize(name:gameObject.name);
                
                    Random.InitState(unityInitializationInput.seed);
                    m_ExternalConfiguration = unityInitializationInput.engine_configuration;
                }
                catch
                {
                    Communicator = null;
                }

                if (Communicator != null)
                {
                    Communicator.QuitCommandReceived += OnQuitCommandReceived;
                    Communicator.ResetCommandReceived += OnResetCommandReceived;
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
        }



        void EnvironmentStep()
        {
            if (!m_FirstAcademyReset)
                return;

            using (TimerStack.Instance.Scoped("AgentUpdateObservations"))
            {
                AgentUpdateObservations?.Invoke();
            }

            CommandProto resultCommand;
            using (TimerStack.Instance.Scoped("DecideAction"))
            {
                resultCommand = Communicator.ExchangeDataWithPython(brains);
                if (resultCommand == CommandProto.Reset)
                    return;
            }

            using (TimerStack.Instance.Scoped("AcademyStep"))
            {
                AcademyStep();
            }

            using (TimerStack.Instance.Scoped("AgentUpdateMovement"))
            {
                AgentUpdateMovement?.Invoke();
            }

            m_StepCount += 1;
        }

        void OnResetCommandReceived()
        {
            m_StepCount = 0;
            m_EpisodeCount++;

            AcademyReset();

            m_FirstAcademyReset = true;
        }

        void FixedUpdate()
        {
            EnvironmentStep();
        }


        protected virtual void OnDestroy()
        {
            // TODO - Pass worker ID or some other identifier,
            // so that multiple envs won't overwrite each others stats.
            TimerStack.Instance.SaveJsonTimers();
        }

        static void OnQuitCommandReceived()
        {
#if UNITY_EDITOR

            EditorApplication.isPlaying = false;
#endif
            Application.Quit();
        }

        void OnApplicationQuit()
        {
            //Communicator.Dispose();
        }
    }
}
