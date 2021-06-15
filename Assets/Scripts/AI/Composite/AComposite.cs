using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class AComposite : ABTPrioritizedChildren
{
    protected KeyValuePair<int, ABTNode> runningChild;
    protected abstract BTResult CONTINUE_ITERATION_STATE { get; } // the only abstract aspect of this class

    protected override int MinCount => 1;
    protected override int MaxCount => int.MaxValue;

    public override BTResult Continue()
    {
        State = runningChild.Value.Continue();
        if (State != CONTINUE_ITERATION_STATE) return State;
        var previousKey = runningChild.Key;
        foreach (var child in children)
        {
            // ignore all previous keys when continuing
            if (child.Key >= previousKey) continue;
            State = child.Value.Start();
            runningChild = child;
            if (State != CONTINUE_ITERATION_STATE) return State;
        }
        return State;
    }

    public override BTResult Start()
    {
        foreach (var child in children)
        {
            State = child.Value.Start();
            runningChild = child;
            if (State != CONTINUE_ITERATION_STATE) return State;
        }
        return State;
    }
}
