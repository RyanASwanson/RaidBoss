using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class SpecificHeroFramework : MonoBehaviour
{
    private UnityEvent _basicAttackAbilities = new UnityEvent();
    private UnityEvent _manualAbilities = new UnityEvent();
    private UnityEvent _passiveAbilities = new UnityEvent();


    public virtual void SubscribeToEvents()
    {
        
    }
}
