
/// <summary>
/// this is probably too dangerous to every bother using. i'll be using better control methods
/// through the implementation of NPCController itself, but i'm including this as it may still
/// be useful to interact with some leaves
/// 
/// can be combined with inverter to repeat until fail instead or succeeder to continue ... 
/// forever!
/// </summary>
public class RepeatUntilFail : ADecorator
{
    public override sealed BTResult Continue()
    {
        State = BTResult.RUNNING;
        while (State != BTResult.FAILURE)
        {
            State = Child.Continue();
        }
        return State;
    }

    public override sealed BTResult Start()
    {
        State = Child.Start();
        while (State != BTResult.FAILURE)
        {
            State = Child.Continue();
        }
        return State;
    }
}
