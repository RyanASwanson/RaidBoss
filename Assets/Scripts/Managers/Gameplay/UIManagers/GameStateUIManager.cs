using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Handles the ui of the game state in gamepaly such as win/lose
/// </summary>
public class GameStateUIManager : GameUIChildrenFunctionality
{
    [Header("Game Conclusion")]
    [SerializeField] private GameObject _winUI;
    [Space]
    [SerializeField] private GameObject _loseUI;

    private void BattleWinUI()
    {
        _winUI.SetActive(true);
    }
    
    private void BattleLoseUI()
    {
        _loseUI.SetActive(true);
    }

    public override void ChildFuncSetup()
    {
        base.ChildFuncSetup();
    }

    public override void SubscribeToEvents()
    {
        GameplayManagers.Instance.GetGameStateManager().GetBattleWonEvent().AddListener(BattleWinUI);
        GameplayManagers.Instance.GetGameStateManager().GetBattleLostEvent().AddListener(BattleLoseUI);
    }

    #region Getters

    #endregion
}
