using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animal : MonoBehaviour
{
    public AnimalType Type;
    public bool steer = false;
    public float maxMoveSpeed = 5f;
    public float turnSpeed = 100f;

    public float energy = 100f;
    public float energyDrainPerStep = 0.1f;
    public float maxEnergyDrainPerSpeed = 0.1f;

    Vector2 velocity;
    float moveSpeed;
    float angularVelocity;
    float energyDrainSpeed;

    MLAgents.Agent agent;
    Vision vision;

    Transform nearFood;
    Transform nearMate;

    Rigidbody2D rigidBody2D;
    public enum AnimalType { Prey, Predator }

    void Awake()
    {
        rigidBody2D = GetComponent<Rigidbody2D>();
        vision = GetComponent<Vision>();
        agent = GetComponent<MLAgents.Agent>();

    }

    public void AnimalStep()
    {
        rigidBody2D.angularVelocity = angularVelocity;
        rigidBody2D.velocity = velocity;

        TryEat();

        energy -= (energyDrainSpeed + energyDrainPerStep);

        if (energy <= 0) Die();
    }

    public void SetMovement(float vel, float angVel)
    {
        if (angVel != 0f)
            angularVelocity = (angVel / Mathf.Abs(angVel)) * turnSpeed;
        else
            angularVelocity = 0f;

        moveSpeed = vel * maxMoveSpeed;

        energyDrainSpeed = Mathf.Pow(moveSpeed / maxMoveSpeed, 2f)* maxEnergyDrainPerSpeed;
        velocity = transform.up * moveSpeed;

        rigidBody2D.angularVelocity = angularVelocity;
        rigidBody2D.velocity = velocity;
    }


    public void SetNearObject(Transform nearObject)
    {
        if (Type == AnimalType.Predator)
        {
            if (nearObject.tag == "Predator") nearMate = nearObject;
            else if (nearObject.tag == "Prey") nearFood = nearObject;
        }

        if(Type == AnimalType.Prey)
        {
            if (nearObject.tag == "Prey") nearMate = nearObject;
            else if (nearObject.tag == "Plant") nearFood = nearObject;
        }
    }

    public void ResetNearObject()
    {
        nearMate = null;
        nearFood = null;
    }

    void Die()
    {
        agent.UpdateFitness();
        Destroy(this.gameObject);
    }

    void TryEat()
    {
        if (nearFood)
        {
            // Eat effect
            energy += 50f;
            Destroy(nearFood.gameObject);
            nearFood = null;
        }
    }
}
