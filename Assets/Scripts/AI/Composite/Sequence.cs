using System;

[Serializable]
public class Sequence : AComposite
{
    protected override BTResult CONTINUE_ITERATION_STATE => BTResult.SUCCESS;
}
