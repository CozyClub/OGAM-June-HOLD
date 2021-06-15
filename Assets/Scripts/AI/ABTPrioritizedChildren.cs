using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// an abstract BT node that requires a certain amount of children. all non-leaf nodes require
/// children and should inherit from this in some way
/// </summary>
public abstract class ABTPrioritizedChildren : ABTNode
{
    protected SortedList<int, ABTNode> children = new SortedList<int, ABTNode>(Comparer<int>.Create((x, y) => y.CompareTo(x)));
    protected abstract int MinCount { get; }
    protected abstract int MaxCount { get; }

    public void SetChildren(List<Tuple<int, ABTNode>> newKids)
    {
        children = new SortedList<int, ABTNode>();
        AddChildren(newKids);
    }

    public void AddChildren(List<Tuple<int, ABTNode>> newKids)
    {
        foreach (var kid in newKids)
        {
            if (children.ContainsKey(kid.Item1))
                Debug.LogError("duplicate key in selector creation " + kid);
            children.Add(kid.Item1, kid.Item2);
        }
    }

    // must set children or add to children before calling initialize
    protected override void Initialize(Dictionary<string, object> context)
    {
        if (children.Count < MinCount || children.Count > MaxCount)
            throw new Exception("Any ABTPrioritizedChildren node must have the correct number of children: " +
                "For this node, between " + MinCount + " and " + MaxCount + ", inclusive");
    }
}
