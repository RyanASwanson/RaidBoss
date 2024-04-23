using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionManager : BaseUniversalManager
{
    private HeroSO[] _selectedHeroes = new HeroSO[_maxHeroes];
    private int _selectedCount = 0;
    private const int _maxHeroes = 5;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void AddNewSelectedHero(HeroSO newHeroSO)
    {
        if (_selectedCount < _maxHeroes)
        {
            _selectedHeroes[_selectedCount] = newHeroSO;
            _selectedCount++;
        }
    }

    public void RemoveSpecificHero(HeroSO removingHero)
    {
        
    }


    #region Getters
    public HeroSO[] GetAllSelectedHeroes() => _selectedHeroes;
    public HeroSO GetHeroAtValue(int val) => _selectedHeroes[val];
    public int GetSelectedHeroesCount() => _selectedCount;
    #endregion
}
