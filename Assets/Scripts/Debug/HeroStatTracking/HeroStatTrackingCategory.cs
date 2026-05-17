using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroStatTrackingCategory : MonoBehaviour
{
#if UNITY_EDITOR || DEVELOPMENT_BUILD
    [SerializeField] private float _statDistance;
    
    [SerializeField] private HeroStatTrackingIndividualStat[] _categoryStats;
    
    private float _highestStat = 0;
    private float _currentStat = 0;

    private float[] _heroStatsRank;

    public void SetUpStats()
    {
        for (int i = 0; i < HeroesManager.Instance.GetCurrentHeroes().Count; i++)
        {
            _categoryStats[i].PerformInitialStatSetUp(HeroesManager.Instance.GetCurrentHeroes()[i].GetHeroSO());
        }
        
        _heroStatsRank = new float[HeroesManager.Instance.GetCurrentHeroes().Count];
    }

    public void UpdateDamageDealtStatCategory()
    {
        for (int i = 0; i < HeroesManager.Instance.GetCurrentHeroes().Count; i++)
        {
            _currentStat = HeroesManager.Instance.GetCurrentHeroes()[i].GetHeroStats().GetTotalHeroDamageDealt();

            if (_currentStat > _highestStat)
            {
                _highestStat = _currentStat;
            }
        }
        
        for (int i = 0; i < HeroesManager.Instance.GetCurrentHeroes().Count; i++)
        {
            _currentStat = HeroesManager.Instance.GetCurrentHeroes()[i].GetHeroStats().GetTotalHeroDamageDealt();

            if (_currentStat == 0)
            {
                continue;
            }
            
            _categoryStats[i].UpdateIndividualStat(
                _currentStat, _currentStat/_highestStat);
        }

        _highestStat = 0;
    }
    
    public void UpdateStaggerDealtStatCategory()
    {
        for (int i = 0; i < HeroesManager.Instance.GetCurrentHeroes().Count; i++)
        {
            _currentStat = HeroesManager.Instance.GetCurrentHeroes()[i].GetHeroStats().GetTotalHeroStaggerDealt();

            if (_currentStat > _highestStat)
            {
                _highestStat = _currentStat;
            }
        }
        
        for (int i = 0; i < HeroesManager.Instance.GetCurrentHeroes().Count; i++)
        {
            _currentStat = HeroesManager.Instance.GetCurrentHeroes()[i].GetHeroStats().GetTotalHeroStaggerDealt();
            
            if (_currentStat == 0)
            {
                continue;
            }
            
            _categoryStats[i].UpdateIndividualStat(
                _currentStat, _currentStat/_highestStat);
        }

        _highestStat = 0;
    }
    
    public void UpdateHealingDealtStatCategory()
    {
        for (int i = 0; i < HeroesManager.Instance.GetCurrentHeroes().Count; i++)
        {
            _currentStat = HeroesManager.Instance.GetCurrentHeroes()[i].GetHeroStats().GetTotalHeroHealingDealt();

            if (_currentStat > _highestStat)
            {
                _highestStat = _currentStat;
            }
        }
        
        for (int i = 0; i < HeroesManager.Instance.GetCurrentHeroes().Count; i++)
        {
            _currentStat = HeroesManager.Instance.GetCurrentHeroes()[i].GetHeroStats().GetTotalHeroHealingDealt();
            
            if (_currentStat == 0)
            {
                continue;
            }
            
            _categoryStats[i].UpdateIndividualStat(
                _currentStat, _currentStat/_highestStat);
        }

        _highestStat = 0;
    }

    public void SortIndividualStats()
    {
        Array.Sort(_heroStatsRank);
    }
#endif
}
