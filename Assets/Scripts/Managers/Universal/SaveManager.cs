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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            PopulateBossHeroDifficultyDictionary();
            foreach (BossSO bossSO in _bossesInGame)
            {
                foreach (HeroSO heroSO in _heroesInGame)
                {
                    print(GSD._bossHeroBestDifficultyComplete[bossSO.GetBossName()][heroSO.GetHeroName()]);
                }
            }
        }
    }

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

        GSD._screenShakeStrength = 1;

        GSD._masterVolume = 1;
        GSD._musicVolume = 1;
        GSD._sfxVolume = 1;

        /*System.Array.Fill(GSD.SaveNames, "");
        System.Array.Fill(GSD.SaveScore, 0);*/
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
            /*var a = JsonConvert.DeserializeObject<GameSaveData>(json);
            Debug.Log(a._screenShakeStrength);*/
        }
        else
        {
            //Sets the initial values
            StartingValues();

            SaveText();
        }
    }

    public void ResetSaveData()
    {
        PopulateBossHeroDifficultyDictionary();
        
        SaveText();
    }

    #region Getters

    public float GetScreenShakeIntensity() => GSD._screenShakeStrength;
    public float GetMasterVolume() => GSD._masterVolume;
    public float GetMusicVolume() => GSD._musicVolume;
    public float GetSFXVolume() => GSD._sfxVolume;

    #endregion

    #region Setters
    public void SaveBossDifficultyHeroesDictionary()
    {
        SelectionManager tempSelectionManager = UniversalManagers.Instance.GetSelectionManager();
        BossSO tempBoss = tempSelectionManager.GetSelectedBoss();
        GameDifficulty tempDifficulty = tempSelectionManager.GetSelectedDifficulty();
        List<HeroSO> tempHeroes = tempSelectionManager.GetAllSelectedHeroes();

        foreach (HeroSO currentTempHero in tempHeroes)
        {
            if ((int)tempDifficulty > (int)GSD._bossHeroBestDifficultyComplete[tempBoss.GetBossName()][currentTempHero.GetHeroName()])
            {
                GSD._bossHeroBestDifficultyComplete[tempBoss.GetBossName()][currentTempHero.GetHeroName()] = tempDifficulty;
            }
        }

        SaveText();
    }

    public void SetScreenShakeIntensity(float val)
    {
        GSD._screenShakeStrength = val;
    }

    public void SetMasterAudioVolume(float volume)
    {
        GSD._masterVolume = volume;
    }
    public void SetMusicAudioVolume(float volume)
    {
        GSD._musicVolume = volume;
    }
    public void SetSFXAudioVolume(float volume)
    {
        GSD._sfxVolume = volume;
    }
    #endregion

    public override void SetupUniversalManager()
    {
        base.SetupUniversalManager();
        EstablishPath();
        //PopulateBossHeroDifficultyDictionary();
        Load();
    }

    public override void SubscribeToEvents()
    {

    }
}

[System.Serializable]
public class GameSaveData
{
    //First string is boss name, second string is hero name
    public Dictionary<string, Dictionary<string,GameDifficulty>> _bossHeroBestDifficultyComplete = new();

    [Space]
    [Header("Settings")]
    [Range(0, 1)] public float _screenShakeStrength;

    [Range(0, 1)] public float _masterVolume;
    [Range(0, 1)] public float _musicVolume;
    [Range(0, 1)] public float _sfxVolume;
}
