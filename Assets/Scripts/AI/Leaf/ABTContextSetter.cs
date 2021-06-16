using System;
using System.Collections.Generic;

/// <summary>
/// checks iteration count of the behaviour tree, compare to iteration count of previous update
/// if difference exceeds UpdateFrequency, set context again, if context is unsettable or none 
/// were found, return failure and remove context's key
/// </summary>
public abstract class ABTContextSetter : ABTNode
{
    // less than 1 means it never updates after initializing in the context
    public virtual int UpdateFrequency { get; protected set; } = 0;
    public abstract string Key { get; }

    public abstract BTResult SetContext(int version);

    /// <summary>
    /// continue for this function can assume that no checking need be made
    /// </summary>
    /// <returns></returns>
    public override BTResult Continue()
    {
        return State = SetContext(context[NPCController.VERSION_KEY].Item2);
    }

    public override BTResult Start()
    {
        // directly calling, these will never be not found
        State = BTResult.SUCCESS;
        if (UpdateFrequency < 1) 
            return State;
        var myVal = context[Key];
        var version = context[NPCController.VERSION_KEY];
        if (myVal.Item2 + UpdateFrequency < version.Item2) 
            State = SetContext(version.Item2);
        return State;
    }

    protected override void Initialize(Dictionary<string, Tuple<object, int>> context, bool recursiveInit)
    {
        if (Key == "" || 
            Key == NPCController.PARENT_KEY || 
            Key == NPCController.VERSION_KEY)
        {
            throw new Exception("context setter nodes requires a key and cannot be the same as the version / " +
                "parent reference keys");
        }
        State = SetContext(context[NPCController.VERSION_KEY].Item2);
    }
}
