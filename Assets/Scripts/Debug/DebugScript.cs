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
    }

    public void ManagerSetUpComplete()
    {
#if UNITY_EDITOR
        SubscribeToEvents();
#endif
    }

#if UNITY_EDITOR

    [SerializeField] private GameObject _heroStatTrackingObject;
    private bool _isSubscribeToGameplayEvents = false;
    
    
    
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Plus) || Input.GetKeyDown(KeyCode.Equals))
        {
            SaveManager.Instance.UnlockNextMissions();
        }

        if (Input.GetKeyDown(KeyCode.Minus) || Input.GetKeyDown(KeyCode.Underscore))
        {
            BossStats.Instance.DecreaseTimeUntilEnraged(30);
        }
        
        if(Input.GetKeyDown(KeyCode.Y))
        {
            SaveManager.Instance.UnlockAllCharacters();

            SaveManager.Instance.UnlockAllMissionModifiers();
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
            switch (SelectionManager.Instance.GetSelectedBoss().GetBossID())
            {
                case 0:
                    VolcanoHeroMovementTracking[] volcanoHeroMovementTracking = FindObjectsOfType<VolcanoHeroMovementTracking>();
                    foreach (VolcanoHeroMovementTracking volcanoTracking in volcanoHeroMovementTracking)
                    {
                        volcanoTracking.MaxVolcanoMovementAmount();
                    }
                    return;
                case 1:
                    SB_TerraLord.Instance.TerraLordDebug();
                    return;
                case 2:
                    SB_GlacialLord glacialLord = (SB_GlacialLord)BossBase.Instance.GetSpecificBossScript();
                    glacialLord.FreezeAllFrostFiends();
                    return;
                case 3:
                    return;
                default:
                    return;
            }
        }

        if (Input.GetKeyDown(KeyCode.Quote))
        {
            BossBase.Instance.GetSpecificBossScript().SkipCurrentAttack();
        }

        if (Input.GetKeyDown(KeyCode.Slash))
        {
            SaveManager.Instance.UnlockNextMythicPlusLevel();
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            DisplayTrackingHeroStats();
        }

        if (Input.GetKeyDown(KeyCode.N))
        {
            // Low FPS toggle
            Application.targetFrameRate = Application.targetFrameRate == 30 ? 500 : 30;
        }
    }

    private void BattleStart()
    {
        
    }

    private void DisplayTrackingHeroStats()
    {
        Instantiate(_heroStatTrackingObject,Vector3.zero,Quaternion.identity);
    }

    private void SubscribeToEvents()
    {
        SceneLoadManager.Instance.GetOnGameplaySceneLoaded().AddListener(GameplaySceneLoaded);
        SceneLoadManager.Instance.GetOnStartOfSceneLoad().AddListener(UnsubscribeFromGameplayEvents);
    }

    private void GameplaySceneLoaded()
    {
        SubscribeToGameplayEvents();
    }

    
    private void SubscribeToGameplayEvents()
    {
        if (_isSubscribeToGameplayEvents)
        {
            return;
        }
        
        GameStateManager.Instance.GetStartOfBattleEvent().AddListener(BattleStart);
        
        _isSubscribeToGameplayEvents = true;
    }

    private void UnsubscribeFromGameplayEvents()
    {
        if (!_isSubscribeToGameplayEvents)
        {
            return;
        }
        
        GameStateManager.Instance.GetStartOfBattleEvent().RemoveListener(BattleStart);

        _isSubscribeToGameplayEvents = false;
    }
#endif
}

