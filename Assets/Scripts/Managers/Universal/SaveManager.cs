using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SaveManager : BaseUniversalManager
{
    public GameSaveData GSD;
    private string _path;

    void Awake()
    {
        EstablishPath();
        Load();
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
        //Fills the arrays with default values when the file is created
        GSD.a = 1;
        GSD.b = true;

        /*System.Array.Fill(GSD.SaveNames, "");
        System.Array.Fill(GSD.SaveScore, 0);*/
    }

    public void SaveText()
    {
        //Writes all variables in the Game Save Data class into Json
        var convertedJson = JsonUtility.ToJson(GSD);
        File.WriteAllText(_path + "Data.json", convertedJson);
    }

    public void Load()
    {
        //Loads all variables in Json into the Game Save Data class
        if (File.Exists(_path + "Data.json"))
        {
            var json = File.ReadAllText(_path + "Data.json");
            GSD = JsonUtility.FromJson<GameSaveData>(json);
            print(GSD.b);
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
        GSD = new GameSaveData();
        StartingValues();
        SaveText();
    }

    public override void SetupUniversalManager()
    {
        base.SetupUniversalManager();
    }

    public override void SubscribeToEvents()
    {

    }
}

[System.Serializable]
public class GameSaveData
{
    public int a;

    [Space]
    public bool b;
}
