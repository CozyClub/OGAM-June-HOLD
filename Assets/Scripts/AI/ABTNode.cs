using System;
using System.Collections.Generic;

public abstract class ABTNode
{
    protected Dictionary<string, Tuple<object, int>> context;
    public BTResult State { get; protected set; } = BTResult.FAILURE; // property being set during iteration of node
    public void Init(Dictionary<string, Tuple<object, int>> context, bool recursiveInit = false) // final version
    {
        this.context = context;
        Initialize(context, recursiveInit);
    }

    #region abstracts
    public abstract BTResult Start();
    public abstract BTResult Continue();
    protected abstract void Initialize(Dictionary<string, Tuple<object, int>> context, bool recursiveInit);
    #endregion
}
