using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    public int offset;
    public void Update()
    {
        transform.rotation = Quaternion.Euler(360*TimeStamps.wholeDayTime-offset,0,0);
    }
}
