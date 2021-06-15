public class Inverter : ABTPrioritizedChildren
{
    protected override int MaxCount => 1;
    protected override int MinCount => 1;

    public override BTResult Continue()
    {
        foreach (var child in children.Values)
        {
            State = child.Continue();
            AttemptInversion();
            break;
        }
        return State;
    }

    public override BTResult Start()
    {
        foreach (var child in children.Values)
        {
            State = child.Start();
            AttemptInversion();
            break;
        }
        return State;
    }

    protected void AttemptInversion()
    {
        if (State == BTResult.RUNNING) return;
        State = State == BTResult.SUCCESS ?
            BTResult.FAILURE : BTResult.SUCCESS;
    }
}
