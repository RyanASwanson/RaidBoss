using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Provides the functionality for the Alchemist hero
/// </summary>
public class SH_Alchemist : SpecificHeroFramework
{
    [Space]
    [SerializeField] private GameObject _basicProjectile;

    [Space]
    [SerializeField] private List<GameObject> _manualProjectiles;

    [Space]
    [SerializeField] private GameObject _passiveProjectile;

    [Space]
    [SerializeField] private float _potionDistanceMultiplier;

    #region Basic Abilities
    public override void ActivateBasicAbilities()
    {
        base.ActivateBasicAbilities();

        //Throws a healing potion at the target location
        CreatePotion(_basicProjectile, GetBasicPotionTargetLocation());
    }

    /// <summary>
    /// Determines where the basic projectile should be targeted at
    /// </summary>
    /// <returns></returns>
    protected Vector3 GetBasicPotionTargetLocation()
    {
        //Finds a random point on the edge of a circle
        Vector2 randomVector = Random.insideUnitCircle.normalized;
        
        //Sets the center of the circle to be at the starting location of the potion
        Vector3 targetPosition = transform.position;
        //Converts the circle into 3D space having it in the X and Z directions
        //Multiplies the how far away the edge of the circle is by the potion distance multiplier
        targetPosition += new Vector3(randomVector.x, 0, randomVector.y) * _potionDistanceMultiplier;

        //Gets the closest valid point in the environment to where the target position is
        targetPosition = EnvironmentManager.Instance.GetClosestPointToFloor(targetPosition);
        //Keeps the y value consistent
        targetPosition = new Vector3(targetPosition.x, transform.position.y, targetPosition.z);

        return targetPosition;
    }

    #endregion

    #region Manual Abilities
    public override void ActivateManualAbilities(Vector3 attackLocation)
    {
        base.ActivateManualAbilities(attackLocation);

        //Keeps the y value the same
        attackLocation = new Vector3(attackLocation.x, transform.position.y, attackLocation.z);
        //Creates a random potion from the manual options and sets its end location
        CreatePotion(PickManualPotion(), attackLocation);
    }

    /// <summary>
    /// Decides which of the manual potions to create
    /// </summary>
    /// <returns></returns>
    private GameObject PickManualPotion()
    {
        return _manualProjectiles[Random.Range(0, _manualProjectiles.Count)];
    }
    #endregion

    #region Passive Abilities
    /// <summary>
    /// Creates and sets up the passive projectile
    /// </summary>
    /// <param name="spawnLocation"></param> Determined by where the potion was picked up
    public void ActivatePassiveAbilities(Vector3 spawnLocation)
    {
        //Spawns the passive projectile at the input vector
        GameObject newestPassiveProjectile = Instantiate(_passiveProjectile, spawnLocation, Quaternion.identity);

        SHP_AlchemistPassiveProjectile passiveProj = newestPassiveProjectile.
            GetComponent<SHP_AlchemistPassiveProjectile>();

        //Sets up the projectile
        passiveProj.SetUpProjectile(_myHeroBase);
    }
    #endregion

    #region General Abilities
    /// <summary>
    /// Provides the general functionality to create any potion
    /// </summary>
    /// <param name="potion"></param>
    /// <param name="endLocation"></param>
    protected void CreatePotion(GameObject potion, Vector3 endLocation)
    {
        GameObject newestPotion = Instantiate(potion, transform.position, Quaternion.identity);

        SHP_AlchemistPotion potionProjectile = newestPotion.GetComponent<SHP_AlchemistPotion>();

        //Provides the needed setup for the potion
        potionProjectile.SetUpProjectile(_myHeroBase);
        potionProjectile.AdditionalSetup(endLocation);
        
        newestPotion.GetComponent<GeneralHeroHealArea>().SetUpHealingArea(_myHeroBase);
    }

    #endregion

    #region Base Hero
    
    #endregion
}

/// <summary>
/// The types of potions the Alchemist can create
/// </summary>
public enum PotionTypes
{
    HealingPotion,
    DamagePotion,
    StaggerPotion,
    SpeedPotion,
    UtilityPotion
};
