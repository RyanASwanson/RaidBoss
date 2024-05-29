using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroPillar : MonoBehaviour
{
    [SerializeField] private GameObject _heroSpawnPoint;
    private GameObject _currentHeroVisual;

    private HeroSO _storedHero;
    [Space]

    [SerializeField] private Animator _animator;

    private const string _heroPillarMoveAnimBool = "PillarUp";

    public void MovePillar(bool moveUp)
    {
        _animator.SetBool(_heroPillarMoveAnimBool, moveUp);
    }

    public void ShowHeroOnPillar(HeroSO heroSO)
    {
        

        if (_currentHeroVisual != null)
            RemoveHeroOnPillar();

        if (heroSO == null)
            Debug.Log("Failed to find hero prefab");
        if (_heroSpawnPoint == null)
            Debug.Log("failed to find hero spawn point");
        _currentHeroVisual = Instantiate(heroSO.GetHeroPrefab(), _heroSpawnPoint.transform);
        _currentHeroVisual.transform.eulerAngles += new Vector3(0,180,0);
        _storedHero = heroSO;
    }

    public void RemoveHeroOnPillar()
    {
        _storedHero = null;
        Destroy(_currentHeroVisual);
    }

    #region Getters
    public GameObject GetHeroSpawnPoint() => _heroSpawnPoint;
    public HeroSO GetStoredHero() => _storedHero;
    public bool HasStoredHero() => _storedHero != null;
    #endregion
}
