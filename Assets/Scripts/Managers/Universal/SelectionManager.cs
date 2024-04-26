using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionManager : BaseUniversalManager
{
    private List<HeroSO> _selectedHeroes = new List<HeroSO>();
    private const int _maxHeroes = 5;

    private BossSO _selectedBoss;

    private GameDifficulty _currentGameDifficulty;

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
    public int GetMaxHeroesCount() => _maxHeroes;
    #endregion

    #region Setters
    public void SetSelectedBoss(BossSO bossSO)
    {
        _selectedBoss = bossSO;
    }
    #endregion
}

public enum GameDifficulty
{
    Easy,
    Medium,
    Hard
};
