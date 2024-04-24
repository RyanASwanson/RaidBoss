using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionManager : BaseUniversalManager
{
    private List<HeroSO> _selectedHeroes = new List<HeroSO>();
    private const int _maxHeroes = 5;

    public void AddNewSelectedHero(HeroSO newHeroSO)
    {
        if (_selectedHeroes.Count < _maxHeroes)
            _selectedHeroes.Add(newHeroSO);
       
    }

    public void RemoveSpecificHero(HeroSO removingHero)
    {
        if(_selectedHeroes.Contains(removingHero))
            _selectedHeroes.Remove(removingHero);    
    }


    #region Getters
    public List<HeroSO> GetAllSelectedHeroes() => _selectedHeroes;
    public HeroSO GetHeroAtValue(int val) => _selectedHeroes[val];
    public int GetSelectedHeroesCount() => _selectedHeroes.Count;
    #endregion
}
