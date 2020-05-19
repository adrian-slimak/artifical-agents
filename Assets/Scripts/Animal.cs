using UnityEditor;
using UnityEngine;

public class Animal : MonoBehaviour
{
    public enum AnimalType { Prey, Predator }
    public AnimalType m_Type;

    public Agent m_Agent;
    protected MMArray m_ActionsVector;

    [Parameter("max_move_speed")]
    public float maxMoveSpeed = 5f;
    [Parameter("max_turn_speed")]
    public float maxTurnSpeed = 100f;

    [Parameter("energy")]
    public float energy = 100f;
    [Parameter("energy_gain_per_eat")]
    public float energyGainPerEat = 50f;
    [Parameter("energy_drain_per_step")]
    public float energyDrainPerStep = 0.1f;
    [Parameter("energy_drain_per_speed")]
    public float energyDrainPerSpeed = 0.1f;

    [Parameter("communication_enabled")]
    bool communicationEnabled = false;
    public float currentSound = 0f;

    protected Transform nearFood;
    protected Transform nearMate;
    [Parameter("rest_after_eat")]
    protected int restAfterEat = 0;
    protected int stepsToEat = 0;
    public int collectedFood = 0;

    Rigidbody2D rigidBody2D;

    Vector3 lastpos;

    protected virtual void Awake()
    {
        rigidBody2D = GetComponent<Rigidbody2D>();
        m_Agent = GetComponent<Agent>();

        //Time.timeScale = 1f;
        //lastpos = transform.position;
        VirtualAcademy.Instance.m_ResetParameters.LoadEnvParams(this, m_Agent.m_BrainName);
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

        if (energy <= 0) m_Agent.OnDie();
    }

    public void UpdateMovement()
    {
        rigidBody2D.angularVelocity = m_ActionsVector[1] * maxTurnSpeed;
        rigidBody2D.velocity = transform.up * (m_ActionsVector[0] * maxMoveSpeed);

        KeepInBounds();
    }

    protected virtual void TryEat()
    {
        stepsToEat--;
        if (nearFood && stepsToEat<0)
        {
            // Eat effect
            energy += energyGainPerEat;
            stepsToEat = restAfterEat;
            collectedFood++;
            m_Agent.m_Brain.totalFoodCollected++;
            Destroy(nearFood.gameObject);
            nearFood = null;
        }
    }

    protected virtual void TryMakeSound()
    {    }

    public void KeepInBounds()
    {
        Vector2 pos = transform.position;
        if (pos.x > VirtualAcademy.Instance.m_HalfWorldSize) pos.x -= VirtualAcademy.Instance.m_WorldSize;
        if (pos.x < -VirtualAcademy.Instance.m_HalfWorldSize) pos.x += VirtualAcademy.Instance.m_WorldSize;
        if (pos.y > VirtualAcademy.Instance.m_HalfWorldSize) pos.y -= VirtualAcademy.Instance.m_WorldSize;
        if (pos.y < -VirtualAcademy.Instance.m_HalfWorldSize) pos.y += VirtualAcademy.Instance.m_WorldSize;
        transform.position = pos;
    }

    public virtual void SetNearObject(Transform nearObject)
    {    }

    public void ResetNearObject()
    {
        nearMate = null;
        nearFood = null;
    }

    //private void FixedUpdate()
    //{
    //    rigidBody2D.angularVelocity = maxTurnSpeed;
    //    rigidBody2D.velocity = transform.up * maxMoveSpeed;
    //    Debug.Log(Time.fixedDeltaTime);
    //    float distance = Vector3.Distance(transform.position, lastpos);
    //    lastpos = transform.position;

    //    Debug.Log(distance);
    //}

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
