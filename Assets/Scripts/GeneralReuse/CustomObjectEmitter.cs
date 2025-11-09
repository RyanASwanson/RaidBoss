using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class CustomObjectEmitter : MonoBehaviour
{
    [SerializeField] private bool _hasEmissionDuration;
    [SerializeField] private float _emissionDuration;
    private float _emissionTime;
    [Space]
    
    [SerializeField] private float _emissionRate;
    [Space]
    
    [SerializeField] private float _emittedObjectMoveSpeed;
    [SerializeField] private float _emittedObjectDuration;

    [Space] 
    [SerializeField] private ECustomObjectEmitterType _emitterType;
    
    [Space]
    [SerializeField] private float _minimumAngleIncrease;
    [SerializeField] private float _maximumAngleIncrease;

    private Vector3 _currentRotation;
    
    [Space]
    [SerializeField] private GameObject _emitObject;
    
    private Coroutine _emittingCoroutine;
    
    bool _isEmitting;

    private void Start()
    {
        _currentRotation = FullRandomVector();
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
            if (emitCreationTimer > _emissionRate)
            {
                CreateEmittingObject();
                emitCreationTimer = 0;
            }
            
            yield return null;
        }
        _isEmitting = false;
    }

    private void CreateEmittingObject()
    {
        GameObject createdObject = Instantiate(_emitObject, transform.position, Quaternion.identity);
        
        createdObject.transform.eulerAngles = RandomDirectionVector();
        
        StartCoroutine(MoveEmittedObject(createdObject));
    }

    private Vector3 RandomDirectionVector()
    {
        switch (_emitterType)
        {
            case ECustomObjectEmitterType.FullRandom:
                return FullRandomVector();
            case ECustomObjectEmitterType.RangeIncreaseRandom:
                _currentRotation += new Vector3(0, Random.Range(_minimumAngleIncrease, _maximumAngleIncrease), 0);
                return _currentRotation;
            default:
                return Vector3.zero;
        }
    }

    private Vector3 FullRandomVector()
    {
        return new Vector3(0, Random.Range(0,360), 0);
    }

    private IEnumerator MoveEmittedObject(GameObject gameObject)
    {
        float objectDuration = 0;
        while (objectDuration < _emittedObjectDuration)
        {
            objectDuration += Time.deltaTime;
            gameObject.transform.position += gameObject.transform.forward * (_emittedObjectMoveSpeed * Time.deltaTime);
            yield return null;
        }
        Destroy(gameObject);
    }
}

public enum ECustomObjectEmitterType
{
    FullRandom,
    RangeIncreaseRandom
};
