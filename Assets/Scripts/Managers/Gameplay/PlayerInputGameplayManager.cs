using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

/// <summary>
/// Handles player input while in a gameplay scene
/// </summary>
public class PlayerInputGameplayManager : BaseGameplayManager
{
    [SerializeField] private LayerMask _selectClickLayerMask;
    [SerializeField] private LayerMask _directClickLayerMask;

    private const float _playerClickRange = 50;

    private List<HeroBase> _controlledHeroes = new List<HeroBase>();

    [Space]
    [SerializeField] private GameObject _heroDirectIcon;

    private UniversalPlayerInputActions UPIA;

    private bool _subscribedToInput = false;

    public override void SetupGameplayManager()
    {
        base.SetupGameplayManager();
        SubscribeToPlayerInput();
    }

    private void OnDestroy()
    {
        UnsubscribeToPlayerInput();
    }

    private bool ClickOnPoint(LayerMask detectionLayerMask, out RaycastHit rayHit)
    {
        Camera mainCam = GameplayManagers.Instance.GetCameraManager().GetGameplayCamera();

        Ray clickRay = mainCam.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(clickRay, out rayHit, _playerClickRange,detectionLayerMask))
        {
            return true;
        }

        return false;
    }

    #region Controlling Heroes
    private void NewControlledHero(HeroBase newHero)
    {
        ClearControlledHeroes();

        newHero.InvokeHeroControlledBegin();
        _controlledHeroes.Add(newHero);
    }

    private void NewControlledHeroes()
    {

    }

    private void ClearControlledHeroes()
    {
        foreach (HeroBase newHero in _controlledHeroes)
            newHero.InvokeHeroControlledEnd();
        _controlledHeroes.Clear();
    }

    private void DirectAllHeroesTo(Vector3 newDestination)
    {
        foreach(HeroBase currentHero in _controlledHeroes)
        {
            currentHero.GetPathfinding().DirectNavigationTo(newDestination);
        }
    }

    private void ActivateAllManualAbilities()
    {
        if(ClickOnPoint(_directClickLayerMask, out RaycastHit clickedOn))
        {
            Vector3 targetLoc = GameplayManagers.Instance.GetEnvironmentManager()
                .GetClosestPointToFloor(clickedOn.point);

            foreach (HeroBase currentHero in _controlledHeroes)
            {
                currentHero.InvokeHeroManualAbilityAttemptEvent(targetLoc);
            }
            
        }

        

    }
    #endregion

    #region InputActions
    private void PlayerSelectClicked(InputAction.CallbackContext context)
    {
        if(ClickOnPoint(_selectClickLayerMask, out RaycastHit clickedOn))
        {
            NewControlledHero(clickedOn.collider.gameObject.GetComponentInParent<HeroBase>());
        }
    }


    private void PlayerDirectClicked(InputAction.CallbackContext context)
    {
        if (ClickOnPoint(_directClickLayerMask, out RaycastHit clickedOn))
        {
            DirectAllHeroesTo(clickedOn.point);
            CreateHeroDirectIcon(clickedOn.point);
        }
    }

    private void CreateHeroDirectIcon(Vector3 location)
    {
        location = GameplayManagers.Instance.GetEnvironmentManager().GetClosestPointToFloor(location);
        location = new Vector3(location.x, -.75f, location.z);
        Instantiate(_heroDirectIcon, location, Quaternion.identity);
        
    }

    private void HeroActiveButton(InputAction.CallbackContext context)
    {
        ActivateAllManualAbilities();
    }

    private void HeroNumberPress(InputAction.CallbackContext context)
    {
        int pressNumVal = (int)context.ReadValue<float>();

        HeroesManager heroesManager = GameplayManagers.Instance.GetHeroesManager();
        if (heroesManager.GetCurrentHeroes().Count <= pressNumVal
            || heroesManager.GetCurrentHeroes()[pressNumVal] == null)
            return;

            NewControlledHero(heroesManager.GetCurrentHeroes()[pressNumVal]);
        
    }

    private void EscapePress(InputAction.CallbackContext context)
    {
        UniversalManagers.Instance.GetTimeManager().PressGamePauseButton();
    }

    private void SubscribeToPlayerInput()
    {
        UPIA = new UniversalPlayerInputActions();
        UPIA.GameplayActions.Enable();

        UPIA.GameplayActions.SelectClick.started += PlayerSelectClicked;
        UPIA.GameplayActions.DirectClick.started += PlayerDirectClicked;
        UPIA.GameplayActions.ActiveAbility.started += HeroActiveButton;
        UPIA.GameplayActions.NumberPress.started += HeroNumberPress;
        UPIA.GameplayActions.EscapePress.started += EscapePress;

        _subscribedToInput = true;
    }
    private void UnsubscribeToPlayerInput()
    {
        if (!_subscribedToInput) return;

        UPIA.GameplayActions.SelectClick.started -= PlayerSelectClicked;
        UPIA.GameplayActions.DirectClick.started -= PlayerDirectClicked;
        UPIA.GameplayActions.ActiveAbility.started -= HeroActiveButton;
        UPIA.GameplayActions.NumberPress.started -= HeroNumberPress;
        UPIA.GameplayActions.EscapePress.started -= EscapePress;

        UPIA.Disable();

        _subscribedToInput = false;
    }
    #endregion



    #region BaseManager
    public override void SubscribeToEvents()
    {
        GameplayManagers.Instance.GetGameStateManager().GetBattleWonOrLostEvent().AddListener(UnsubscribeToPlayerInput);
    }
    #endregion

    #region Getters
    #endregion
}
