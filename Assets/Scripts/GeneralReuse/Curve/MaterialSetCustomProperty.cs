using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialSetCustomProperty : MonoBehaviour
{
    [SerializeField] private EMaterialSetCustomPropertyType _propertyType;
    [SerializeField] private string _propertyName;

    [Space] 
    [SerializeField] private float _progressMultiplier = 1;

    [Space] 
    [SerializeField] private bool _isUsingSharedMaterial;
    [SerializeField] private MeshRenderer _meshRenderer;
    private Material _associatedMaterial;

    [Space] [SerializeField] private CurveProgression _curveProgression;

    private int _property;
    //_HeightFogDensity

    void OnEnable()
    {
        SetMaterial();
        
        SetUpProperty();
        SubscribeToEvents();
    }

    void OnDisable()
    {
        UnsubscribeFromEvents();
    }

    private void SetMaterial()
    {
        if (_isUsingSharedMaterial)
        {
            _associatedMaterial = _meshRenderer.sharedMaterial;
        }
        else
        {
            _associatedMaterial = _meshRenderer.material;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            _curveProgression.StartMovingUpOnCurve();  
        }
        else if (Input.GetKeyDown(KeyCode.B))
        {
            _curveProgression.StartMovingDownOnCurve();  
        }
    }

    private void SetUpProperty()
    {
        _property = Shader.PropertyToID(_propertyName);
    }

    public void UpdateMaterialFloatProperty(float progress)
    {
        _associatedMaterial.SetFloat(_property, progress * _progressMultiplier);
    }

    public void UpdateMaterialColorOpacityProperty(float progress)
    {
        _associatedMaterial.SetColor(_property, new Color(_associatedMaterial.GetColor(_property).r, 
                _associatedMaterial.GetColor(_property).g, 
                _associatedMaterial.GetColor(_property).b, progress * _progressMultiplier));
    }
    
    private void SubscribeToEvents()
    {
        if (_curveProgression)
        {
            switch (_propertyType)
            {
                case EMaterialSetCustomPropertyType.FloatProperty:
                    _curveProgression.OnCurveValueChanged.AddListener(UpdateMaterialFloatProperty);
                    return;
                case EMaterialSetCustomPropertyType.ColorProperty:
                    _curveProgression.OnCurveValueChanged.AddListener(UpdateMaterialColorOpacityProperty);
                    return;
            }

        }
    }

    private void UnsubscribeFromEvents()
    {
        if (_curveProgression)
        {
            switch (_propertyType)
            {
                case EMaterialSetCustomPropertyType.FloatProperty:
                    _curveProgression.OnCurveValueChanged.RemoveListener(UpdateMaterialFloatProperty);
                    return;
                case EMaterialSetCustomPropertyType.ColorProperty:
                    _curveProgression.OnCurveValueChanged.RemoveListener(UpdateMaterialColorOpacityProperty);
                    return;
                    
            }

        }
    }
}

public enum EMaterialSetCustomPropertyType
{
    FloatProperty,
    ColorProperty
};
