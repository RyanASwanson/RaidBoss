using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollUIContents : MonoBehaviour
{
    [SerializeField] protected bool _doesCountLines;
    [SerializeField] protected float _lineDistance;
    
    internal int TotalLines;
    internal float LineLength;
    public virtual int UpdateContentsAndCountLines()
    {
        return TotalLines;
    }
}
