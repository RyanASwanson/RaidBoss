using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MaterialSetCustomProperty : MonoBehaviour
{
    [SerializeField] private bool _doesPerformSetUpOnEnable = true;
    
    [SerializeField] private EMaterialSetCustomPropertyType _propertyType;
    [SerializeField] private string _propertyName;

    [Space] 
    [SerializeField] private float _progressMultiplier = 1;

    [Space] 
    [SerializeField] private bool _hasOverrideStartingColors;
    [SerializeField] private Color[] _overrideStartingColors;
    [SerializeField] private Color[] _endColors;
    private Color[] _startingColors;
    private bool _hasPerformedFirstSetUp;

    [Space] 
    [SerializeField] private bool _isUsingSharedMaterial;
    [SerializeField] private MeshRenderer[] _meshRenderers;
    private List<Material> _associatedMaterials = new List<Material>();

    [Space] [SerializeField] private CurveProgression _curveProgression;

    private int _property;
    //_HeightFogDensity

    void OnEnable()
    {
        if (_doesPerformSetUpOnEnable)
        {
            SetUp();
        }
        
    }

    public void SetUp()
    {
        SetMaterial();
        
        SetUpProperty();
        
        SetDefaultColors();
        
        SubscribeToEvents();

        _hasPerformedFirstSetUp = true;
    }

    void OnDisable()
    {
        UnsubscribeFromEvents();

        ResetToDefaultValues();
    }

    private void SetMaterial()
    {
        if (_hasPerformedFirstSetUp)
        {
            return;
        }
        
        if (_isUsingSharedMaterial)
        {
            foreach (MeshRenderer meshRenderer in _meshRenderers)
            {
                if (meshRenderer.IsUnityNull())
                {
                    continue;
                }
                
                Material[] materials = meshRenderer.sharedMaterials;
                foreach (Material mat in materials)
                {
                    _associatedMaterials.Add(mat);
                }
            }
        }
        else
        {
            foreach (MeshRenderer meshRenderer in _meshRenderers)
            {
                if (meshRenderer.IsUnityNull())
                {
                    continue;
                }
                
                Material[] materials = meshRenderer.materials;
                foreach (Material mat in materials)
                {
                    _associatedMaterials.Add(mat);
                }
            }
        }
    }

    private void SetUpProperty()
    {
        _property = Shader.PropertyToID(_propertyName);
    }

    public void UpdateMaterialFloatProperty(float progress)
    {
        progress *= _progressMultiplier;
        foreach (Material mat in _associatedMaterials)
        {
            if (mat.IsUnityNull())
            {
                continue;
            }
            mat.SetFloat(_property, progress);
        }
    }

    public void UpdateMaterialColorOpacityProperty(float progress)
    {
        Color newColor;
        
        foreach (Material mat in _associatedMaterials)
        {
            if (mat.IsUnityNull())
            {
                continue;
            }

            newColor = new Color(mat.GetColor(_property).r,
                mat.GetColor(_property).g,
                mat.GetColor(_property).b, progress * _progressMultiplier);
            mat.SetColor(_property, newColor);
        }
    }

    public void UpdateMaterialColorProperty(float progress)
    {
        for (int i = 0; i < _associatedMaterials.Count; i++)
        {
            if (_associatedMaterials[i].IsUnityNull())
            {
                continue;
            }
            
            _associatedMaterials[i].SetColor(_property, Color.Lerp(_startingColors[i], _endColors[i], progress));
        }
    }

    private void SetDefaultColors()
    {
        if (_propertyType != EMaterialSetCustomPropertyType.ColorProperty)
        {
            return;
        }
        if (_hasPerformedFirstSetUp)
        {
            return;
        }
        if (_hasOverrideStartingColors)
        {
            _startingColors = _overrideStartingColors;
        }
        else
        {
            _startingColors = new Color[_associatedMaterials.Count];
            for (int i = 0; i < _startingColors.Length; i++)
            {
                _startingColors[i] = _associatedMaterials[i].GetColor(_property);
            }
        }
    }

    private void ResetToDefaultValues()
    {
        if (!_isUsingSharedMaterial)
        {
            return;
        }

        switch (_propertyType)
        {
            case EMaterialSetCustomPropertyType.FloatProperty:
                UpdateMaterialFloatProperty(0);
                return;
            case EMaterialSetCustomPropertyType.ColorOpacityProperty:
                UpdateMaterialColorOpacityProperty(0);
                return;
            case EMaterialSetCustomPropertyType.ColorProperty:
                UpdateMaterialColorProperty(0);
                return;
        }


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
                case EMaterialSetCustomPropertyType.ColorOpacityProperty:
                    _curveProgression.OnCurveValueChanged.AddListener(UpdateMaterialColorOpacityProperty);
                    return;
                case EMaterialSetCustomPropertyType.ColorProperty:
                    _curveProgression.OnCurveValueChanged.AddListener(UpdateMaterialColorProperty);
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
                case EMaterialSetCustomPropertyType.ColorOpacityProperty:
                    _curveProgression.OnCurveValueChanged.RemoveListener(UpdateMaterialColorOpacityProperty);
                    return;
                case EMaterialSetCustomPropertyType.ColorProperty:
                    _curveProgression.OnCurveValueChanged.RemoveListener(UpdateMaterialColorProperty);
                    return;
                    
            }

        }
    }
}

public enum EMaterialSetCustomPropertyType
{
    FloatProperty,
    ColorOpacityProperty,
    ColorProperty
};
