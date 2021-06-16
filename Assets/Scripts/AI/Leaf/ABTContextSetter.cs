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
    public override sealed BTResult Continue()
    {
        return State = SetContext(context[ANPCController.PARENT_KEY].Item2);
    }

    public override sealed BTResult Start()
    {
        // directly calling, these will never be not found
        // return state from initial update when this is an update once node
        if (UpdateFrequency < 1) 
            return State;
        var myVal = context[Key];
        var version = context[ANPCController.PARENT_KEY];
        if (myVal.Item2 + UpdateFrequency < version.Item2) 
            State = SetContext(version.Item2);
        return State;
    }

    /// <summary>
    /// must be a non-empty key and must not coincide with the parent key (which also says the behaviour tree's
    /// verion number)
    /// </summary>
    /// <param name="context"></param>
    /// <param name="recursiveInit"></param>
    protected override sealed void Initialize(Dictionary<string, Tuple<object, int>> context, bool recursiveInit)
    {
        if (Key == "" || 
            Key == ANPCController.PARENT_KEY)
        {
            throw new Exception("context setter nodes requires a key and cannot be the same as the version / " +
                "parent reference keys");
        }
        State = SetContext(context[ANPCController.PARENT_KEY].Item2);
    }
}
