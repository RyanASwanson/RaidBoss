using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameplayModifiersManager : MainGameplayManagerFramework
{
    public static GameplayModifiersManager Instance;

    private List<MissionModifierBase> _missionModifiers;

    
    private void CreateModifiers()
    {
        _missionModifiers = new List<MissionModifierBase>();

        for (int i = 0; SelectionManager.Instance.GetMissionModifiers().Count > i; i++)
        {
            if (SelectionManager.Instance.GetMissionModifiers()[i].GetMissionModifierObject().IsUnityNull())
            {
                continue;
            }
            
            GameObject modifierObject = Instantiate(SelectionManager.Instance.GetMissionModifiers()[i].GetMissionModifierObject(), transform);

            MissionModifierBase[] missionModifierScripts = modifierObject.GetComponents<MissionModifierBase>();

            foreach (MissionModifierBase missionModifierScript in missionModifierScripts)
            {
                missionModifierScript.SetUpMissionModifier();
                _missionModifiers.Add(missionModifierScript);
            }
        }
    }

    public void AdjustBossStatsFromModifiers(BossStats bossStats)
    {
        foreach (MissionModifierBase missionModifier in _missionModifiers)
        {
            missionModifier.AdjustBossStatsModifier(bossStats);
        }
    }

    public void AdjustHeroStatsFromModifiers(HeroStats heroStats)
    {
        foreach (MissionModifierBase missionModifier in _missionModifiers)
        {
            missionModifier.AdjustHeroStatsModifier(heroStats);
        }
    }
    
    #region BaseManager
    /// <summary>
    /// Establishes the Instance for the GameStateManager
    /// </summary>
    public override void SetUpInstance()
    {
        base.SetUpInstance();
        Instance = this;
    }
    public override void SetUpMainManager()
    {
        base.SetUpMainManager();

        CreateModifiers();
    }
    #endregion
}
