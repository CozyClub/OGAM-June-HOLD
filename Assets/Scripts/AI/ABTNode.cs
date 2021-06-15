using System.Collections.Generic;

public abstract class ABTNode
{
    protected Dictionary<string, object> context;
    public BTResult State { get; protected set; } = BTResult.FAILURE; // property being set during iteration of node
    public void Init(Dictionary<string, object> context) // final version
    {
        this.context = context;
        Initialize(context);
    }

    #region abstracts
    public abstract BTResult Start();
    public abstract BTResult Continue();
    protected abstract void Initialize(Dictionary<string, object> context);
    #endregion
}
