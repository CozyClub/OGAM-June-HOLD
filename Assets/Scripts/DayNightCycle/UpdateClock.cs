using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateClock : MonoBehaviour
{
    void Update()
    {
        TimeStamps.ClockUpdate(Time.deltaTime);
        Debug.Log(25%24);
    }
}
