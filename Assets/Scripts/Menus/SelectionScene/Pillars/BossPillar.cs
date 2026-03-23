using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Provides the functionality for the boss pillar on the selection scene
/// </summary>
public class BossPillar : MonoBehaviour
{
    [SerializeField] private Vector3 _bossSpawnPointOffset;
    [SerializeField] private GameObject _bossSpawnPoint;
    [SerializeField] private Animator _bossSpawnAnimator;
    
    [Space]
    [SerializeField] private MeshRenderer[] _bossPlatformRenderers;

    [SerializeField] private CurveProgression _squishScaleCurve;
    [SerializeField] private CurveProgression _selectionScaleCurve;

    private GameObject _currentBossVisual;

    private BossSO _bossSelectedOnPillar;
    private BossSO _storedBoss;
    
    [Space]
    [SerializeField] private Animator _pillarAnimator;

    private const string BOSS_PILLAR_MOVE_ANIM_BOOL = "PillarUp";

    private const string NEW_BOSS_HOVER_ANIM_TRIGGER = "NewHover";
    private const string REMOVE_BOSS_ON_PILLAR_ANIM_TRIGGER = "RemoveBoss";
    
    private Animator _bossSpecificAnimator;
    
    private const string BOSS_PILLAR_SELECTED_ANIM_BOOL = "BossSelected";
    

    private void Start()
    {
        SetPillarBossPreviewAnimation(false);
    }

    public void ShowBossOnPillar(BossSO bossSO,bool newBoss)
    {
        if (!_currentBossVisual.IsUnityNull())
        {
            RemoveBossOnPillar();
        }

        _currentBossVisual = Instantiate(bossSO.GetBossPrefab(), _bossSpawnPoint.transform);
        _currentBossVisual.transform.localPosition += _bossSpawnPointOffset;
        
        _bossSpecificAnimator = _currentBossVisual.GetComponentInChildren<Animator>();
        
        _currentBossVisual.transform.eulerAngles += new Vector3(0, 315, 0);
        _storedBoss = bossSO;

        UpdateBossPlatform();
        
        if (_bossSelectedOnPillar == bossSO)
        {
            SetPillarBossPreviewAnimation(true);
            PlayBossIdleAnimation();
        }
        else
        {
            SetPillarBossPreviewAnimation(!newBoss);
        }

        if (!newBoss)
        {
            BossSelectedOnPillar();
            return;
        }
        
        PlayBossHoverAnimation();
        _pillarAnimator.ResetTrigger(REMOVE_BOSS_ON_PILLAR_ANIM_TRIGGER);
        _bossSpawnAnimator.ResetTrigger(REMOVE_BOSS_ON_PILLAR_ANIM_TRIGGER);
        //_squishScaleCurve.StartMovingUpOnCurve();
    }

    public void BossSelectedOnPillar()
    {
        _bossSelectedOnPillar = _storedBoss;
        
        StartBossSelectedAnimation();
        PlayBossIdleAnimation();
        //_selectionScaleCurve.StartMovingUpOnCurve();
    }

    public void RemoveBossOnPillar()
    {
        _storedBoss = null;
        SetPillarBossPreviewAnimation(false);
        Destroy(_currentBossVisual);
    }
    
    public void DeselectBossOnPillar()
    {
        _storedBoss = null;
        _bossSelectedOnPillar = null;
        SetPillarBossPreviewAnimation(false);
        //_selectionScaleCurve.StartMovingDownOnCurve();
    }

    public void AnimateOutBossOnPillar()
    {
        _pillarAnimator.ResetTrigger(NEW_BOSS_HOVER_ANIM_TRIGGER);
        _bossSpawnAnimator.ResetTrigger(NEW_BOSS_HOVER_ANIM_TRIGGER);
        _pillarAnimator.SetTrigger(REMOVE_BOSS_ON_PILLAR_ANIM_TRIGGER);
        _bossSpawnAnimator.SetTrigger(REMOVE_BOSS_ON_PILLAR_ANIM_TRIGGER);
    }

    public void PlayBossHoverAnimation()
    {
        _pillarAnimator.SetTrigger(NEW_BOSS_HOVER_ANIM_TRIGGER);
        _bossSpawnAnimator.SetTrigger(NEW_BOSS_HOVER_ANIM_TRIGGER);
    }
    
    public void SetPillarBossPreviewAnimation(bool isBossSelected)
    {
        _pillarAnimator.SetBool(BOSS_PILLAR_SELECTED_ANIM_BOOL,isBossSelected);
        _bossSpawnAnimator.SetBool(BOSS_PILLAR_SELECTED_ANIM_BOOL,isBossSelected);
    }
    
    public void StartBossSelectedAnimation()
    {
        _bossSpecificAnimator.SetTrigger(BossVisuals.BOSS_SPECIFIC_SELECTED_ANIM_TRIGGER);

    }
    
    public void PlayBossIdleAnimation()
    {
        _bossSpecificAnimator.SetBool(BossVisuals.SPECIFIC_BOSS_IDLE_ANIM_BOOL,true);
    }

    private void UpdateBossPlatform()
    {
        foreach (MeshRenderer meshRenderer in _bossPlatformRenderers)
        {
            if (_bossPlatformRenderers.IsUnityNull())
            {
                continue;
            }

            meshRenderer.material = _storedBoss.GetMiniFloorMaterial();
        }
        
    }
    
    #region Getters
    public GameObject GetBossSpawnPoint() => _bossSpawnPoint;
    public BossSO GetStoredBoss() => _storedBoss;
    public bool HasStoredBoss() => _storedBoss != null;
    #endregion
}
