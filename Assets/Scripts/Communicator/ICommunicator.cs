using System;
using System.Collections.Generic;
using UnityEngine;
using UPC.CommunicatorObjects;

public struct UnityInitializationParameters
{
    public int seed;
    public EngineConfiguration engine_configuration;
    public Dictionary<string, float> custom_reset_parameters;
}

public delegate void QuitCommandHandler();


public delegate void ResetCommandHandler(Dictionary<string, float> customResetParameters);


public delegate void StepCommandHandler();


public delegate void EpisodeCompletedCommandHandler();
