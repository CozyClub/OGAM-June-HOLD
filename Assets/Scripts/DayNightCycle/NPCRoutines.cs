using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NPCRoutine", menuName = "Routines/BasicMovement", order = 1)]
public class NPCRoutines : ScriptableObject
{
    public List<RoutineAction> timeSets;
}
[System.Serializable]
public class RoutineAction
{
    public int hour;
    public int minute;
    public int second;
    public Vector3 position;
}
