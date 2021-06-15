using System;

[Serializable]
public class Selector : AComposite
{
    protected override BTResult CONTINUE_ITERATION_STATE => BTResult.FAILURE;
}
