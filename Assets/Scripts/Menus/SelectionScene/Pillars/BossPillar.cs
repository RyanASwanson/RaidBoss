using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Provides the functionality for the boss pillar on the selection scene
/// </summary>
public class BossPillar : MonoBehaviour
{
    [SerializeField] private GameObject _bossSpawnPoint;
    [SerializeField] private Animator _bossSpawnAnimator;

    private GameObject _currentBossVisual;

    private BossSO _bossSelectedOnPillar;
    private BossSO _storedBoss;
    
    private const string BOSS_SPECIFIC_SELECTED_ANIM_TRIGGER = "G_BossSelected";
    
    [Space]
    [SerializeField] private Animator _pillarAnimator;

    private const string BOSS_PILLAR_MOVE_ANIM_BOOL = "PillarUp";

    private const string NEW_BOSS_HOVER_ANIM_TRIGGER = "NewHover";
    private const string REMOVE_BOSS_ON_PILLAR_ANIM_TRIGGER = "RemoveBoss";
    
    private Animator _bossSpecificAnimator;
    
    private const string BOSS_SELECTED_ANIM_BOOL = "BossSelected";
    private const string BOSS_IDLE_ANIM_BOOL = "G_BossIdle";

    private void Start()
    {
        SetBossPreviewAnimation(false);
    }
    
    public void MovePillar(bool moveUp)
    {
        _pillarAnimator.SetBool(BOSS_PILLAR_MOVE_ANIM_BOOL, moveUp);
    }

    public void ShowBossOnPillar(BossSO bossSO,bool newBoss)
    {
        if (!_currentBossVisual.IsUnityNull())
        {
            RemoveBossOnPillar();
        }

        _currentBossVisual = Instantiate(bossSO.GetBossPrefab(), _bossSpawnPoint.transform);
        
        _bossSpecificAnimator = _currentBossVisual.GetComponentInChildren<Animator>();
        
        _currentBossVisual.transform.eulerAngles += new Vector3(0, 315, 0);
        _storedBoss = bossSO;
        
        if (_bossSelectedOnPillar == bossSO)
        {
            SetBossPreviewAnimation(true);
            PlayBossIdleAnimation();
        }
        else
        {
            SetBossPreviewAnimation(!newBoss);
        }

        if (!newBoss)
        {
            BossSelectedOnPillar();
            return;
        }
        
        PlayBossHoverAnimation();
        _bossSpawnAnimator.ResetTrigger(REMOVE_BOSS_ON_PILLAR_ANIM_TRIGGER);
    }

    public void BossSelectedOnPillar()
    {
        _bossSelectedOnPillar = _storedBoss;
        
        StartBossSelectedAnimation();
        PlayBossIdleAnimation();
    }

    public void RemoveBossOnPillar()
    {
        _storedBoss = null;
        SetBossPreviewAnimation(false);
        Destroy(_currentBossVisual);
    }
    
    public void DeselectBossOnPillar()
    {
        _storedBoss = null;
        _bossSelectedOnPillar = null;
        SetBossPreviewAnimation(false);
    }

    public void AnimateOutBossOnPillar()
    {
        _bossSpawnAnimator.ResetTrigger(NEW_BOSS_HOVER_ANIM_TRIGGER);
        _bossSpawnAnimator.SetTrigger(REMOVE_BOSS_ON_PILLAR_ANIM_TRIGGER);
    }

    public void PlayBossHoverAnimation()
    {
        _bossSpawnAnimator.SetTrigger(NEW_BOSS_HOVER_ANIM_TRIGGER);
    }
    
    public void SetBossPreviewAnimation(bool isBossSelected)
    {
        _bossSpawnAnimator.SetBool(BOSS_SELECTED_ANIM_BOOL,isBossSelected);
    }
    
    public void StartBossSelectedAnimation()
    {
        _bossSpecificAnimator.SetTrigger(BOSS_SPECIFIC_SELECTED_ANIM_TRIGGER);
    }
    
    public void PlayBossIdleAnimation()
    {
        _bossSpecificAnimator.SetBool(BOSS_IDLE_ANIM_BOOL,true);
    }

    #region Getters
    public GameObject GetBossSpawnPoint() => _bossSpawnPoint;
    public BossSO GetStoredBoss() => _storedBoss;
    public bool HasStoredBoss() => _storedBoss != null;
    #endregion
}
