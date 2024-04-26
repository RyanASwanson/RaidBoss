using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossStats : BossChildrenFunctionality
{
    private float _bossMaxHealth;
    private float _currentHealth;

    private float _bossDefaultStaggerCounter;
    private float _currentStaggerCounter;

    public override void SubscribeToEvents()
    {
        
    }
}
