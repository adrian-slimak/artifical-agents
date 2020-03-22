using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prey : Animal
{
    override public void SetNearObject(Transform nearObject)
    {
        if (nearObject.tag == "Prey") nearMate = nearObject;
        else if (nearObject.tag == "Plant") nearFood = nearObject;
    }

    override protected void TryMakeSound()
    {
        if (m_ActionsVector[3] > 0.5)
            currentSound = 5;
        else if (m_ActionsVector[4] > 0.5)
            currentSound = 10;
    }
}
