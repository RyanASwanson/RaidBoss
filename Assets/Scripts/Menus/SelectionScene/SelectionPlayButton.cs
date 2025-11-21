using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectionPlayButton : MonoBehaviour
{
    [SerializeField] private Image _bossSelectedIcon;
    [SerializeField] private Image[] _heroSelectedIcons;
    [SerializeField] private CurveProgression _curveScaleProgression;

    private Button _button;
    
    public void SetUpPlayButton()
    {
        _button = GetComponent<Button>();
        ToggleInteractability(false);
        SubscribeToEvents();
    }
    
    /// <summary>
    /// Causes the game to proceed to the currently selected level
    /// Called by play button press
    /// </summary>
    public void PlayLevel()
    {
        SceneLoadManager.Instance.LoadCurrentlySelectedLevelSO();
    }

    public void MaxCharactersSelected(bool areMaxCharactersSelected)
    {
        ToggleInteractability(areMaxCharactersSelected);
        if (areMaxCharactersSelected)
        {
            _curveScaleProgression.StartMovingUpOnCurve();
        }
    }

    public void ToggleInteractability(bool isInteractable)
    {
        _button.interactable = isInteractable;
    }

    private void UpdateBossSelectionIcon()
    {
        _bossSelectedIcon.enabled = SelectionManager.Instance.AtMaxBossSelected();
        if (SelectionManager.Instance.AtMaxBossSelected())
        {
            _bossSelectedIcon.sprite = SelectionManager.Instance.GetSelectedBoss().GetBossSelectionIcon();
        }
        
    }

    private void UpdateHeroSelectionIcons()
    {
        List<HeroSO> selectedHeroes = SelectionManager.Instance.GetAllSelectedHeroes();
        int heroTotal = selectedHeroes.Count;

        for (int i = 0; i < selectedHeroes.Count; i++)
        {
            _heroSelectedIcons[i].enabled = true;
            _heroSelectedIcons[i].color = selectedHeroes[i].GetHeroUIColor();
            
        }

        for (int j = heroTotal; j < _heroSelectedIcons.Length; j++)
        {
            _heroSelectedIcons[j].enabled = false;
        }
    }

    private void SubscribeToEvents()
    {
        SelectionManager.Instance.GetBossSelectionChangedEvent().AddListener(UpdateBossSelectionIcon);
        
        SelectionManager.Instance.GetHeroesSelectionChangedEvent().AddListener(UpdateHeroSelectionIcons);
    }
}
