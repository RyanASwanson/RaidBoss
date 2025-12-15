using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using Unity.VisualScripting;
using UnityEngine.Serialization;

public class SaveManager : MainUniversalManagerFramework
{
    public static SaveManager Instance;
    
    public GameSaveData GSD {get; set;}
    private string _path;

    [SerializeField] private List<BossSO> _bossesInGame = new();
    [SerializeField] private List<HeroSO> _heroesInGame = new();
    [SerializeField] private MissionSO[] _missionsInGame;

    [Space]
    [SerializeField] private List<BossSO> _bossesStartingUnlocked = new();
    [SerializeField] private List<HeroSO> _heroesStartingUnlocked = new();
    
    [Space]
    [SerializeField] private List<MissionSO> _missionsStartingUnlocked = new();

    /// <summary>
    /// Sets the path to create the save file
    /// </summary>
    private void EstablishPath()
    {
        _path = Application.isEditor ? Application.dataPath : Application.persistentDataPath; //checks to see if we're in editor, if so use datapath instead of persistent datapath
        _path = _path + "/SaveData/"; //append /SaveData/ to said path
        if (!Application.isEditor && !Directory.Exists(_path)) //check if we're in a build, check if the directory exists, if not
        {
            Directory.CreateDirectory(_path); //create it
        }
    }

    /// <summary>
    /// Fills the save data with its initial values
    /// </summary>
    private void StartingValues()
    {
        GSD = new();
        GSD.ResetGameSaveData();
        
        GSD.GetGeneralSaveData().SetGameVersion(Application.version);
        
        //Fills the GSD with default values when the file is created
        UnlockStartingUnlocks();
        
        PopulateBossHeroDifficultyDictionary();

        GSD.GetGeneralSaveData().SetGSDScreenShakeStrength(1);
        GSD.GetGeneralSaveData().SetGSDHeroClickAndDrag(false);

        GSD.GetGeneralSaveData().SetGSDMasterVolume(.5f);
        GSD.GetGeneralSaveData().SetGSDMusicVolume(.5f);
        GSD.GetGeneralSaveData().SetGSDSFXVolume(.5f);
    }

    private void UnlockStartingUnlocks()
    {
        UnlockStartingCharacters();
        UnlockStartingMissions();
    }

    private void UnlockStartingCharacters()
    {
        UnlockStartingBosses();
        UnlockStartingHeroes();
    }

    private void UnlockStartingBosses()
    {
        foreach (BossSO boss in _bossesStartingUnlocked)
        {
            UnlockBoss(boss);
        }
    }

    private void UnlockStartingHeroes()
    {
        foreach (HeroSO hero in _heroesStartingUnlocked)
        {
            UnlockHero(hero);
        }
    }

    private void UnlockStartingMissions()
    {
        foreach (MissionSO mission in _missionsStartingUnlocked)
        {
            UnlockMission(mission);
        }
    }

    /// <summary>
    /// Populates the dictionary for what heroes have beaten what bosses on what difficulty
    /// </summary>
    private void PopulateBossHeroDifficultyDictionary()
    {
        //Reset the dictionary
        GSD.GetGameplaySaveData().ResetBossHeroDifficulties();

        //Iterate through the boss scriptable objects
        foreach(BossSO bossSO in _bossesInGame)
        {
            // Adds the boss to the dictionary
            GSD.GetGameplaySaveData().GetBossHeroBestDifficulty().Add(bossSO.GetBossName(), new Dictionary<string, EGameDifficulty>());

            foreach(HeroSO heroSO in _heroesInGame)
            {
                //Sets each best difficulty beaten to empty
                GSD.GetGameplaySaveData().GetBossHeroBestDifficulty()[bossSO.GetBossName()].Add(heroSO.GetHeroName(), EGameDifficulty.Empty);
            }
        }
    }

    /// <summary>
    /// Saves all data into the Json file.
    /// </summary>
    public void SaveText()
    {
        //Writes all variables in the Game Save Data class into Json
        var convertedJson = JsonConvert.SerializeObject(GSD);
        //var convertedJson = JsonUtility.ToJson(GSD);
        File.WriteAllText(_path + "Data.json", convertedJson);
    }

    /// <summary>
    /// Loads all data from the Json file
    /// </summary>
    public void Load()
    {
        //Loads all variables in Json into the Game Save Data class
        if (File.Exists(_path + "Data.json"))
        {
            var json = File.ReadAllText(_path + "Data.json");
            GSD = JsonUtility.FromJson<GameSaveData>(json);

            GSD = JsonConvert.DeserializeObject<GameSaveData>(json);
            
            SelectionManager.Instance.SetSelectedDifficulty((EGameDifficulty)GSD.GetGameplaySaveData().GetCurrentDifficultySelected());
        }
        else
        {
            //Sets the initial values
            StartingValues();

            SaveText();
        }
    }

