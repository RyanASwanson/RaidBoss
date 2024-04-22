using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroesManager : BaseGameplayManager
{
    private List<HeroSO> _currentHeroes = new List<HeroSO>();
    private List<HeroSO> _currentLivingHeroes = new List<HeroSO>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public override void SubscribeToEvents()
    {
        
    }
}
