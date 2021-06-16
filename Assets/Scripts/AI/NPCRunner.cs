using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCRunner : MonoBehaviour
{
    [SerializeField]
    protected int reevaluateFreq = 5;

    public static NPCRunner Instance { get; private set; }
    public ANPCController this[int index] { get { return AIs[index]; } }
    public int AICount => AIs.Count;

    protected int frameCounter = 0;
    protected List<ANPCController> AIs = new List<ANPCController>();          // all known ais
    protected List<ANPCController> runningAIs = new List<ANPCController>();   // ais that should be processing
    protected HashSet<int> aiIDSet = new HashSet<int>();                    // respective membership sets
    protected HashSet<int> runningAIIDSet = new HashSet<int>();

    #region monobehaviour fns
    private void Awake()
    {
        if (Instance && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }

    // important note: npc controller MUST run init() on its nodes, THEN
    // pass the ai to the npc runner

    // for now, npc runner runs continue on all nodes every frame & startsai
    // semi frequently
    private void FixedUpdate()
    {
        foreach (var ai in runningAIs)
        {
            ai.ContinueAI();
        }
        for (int i = frameCounter; i < runningAIs.Count; i+=frameCounter)
        {
            runningAIs[i].StartAI();
        }
        frameCounter++;
    }
    #endregion

    #region public fns
    public void AddAI(ANPCController ai, bool activeAI = true)
    {
        var id = ai.GetInstanceID();
        if (!aiIDSet.Contains(id))
        {
            AIs.Add(ai);
            aiIDSet.Add(id);
        }
        if (!runningAIIDSet.Contains(id) && activeAI)
        {
            runningAIs.Add(ai);
            runningAIIDSet.Add(id);
        }
    }

    public void RemoveAI(ANPCController ai)
    {
        var id = ai.GetInstanceID();
        if (aiIDSet.Contains(id))
        {
            AIs.Remove(ai);
            aiIDSet.Remove(id);
        }
        if (runningAIIDSet.Contains(id))
        {
            runningAIs.Remove(ai);
            runningAIIDSet.Remove(id);
        }
    }

    public void StopAI(ANPCController ai)
    {
        var id = ai.GetInstanceID();
        if (runningAIIDSet.Contains(id))
        {
            runningAIs.Remove(ai);
            runningAIIDSet.Remove(id);
        }
    }

    public void StartAI(ANPCController ai)
    {
        AddAI(ai, true);
    }
    #endregion
}
