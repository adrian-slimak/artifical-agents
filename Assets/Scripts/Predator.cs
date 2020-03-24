using UnityEngine;

public class Predator : Animal
{
    bool confusionEffectEnabled = false;
    float confusionEffectValue = 1f;
    float confusionEffectDistance = 3f;

    int numberOfAttacks = 0;

    protected override void Awake()
    {
        base.Awake();

        confusionEffectEnabled = (VirtualAcademy.Instance.m_ResetParameters[$"{m_Agent.m_BrainName}_confusion_effect_value"] ?? 0)>0;
        confusionEffectValue = (VirtualAcademy.Instance.m_ResetParameters[$"{m_Agent.m_BrainName}_confusion_effect_value"] ?? confusionEffectValue);
        confusionEffectDistance = (VirtualAcademy.Instance.m_ResetParameters[$"{m_Agent.m_BrainName}_confusion_effect_distance"] ?? confusionEffectDistance);
    }

    override public void SetNearObject(Transform nearObject)
    {
        if (nearObject.tag == "Predator") nearMate = nearObject;
        else if (nearObject.tag == "Prey") nearFood = nearObject;
    }

    override protected void TryMakeSound()
    {
        if (m_ActionsVector[3] > 0.5)
            currentSound = 5;
        else if (m_ActionsVector[4] > 0.5)
            currentSound = 10;
    }

    override protected void TryEat()
    {
        if (nearFood)
        {
            bool canEat = true;
            numberOfAttacks++;

            // Confusion Effect
            if (confusionEffectEnabled)
            {
                float eatChance = confusionEffectValue / m_Agent.m_Vision.GetNearTargetObjects(nearFood, confusionEffectDistance, "Prey");
                canEat = Random.value < eatChance;
            }

            if (canEat) // Consumption attempt successful
            {
                // Eat Effect
                energy += 50f;
                collectedFood++;
                Destroy(nearFood.gameObject);
                nearFood = null;
            }
        }
    }
}
