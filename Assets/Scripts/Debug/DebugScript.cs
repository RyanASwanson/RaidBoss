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
    
    internal bool IsEditor = false;
    
    public bool RequiresMaxCharactersSelected;
    public bool ShowAchievementUnlocks;
    
    public void PerformDebugScriptSetUp()
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
        IsEditor = true;
        RequiresMaxCharactersSelected = true;
#endif
    }

#if UNITY_EDITOR
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.T))
        {
            SaveManager.Instance.UnlockNextMissions();
        }
        
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
        
        if (Input.GetKeyDown(KeyCode.LeftBracket))
        {
            MainMenuCharacterDisplay mainMenuDisplay = FindObjectOfType<MainMenuCharacterDisplay>();
            mainMenuDisplay.StopDisplay();
            mainMenuDisplay.MoveToNextDisplaySection();
        }
        if (Input.GetKeyDown(KeyCode.RightBracket))
        {
            MainMenuCharacterDisplay mainMenuDisplay = FindObjectOfType<MainMenuCharacterDisplay>();
            mainMenuDisplay.StopDisplay();
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

