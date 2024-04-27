using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class SpecificHeroFramework : MonoBehaviour
{
    public int _basicAbilityChargeTime;
    public int _manualAbilityChargeTime;

    public abstract void ActivateBasicAbilities();
    public abstract void ActivateManualAttack(Vector3 attackLocation);
    public abstract void ActivatePassiveAbilities();
    public virtual void SubscribeToEvents()
    {
        
    }
}
