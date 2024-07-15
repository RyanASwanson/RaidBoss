using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SH_Alchemist : SpecificHeroFramework
{
    [Space]
    [SerializeField] private GameObject _basicProjectile;

    [Space]
    [SerializeField] private List<GameObject> _manualProjectiles;

    [Space]
    [SerializeField] private GameObject _passiveProjectile;

    [SerializeField] private float _potionDistanceMultiplier;

    #region Basic Abilities
    public override bool ConditionsToActivateBasicAbilities()
    {
        return !myHeroBase.GetPathfinding().IsHeroMoving();
    }

    public override void ActivateBasicAbilities()
    {
        base.ActivateBasicAbilities();

        CreatePotion(_basicProjectile, GetBasicPotionTargetLocation());


    }

    protected Vector3 GetBasicPotionTargetLocation()
    {
        Vector2 randomVector = Random.insideUnitCircle.normalized;
        Vector3 targetPosition = transform.position;
        targetPosition += new Vector3(randomVector.x, 0, randomVector.y) * _potionDistanceMultiplier;
        return GameplayManagers.Instance.GetEnvironmentManager().GetClosestPointToFloor(targetPosition);
    }

    #endregion

    #region Manual Abilities
    public override void ActivateManualAbilities(Vector3 attackLocation)
    {
        base.ActivateManualAbilities(attackLocation);

        GameObject randomManualPotion = _manualProjectiles[Random.Range(0, _manualProjectiles.Count)];
        CreatePotion(randomManualPotion, attackLocation);
    }
    #endregion

    #region Passive Abilities

    #endregion

    #region General Abilities
    protected void CreatePotion(GameObject potion, Vector3 endLocation)
    {
        GameObject newestPotion = Instantiate(potion, transform.position, Quaternion.identity);

        StartCoroutine(MovePotionToEndLocation(newestPotion, endLocation));
        
    }

    protected IEnumerator MovePotionToEndLocation(GameObject potion, Vector3 targetLocation)
    {
        Vector3 startingPotionLocation = potion.transform.position;
        float lerpProgress = 0;

        while(lerpProgress < 1)
        {
            lerpProgress += Time.deltaTime;
            potion.transform.position = Vector3.Lerp(startingPotionLocation, targetLocation, lerpProgress);
            yield return null;
        }

        potion.transform.position = targetLocation;
    }

    
    #endregion

    public override void ActivateHeroSpecificActivity()
    {
        base.ActivateHeroSpecificActivity();
    }

    public override void DeactivateHeroSpecificActivity()
    {
        base.DeactivateHeroSpecificActivity();
    }

    protected override void BattleStarted()
    {
        base.BattleStarted();
    }

    protected override void HeroDied()
    {
        base.HeroDied();
    }

    public override void SubscribeToEvents()
    {
        base.SubscribeToEvents();

        
    }
}
