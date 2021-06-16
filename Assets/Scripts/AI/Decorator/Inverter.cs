public class Inverter : ADecorator
{
    public override BTResult Continue()
    {
        State = Child.Continue();
        AttemptInversion();
        return State;
    }

    public override BTResult Start()
    {
        State = Child.Start();
        AttemptInversion();
        return State;
    }

    protected void AttemptInversion()
    {
        if (State == BTResult.RUNNING) return;
        State = State == BTResult.SUCCESS ?
            BTResult.FAILURE : BTResult.SUCCESS;
    }
}
