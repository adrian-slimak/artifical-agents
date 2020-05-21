using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using UPC.CommunicatorObjects;
using System.Diagnostics;

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
    public EnvironmentParameters m_ResetParameters = new EnvironmentParameters();

    [SerializeField]
    [Tooltip("The engine-level settings which correspond to rendering quality and engine speed during Training.")]
    EngineConfiguration m_EngineConfiguration = new EngineConfiguration(80, 80, 1, 100.0f, -1);

    NPCommunicator m_Communicator;
    int m_WorkerID = 0;

    bool m_FirstAcademyReset;

    public event System.Action AgentUpdateObservations;
    public event System.Action AgentUpdateMovement;
    public event System.Action AgentUpdateFitness;
    //public event System.Action AgentUpdateStats;

    public List<Brain> brains;
    public Dictionary<string, Brain> m_Brains;

    [HideInInspector]
    public int m_EpisodeCount;
    [HideInInspector]
    public int m_StepCount;

    [Parameter("environment_predator_spawn_step")]
    public int _predatorSpawnStep;

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
            m_Brains.Add(brain.m_BrainName, brain);
            brain.CreateMemory(m_WorkerID);
        }

        OnAwakeAcademyInitialization();
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
                m_Communicator.QuitCommandReceived += QuitCommandReceived;
                m_Communicator.ResetCommandReceived += ResetCommandReceived;
                m_Communicator.StepCommandReceived += StepCommandReceived;
                m_Communicator.EpisodeCompletedCommandReceived += EpisodeCompletedCommandReceived;
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

    public virtual void OnAwakeAcademyInitialization()
    { }

    public virtual void AcademyStep()
    { }

    protected virtual void PredatorSpawnStep()
    { }

    void EnvironmentStep()
    {
        if (m_FirstAcademyReset)
        {
            //using (TimerStack.Instance.Scoped("AgentUpdateObservations"))
            {
                AgentUpdateObservations?.Invoke();

                foreach (Brain brain in brains)
                    brain.UpdateStats();
            }
        }

        //using (TimerStack.Instance.Scoped("CommunicateWithPython"))
        {
            m_Communicator?.CommunicateWithPython();
        }
    }

    void StepCommandReceived()
    {
        AgentUpdateMovement?.Invoke();
        if (m_StepCount == _predatorSpawnStep)
        {
            PredatorSpawnStep();
        }

        m_StepCount += 1;
    }
        
    void ResetCommandReceived(Dictionary<string, float> customResetParameters)
    {
        if (customResetParameters != null)
            m_ResetParameters.SetCustomParameters(customResetParameters);

        m_EpisodeCount++;
        m_StepCount = 0;

        AgentUpdateObservations = null;
        AgentUpdateMovement = null;
        AgentUpdateFitness = null;

        OnAcademyReset();

        m_FirstAcademyReset = true;
    }

    protected virtual void OnAcademyReset() { }

    void EpisodeCompletedCommandReceived()
    {
        m_FirstAcademyReset = false;

        //AgentUpdateFitness?.Invoke();
        foreach (Brain brain in brains)
            brain.UpdateStatsLate();
    }

    void QuitCommandReceived()
    {
#if UNITY_EDITOR

        EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }

    public static void LoadEnvironmentParameters(object _object, string _prefix = "")
    {
        Instance.m_ResetParameters.LoadEnvParams(_object, _prefix);
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
