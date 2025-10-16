using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class BossTargetZone : BossProjectileFramework
{
    [SerializeField] protected Material _noHeroInRangeMat;
    [SerializeField] protected Material _heroInRangeMat;

    [Space]
    [SerializeField] private EBossTargetZoneAppearType _targetZoneAppearType;
    [SerializeField] private float _targetZoneAppearSpeedMultiplier;
    
    [Space]
    [SerializeField] private EBossTargetZoneDisappearType _targetZoneDisappearType;
    [SerializeField] private float _targetZoneDisappearSpeedMultiplier;
    
    [Space]
    [SerializeField] private float _materialChangeDelay;

    private bool _isDelayingMaterialChange = true;

    [Space] 
    [SerializeField] private GameObject _visualsHolder;
    [SerializeField] private MeshRenderer[] _targetZones;
    
    [SerializeField] private GameObject[] _additionalGameObjectReferences;
    
    private Animator _targetAnimator;

    private const string TARGET_ZONE_APPEAR_ANIM_INT = "TargetZoneAppear";
    private const string TARGET_ZONE_DISAPPEAR_ANIM_INT = "TargetZoneDisappear";

    private const float TARGET_ZONE_APPEAR_DISAPPEAR_TIME = .1f;
    
    protected List<HeroBase> _heroesInRange = new List<HeroBase>();
    
    protected BossTargetZoneParent _bossTargetZoneParent;

    protected void Start()
    {
        PlayAppearAnimation();
        StartCoroutine(DelayInitialMaterialChange());
    }

    protected void PlayAppearAnimation()
    {
        _targetAnimator = GetComponent<Animator>();
        if (_targetZoneAppearType == EBossTargetZoneAppearType.Grow)
        {
            _visualsHolder.transform.localScale = Vector3.zero;
        }
        _targetAnimator.SetInteger(TARGET_ZONE_APPEAR_ANIM_INT, (int)_targetZoneAppearType);
        _targetAnimator.speed = _targetZoneAppearSpeedMultiplier;
    }

    protected IEnumerator DelayInitialMaterialChange()
    {
        yield return new WaitForSeconds(_materialChangeDelay);
        _isDelayingMaterialChange = false;
        if (DoesZoneContainHero())
        {
            SetTargetZonesToHeroInRange();
        }
        else
        {
            SetTargetZonesToNoHero();
        }
    }

    /// <summary>
    /// Adds a new hero to the target zone
    /// </summary>
    /// <param name="enterHero"></param>
    protected void AddHeroInRange(HeroBase enterHero)
    {
        _heroesInRange.Add(enterHero);
        if (_heroesInRange.Count == 1)
        {
            FirstHeroInRange();
        }
    }
    
    /// <summary>
    /// Called when the first hero is added to the range
    /// </summary>
    protected virtual void FirstHeroInRange()
    {
        SetTargetZonesToHeroInRange();
    }

    /// <summary>
    /// Removes a hero from the target zone
    /// </summary>
    /// <param name="exitHero"></param>
    protected void RemoveHeroInRange(HeroBase exitHero)
    {
        if (_heroesInRange.Contains(exitHero))
        {
            _heroesInRange.Remove(exitHero);

            if (_heroesInRange.Count == 0)
            {
                NoMoreHeroesInRange();
            }
        }
    }

    /// <summary>
    /// Called when all heroes in range are removed
    /// </summary>
    protected virtual void NoMoreHeroesInRange()
    {
        SetTargetZonesToNoHero();
    }

    protected virtual void SetTargetZonesToHeroInRange()
    {
        SetAllTargetZonesToMaterial(_heroInRangeMat);
    }

    protected virtual void SetTargetZonesToNoHero()
    {
        SetAllTargetZonesToMaterial(_noHeroInRangeMat);
    }

    protected void SetAllTargetZonesToMaterial(Material newMaterial)
    {
        if (_isDelayingMaterialChange)
        {
            return;
        }
        foreach (MeshRenderer renderer in _targetZones)
        {
            renderer.material = newMaterial;
        }
    }
    
    #region Removal

    public void RemoveTargetZone()
    {
        PlayDisappearAnimation();
    }

    /// <summary>
    /// Plays the animation of the target zone disappearing
    /// </summary>
    private void PlayDisappearAnimation()
    {
        _targetAnimator.SetInteger(TARGET_ZONE_DISAPPEAR_ANIM_INT, (int)_targetZoneDisappearType);
        _targetAnimator.speed = _targetZoneDisappearSpeedMultiplier;
    }
    #endregion
    
    #region Collision
    /// <summary>
    /// When a hero enters the target zone they are added to the list of heroes in range
    /// </summary>
    /// <param name="collision"></param>
    protected virtual void OnTriggerEnter(Collider collision)
    {
        if (TagStringData.DoesColliderBelongToHero(collision))
        {
            HeroBase tempHero = collision.GetComponentInParent<HeroBase>();
            if (!tempHero.IsUnityNull())
            {
                AddHeroInRange(tempHero);
            }
        }
        
    }

    /// <summary>
    /// When a hero leaves the target zone they are removed from the list of heroes in range
    /// </summary>
    /// <param name="collision"></param>
    protected virtual void OnTriggerExit(Collider collision)
    {
        if (TagStringData.DoesColliderBelongToHero(collision))
        {
            HeroBase tempHero = collision.GetComponentInParent<HeroBase>();
            if (!tempHero.IsUnityNull())
            {
                RemoveHeroInRange(tempHero);
            }
        }
        
    }
    #endregion
    
    #region Getters
    /// <summary>
    /// Check if any heroes are currently in the safe zone
    /// </summary>
    /// <returns></returns>
    public bool DoesZoneContainHero() => (_heroesInRange.Count > 0);
    
    public GameObject[] GetAdditionalGameObjectReferences() => _additionalGameObjectReferences;

    public float GetDisappearTime() => TARGET_ZONE_APPEAR_DISAPPEAR_TIME / _targetZoneDisappearSpeedMultiplier;
    #endregion

    #region Setters

    public void SetBossTargetZoneParent(BossTargetZoneParent bossTargetZoneParent)
    {
        _bossTargetZoneParent = bossTargetZoneParent;
    }

    #endregion
}

public enum EBossTargetZoneAppearType
{
    None,
    Grow
}

public enum EBossTargetZoneDisappearType
{
    None,
    Shrink
}