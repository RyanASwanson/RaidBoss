using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EngineSettingsManager : MainUniversalManagerFramework
{
    public static EngineSettingsManager Instance;
    
    [Header("Cursor")]
    [SerializeField] private CursorData _defaultCursor;
    
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