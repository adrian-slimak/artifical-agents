using UnityEngine;

public class Animal : MonoBehaviour
{
    public enum AnimalType { Prey, Predator }
    public AnimalType m_Type;

    public Agent m_Agent;
    protected MMArray m_ActionsVector;

    public float maxMoveSpeed = 5f;
    public float maxTurnSpeed = 100f;

    public float energy = 100f;
    float energyDrainPerStep = 0.1f;
    float energyDrainPerSpeed = 0.1f;
    float currentEnergyDrain;

    bool communicationEnabled = false;
    public float currentSound = 0f;

    protected Transform nearFood;
    protected Transform nearMate;
    public int collectedFood = 0;

    Rigidbody2D rigidBody2D;

    void Awake()
    {
        rigidBody2D = GetComponent<Rigidbody2D>();
        m_Agent = GetComponent<Agent>();

        maxMoveSpeed = (int)(VirtualAcademy.Instance.m_ResetParameters[$"{m_Agent.m_BrainName}_max_move_speed"] ?? maxMoveSpeed);
        maxTurnSpeed = (int)(VirtualAcademy.Instance.m_ResetParameters[$"{m_Agent.m_BrainName}_max_turn_speed"] ?? maxTurnSpeed);


        energy = (int)(VirtualAcademy.Instance.m_ResetParameters[$"{m_Agent.m_BrainName}_energy"]?? energy);
        energyDrainPerStep = (int)(VirtualAcademy.Instance.m_ResetParameters[$"{m_Agent.m_BrainName}_energy_drain_per_step"]?? energyDrainPerStep);
        energyDrainPerSpeed = (int)(VirtualAcademy.Instance.m_ResetParameters[$"{m_Agent.m_BrainName}_energy_drain_per_speed"]?? energyDrainPerSpeed);
    }

    public void SetActionsVector(MMArray actionsVector)
    {
        m_ActionsVector = actionsVector;
    }

    public float GetSpeed()
    {
        return Mathf.Abs(m_ActionsVector[0]);
    }

    public void AnimalStep()
    {
        TryEat();
        if(communicationEnabled)
            TryMakeSound();

        energy -= Mathf.Pow(Mathf.Abs(m_ActionsVector[0]), 2f) * energyDrainPerSpeed;
        energy -= energyDrainPerStep;

        if (energy <= 0) OnDie();
    }

    public void UpdateMovement()
    {
        rigidBody2D.angularVelocity = m_ActionsVector[1] * maxTurnSpeed;
        rigidBody2D.velocity = transform.up * (m_ActionsVector[0] * maxMoveSpeed);

        StopInBorders();
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

    protected virtual void TryMakeSound()
    {    }

    public void StopInBorders()
    {
        Vector2 pos = transform.position;
        if (pos.x > 50f) pos.x -= 100f;
        if (pos.x < -50f) pos.x += 100f;
        if (pos.y > 50f) pos.y -= 100f;
        if (pos.y < -50f) pos.y += 100f;
        transform.position = pos;
    }

    public virtual void SetNearObject(Transform nearObject)
    {    }

    public void ResetNearObject()
    {
        nearMate = null;
        nearFood = null;
    }

    void OnDie()
    {
        m_Agent.OnDie();

        Destroy(this.gameObject);
    }

    //public bool steer = false;
    //private void FixedUpdate()
    //{
    //    if (steer)
    //    {
    //        float s = Input.GetAxis("Vertical");
    //        float r = -Input.GetAxis("Horizontal");

    //        UpdateMovement(s, r);
    //        AnimalStep();
    //    }
    //}
}
