using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class ANPCController : MonoBehaviour
{
    public const string PARENT_KEY = "PARENT";

    protected ABTNode root = null;
    protected virtual bool Recursive => true;
    public int ReEvaluateInterval { get; set; }
    public BTResult State { get; protected set; } = BTResult.SUCCESS;
    protected Dictionary<string, Tuple<object, int>> context = new Dictionary<string, Tuple<object, int>>();

    protected void Start()
    {
        InitAI();
    }

    public void InitAI()
    {
        // init nodes and context
        context[PARENT_KEY] = new Tuple<object, int>(this, 0);
        root.Init(context, Recursive);
        // add self to NPCRunner
        NPCRunner.Instance.AddAI(this);
        enabled = false;
    }

    // continues from previously running state when possible
    public BTResult ContinueAI()
    {
        context[PARENT_KEY] = new Tuple<object, int>(this, context[PARENT_KEY].Item2 + 1);
        // if last state was not running, should instead start
        if (State == BTResult.RUNNING)
            return State = root.Continue();
        return State = root.Start();
    }

    // can ignore previously running state, does not increment version
    public BTResult StartAI(bool continueIfRunning = false)
    {
        if (continueIfRunning && State == BTResult.RUNNING)
            return State = root.Continue();
        return State = root.Start();
    }

    public void OnEnable()
    {
        if (context.ContainsKey(PARENT_KEY))
            NPCRunner.Instance?.AddAI(this);
    }

    private void OnDisable()
    {
        NPCRunner.Instance?.RemoveAI(this);
    }

    private void OnDestroy()
    {
        NPCRunner.Instance?.RemoveAI(this);
    }
}
