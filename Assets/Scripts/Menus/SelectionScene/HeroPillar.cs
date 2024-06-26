using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroPillar : MonoBehaviour
{
    [SerializeField] private GameObject _heroSpawnPoint;
    [SerializeField] private Animator _heroSpawnAnimator;

    private GameObject _currentHeroVisual;

    private HeroSO _storedHero;
    [Space]

    [SerializeField] private Animator _pillarAnimator;

    private const string _heroPillarMoveAnimBool = "PillarUp";

    private const string _newHeroHoverAnimTrigger = "NewHover";

    public void MovePillar(bool moveUp)
    {
        _pillarAnimator.SetBool(_heroPillarMoveAnimBool, moveUp);
    }

    /// <summary>
    /// Displays a hero on the pillar
    /// </summary>
    /// <param name="heroSO"></param>
    public void ShowHeroOnPillar(HeroSO heroSO, bool newHero)
    {
        //If there is a hero on the pillar remove them
        if (_currentHeroVisual != null)
            RemoveHeroOnPillar();

        //Spawn the hero onto the pillar
        _currentHeroVisual = Instantiate(heroSO.GetHeroPrefab(), _heroSpawnPoint.transform);
        
        //Rotates the hero
        _currentHeroVisual.transform.eulerAngles += new Vector3(0,180,0);
        //Sets the stored hero
        _storedHero = heroSO;

        if (!newHero) return;
        _heroSpawnAnimator.SetTrigger(_newHeroHoverAnimTrigger);
    }

    /// <summary>
    /// Removes the current hero from the pillar
    /// </summary>
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
