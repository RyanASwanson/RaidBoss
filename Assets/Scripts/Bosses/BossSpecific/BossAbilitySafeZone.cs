using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Functionality for safe zones used to prevent attacks from dealing damage
/// </summary>
public class BossAbilitySafeZone : BossProjectileFramework
{
    private List<HeroBase> _heroesInRange = new List<HeroBase>();

    /// <summary>
    /// Adds a new hero to the safe zone
    /// </summary>
    /// <param name="enterHero"></param>
    public void AddHeroInRange(HeroBase enterHero)
    {
        _heroesInRange.Add(enterHero.gameObject.GetComponent<HeroBase>());
    }

    /// <summary>
    /// Removes a hero from the safe zone
    /// </summary>
    /// <param name="exitHero"></param>
    public void RemoveHeroInRange(HeroBase exitHero)
    {
        if (_heroesInRange.Contains(exitHero))
        {
            _heroesInRange.Remove(exitHero);
        }
    }

    /// <summary>
    /// Check if any heroes are currently in the safe zone
    /// </summary>
    /// <returns></returns>
    public bool DoesSafeZoneContainHero()
    {
        return (_heroesInRange.Count > 0);
    }

    #region Collision
    /// <summary>
    /// When a hero enters the safe zone they are added to the list of heroes in range
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter(Collider collision)
    {
        HeroBase tempHero = collision.GetComponentInParent<HeroBase>();
        if (!tempHero.IsUnityNull())
        {
            AddHeroInRange(tempHero);
        }
    }

    /// <summary>
    /// When a hero leaves the safe zone they are removed from the list of heroes in range
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerExit(Collider collision)
    {
        HeroBase tempHero = collision.GetComponentInParent<HeroBase>();
        if (!tempHero.IsUnityNull())
        {
            RemoveHeroInRange(tempHero);
        }
    }

    #endregion
}
