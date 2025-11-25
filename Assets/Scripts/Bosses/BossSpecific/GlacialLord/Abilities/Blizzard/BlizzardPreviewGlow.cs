using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BlizzardPreviewGlow : MonoBehaviour
{
    [SerializeField] private CurveProgression _scaleCurveProgression;
    
    [SerializeField] private MeshRenderer _meshRenderer;

    private Vector3 _targetLocation;
    
    public void MovePreviewGlowToTargetLocation(Vector3 moveLocation)
    {
        _targetLocation = moveLocation;
        if (_scaleCurveProgression.IsOppositeDirectionUpOnCurve())
        {
            SetLocationToTarget();
        }
        
        _scaleCurveProgression.StartMovingOppositeDirectionOnCurve();
    }

    private void SetLocationToTarget()
    {
        transform.position = _targetLocation;
    }

    public void ScaleDownComplete()
    {
        SetLocationToTarget();
        _scaleCurveProgression.StartMovingUpOnCurve();
    }

    public void ScaleUpComplete()
    {
        
    }
}
