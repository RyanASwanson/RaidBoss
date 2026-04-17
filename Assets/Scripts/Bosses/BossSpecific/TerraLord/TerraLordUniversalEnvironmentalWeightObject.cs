using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TerraLordUniversalEnvironmentalWeightObject : MonoBehaviour
{
    [SerializeField] private bool _doesAddSelfToObjectListOnStart;
    [SerializeField] private bool _doesPerformConstantWeightChecks;
    
    [Space]
    [SerializeField] private float _baseWeightMultiplier;
    [SerializeField] private float _minimumCurrentWeight;
    [SerializeField] private float _maximumCurrentWeight;
    private float _currentWeight = 0;

    private void Start()
    {
        if (_doesAddSelfToObjectListOnStart)
        {
            AddObjectToTerraLordList();
        }
    }

    private void OnEnable()
    {
        SubscribeToEvents();
    }

    private void OnDisable()
    {
        RemoveObjectFromTerraLordList();
        UnsubscribeFromEvents();
    }
    
    public void AddObjectToTerraLordList()
    {
        CalculateCurrentWeight();
        SB_TerraLord.Instance.AddObjectToEnvironmentalWeightObjects(this);
    }

    public void RemoveObjectFromTerraLordList()
    {
        SB_TerraLord.Instance.RemoveObjectFromEnvironmentalWeightObjects(this);
    }

    private void CalculateCurrentWeight()
    {
        _currentWeight = transform.position.x * _baseWeightMultiplier;

        if (_currentWeight > 0)
        {
            _currentWeight = Mathf.Clamp(_currentWeight, _minimumCurrentWeight, _maximumCurrentWeight);
        }
        else
        {
            _currentWeight = Mathf.Clamp(_currentWeight, -_maximumCurrentWeight,-_minimumCurrentWeight);
        }
    }

    private void SubscribeToEvents()
    {
        if (SB_TerraLord.Instance.IsUnityNull())
        {
            return;
        }

        if (_doesPerformConstantWeightChecks)
        {
            SB_TerraLord.Instance.GetOnStartOfPassiveTickEvent().AddListener(CalculateCurrentWeight);
        }
        
    }

    private void UnsubscribeFromEvents()
    {
        if (SB_TerraLord.Instance.IsUnityNull())
        {
            return;
        }

        if (_doesPerformConstantWeightChecks)
        {
            SB_TerraLord.Instance.GetOnStartOfPassiveTickEvent().RemoveListener(CalculateCurrentWeight);
        }
    }
    
    #region Getters
    public float GetCurrentWeight() => _currentWeight;
    #endregion
}
