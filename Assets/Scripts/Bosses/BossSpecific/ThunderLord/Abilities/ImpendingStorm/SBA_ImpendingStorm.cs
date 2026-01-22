using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SBA_ImpendingStorm : SpecificBossAbilityFramework
{
    [Space]
    [SerializeField] private float _baseRotationSpeed;
    [SerializeField] private float[] _difficultyRotationMultiplier;

    private float _rotationSpeed;

    private float _attackRotation = 0;
    
    [Space]
    [SerializeField] private GameObject _impendingStormTargetZone;
    private GameObject _currentImpendingStormTargetZone;
    
    private Coroutine _rotationCoroutine;

    public GameObject BattleStart()
    {
        _rotationSpeed = _baseRotationSpeed * _difficultyRotationMultiplier[(int)SelectionManager.Instance.GetSelectedDifficulty()-1];

        CreateImpendingStormTargetZone();
        
        StartRotateImpendingStorm();

        return _currentImpendingStormTargetZone;
    }

    public void ActivateOvercharge()
    {
        RotateImpendingStorm(180);
    }

    private void CreateImpendingStormTargetZone()
    {
        _currentImpendingStormTargetZone = Instantiate(_impendingStormTargetZone, transform.position, Quaternion.identity);
        
        //_currentImpendingStormTargetZone.transform.SetParent(BossBase.Instance.GetSpecificBossScript().transform);
        
        _currentImpendingStormTargetZone.transform.position = new Vector3(_currentImpendingStormTargetZone.transform.position.x,
            _specificAreaTarget.y, _currentImpendingStormTargetZone.transform.position.z);
    }

    private void StartRotateImpendingStorm()
    {
        if (_rotationCoroutine.IsUnityNull())
        {
            StopRotateImpendingStorm();
        }

        _rotationCoroutine = StartCoroutine(RotateImpendingStormProcess());
    }

    private void StopRotateImpendingStorm()
    {
        if (!_rotationCoroutine.IsUnityNull())
        {
            StopCoroutine(_rotationCoroutine);
        }
    }

    private IEnumerator RotateImpendingStormProcess()
    {
        while (true)
        {
            RotateImpendingStorm(_rotationSpeed * Time.deltaTime);
            
            yield return null;
        }
    }

    private void RotateImpendingStorm(float rotationAmount)
    {
        _attackRotation += rotationAmount;
        if (_attackRotation > 360)
        {
            _attackRotation -= 360;
        }
        UpdateTargetZone();
    }
    
    private void UpdateTargetZone()
    {
        _currentImpendingStormTargetZone.transform.eulerAngles = new Vector3(0, _attackRotation, 0);
    }
    
    
    #region Getters

    public float GetAttackRotation() => _attackRotation;

    public float GetOppositeAttackRotation() => _attackRotation + 180;

    #endregion
}
