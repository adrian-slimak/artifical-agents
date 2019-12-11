using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SensesController : MonoBehaviour
{
    static List<Vision> visionSensorList = new List<Vision>();

    public void FixedUpdate()
    {
        
    }

    public void addVisionSensor(Vision visionSensor)
    {
        visionSensorList.Add(visionSensor);
    }
}
