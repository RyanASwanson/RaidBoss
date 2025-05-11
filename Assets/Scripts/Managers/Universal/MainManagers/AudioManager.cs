using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// Contains the functionality for audio in the game
/// </summary>
public class AudioManager : MainUniversalManagerFramework
{
    public static AudioManager Instance;

    #region AudioReferences
    [Header("Test Audio")]
    public SpecificAudio TestAudio;
    
    [Space]
    [Header("Boss Audio")]
    public SpecificBossAudio[] AllBossAudio;
    
    [Space]
    [Header("Hero Audio")]
    public SpecificHeroAudio[] AllHeroAudio;
    #endregion

    
    #region BaseManager
    /// <summary>
    /// Establishes the Instance of the Audio Manager
    /// </summary>
    public override void SetUpInstance()
    {
        base.SetUpInstance();
        Instance = this;
    }
    #endregion
}