    /// <summary>
    /// Called when the boss dies. Saves any needed information as a result.
    /// </summary>
    public void BossDead()
    {
        SaveBossDifficultyHeroesDictionary();

        UnlockStartingCharacters();

        if (SelectionManager.Instance.IsPlayingMissionsMode())
        {
            MissionComplete();
        }
        
        SaveText();
    }

    /// <summary>
    /// Saves the heroes best difficulty beaten on a boss
    /// </summary>
    public void SaveBossDifficultyHeroesDictionary()
    {
        BossSO tempBoss = SelectionManager.Instance.GetSelectedBoss();
        EGameDifficulty tempDifficulty = SelectionManager.Instance.GetSelectedDifficulty();
        List<HeroSO> tempHeroes = SelectionManager.Instance.GetAllSelectedHeroes();

        //Iterate through the heroes that are in play
        foreach (HeroSO currentTempHero in tempHeroes)
        {
            //If the current difficulty is more than the current best beaten difficulty against this boss
            if ((int)tempDifficulty > (int)GSD.GetGameplaySaveData().GetBossHeroBestDifficulty()[tempBoss.GetBossName()][currentTempHero.GetHeroName()])
            {
                //Save the current difficulty into the best beaten difficulty
                GSD.GetGameplaySaveData().GetBossHeroBestDifficulty()[tempBoss.GetBossName()]
                    [currentTempHero.GetHeroName()] = tempDifficulty;
            }
        }
    }

    private void DifficultyChanged(EGameDifficulty gameDifficulty)
    {
        GSD.GetGameplaySaveData().SetCurrentDifficultySelectedFromEnum(gameDifficulty);
        SaveText();
    }

    /// <summary>
    /// Resets the best difficulties beaten for heroes and bosses
    /// DOESN'T reset the settings the player has changed
    /// </summary>
    public void ResetGameplaySaveData()
    {
        GSD.GetGameplaySaveData().ResetGameplaySaveData();
        
        UnlockStartingUnlocks();
        
        // Resets the best difficulties beaten
        PopulateBossHeroDifficultyDictionary();

        // Saves the changes into the text file
        SaveText();
    }

    #region Character Unlocks

    public void UnlockCharacter(CharacterSO character)
    {
        if (character.IsUnityNull())
        {
            return;
        }
        
        if (character is BossSO bossSO)
        {
            UnlockBoss(bossSO);
        }
        else if (character is HeroSO heroSO)
        {
            UnlockHero(heroSO);
        }
    }

    public void UnlockBoss(BossSO bossSO)
    {
        if (bossSO.IsUnityNull())
        {
            return;
        }
        
        if (!GSD.GetGameplaySaveData().GetBossesUnlocked().Contains(bossSO.GetBossName()))
        {
            GSD.GetGameplaySaveData().GetBossesUnlocked().Add(bossSO.GetBossName());
        }
    }

    public void UnlockHero(HeroSO heroSO)
    {
        if (heroSO.IsUnityNull())
        {
            return;
        }
        
        if (!GSD.GetGameplaySaveData().GetHeroesUnlocked().Contains(heroSO.GetHeroName()))
        {
            GSD.GetGameplaySaveData().GetHeroesUnlocked().Add(heroSO.GetHeroName());
        }
    }
    
    #endregion
    
    #region MissionUnlocks

    public void UnlockMission(MissionSO mission)
    {
        if (!GSD.GetGameplaySaveData().GetMissionsUnlocked().Contains(mission.GetMissionID()))
        {
            GSD.GetGameplaySaveData().GetMissionsUnlocked().Add(mission.GetMissionID());
        }
        
    }

    public void MissionComplete()
    {
        if (!GSD.GetGameplaySaveData().GetMissionsComplete().Contains(SelectionManager.Instance.GetCurrentMission().GetMissionID()))
        {
            GSD.GetGameplaySaveData().GetMissionsComplete().Add(SelectionManager.Instance.GetCurrentMission().GetMissionID());
            
            UnlockCharacter(SelectionManager.Instance.GetCurrentMission().GetCharacterUnlock());

            foreach (MissionSO mission in SelectionManager.Instance.GetCurrentMission().GetMissionUnlocks())
            {
                UnlockMission(mission);
            }
        }
        
        
    }
    #endregion

