using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class FloatCurveScalar : MonoBehaviour
{
    [SerializeField] private float _inputForMinScale;
    [SerializeField] private float _inputForMaxScale;

    [Space]
    [SerializeField] private float _minScale;
    [SerializeField] private float _maxScale;
    
    [Space]
    [SerializeField] private AnimationCurve _scaleCurve;

    public float FloatFromFloatCurve(float factor)
    {
        float resultScale = (factor - _inputForMinScale) / (_inputForMaxScale - _inputForMinScale);
        resultScale = _scaleCurve.Evaluate(resultScale);
        resultScale = Mathf.Lerp(_minScale, _maxScale, resultScale);
        return resultScale;
    }

    public Vector3 LocalScaleWithFloat(float factor)
    {
        float resultScale = FloatFromFloatCurve(factor);
        return new Vector3(resultScale, resultScale, resultScale);
    }

    public void SetLocalScaleWithFloat(float factor)
    {
        transform.localScale = LocalScaleWithFloat(factor);
    }
}
