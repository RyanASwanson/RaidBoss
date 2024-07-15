using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SHP_AlchemistPotion : HeroProjectileFramework
{
    [SerializeField] private float _moveTime;
    [SerializeField] private PotionTypes _potionType;

    [Space]
    [SerializeField] private GeneralHeroHealArea _healArea;

    public override void SetUpProjectile(HeroBase heroBase)
    {
        base.SetUpProjectile(heroBase);

        
    }

    public void AdditionalSetup(Vector3 targetLocation)
    {
        PotionTypeSetup();
        StartCoroutine(MovePotionToEndLocation(targetLocation));
    }

    private void PotionTypeSetup()
    {
        switch (_potionType)
        {
            case (PotionTypes.DamagePotion):
                _healArea.GetEnterEvent().AddListener(DamageBuff);
                return;
            case (PotionTypes.StaggerPotion):
                _healArea.GetEnterEvent().AddListener(StaggerBuff);
                return;
            case (PotionTypes.SpeedPotion):
                _healArea.GetEnterEvent().AddListener(SpeedBuff);
                return;

        }
        
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
        ReachedEndLocation();
    }

    private void ReachedEndLocation()
    {
        _healArea.ToggleProjectileCollider(true);
    }

    private void DamageBuff(Collider collider)
    {

    }

    private void StaggerBuff(Collider collider)
    {

    }

    private void SpeedBuff(Collider collider)
    {

    }
}
