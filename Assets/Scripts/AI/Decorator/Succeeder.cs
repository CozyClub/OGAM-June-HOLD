
public class Succeeder : ADecorator
{ 
    public override sealed BTResult Continue()
    {
        State = Child.Continue();
        AttemptSucceed();
        return State;
    }

    public override sealed BTResult Start()
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