    #region BaseManager
    /// <summary>
    /// Establishes the Instance for the Save Manager
    /// </summary>
    public override void SetUpInstance()
    {
        base.SetUpInstance();
        Instance = this;
    }
    
    /// <summary>
    /// Performs needed set up for the manager
    /// </summary>
    public override void SetUpMainManager()
    {
        base.SetUpMainManager();  
        EstablishPath();
        Load();
    }

    protected override void SubscribeToEvents()
    {
        base.SubscribeToEvents();
        SelectionManager.Instance.GetDifficultySelectionEvent().AddListener(DifficultyChanged);
    }

    #endregion

    #region Getters

    public MissionSO[] GetMissionsInGame() => _missionsInGame;

    public bool IsBossUnlocked(BossSO bossSO) => GSD.GetGameplaySaveData().GetBossesUnlocked().Contains(bossSO.GetBossName());
    public bool IsHeroUnlocked(HeroSO heroSO) => GSD.GetGameplaySaveData().GetHeroesUnlocked().Contains(heroSO.GetHeroName());
    
    public bool IsMissionUnlocked(MissionSO missionSO) => GSD.GetGameplaySaveData().GetMissionsUnlocked().Contains(missionSO.GetMissionID());
    
    public EGameDifficulty GetBestDifficultyBeatenOnHeroForBoss(BossSO bossSO, HeroSO heroSO)
    {
        return GSD.GetGameplaySaveData().GetBossHeroBestDifficulty()[bossSO.GetBossName()][heroSO.GetHeroName()];
    }

    public float GetScreenShakeIntensity() => GSD.GetGeneralSaveData().GetGSDScreenShakeStrength();

    public bool GetClickAndDragEnabled() => GSD.GetGeneralSaveData().GetGSDHeroClickAndDragEnabled();

    public float GetMasterVolume() => GSD.GetGeneralSaveData().GetGSDMasterVolume();
    public float GetMusicVolume() => GSD.GetGeneralSaveData().GetGSDMusicVolume();
    public float GetSFXVolume() => GSD.GetGeneralSaveData().GetGSDSFXVolume();

    public float GetVolumeFromAudioVCAType(EAudioVCAType audioType)
    {
        switch (audioType)
        {
            case(EAudioVCAType.Master):
                return GSD.GetGeneralSaveData().GetGSDMasterVolume();
            case(EAudioVCAType.Music):
                return GSD.GetGeneralSaveData().GetGSDMusicVolume();
            case(EAudioVCAType.SoundEffect):
                return GSD.GetGeneralSaveData().GetGSDSFXVolume();
            default:
                return 0;
        }
    }

    #endregion

    #region Setters
    /// <summary>
    /// Saves the current screen shake intensity into the game save data
    /// </summary>
    /// <param name="val"></param>
    public void SetScreenShakeStrength(float val)
    {
        GSD.GetGeneralSaveData().SetGSDScreenShakeStrength(val);
        SaveText();
    }

    /// <summary>
    /// Saves the current master audio volume into the game save data
    /// </summary>
    /// <param name="volume"></param>
    public void SetMasterAudioVolume(float volume)
    {
        GSD.GetGeneralSaveData().SetGSDMasterVolume(volume);
        SaveText();
    }

    /// <summary>
    /// Saves the current music audio volume into the game save data
    /// </summary>
    /// <param name="volume"></param>
    public void SetMusicAudioVolume(float volume)
    {
        GSD.GetGeneralSaveData().SetGSDMusicVolume(volume);
        SaveText();
    }

    /// <summary>
    /// Saves the current sfx audio volume into the game save data
    /// </summary>
    /// <param name="volume"></param>
    public void SetSFXAudioVolume(float volume)
    {
        GSD.GetGeneralSaveData().SetGSDSFXVolume(volume);
        SaveText();
    }

    public void SetVolumeFromAudioVCAType(EAudioVCAType audioType, float volume)
    {
        switch (audioType)
        {
            case (EAudioVCAType.Master):
                SetMasterAudioVolume(volume);
                return;
            case (EAudioVCAType.Music):
                SetMusicAudioVolume(volume);
                return;
            case (EAudioVCAType.SoundEffect):
                SetSFXAudioVolume(volume);
                return;
            default:
                return;
        }
    }

