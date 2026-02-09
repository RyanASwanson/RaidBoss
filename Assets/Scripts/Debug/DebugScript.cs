using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Script purely used for testing misc functionality. Is not active in builds.
/// </summary>
public class DebugScript : MonoBehaviour
{
    public static DebugScript Instance;
    
    public bool RequiresMaxCharactersSelected;
    
    private void Start()
    {
        if(Instance.IsUnityNull())
        {
            Instance = this;
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        
#if !UNITY_EDITOR && !DEVELOPMENT_BUILD
        RequiresMaxCharactersSelected = true;
#endif
    }

#if UNITY_EDITOR
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Y))
        {
            SaveManager.Instance.UnlockAllCharacters();
        }

        if (Input.GetKeyDown(KeyCode.U))
        {
            BossStats.Instance.DealDamageToBoss((BossStats.Instance.GetBossMaxHealth()/2) + 1);
        }
        
        if (Input.GetKeyDown(KeyCode.I))
        {
            BossStats.Instance.DealStaggerToBoss(99999);
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            List<HeroBase> livingHeroes = HeroesManager.Instance.GetCurrentLivingHeroes();
            for (int i = 0; i < livingHeroes.Count; i++)
            {
                livingHeroes[i].GetSpecificHeroScript().AddToManualAbilityChargeTime(999);
            }
            
        }
        
        if (Input.GetKeyDown(KeyCode.P))
        {
            
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            List<HeroBase> heroes = PlayerInputGameplayManager.Instance.GetAllControlledHeroes();
            
            heroes[0].GetHeroStats().KillHero();
        }

        if (Input.GetKeyDown(KeyCode.Semicolon))
        {
            if (SelectionManager.Instance.GetSelectedBoss().GetBossID() == 2)
            {
                SB_GlacialLord glacialLord = (SB_GlacialLord)BossBase.Instance.GetSpecificBossScript();
                glacialLord.FreezeAllFrostFiends();
            }
        }

        if (Input.GetKeyDown(KeyCode.Quote))
        {
            BossBase.Instance.GetSpecificBossScript().SkipCurrentAttack();
        }
    }
#endif
}

