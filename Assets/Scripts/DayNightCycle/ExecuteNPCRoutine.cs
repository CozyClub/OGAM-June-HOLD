using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExecuteNPCRoutine : MonoBehaviour
{
    public NPCRoutines routineList;
    private int routineStep = 0;
    void Start()
    {
        TimeStamps.hourTime += CheckHours;
    }
    void CheckHours(int hours)
    {
        if(hours == routineList.timeSets[routineStep].hour)
            TimeStamps.minuteTime += CheckMinutes;
        else
            TimeStamps.minuteTime -= CheckMinutes;
    }
    void CheckMinutes(int minutes)
    {
        if(minutes >= routineList.timeSets[routineStep].minute)
        {
            ExecuteNextStep(routineList.timeSets[routineStep]);
            TimeStamps.minuteTime -= CheckMinutes;
        }
    }
    void ExecuteNextStep(RoutineAction routine)
    {
        transform.position = routine.position;
        routineStep += 1;
        if(routineStep >= routineList.timeSets.Count)
            routineStep = 0;
    }
}
