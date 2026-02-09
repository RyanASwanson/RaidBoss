using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class CustomObjectEmitter : MonoBehaviour
{
    [SerializeField] private bool _doesStartEmissionOnEnable;
    
    [Space]
    [SerializeField] private bool _hasEmissionDuration;
    [SerializeField] private float _emissionDuration;
    private float _emissionTime;
    
    [Space]
    [SerializeField] private float _emissionRate;
    [SerializeField] private float _emissionRateVariance;
    private float _currentEmissionRate;

    [Space] 
    [SerializeField] private bool _doesSetSelfAsParent;
    
    [Space]
    [SerializeField] private float _emittedObjectMoveSpeed;
    [SerializeField] private float _emittedObjectDuration;

    [Space] 
    [SerializeField] private ECustomObjectEmitterType _emitterType;
    
    [Space]
    [SerializeField] private Vector3 _minimumAngleIncrease;
    [SerializeField] private Vector3 _maximumAngleIncrease;

    [Space] 
    [SerializeField] private float _minimumObjectScaleMultiplier = 1;
    [SerializeField] private float _maximumObjectScaleMultiplier = 1;

    private Vector3 _currentRotation;
    
    [Space]
    [SerializeField] private GameObject _emitObject;
    
    private Coroutine _emittingCoroutine;
    
    bool _isEmitting;

    private void OnEnable()
    {
        _currentRotation = FullRandomVector();
        CalculateEmissionRate();
        
        if (_doesStartEmissionOnEnable)
        {
            StartEmittingObject();
        }
    }
    
    public void StartEmittingObject()
    {
        StopEmittingObject();
        
        _emittingCoroutine = StartCoroutine(ObjectEmittingProcess());
    }

    public void StopEmittingObject()
    {
        if (!_emittingCoroutine.IsUnityNull())
        {
            StopCoroutine(_emittingCoroutine);
        }
    }

    private IEnumerator ObjectEmittingProcess()
    {
        _isEmitting = true;
        _emissionTime = 0;
        float emitCreationTimer = 0;
        while (!_hasEmissionDuration || _emissionTime < _emissionDuration)
        {
            _emissionTime += Time.deltaTime;
            emitCreationTimer += Time.deltaTime;
            if (emitCreationTimer > _currentEmissionRate)
            {
                CreateEmittingObject();
                emitCreationTimer = 0;
            }
            
            yield return null;
        }
        _isEmitting = false;
    }

    private void CalculateEmissionRate()
    {
        _currentEmissionRate = _emissionRate + Random.Range(-_emissionRateVariance, _emissionRateVariance);
    }

    private void CreateEmittingObject()
    {
        CalculateEmissionRate();
        
        GameObject createdObject = Instantiate(_emitObject, transform.position, Quaternion.identity);

        if (_doesSetSelfAsParent)
        {
            createdObject.transform.SetParent(transform);
            createdObject.transform.localScale = Vector3.one;
        }
        
        createdObject.transform.eulerAngles = RandomDirectionVector();
        createdObject.gameObject.transform.localScale *= Random.Range(_minimumObjectScaleMultiplier, _maximumObjectScaleMultiplier);

        if (_emittedObjectMoveSpeed > 0)
        {
            StartCoroutine(MoveEmittedObject(createdObject));
        }
        else
        {
            Destroy(createdObject, _emittedObjectDuration);
        }
        
    }

    private Vector3 RandomDirectionVector()
    {
        switch (_emitterType)
        {
            case ECustomObjectEmitterType.FullRandom:
                return FullRandomVector();
            case ECustomObjectEmitterType.RangeIncreaseRandom:
                _currentRotation += new Vector3(
                    Random.Range(_minimumAngleIncrease.x, _maximumAngleIncrease.x),
                    Random.Range(_minimumAngleIncrease.y, _maximumAngleIncrease.y),
                    Random.Range(_minimumAngleIncrease.z, _maximumAngleIncrease.z));
                return _currentRotation;
            default:
                return Vector3.zero;
        }
    }

    private Vector3 FullRandomVector()
    {
        //return Random.rotation.eulerAngles;
        return new Vector3(Random.Range(0,_maximumAngleIncrease.x), Random.Range(0,_maximumAngleIncrease.y), Random.Range(0,_maximumAngleIncrease.x));
    }

    private IEnumerator MoveEmittedObject(GameObject gameObject)
    {
        float objectDuration = 0;
        while (!gameObject.IsUnityNull() && objectDuration < _emittedObjectDuration)
        {
            objectDuration += Time.deltaTime;
            gameObject.transform.position += gameObject.transform.forward * (_emittedObjectMoveSpeed * Time.deltaTime);
            yield return null;
        }
        Destroy(gameObject);
    }
    
    #region Getters
    public bool GetIsEmitting() => _isEmitting;
    #endregion
}

public enum ECustomObjectEmitterType
{
    FullRandom,
    RangeIncreaseRandom
};
