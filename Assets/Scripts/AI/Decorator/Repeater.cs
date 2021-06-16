using System;
using System.Collections.Generic;

/// <summary>
/// tree/subtree is required to set Repeat to 1+ and needs a single child (like other decorators)
/// very different from repeat until fail as it doesn't care about the return state and will 
/// run start AND THEN continue on the node until it hits the repeat limit on both start and 
/// continue invocations
/// </summary>
public class Repeater : ADecorator
{
    public int Repeat { get; set; } = -1;

    public override sealed BTResult Continue()
    {
        for (var i = 0; i < Repeat; i++)
        {
            State = Child.Continue();
        }
        return State;
    }

    public override sealed BTResult Start()
    {
        State = Child.Start();
        for (var i = 1; i < Repeat; i++)
        {
            State = Child.Continue();
        }
        return State;
    }

    protected override sealed void Initialize(Dictionary<string, Tuple<object, int>> context, bool recursiveInit)
    {
        base.Initialize(context, recursiveInit);
        if (Repeat < 1) throw new Exception("Repeat must be set to value 1+");
    }
}
