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

    public int collectedFood = 0;

    float moveSpeed;
    float angularVelocity;
    float energyDrainSpeed;

    MLAgents.Agent agent;

    Transform nearFood;
    Transform nearMate;

    Rigidbody2D rigidBody2D;
    public enum AnimalType { Prey, Predator }

    void Awake()
    {
        rigidBody2D = GetComponent<Rigidbody2D>();
        agent = GetComponent<MLAgents.Agent>();

    }

    public void AnimalStep()
    {
        TryEat();

        energy -= (energyDrainSpeed + energyDrainPerStep);

        if (energy <= 0) Die();
    }

    public void SetMovement(float vel, float angVel)
    {
        Vector2 pos = transform.position;
        if (pos.x > 50f) pos.x -= 100f;
        if (pos.x < -50f) pos.x += 100f;
        if (pos.y > 50f) pos.y -= 100f;
        if (pos.y < -50f) pos.y += 100f;
        transform.position = pos;

        if (angVel != 0f)
            angularVelocity = (angVel / Mathf.Abs(angVel)) * turnSpeed;
        if (Mathf.Abs(vel) > 1)
            vel = vel / Mathf.Abs(vel);

        energyDrainSpeed = Mathf.Pow(moveSpeed / maxMoveSpeed, 2f) * maxEnergyDrainPerSpeed;

        rigidBody2D.angularVelocity = angularVelocity;
        rigidBody2D.velocity = transform.up * (vel * maxMoveSpeed);
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
            collectedFood++;
            Destroy(nearFood.gameObject);
            nearFood = null;
        }
    }
}
