using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using UPC.CommunicatorObjects;

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

    [SerializeField]
    [Tooltip("The engine-level settings which correspond to rendering quality and engine speed during Training.")]
    EngineConfiguration m_EngineConfiguration = new EngineConfiguration(80, 80, 1, 100.0f, -1);

    public NPCommunicator m_Communicator;
    public int m_WorkerID = 0;

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

        InitializeCommunicator();
        ConfigureEngine();

        m_Brains = new Dictionary<string, Brain>();
        foreach (Brain brain in brains)
        {
            m_Brains.Add(brain.brainName, brain);
            brain.CreateMemory(m_WorkerID);
        }

        AcademyInitialization();
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
            m_WorkerID = ReadArgs();
            m_Communicator = new NPCommunicator(workerID: m_WorkerID);
        }
        catch
        {
            m_Communicator = new NPCommunicator(workerID: m_WorkerID);
        }

        if (m_Communicator != null)
        {
            try
            {
                var unityInitializationInput = m_Communicator.Initialize(academyName: gameObject.name, resetParameters: m_ResetParameters);

                //UnityEngine.Random.InitState(unityInitializationInput.seed);
                m_EngineConfiguration = unityInitializationInput.engine_configuration;
                m_ResetParameters.SetCustomParameters(unityInitializationInput.custom_reset_parameters);
            }
            catch
            {
                m_Communicator = null;
            }

            if (m_Communicator != null)
            {
                m_Communicator.QuitCommandReceived += OnQuitCommandReceived;
                m_Communicator.ResetCommandReceived += OnResetCommandReceived;
                m_Communicator.StepCommandReceived += OnStepCommandReceived;
                m_Communicator.EpisodeCompletedCommandReceived += OnEpisodeCompletedCommandReceived;
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
        }

        using (TimerStack.Instance.Scoped("CommunicateWithPython"))
        {
            m_Communicator?.CommunicateWithPython();
        }
    }

    void OnStepCommandReceived()
    {
        AgentUpdateMovement?.Invoke();

        m_StepCount += 1;
    }
        
    void OnResetCommandReceived(Dictionary<string, float> customResetParameters)
    {
        if (customResetParameters != null)
            m_ResetParameters.SetCustomParameters(customResetParameters);

        m_EpisodeCount++;
        m_StepCount = 0;

        AcademyReset();
        PlantsSpawner.Instance.OnReset();

        m_FirstAcademyReset = true;
    }

    public void OnEpisodeCompletedCommandReceived()
    {
        m_FirstAcademyReset = false;

        AgentUpdateFitness?.Invoke();
    }

    static void OnQuitCommandReceived()
    {
#if UNITY_EDITOR

        EditorApplication.isPlaying = false;
#endif
        Application.Quit();
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
