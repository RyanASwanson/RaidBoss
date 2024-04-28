using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BossUIManager : GameUIChildrenFunctionality
{
    [Header("BossHealthBar")]
    [SerializeField] private Image _healthRecentBar;
    [SerializeField] private Image _healthBar;

    [SerializeField] private float _timeForRecentBarDrainStart;

    private Coroutine _startBarDrainCoroutine;

    private void BossTookDamage(float damage)
    {
        SetHealthBarPercentage(damage);
    }

    private void SetHealthBarPercentage(float damage)
    {
        _healthBar.fillAmount = GameplayManagers.Instance.
            GetBossManager().GetBossBase().GetBossStats().GetBossHealthPercentage();
    }

    public override void SubscribeToEvents()
    {
        GameplayManagers.Instance.GetBossManager().GetBossBase().GetBossDamagedEvent().AddListener(BossTookDamage);
    }
}
