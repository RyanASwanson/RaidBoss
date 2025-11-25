using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialSetCustomProperty : MonoBehaviour
{
    [SerializeField] private string _propertyName;
    
    [Space]
    [SerializeField] private MeshRenderer _meshRenderer;
    private Material _associatedMaterial;

    [Space] [SerializeField] private CurveProgression _curveProgression;

    private int _property;
    //_HeightFogDensity

    void OnEnable()
    {
        _associatedMaterial = _meshRenderer.sharedMaterial;
        SetUpProperty();
        SubscribeToEvents();
    }

    void OnDisable()
    {
        UnsubscribeFromEvents();
    }

    private void SetUpProperty()
    {
        _property = Shader.PropertyToID(_propertyName);
    }

    public void UpdateMaterialProperty(float progress)
    {
        _associatedMaterial.SetFloat(_property, progress);
    }
    
    private void SubscribeToEvents()
    {
        if (_curveProgression)
        {
            _curveProgression.OnCurveValueChanged.AddListener(UpdateMaterialProperty);
        }
    }

    private void UnsubscribeFromEvents()
    {
        if (_curveProgression)
        {
            _curveProgression.OnCurveValueChanged.RemoveListener(UpdateMaterialProperty);
        }
    }
}
