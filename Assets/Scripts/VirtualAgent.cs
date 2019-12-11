using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;


public class VirtualAgent : Agent
{
    Vision vision;
    Animal animal;

    private void Start()
    {
        vision = GetComponent<Vision>();
        animal = GetComponent<Animal>();
    }

    public override void InitializeAgent()
    {

    }

    public override void CollectObservations()
    {
        AddVectorObs(vision.GetVisionVector());
    }

    public override void AgentAction(float[] vectorAction)
    {
        if (vectorAction == null) return;
        animal.SetMovement(vectorAction[0], vectorAction[1]);
    }

    public override float[] Heuristic()
    {
        return new float[] { Input.GetAxisRaw("Vertical"), -Input.GetAxisRaw("Horizontal") };
    }
}
