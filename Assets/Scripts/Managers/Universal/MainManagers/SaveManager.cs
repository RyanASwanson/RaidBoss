using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using UnityEngine.Serialization;

public class SaveManager : MainUniversalManagerFramework
{
    public static SaveManager Instance;
    
    public GameSaveData GSD {get; set;}
    private string _path;

    [SerializeField] private List<BossSO> _bossesInGame = new();
    [SerializeField] private List<HeroSO> _heroesInGame = new();

    [Space]
    [SerializeField] private List<BossSO> _bossesStartingUnlocked = new();
    [SerializeField] private List<HeroSO> _heroesStartingUnlocked = new();

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
        
        //Fills the GSD with default values when the file is created
        PopulateUnlockedBosses();
        PopulateUnlockedHeroes();
        PopulateBossHeroDifficultyDictionary();

        GSD.SetGSDScreenShakeStrength(1);
        GSD.SetGSDHeroClickAndDrag(false);

        GSD.SetGSDMasterVolume(.5f);
        GSD.SetGSDMusicVolume(.5f);
        GSD.SetGSDSFXVolume(.5f);
    }

    /// <summary>
    /// Adds the starting bosses which are unlocked on new save data
    /// </summary>
    private void PopulateUnlockedBosses()
    {
        // Iterate through each boss
        foreach (BossSO bossSO in _bossesInGame)
        {
            // Add them to a dictionary with a bool for if they are unlocked or not
            GSD._bossesUnlocked.Add(bossSO.GetBossName(), _bossesStartingUnlocked.Contains(bossSO));
        }
    }

    /// <summary>
    /// Adds the starting heroes which are unlocked on new save data
    /// </summary>
    private void PopulateUnlockedHeroes()
    {
        // Iterate through each hero
        foreach(HeroSO heroSO in _heroesInGame)
        {
            // Add them to a dictionary with a bool for if they are unlocked or not
            GSD._heroesUnlocked.Add(heroSO.GetHeroName(), _heroesStartingUnlocked.Contains(heroSO));
        }
    }

    /// <summary>
    /// Populates the dictionary for what heroes have beaten what bosses on what difficulty
    /// </summary>
    private void PopulateBossHeroDifficultyDictionary()
    {
        //Reset the dictionary
        GSD._bossHeroBestDifficultyComplete = new();

        //Iterate through the boss scriptable objects
        foreach(BossSO bossSO in _bossesInGame)
        {
            // Adds the boss to the dictionary
            GSD._bossHeroBestDifficultyComplete.Add(bossSO.GetBossName(), new Dictionary<string, EGameDifficulty>());

            foreach(HeroSO heroSO in _heroesInGame)
            {
                //Sets each best difficulty beaten to empty
                GSD._bossHeroBestDifficultyComplete[bossSO.GetBossName()].Add(heroSO.GetHeroName(), EGameDifficulty.Empty);
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
        UnlockNextBoss();
        UnlockNextHero();
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
            if ((int)tempDifficulty > (int)GSD._bossHeroBestDifficultyComplete[tempBoss.GetBossName()][currentTempHero.GetHeroName()])
            {
                //Save the current difficulty into the best beaten difficulty
                GSD._bossHeroBestDifficultyComplete[tempBoss.GetBossName()]
                    [currentTempHero.GetHeroName()] = tempDifficulty;
            }
        }
    }

    /// <summary>
    /// Resets the best difficulties beaten for heroes and bosses
    /// DOESN'T reset the settings the player has changed
    /// </summary>
    public void ResetSaveData()
    {
        // Resets the best difficulties beaten
        PopulateBossHeroDifficultyDictionary();

        // Saves the changes into the text file
        SaveText();
    }

    #region Character Unlocks
    #region Boss Unlocks
    /// <summary>
    /// Unlocks the next boss
    /// </summary>
    private void UnlockNextBoss()
    {
        if (GSD._bossesUnlocked[_bossesInGame[_bossesInGame.Count - 1].GetBossName()])
            return;

        // Iterate through each boss
        foreach (BossSO bossSO in _bossesInGame)
        {
            // Check if the boss hasn't been unlocked yet
            if (!GSD._bossesUnlocked[bossSO.GetBossName()])
            {
                // Unlock the boss
                GSD._bossesUnlocked[bossSO.GetBossName()] = true;
                break;
            }
        }
    }
    #endregion

    #region Hero Unlocks
    /// <summary>
    /// Unlocks the next hero
    /// </summary>
    private void UnlockNextHero()
    {
        if (GSD._heroesUnlocked[_heroesInGame[_heroesInGame.Count-1].GetHeroName()])
                return;

        // Iterate through each hero
        foreach(HeroSO heroSO in _heroesInGame)
        {
            // Check if the hero hasn't been unlocked yet
            if (!GSD._heroesUnlocked[heroSO.GetHeroName()])
            {
                // Unlock the hero
                GSD._heroesUnlocked[heroSO.GetHeroName()] = true;
                break;
            }
        }
    }
    #endregion
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
    #endregion

    #region Getters

    public EGameDifficulty GetBestDifficultyBeatenOnHeroForBoss(BossSO bossSO, HeroSO heroSO)
    {
        return GSD._bossHeroBestDifficultyComplete[bossSO.GetBossName()][heroSO.GetHeroName()];
    }

    public float GetScreenShakeIntensity() => GSD.GetGSDScreenShakeStrength();

    public bool GetClickAndDragEnabled() => GSD.GetGSDHeroClickAndDragEnabled();

    public float GetMasterVolume() => GSD.GetGSDMasterVolume();
    public float GetMusicVolume() => GSD.GetGSDMusicVolume();
    public float GetSFXVolume() => GSD.GetGSDSFXVolume();

    public float GetVolumeFromAudioVCAType(EAudioVCAType audioType)
    {
        switch (audioType)
        {
            case(EAudioVCAType.Master):
                return GSD.GetGSDMasterVolume();
            case(EAudioVCAType.Music):
                return GSD.GetGSDMusicVolume();
            case(EAudioVCAType.SoundEffect):
                return GSD.GetGSDSFXVolume();
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
        GSD.SetGSDScreenShakeStrength(val);
        SaveText();
    }

    /// <summary>
    /// Saves the current master audio volume into the game save data
    /// </summary>
    /// <param name="volume"></param>
    public void SetMasterAudioVolume(float volume)
    {
        GSD.SetGSDMasterVolume(volume);
        SaveText();
    }

    /// <summary>
    /// Saves the current music audio volume into the game save data
    /// </summary>
    /// <param name="volume"></param>
    public void SetMusicAudioVolume(float volume)
    {
        GSD.SetGSDMusicVolume(volume);
        SaveText();
    }

    /// <summary>
    /// Saves the current sfx audio volume into the game save data
    /// </summary>
    /// <param name="volume"></param>
    public void SetSFXAudioVolume(float volume)
    {
        GSD.SetGSDSFXVolume(volume);
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
        GSD.SetGSDScreenShakeStrength(screenShake);
        GSD.SetGSDHeroClickAndDrag(clickDrag);

        GSD.SetGSDMasterVolume(masterVol);
        GSD.SetGSDMusicVolume(musicVol);
        GSD.SetGSDSFXVolume(sfxVol);

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
    //TODO Seperate into class for game data and settings data
    public Dictionary<string, bool> _bossesUnlocked = new();
    //String is hero name
    public Dictionary<string, bool> _heroesUnlocked = new();
    //First string is boss name, second string is hero name
    //Represents the best difficulty each hero has beaten each boss at
    public Dictionary<string, Dictionary<string,EGameDifficulty>> _bossHeroBestDifficultyComplete = new();
    
    [Space]
    [Header("Settings")]
    [Range(0, 1)] public float ScreenShakeStrength = 1;
    private bool HeroClickAndDragMovementEnabled;

    [Range(0, 1)] public float MasterVolume = .5f;
    [Range(0, 1)] public float MusicVolume = .5f;
    [Range(0, 1)] public float SfxVolume = .5f;
    
    #region Getters
    public Dictionary<string, Dictionary<string, EGameDifficulty>> GetGSDBossHeroBestDifficulty() => _bossHeroBestDifficultyComplete;

    public float GetGSDScreenShakeStrength() => ScreenShakeStrength;
    public bool GetGSDHeroClickAndDragEnabled() => HeroClickAndDragMovementEnabled;

    public float GetGSDMasterVolume() => MasterVolume;
    public float GetGSDMusicVolume() => MusicVolume;
    public float GetGSDSFXVolume() => SfxVolume;

    #endregion

    #region Setters
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
