using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPillar : MonoBehaviour
{
    [SerializeField] private GameObject _bossSpawnPoint;
    private GameObject _currentBossVisual;

    private BossSO _storedBoss;
    [Space]

    [SerializeField] private Animator _animator;

    private const string _bossPillarMoveAnimBool = "PillarUp";

    public void MovePillar(bool moveUp)
    {
        _animator.SetBool(_bossPillarMoveAnimBool, moveUp);
    }

    public void ShowBossOnPillar(BossSO bossSO)
    {
        if (_currentBossVisual != null)
            RemoveBossOnPillar();

        _currentBossVisual = Instantiate(bossSO.GetBossPrefab(), _bossSpawnPoint.transform);
        _currentBossVisual.transform.eulerAngles += new Vector3(0, 315, 0);
        _storedBoss = bossSO;
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