using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class GeneralBossEnrageEvent : MonoBehaviour
{
    [SerializeField] private bool _doesPerformEnrageChecksOnStart = false;
    
    [Space]
    [SerializeField] private bool _hasScaleMultiplier;
    [SerializeField] private Vector3 _localScaleEnrageMultiplier;

    [Space] 
    [SerializeField] private bool _hasEnrageEvent;
    [SerializeField] private UnityEvent _bossEnragedEvent;
    
    void Start()
    {
        if (_doesPerformEnrageChecksOnStart)
        {
            PerformEnrageChecks(BossStats.Instance.GetIsBossEnraged());
        }
    }

    public void PerformEnrageChecks(bool isBossEnraged)
    {
        if (!BossStats.Instance.IsUnityNull() && isBossEnraged)
        {
            if (_hasScaleMultiplier)
            {
                AdjustScale();
            }

            if (_hasEnrageEvent)
            {
                InvokeBossEnragedEvent();
            }
        }
    }

    private void AdjustScale()
    {
        transform.localScale = new Vector3(transform.localScale.x * _localScaleEnrageMultiplier.x, 
            transform.localScale.y * _localScaleEnrageMultiplier.y, transform.localScale.z * _localScaleEnrageMultiplier.z);
    }

    private void InvokeBossEnragedEvent()
    {
        _bossEnragedEvent?.Invoke();
    }
    
}
