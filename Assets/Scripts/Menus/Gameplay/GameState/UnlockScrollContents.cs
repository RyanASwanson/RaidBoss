using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UnlockScrollContents : ScrollUIContents
{
    [Space]
    [SerializeField] private GameObject _bossUnlockSection;
    [SerializeField] private Image _bossIconImage;
    
    [Space]
    [SerializeField] private GameObject _heroUnlockSection;
    [SerializeField] private Image _heroIconImage;

    [Space]
    [SerializeField] private GameObject _difficultyUnlockSection;
    [SerializeField] private Image _difficultyIconImage;
    [SerializeField] private TextWithBackground _difficultyMythicPlusLevelText;
    
    [Space]
    [SerializeField] private GameObject _missionModifierUnlockSection;
    [SerializeField] private Image _missionModifierIconImage;
    
    [Space]
    [SerializeField] private GameObject _freePlayUnlockedSection;


    public override int UpdateContentsAndCountLines()
    {
        if (SelectionManager.Instance.IsPlayingMissionsMode())
        {
            ShowMissionModeUnlocks();
        }
        else if (SelectionManager.Instance.IsPlayingFreeMode())
        {
            ShowFreePlayModeUnlocks();
        }
        
        return TotalLines;
    }

    private void ShowMissionModeUnlocks()
    {
        SelectionManager.Instance.GetSelectedMissionOut(out MissionSO mission);
        
        CharacterSO character = mission.GetCharacterUnlock();
        
        if (!character.IsUnityNull())
        {
            if (character is BossSO bossSO)
            {
                ShowBossUnlockUI(bossSO);
            }
            else if (character is HeroSO heroSO)
            {
                ShowHeroUnlockUI(heroSO);
            }
        }
        else if (mission.GetIsDifficultyUnlockNotEmpty())
        {
            ShowDifficultyUnlockUI(mission.GetDifficultyUnlock(), 0);
        }
        else if (!mission.GetMissionModifierUnlock().IsUnityNull())
        {
            ShowMissionModifierUnlockUI(mission.GetMissionModifierUnlock());
        }
        
        if (mission.GetHasGeneralMissionUnlock())
        {
            switch (mission.GetGeneralMissionUnlocks())
            {
                case EGeneralMissionUnlocks.FreePlay:
                    ShowFreePlayUnlockUI();
                    break;
            }
        }
    }

    private void ShowFreePlayModeUnlocks()
    {
        if (SelectionManager.Instance.GetSelectedDifficulty() == EGameDifficulty.MythicPlus)
        {
            ShowDifficultyUnlockUI(SelectionManager.Instance.GetSelectedDifficulty(), SelectionManager.Instance.GetMythicPlusLevel()+1);
        }
    }
    
    private void ShowBossUnlockUI(BossSO bossSO)
    {
        _bossUnlockSection.SetActive(true);
        _bossIconImage.sprite = bossSO.GetBossIcon();
    }

    private void ShowHeroUnlockUI(HeroSO heroSO)
    {
        _heroUnlockSection.SetActive(true);
        _heroIconImage.sprite = heroSO.GetHeroIcon();
    }

    private void ShowDifficultyUnlockUI(EGameDifficulty difficulty, int mythicPlusLevel)
    {
        _difficultyUnlockSection.SetActive(true);
        _difficultyIconImage.sprite = SelectionManager.Instance.GetDifficultyIcons()[(int)difficulty-1];

        if (GameStateManager.Instance.GetIsCurrentBattleAtHighestMythicPlusLevel() && mythicPlusLevel > 0)
        {
            _difficultyMythicPlusLevelText.UpdateText(mythicPlusLevel.ToString());
        }
    }

    private void ShowMissionModifierUnlockUI(MissionModifierSO missionModifierSO)
    {
        _missionModifierUnlockSection.SetActive(true);
        _missionModifierIconImage.sprite = missionModifierSO.GetModifierSprite();
    }

    private void ShowFreePlayUnlockUI()
    {
        _freePlayUnlockedSection.SetActive(true);
    }
}
