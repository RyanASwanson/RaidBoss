using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollUIContents : MonoBehaviour
{
    [SerializeField] protected bool _doesCountLines;
    [SerializeField] protected float _lineDistance;
    
    internal int TotalLines;
    internal float LineLength;

    protected ScrollUISelection _associatedScrollUISelection;

    public virtual void SetUpContents(ScrollUISelection selection)
    {
        _associatedScrollUISelection = selection;
    }
    
    public virtual int UpdateContentsAndCountLines()
    {
        return TotalLines;
    }
}
