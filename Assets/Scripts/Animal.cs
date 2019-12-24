using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animal : MonoBehaviour
{
    public bool steer = false;
    public float maxMoveSpeed = 3f;
    public float turnSpeed = 4f;

    public float energy = 100f;
    public float energyDrainPerSec = 0.1f;

    Vector2 velocity;
    float moveSpeed;
    float angularVelocity;
    float energyDrainSpeed;

    public Vision vision;

    Rigidbody2D rigidBody2D;
    public enum AnimalType { Fox, Bunny }

    void OnEnable()
    {
        rigidBody2D = GetComponent<Rigidbody2D>();

    }

    void FixedUpdate()
    {
        if (energy <= 0) Die();

        rigidBody2D.angularVelocity = angularVelocity;
        rigidBody2D.velocity = velocity;

        energy -= (energyDrainSpeed + energyDrainPerSec) * Time.fixedDeltaTime;
    }

    public void SetMovement(float vel, float angVel)
    {
        if (Mathf.Abs(angVel) > 0)
            angularVelocity = (angVel / Mathf.Abs(angVel)) * turnSpeed;
        else
            angularVelocity = 0;

        moveSpeed = vel * maxMoveSpeed;

        energyDrainSpeed = Mathf.Pow(moveSpeed / maxMoveSpeed, 2f);
        velocity = transform.up * moveSpeed;

        rigidBody2D.angularVelocity = angularVelocity;
        rigidBody2D.velocity = velocity;
    }

    void Die()
    {
        Destroy(this.gameObject);
    }

    void Eat()
    {
        if (vision.nearFood)
        {
            // Eat effect
            energy += 50f;
            Destroy(vision.nearFood.gameObject);
            vision.nearFood = null;
        }
    }
}
