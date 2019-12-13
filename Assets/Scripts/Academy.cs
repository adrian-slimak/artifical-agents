using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Serialization;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MLAgents
{
    [System.Serializable]
    public class EnvironmentConfiguration
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

        public EnvironmentConfiguration(
            int width, int height, int qualityLevel,
            float timeScale, int targetFrameRate)
        {
            this.width = width;
            this.height = height;
            this.qualityLevel = qualityLevel;
            this.timeScale = timeScale;
            this.targetFrameRate = targetFrameRate;
        }
    }


    public class Academy : MonoBehaviour
    {
        const string k_ApiVersion = "API-12";


        float m_OriginalFixedDeltaTime;
        float m_OriginalMaximumDeltaTime;

        [FormerlySerializedAs("trainingConfiguration")]
        [SerializeField]
        [Tooltip("The engine-level settings which correspond to rendering " +
            "quality and engine speed during Training.")]
        EnvironmentConfiguration m_TrainingConfiguration =
            new EnvironmentConfiguration(80, 80, 1, 100.0f, -1);

        public bool IsCommunicatorOn
        {
            get { return Communicator != null; }
        }

        int m_EpisodeCount;
        int m_StepCount;
        int m_TotalStepCount;

        public RpcCommunicator Communicator;

        bool m_Initialized;
        bool m_FirstAcademyReset;

        public event System.Action AgentUpdateObservations;
        public event System.Action AgentUpdateMovement;

        public List<Agent> m_Agents;

        void Awake()
        {
            m_Agents = new List<Agent>();

            LazyInitialization();
        }

        public void LazyInitialization()
        {

            if (!m_Initialized)
            {
                InitializeEnvironment();
                m_Initialized = true;
            }
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

        void InitializeEnvironment()
        {
            m_OriginalFixedDeltaTime = Time.fixedDeltaTime;
            m_OriginalMaximumDeltaTime = Time.maximumDeltaTime;

            // Try to launch the communicator by using the arguments passed at launch
            try
            {
                Communicator = new RpcCommunicator(
                    new CommunicatorInitParameters
                    {
                        port = ReadArgs()
                    });
            }
            catch
            {
                Communicator = new RpcCommunicator(
                    new CommunicatorInitParameters
                    {
                        port = 5004
                    });
            }

            if (Communicator != null)
            {
                try
                {
                    var unityRLInitParameters = Communicator.Initialize(
                        new CommunicatorInitParameters
                        {
                            version = k_ApiVersion,
                            name = gameObject.name
                        });
                    Random.InitState(unityRLInitParameters.seed);
                }
                catch
                {
                    Communicator = null;
                }

                if (Communicator != null)
                {
                    Communicator.QuitCommandReceived += OnQuitCommandReceived;
                    Communicator.ResetCommandReceived += ForcedFullReset;
                }
            }

            AgentUpdateObservations += () => { };
            AgentUpdateMovement += () => { };

            ConfigureEnvironment();
        }

        static void OnQuitCommandReceived()
        {
            EditorApplication.isPlaying = false;
            Application.Quit();
        }

        /// <summary>
        /// Configures the environment settings depending on the training/inference
        /// mode and the corresponding parameters passed in the Editor.
        /// </summary>
        void ConfigureEnvironment()
        {
            Screen.SetResolution(m_TrainingConfiguration.width, m_TrainingConfiguration.height, false);
            QualitySettings.SetQualityLevel(m_TrainingConfiguration.qualityLevel, true);
            Time.timeScale = m_TrainingConfiguration.timeScale;
            Time.captureFramerate = 60;
            Application.targetFrameRate = m_TrainingConfiguration.targetFrameRate;
        }

        public virtual void AcademyStep()
        {
        }

        public virtual void AcademyReset()
        {
        }

        /// <summary>
        /// Forces the full reset. The done flags are not affected. Is either
        /// called the first reset at inference and every external reset
        /// at training.
        /// </summary>
        void ForcedFullReset()
        {
            EnvironmentReset();
            m_FirstAcademyReset = true;
        }

        void EnvironmentStep()
        {
            if (!m_FirstAcademyReset)
            {
                ForcedFullReset();
            }

            using (TimerStack.Instance.Scoped("AgentSendState"))
            {
                AgentUpdateObservations?.Invoke();
            }

            using (TimerStack.Instance.Scoped("DecideAction"))
            {
                Communicator?.DecideBatch(m_Agents);
            }

            using (TimerStack.Instance.Scoped("AcademyStep"))
            {
                AcademyStep();
            }

            using (TimerStack.Instance.Scoped("AgentAct"))
            {
                AgentUpdateMovement?.Invoke();
            }

            m_StepCount += 1;
            m_TotalStepCount += 1;
        }

        /// <summary>
        /// Resets the environment, including the Academy.
        /// </summary>
        void EnvironmentReset()
        {
            m_StepCount = 0;
            m_EpisodeCount++;
            AcademyReset();
        }

        /// <summary>
        /// MonoBehaviour function that dictates each environment step.
        /// </summary>
        void FixedUpdate()
        {
            EnvironmentStep();
        }

        /// <summary>
        /// Cleanup function
        /// </summary>
        protected virtual void OnDestroy()
        {
            Time.fixedDeltaTime = m_OriginalFixedDeltaTime;
            Time.maximumDeltaTime = m_OriginalMaximumDeltaTime;

            // TODO - Pass worker ID or some other identifier,
            // so that multiple envs won't overwrite each others stats.
            TimerStack.Instance.SaveJsonTimers();
        }
    }
}