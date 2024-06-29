using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPillar : MonoBehaviour
{
    [SerializeField] private GameObject _bossSpawnPoint;
    [SerializeField] private Animator _bossSpawnAnimator;

    private GameObject _currentBossVisual;

    private BossSO _storedBoss;
    [Space]

    [SerializeField] private Animator _pillarAnimator;

    private const string _bossPillarMoveAnimBool = "PillarUp";

    private const string _newBossHoverAnimTrigger = "NewHover";

    public void MovePillar(bool moveUp)
    {
        _pillarAnimator.SetBool(_bossPillarMoveAnimBool, moveUp);
    }

    public void ShowBossOnPillar(BossSO bossSO,bool newBoss)
    {
        if (_currentBossVisual != null)
            RemoveBossOnPillar();

        _currentBossVisual = Instantiate(bossSO.GetBossPrefab(), _bossSpawnPoint.transform);
        _currentBossVisual.transform.eulerAngles += new Vector3(0, 315, 0);
        _storedBoss = bossSO;

        if (!newBoss) return;
        _bossSpawnAnimator.SetTrigger(_newBossHoverAnimTrigger);
    }

    public void RemoveBossOnPillar()
    {
        _storedBoss = null;
        Destroy(_currentBossVisual);
    }

    #region Getters
    public GameObject GetBossSpawnPoint() => _bossSpawnPoint;
    public BossSO GetStoredBoss() => _storedBoss;
    public bool HasStoredBoss() => _storedBoss != null;
    #endregion
}
