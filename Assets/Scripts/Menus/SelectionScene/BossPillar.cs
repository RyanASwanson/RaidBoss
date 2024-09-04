using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Provides the functionality for the boss pillar on the selection scene
/// </summary>
public class BossPillar : MonoBehaviour
{
    [SerializeField] private GameObject _bossSpawnPoint;
    [SerializeField] private Animator _bossSpawnAnimator;

    private GameObject _currentBossVisual;

    private BossSO _storedBoss;
    [Space]

    [SerializeField] private Animator _pillarAnimator;

    private const string BOSS_PILLAR_MOVE_ANIM_BOOL = "PillarUp";

    private const string NEW_BOSS_HOVER_ANIM_TRIGGER = "NewHover";
    private const string REMOVE_BOSS_ON_PILLAR_ANIM_TRIGGER = "RemoveBoss";

    public void MovePillar(bool moveUp)
    {
        _pillarAnimator.SetBool(BOSS_PILLAR_MOVE_ANIM_BOOL, moveUp);
    }

    public void ShowBossOnPillar(BossSO bossSO,bool newBoss)
    {
        if (_currentBossVisual != null)
            RemoveBossOnPillar();

        _currentBossVisual = Instantiate(bossSO.GetBossPrefab(), _bossSpawnPoint.transform);
        _currentBossVisual.transform.eulerAngles += new Vector3(0, 315, 0);
        _storedBoss = bossSO;

        if (!newBoss) return;
        _bossSpawnAnimator.SetTrigger(NEW_BOSS_HOVER_ANIM_TRIGGER);
        _bossSpawnAnimator.ResetTrigger(REMOVE_BOSS_ON_PILLAR_ANIM_TRIGGER);
    }

    public void RemoveBossOnPillar()
    {
        _storedBoss = null;
        Destroy(_currentBossVisual);
    }

    public void AnimateOutBossOnPillar()
    {
        _bossSpawnAnimator.ResetTrigger(NEW_BOSS_HOVER_ANIM_TRIGGER);
        _bossSpawnAnimator.SetTrigger(REMOVE_BOSS_ON_PILLAR_ANIM_TRIGGER);
    }

    #region Getters
    public GameObject GetBossSpawnPoint() => _bossSpawnPoint;
    public BossSO GetStoredBoss() => _storedBoss;
    public bool HasStoredBoss() => _storedBoss != null;
    #endregion
}
