using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SHP_AlchemistPotion : HeroProjectileFramework
{
    [SerializeField] private float _moveTime;

    public override void SetUpProjectile(HeroBase heroBase)
    {
        base.SetUpProjectile(heroBase);

        
    }

    public void AdditionalSetup(Vector3 targetLocation)
    {
        StartCoroutine(MovePotionToEndLocation(targetLocation));
    }

    public IEnumerator MovePotionToEndLocation( Vector3 targetLocation)
    {
        Vector3 startingPotionLocation = transform.position;
        float lerpProgress = 0;

        while (lerpProgress < 1)
        {
            lerpProgress += Time.deltaTime / _moveTime;
            transform.position = Vector3.Lerp(startingPotionLocation, targetLocation, lerpProgress);
            yield return null;
        }

        transform.position = targetLocation;
    }
}
