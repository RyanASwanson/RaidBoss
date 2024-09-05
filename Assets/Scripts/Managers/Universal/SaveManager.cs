using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;

public class SaveManager : BaseUniversalManager
{
    public GameSaveData GSD;
    private string _path;

    [SerializeField] private List<BossSO> _bossesInGame = new();
    [SerializeField] private List<HeroSO> _heroesInGame = new();

    [Space]

    [SerializeField] private List<HeroSO> _heroesStartingUnlocked;


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
    /// Fills the save data with it's initial values
    /// </summary>
    private void StartingValues()
    {
        //Fills the GSD with default values when the file is created
        PopulateBossHeroDifficultyDictionary();

        GSD.SetGSDScreenShakeStrength(1);
        GSD.SetGSDHeroClickAndDrag(false);

        GSD.SetGSDMasterVolume(.5f);
        GSD.SetGSDMusicVolume(.5f);
        GSD.SetGSDSFXVolume(.5f);
    }

    private void PopulateBossHeroDifficultyDictionary()
    {
        //Reset the dictionary
        GSD._bossHeroBestDifficultyComplete = new();

        //Iterate through the boss scriptable objects
        foreach(BossSO bossSO in _bossesInGame)
        {

            GSD._bossHeroBestDifficultyComplete.Add(bossSO.GetBossName(), new Dictionary<string, GameDifficulty>());

            foreach(HeroSO heroSO in _heroesInGame)
            {
                //Sets each best difficulty beaten to empty
                GSD._bossHeroBestDifficultyComplete[bossSO.GetBossName()].Add(heroSO.GetHeroName(), GameDifficulty.Empty);
            }
        }
    }

    public void SaveText()
    {
        //Writes all variables in the Game Save Data class into Json
        var convertedJson = JsonConvert.SerializeObject(GSD);
        //var convertedJson = JsonUtility.ToJson(GSD);
        File.WriteAllText(_path + "Data.json", convertedJson);
    }

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
    /// Saves the heroes best difficulty beaten on a boss
    /// </summary>
    public void SaveBossDifficultyHeroesDictionary()
    {
        SelectionManager tempSelectionManager = UniversalManagers.Instance.GetSelectionManager();
        BossSO tempBoss = tempSelectionManager.GetSelectedBoss();
        GameDifficulty tempDifficulty = tempSelectionManager.GetSelectedDifficulty();
        List<HeroSO> tempHeroes = tempSelectionManager.GetAllSelectedHeroes();

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

        //Saves the changes to the save file
        SaveText();
    }

    /// <summary>
    /// Resets the best difficulties beaten for heroes and bosses
    /// DOESN'T reset the settings the player has changed
    /// </summary>
    public void ResetSaveData()
    {
        //Resets the best difficulties beaten
        PopulateBossHeroDifficultyDictionary();
        
        //Saves the changes into the text file
        SaveText();
    }

    #region BaseManager
    public override void SetupManager()
    {
        base.SetupManager();
        EstablishPath();
        Load();
    }
    #endregion

    #region Getters

    public GameDifficulty GetBestDifficultyBeatenOnHeroForBoss(BossSO bossSO, HeroSO heroSO)
    {
        return GSD._bossHeroBestDifficultyComplete[bossSO.GetBossName()][heroSO.GetHeroName()];
    }

    public float GetScreenShakeIntensity() => GSD.GetGSDScreenShakeStrength();

    public bool GetClickAndDragEnabled() => GSD.GetGSDHeroClickAndDragEnabled();

    public float GetMasterVolume() => GSD.GetGSDMasterVolume();
    public float GetMusicVolume() => GSD.GetGSDMusicVolume();
    public float GetSFXVolume() => GSD.GetGSDSFXVolume();

    #endregion

    #region Setters


    /// <summary>
    /// Saves the current screen shake intensity into the game save data
    /// </summary>
    /// <param name="val"></param>
    public void SetScreenShakeStrength(float val)
    {
        GSD.SetGSDScreenShakeStrength(val);
    }

    /// <summary>
    /// Saves the current master audio volume into the game save data
    /// </summary>
    /// <param name="volume"></param>
    public void SetMasterAudioVolume(float volume)
    {
        GSD.SetGSDMasterVolume(volume);
    }

    /// <summary>
    /// Saves the current music audio volume into the game save data
    /// </summary>
    /// <param name="volume"></param>
    public void SetMusicAudioVolume(float volume)
    {
        GSD.SetGSDMusicVolume(volume);
    }

    /// <summary>
    /// Saves the current sfx audio volume into the game save data
    /// </summary>
    /// <param name="volume"></param>
    public void SetSFXAudioVolume(float volume)
    {
        GSD.SetGSDSFXVolume(volume);
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

[System.Serializable]
public class GameSaveData
{
    //String is hero name
    public Dictionary<string, bool> _heroesUnlocked = new();
    //First string is boss name, second string is hero name
    //Represents the best difficulty each hero has beaten each boss at
    public Dictionary<string, Dictionary<string,GameDifficulty>> _bossHeroBestDifficultyComplete = new();

    [Space]
    [Header("Settings")]
    [Range(0, 1)] private float _screenShakeStrength = 1;
    private bool _heroClickAndDragMovementEnabled;

    [Range(0, 1)] private float _masterVolume = .5f;
    [Range(0, 1)] private float _musicVolume = .5f;
    [Range(0, 1)] private float _sfxVolume = .5f;


    #region Getters
    public Dictionary<string, Dictionary<string, GameDifficulty>> GetGSDBossHeroBestDifficulty() => _bossHeroBestDifficultyComplete;

    public float GetGSDScreenShakeStrength() => _screenShakeStrength;
    public bool GetGSDHeroClickAndDragEnabled() => _heroClickAndDragMovementEnabled;

    public float GetGSDMasterVolume() => _masterVolume;
    public float GetGSDMusicVolume() => _musicVolume;
    public float GetGSDSFXVolume() => _sfxVolume;

    #endregion

    #region Setters
    public void SetGSDScreenShakeStrength(float screenShake)
    {
        _screenShakeStrength = screenShake;
    }

    public void SetGSDHeroClickAndDrag(bool clickDrag)
    {
        _heroClickAndDragMovementEnabled = clickDrag;
    }

    public void SetGSDMasterVolume(float volume)
    {
        _masterVolume = volume;
    }
    public void SetGSDMusicVolume(float volume)
    {
        _musicVolume = volume;
    }

    public void SetGSDSFXVolume(float volume)
    {
        _sfxVolume = volume;
    }

    #endregion
}
