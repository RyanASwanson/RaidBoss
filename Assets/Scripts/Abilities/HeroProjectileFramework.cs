using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class HeroProjectileFramework : MonoBehaviour
{
    protected abstract void OnTriggerEnter(Collider collision);
}
