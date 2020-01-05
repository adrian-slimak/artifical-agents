using System;
using System.Collections.Generic;
using UnityEngine;
using MLAgents.CommunicatorObjects;

namespace MLAgents
{
    public struct UnityInitializationParameters
    {
        public int seed;
        public EngineConfiguration engine_configuration;
    }
    public struct UnityInputParameters
    {
        /// <summary>
        /// Boolean sent back from python to indicate whether or not training is happening.
        /// </summary>
        public bool isTraining;
    }


    public delegate void QuitCommandHandler();


    public delegate void ResetCommandHandler(ResetParameters customResetParameters);


    public delegate void StepCommandHandler();


    public delegate void EpisodeCompletedCommandHandler();

    /**
    This is the interface of the Communicators.
    This does not need to be modified nor implemented to create a Unity environment.

    When the Unity Communicator is initialized, it will wait for the External Communicator
    to be initialized as well. The two communicators will then exchange their first messages
    that will usually contain information for initialization (information that does not need
    to be resent at each new exchange).

    By convention a Unity input is from External to Unity and a Unity output is from Unity to
    External. Inputs and outputs are relative to Unity.

    By convention, when the Unity Communicator and External Communicator call exchange, the
    exchange is NOT simultaneous but sequential. This means that when a side of the
    communication calls exchange, the other will receive the result of its previous
    exchange call.
    This is what happens when A calls exchange a single time:
    A sends data_1 to B -> B receives data_1 -> B generates and sends data_2 -> A receives data_2
    When A calls exchange, it sends data_1 and receives data_2

    Since the messages are sent back and forth with exchange and simultaneously when calling
    initialize, External sends two messages at initialization.

    The structure of the messages is as follows:
    UnityMessage
    ...Header
    ...UnityOutput
    ......UnityRLOutput
    ......UnityRLInitializationOutput
    ...UnityInput
    ......UnityRLInput
    ......UnityRLInitializationInput

    UnityOutput and UnityInput can be extended to provide functionalities beyond RL
    UnityRLOutput and UnityRLInput can be extended to provide new RL functionalities
     */
    public interface ICommunicator
    {
        /// <summary>
        /// Quit was received by the communicator.
        /// </summary>
        event QuitCommandHandler QuitCommandReceived;


        /// <summary>
        /// Reset command sent back from the communicator.
        /// </summary>
        event ResetCommandHandler ResetCommandReceived;

        /// <summary>
        /// Unity RL Input was received by the communicator.
        /// </summary>
        event StepCommandHandler StepCommandReceived;

        /// <summary>
        /// Sends the academy parameters through the Communicator.
        /// Is used by the academy to send the AcademyParameters to the communicator.
        /// </summary>
        /// <returns>The External Initialization Parameters received.</returns>
        /// <param name="initParameters">The Unity Initialization Parameters to be sent.</param>
        UnityInitializationParameters Initialize(int port);
    }
}
