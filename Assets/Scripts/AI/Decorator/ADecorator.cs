
public abstract class ADecorator : ABTPrioritizedChildren
{
    protected override sealed int MinCount => 1;
    protected override sealed int MaxCount => 1;
    protected ABTNode Child => children.Values[0];
}
