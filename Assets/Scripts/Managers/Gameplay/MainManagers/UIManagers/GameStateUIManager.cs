using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles the ui of the game state in gameplay such as win/lose
/// </summary>
public class GameStateUIManager : GameUIChildrenFunctionality
{
    public static GameStateUIManager Instance;
    
    [Header("Game Conclusion")]
    [SerializeField] private GameObject _winUI;
    [SerializeField] private float _winUIDelay;
    [Space]
    [SerializeField] private GameObject _loseUI;
    [SerializeField] private float _loseUIDelay;
    
    [Space]
    [SerializeField] private WinLoseScreenFunctionality _winLoseScreen;

    private void BattleWinUI()
    {
        StartCoroutine(BattleWinUIDelay());
    }

    private IEnumerator BattleWinUIDelay()
    {
        yield return new WaitForSeconds(_winUIDelay);
        _winUI.SetActive(true);
        _winLoseScreen.BattleWon();
    }
    
    private void BattleLoseUI()
    {
        StartCoroutine(BattleLoseUIDelay());
    }

    private IEnumerator BattleLoseUIDelay()
    {
        yield return new WaitForSeconds(_loseUIDelay);
        _loseUI.SetActive(true);
        _winLoseScreen.BattleLost();
    }
    
    #region BaseManager
    /// <summary>
    /// Establishes the Instance for the Game State UI Manager
    /// </summary>
    public override void SetUpInstance()
    {
        base.SetUpInstance();
        Instance = this;
    }

    /// <summary>
    /// Subscribes to all needed events
    /// </summary>
    protected override void SubscribeToEvents()
    {
        GameStateManager.Instance.GetBattleWonEvent().AddListener(BattleWinUI);
        GameStateManager.Instance.GetBattleLostEvent().AddListener(BattleLoseUI);
    }
    #endregion
}