    /// <summary>
    /// Saves all settings at once
    /// </summary>
    /// <param name="screenShake"></param>
    /// <param name="clickDrag"></param>
    /// <param name="masterVol"></param>
    /// <param name="musicVol"></param>
    /// <param name="sfxVol"></param>
    public void SaveSettingsOptions(float screenShake, bool clickDrag, float masterVol, float musicVol, float sfxVol)
    {
        GSD.GetGeneralSaveData().SetGSDScreenShakeStrength(screenShake);
        GSD.GetGeneralSaveData().SetGSDHeroClickAndDrag(clickDrag);

        GSD.GetGeneralSaveData().SetGSDMasterVolume(masterVol);
        GSD.GetGeneralSaveData().SetGSDMusicVolume(musicVol);
        GSD.GetGeneralSaveData().SetGSDSFXVolume(sfxVol);

        SaveText();
    }
    #endregion
}



/// <summary>
/// The data that is saved
/// </summary>
[System.Serializable]
public class GameSaveData
{
    public GameplaySaveData _storedGameplaySaveData;
    
    public GeneralSaveData _storedGeneralSaveData;

    public void ResetGameSaveData()
    {
        _storedGameplaySaveData = new();
        _storedGeneralSaveData = new();
    }
    
    #region Getters
    public GameplaySaveData GetGameplaySaveData() => _storedGameplaySaveData;
    
    public GeneralSaveData GetGeneralSaveData() => _storedGeneralSaveData;
    #endregion
}



[System.Serializable]
public class GameplaySaveData
{
    public HashSet<string> BossesUnlocked = new();
    public HashSet<string> HeroesUnlocked = new();
    
    public HashSet<int> MissionsUnlocked = new();
    public HashSet<int> MissionsComplete = new();
    
    //First string is boss name, second string is hero name
    //Represents the best difficulty each hero has beaten each boss at
    public Dictionary<string, Dictionary<string,EGameDifficulty>> BossHeroBestDifficultyComplete = new();
    
    public int CurrentDifficultySelected = 1;
    
    public void ResetGameplaySaveData()
    {
        BossesUnlocked = new();
        HeroesUnlocked = new();
        
        MissionsComplete = new();
        
        BossHeroBestDifficultyComplete = new();
    }
    
    #region Getters
    public HashSet<string> GetBossesUnlocked() => BossesUnlocked;
    public HashSet<string> GetHeroesUnlocked() => HeroesUnlocked;
    
    public HashSet<int> GetMissionsUnlocked() => MissionsUnlocked;
    public HashSet<int> GetMissionsComplete() => MissionsComplete;
    
    public Dictionary<string, Dictionary<string, EGameDifficulty>> GetBossHeroBestDifficulty() => BossHeroBestDifficultyComplete;
    
    public int GetCurrentDifficultySelected() => CurrentDifficultySelected;
    #endregion
    
    #region Setters
    
    public void SetCurrentDifficultySelectedFromEnum(EGameDifficulty difficulty)
    {
        CurrentDifficultySelected = (int)difficulty;
    }

    public void ResetBossHeroDifficulties()
    {
        BossHeroBestDifficultyComplete = new();
    }
    #endregion
}



[System.Serializable]
public class GeneralSaveData
{
    public string GameVersion; 
    
    [Space]
    [Header("Settings")]
    public float ScreenShakeStrength = 1;
    private bool HeroClickAndDragMovementEnabled;

    [Range(0, 1)] public float MasterVolume = .5f;
    [Range(0, 1)] public float MusicVolume = .5f;
    [Range(0, 1)] public float SfxVolume = .5f;
    
    #region Getters
    public string GetGameVersion() => GameVersion;
    
    public float GetGSDScreenShakeStrength() => ScreenShakeStrength;
    public bool GetGSDHeroClickAndDragEnabled() => HeroClickAndDragMovementEnabled;

    public float GetGSDMasterVolume() => MasterVolume;
    public float GetGSDMusicVolume() => MusicVolume;
    public float GetGSDSFXVolume() => SfxVolume;
    
    #endregion
    
    #region Setters

    public void SetGameVersion(string gameVersion)
    {
        GameVersion = gameVersion;
    }
    
    public void SetGSDScreenShakeStrength(float screenShake)
    {
        ScreenShakeStrength = screenShake;
    }

    public void SetGSDHeroClickAndDrag(bool clickDrag)
    {
        HeroClickAndDragMovementEnabled = clickDrag;
    }

    public void SetGSDMasterVolume(float volume)
    {
        MasterVolume = volume;
    }
    public void SetGSDMusicVolume(float volume)
    {
        MusicVolume = volume;
    }

    public void SetGSDSFXVolume(float volume)
    {
        SfxVolume = volume;
    }

    #endregion
}
