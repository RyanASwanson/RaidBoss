using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlsManager : MainUniversalManagerFramework
{
    public static ControlsManager Instance;

    [SerializeField] private string[] _defaultUseSpecificHeroAbilityInputs;
    
    #region BaseManager
    public override void SetUpInstance()
    {
        base.SetUpInstance();
        Instance = this;
    }
    #endregion
    
    #region Getters
    public string[] GetDefaultUseSpecificHeroAbilityInputs() => _defaultUseSpecificHeroAbilityInputs;
    #endregion
}
