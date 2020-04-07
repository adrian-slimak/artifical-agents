using UnityEngine;

public class Prey : Animal
{
    [Parameter("communication_food_sound_value")]
    public float foodSoundValue;
    [Parameter("communication_predator_sound_value")]
    public float predatorSoundValue;

    [Parameter("communication_food_sound_trigger")]
    public int foodSoundTrigger;
    [Parameter("communication_predator_sound_trigger")]
    public int predatorSoundTrigger;

    protected override void Awake()
    {
        base.Awake();
    }

    override public void SetNearObject(Transform nearObject)
    {
        if (nearObject.tag == "Prey") nearMate = nearObject;
        else if (nearObject.tag == "Plant") nearFood = nearObject;
    }

    override protected void TryMakeSound()
    {
        int foodAmount = m_Agent.m_Vision.GetObjectsInSight("Food");
        int predatorAmount = m_Agent.m_Vision.GetObjectsInSight("Predator");

        currentSound = 0;

        if (foodAmount >= foodSoundTrigger)
            currentSound = foodSoundValue;
        if (predatorAmount >= predatorSoundTrigger)
            currentSound = predatorSoundValue;
    }
}
