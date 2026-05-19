using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EngineSettingsManager : MainUniversalManagerFramework
{
    public static EngineSettingsManager Instance;
    
    [Header("Cursor")]
    [SerializeField] private CursorData _defaultCursor;

    [Space] 
    [SerializeField] private Vector2Int[] _gameResolutions;
    
    #region Cursor

    private void SetCursorToDefault()
    {
        SetCursorTexture(_defaultCursor);
    }
    
    private void SetCursorTexture(CursorData cursor)
    {
        Cursor.SetCursor(cursor.CursorTexture, cursor.CursorHotspot, cursor.CursorMode);
    }
    #endregion
    
    #region Resolution

    private void SetResolutionToDefault()
    {
        Vector2Int currentResolution = new Vector2Int(Screen.currentResolution.width, Screen.currentResolution.height);
        foreach (Vector2Int resolution in _gameResolutions)
        {
            if (resolution == currentResolution)
            {
                return;
            }
        }
        
        // If we did not find a matching resolution find the closest option
        
        float currentRatio = (float)Screen.currentResolution.width / (float)Screen.currentResolution.height;
        float checkRatio;
        float closestRatio = float.MaxValue;
        int closestRationIndex = 0;

        for (int i = 0; i < _gameResolutions.Length; i++)
        {
            checkRatio = (float)_gameResolutions[i].x / (float)_gameResolutions[i].y;
            checkRatio = Mathf.Abs(currentRatio - checkRatio);
            if (checkRatio <= closestRatio)
            {
                closestRatio = checkRatio;
                closestRationIndex = i;
            }
        }
        
        //Debug.Log("Closest resolution to " + currentRatio + " is " + _gameResolutions[closestRationIndex] + " at " + closestRationIndex);
        SetScreenResolution(_gameResolutions[closestRationIndex]);
    }

    public void SetScreenResolutionFromArray(int resolutionID)
    {
        SetScreenResolution(_gameResolutions[resolutionID]);
    }
    
    public void SetScreenResolution(Vector2Int resolution)
    {
        Debug.Log("Setting Resolution To " + resolution.x + " " + resolution.y);
        Screen.SetResolution(resolution.x, resolution.y, Screen.fullScreen);
    }
    
    #endregion
    
    #region BaseManager
    public override void SetUpInstance()
    {
        base.SetUpInstance();
        Instance = this;
    }

    public override void SetUpMainManager()
    {
        base.SetUpMainManager();
        SetCursorToDefault();
        SetResolutionToDefault();
    }
    #endregion
    
    #region Getters
    public Vector2Int GetCurrentResolution() => new Vector2Int(Screen.currentResolution.width, Screen.currentResolution.height);
    public Vector2Int[] GetGameResolutions() => _gameResolutions;

    public int GetResolutionIDFromCurrentResolution() => GetResolutionIDFromResolution(GetCurrentResolution());
    public int GetResolutionIDFromResolution(Vector2Int resolution)
    {
        for (int i = 0; i < _gameResolutions.Length; i++)
        {
            if (resolution == _gameResolutions[i])
            {
                Debug.Log("Found " + resolution + " equals " + _gameResolutions[i]);
                return i;
            }
        }
        return -1;
    }
    #endregion
}

[System.Serializable]
public class CursorData
{
    public Texture2D CursorTexture;
    public Vector2 CursorHotspot;
    public CursorMode CursorMode;
}