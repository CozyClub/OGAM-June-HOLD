using System;
using System.Collections.Generic;

/// <summary>
/// customizable leaf action
/// </summary>
public class BTCustomLeaf : ABTNode
{
    public Func<Dictionary<string, Tuple<object, int>>, BTResult> ContinueFn;
    public Func<Dictionary<string, Tuple<object, int>>, BTResult> StartFn;

    public override sealed BTResult Continue()
    {
        return State = ContinueFn.Invoke(context);
    }

    public override sealed BTResult Start()
    {
        return State = StartFn.Invoke(context);
    }

    protected override sealed void Initialize(Dictionary<string, Tuple<object, int>> context, bool recursiveInit)
    {
        if (StartFn == null || ContinueFn == null)
            throw new Exception("continue and start custom functions are required before initialize");
    }
}
