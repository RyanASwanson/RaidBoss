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

    private void BossTookDamage()
    {

    }

    public override void SubscribeToEvents()
    {
        
    }
}
