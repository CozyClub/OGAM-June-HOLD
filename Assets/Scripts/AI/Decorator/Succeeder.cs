using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Succeeder : ADecorator
{ 
    public override BTResult Continue()
    {
        State = Child.Continue();
        AttemptSucceed();
        return State;
    }

    public override BTResult Start()
    {
        State = Child.Start();
        AttemptSucceed();
        return State;
    }

    protected void AttemptSucceed()
    {
        if (State == BTResult.FAILURE) State = BTResult.SUCCESS;
    }
}
