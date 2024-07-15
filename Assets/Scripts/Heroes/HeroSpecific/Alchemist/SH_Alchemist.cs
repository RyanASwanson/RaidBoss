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

        targetPosition = GameplayManagers.Instance.GetEnvironmentManager().GetClosestPointToFloor(targetPosition);
        targetPosition = new Vector3(targetPosition.x, transform.position.y, targetPosition.z);

        return targetPosition;
    }

    #endregion

    #region Manual Abilities
    public override void ActivateManualAbilities(Vector3 attackLocation)
    {
        base.ActivateManualAbilities(attackLocation);

        attackLocation = new Vector3(attackLocation.x, transform.position.y, attackLocation.z);
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
        newestPotion.GetComponent<SHP_AlchemistPotion>().SetUpProjectile(myHeroBase);
        newestPotion.GetComponent<SHP_AlchemistPotion>().AdditionalSetup(endLocation);
        
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

public enum PotionTypes
{
    HealingPotion,
    DamagePotion,
    StaggerPotion,
    SpeedPotion,
    UtilityPotion
};
