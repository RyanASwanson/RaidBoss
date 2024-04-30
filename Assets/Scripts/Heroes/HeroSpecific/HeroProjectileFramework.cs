using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class HeroProjectileFramework : MonoBehaviour
{
    protected HeroBase _ownerHeroBase;

    public virtual void SetUpProjectile(HeroBase heroBase)
    {
        _ownerHeroBase = heroBase;
    }

    protected virtual void OnTriggerEnter(Collider collision)
    {
        Debug.Log("OnTriggerProjectile");
    }
}
