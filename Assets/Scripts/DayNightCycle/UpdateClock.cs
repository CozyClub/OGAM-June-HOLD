using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateClock : MonoBehaviour
{
    //THIS SCRIPT SHOULD BE MERGED TO TIME MANAGER
    void Start()
    {
        TimeStamps.startTime();
    }
    void Update()
    {
        TimeStamps.ClockUpdate(Time.deltaTime);
    }
}
