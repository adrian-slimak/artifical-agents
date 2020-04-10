using UnityEngine;

public class Predator : Animal
{
    [Parameter("confusion_effect_value")]
    public bool confusionEffectEnabled = false;
    [Parameter("confusion_effect_value")]
    public float confusionEffectValue = 1f;
    [Parameter("confusion_effect_distance")]
    public float confusionEffectDistance = 3f;

    public int numberOfAttacks = 0;

    [Parameter("communication_sound_value")]
    public float soundValue;

    protected override void Awake()
    {
        base.Awake();
    }

    override public void SetNearObject(Transform nearObject)
    {
        if (nearObject.tag == "Predator") nearMate = nearObject;
        else if (nearObject.tag == "Prey") nearFood = nearObject;
    }

    override protected void TryMakeSound()
    {
        if (m_ActionsVector[2] > 0.7f)
            currentSound = soundValue;
        else
            currentSound = 0;
    }

    override protected void TryEat()
    {
        stepsToEat--;
        if (nearFood && stepsToEat<0)
        {
            bool canEat = true;
            numberOfAttacks++;
            m_Agent.m_Brain.totalNumberOfAttacks++;
            stepsToEat = restAfterEat;

            // Confusion Effect
            if (confusionEffectEnabled)
            {
                int nearAgents = m_Agent.m_Vision.GetNearTargetObjects(nearFood, confusionEffectDistance, "Prey");
                float eatChance = confusionEffectValue / nearAgents;
                canEat = Random.value < eatChance;
            }

            if (canEat) // Consumption attempt successful
            {
                // Eat Effect
                energy += 50f;
                collectedFood++;
                m_Agent.m_Brain.totalFoodCollected++;
                nearFood.GetComponent<Agent>().OnDie();
                nearFood = null;
            }
        }
    }
}
