using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class GlacialLordTargetZone : BossTargetZone
{
    [FormerlySerializedAs("_miniontInRangeMat")] [SerializeField] private Material _minionInRangeMat;
    
    protected List<BossMinionBase> _minionsInRange = new List<BossMinionBase>();
    
    
    /// <summary>
    /// Adds a new minion to the target zone
    /// </summary>
    /// <param name="enterMinion"></param>
    protected void AddMinionInRange(BossMinionBase enterMinion)
    {
        _minionsInRange.Add(enterMinion);
        if (_minionsInRange.Count == 1)
        {
            FirstMinionInRange();
        }
    }
    
    /// <summary>
    /// Called when the first minion is added to the range
    /// </summary>
    protected void FirstMinionInRange()
    {
        SetTargetZonesToMinionInRange();
    }

    /// <summary>
    /// Removes a minion from the target zone
    /// </summary>
    /// <param name="exitMinion"></param>
    protected void RemoveMinionInRange(BossMinionBase exitMinion)
    {
        if (_minionsInRange.Contains(exitMinion))
        {
            _minionsInRange.Remove(exitMinion);

            if (_minionsInRange.Count == 0)
            {
                NoMoreMinionsInRange();
            }
        }
    }

    /// <summary>
    /// Called when all minions in range are removed
    /// </summary>
    protected void NoMoreMinionsInRange()
    {
        SetTargetZonesToNoMinion();
    }
    
    protected void SetTargetZonesToMinionInRange()
    {
        AttemptAllTargetZonesToMaterial(_minionInRangeMat);
    }

    protected void SetTargetZonesToNoMinion()
    {
        if (DoesZoneContainHero())
        {
            AttemptAllTargetZonesToMaterial(_heroInRangeMat);
            return;
        }
        AttemptAllTargetZonesToMaterial(_noHeroInRangeMat);
    }
    
    #region BaseTargetZone

    protected override void FirstHeroInRange()
    {
        if (!DoesZoneContainMinion())
        {
            base.FirstHeroInRange();
        }
    }
    
    protected override void NoMoreHeroesInRange()
    {
        if (!DoesZoneContainMinion())
        {
            base.NoMoreHeroesInRange();
        }
    }
    #endregion
    
    #region Collision
    /// <summary>
    /// When a hero enters the target zone they are added to the list of heroes in range
    /// </summary>
    /// <param name="collision"></param>
    protected override void OnTriggerEnter(Collider collision)
    {
        if (TagStringData.DoesColliderBelongToBossMinion(collision))
        {
            BossMinionBase enterMinion = collision.GetComponentInParent<BossMinionBase>();
            AddMinionInRange(enterMinion);
            return;
        }
        base.OnTriggerEnter(collision);
    }

    /// <summary>
    /// When a hero leaves the target zone they are removed from the list of heroes in range
    /// </summary>
    /// <param name="collision"></param>
    protected override void OnTriggerExit(Collider collision)
    {
        if (TagStringData.DoesColliderBelongToBossMinion(collision))
        {
            BossMinionBase exitMinion = collision.GetComponentInParent<BossMinionBase>();
            RemoveMinionInRange(exitMinion);
            return;
        }
        base.OnTriggerExit(collision);
    }
    #endregion

    #region Getters

    public bool DoesZoneContainMinion() => (_minionsInRange.Count > 0);

    #endregion
}
