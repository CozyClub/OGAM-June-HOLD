
public abstract class ADecorator : ABTPrioritizedChildren
{
    protected override int MinCount => 1;
    protected override int MaxCount => 1;
    protected ABTNode Child => children.Values[0];
}
