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
    [SerializeField] private float _scrollCooldown;
    private bool _scrollOnCooldown;

    private Coroutine _scrollCooldownCoroutine;

    [Space]
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

    /// <summary>
    /// Finds the object in 3D space at the location in which you clicked
    /// </summary>
    /// <param name="detectionLayerMask"></param> The layer/layers that can be clicked on
    /// <param name="rayHit"></param> Out variable for the object the ray hit
    /// <returns></returns> Returns if something was clicked
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
    /// <summary>
    /// Swaps which hero is currently being controlled
    /// </summary>
    /// <param name="newHero"></param>
    private void NewControlledHero(HeroBase newHero)
    {
        ClearControlledHeroes();

        newHero.InvokeHeroControlledBegin();
        _controlledHeroes.Add(newHero);
    }

    public void NewControlledHeroByID(int id)
    {
        NewControlledHero(GameplayManagers.Instance.GetHeroesManager().GetCurrentHeroes()[id]);
    }

    private void ScrollControlledHero(int direction)
    {
        direction += _controlledHeroes[0].GetHeroID();
        if (direction > GameplayManagers.Instance.GetHeroesManager().GetCurrentHeroes().Count-1)
            direction = 0;
        else if (direction < 0)
            direction = GameplayManagers.Instance.GetHeroesManager().GetCurrentHeroes().Count-1;

        NewControlledHeroByID(direction);
        
    }

    private void NewControlledHeroes()
    {

    }

    /// <summary>
    /// Clears out the list of controlled heroes
    /// </summary>
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
        //No icon is created if no heroes as controlled
        if (_controlledHeroes.Count <= 0) return;

        location = CalculateDirectIconLocation(location);
        Instantiate(_heroDirectIcon, location, Quaternion.identity);
        
    }

    public Vector3 CalculateDirectIconLocation(Vector3 location)
    {
        location = GameplayManagers.Instance.GetEnvironmentManager().GetClosestPointToFloor(location);
        location = new Vector3(location.x, -.75f, location.z);
        return location;
    }

    private void HeroActiveButton(InputAction.CallbackContext context)
    {
        ActivateAllManualAbilities();
    }

    /// <summary>
    /// Called when numbers 1-5 are pressed on the keyboard
    /// </summary>
    /// <param name="context"></param>
    private void HeroNumberPress(InputAction.CallbackContext context)
    {
        int pressNumVal = (int)context.ReadValue<float>();

        HeroesManager heroesManager = GameplayManagers.Instance.GetHeroesManager();

        //Returns if the number is more than the number of heroes
        //Or if the hero in that slot doesn't exist
        if (heroesManager.GetCurrentHeroes().Count <= pressNumVal
            || heroesManager.GetCurrentHeroes()[pressNumVal] == null)
            return;

            NewControlledHero(heroesManager.GetCurrentHeroes()[pressNumVal]);
        
    }

    private void MouseScroll(InputAction.CallbackContext context)
    {
        if (_scrollCooldownCoroutine != null)
            return;


        int storedDirection = (int)context.ReadValue<float>();
        if (storedDirection > 0)
            storedDirection = 1;
        else if (storedDirection < 0)
            storedDirection = -1;

        ScrollControlledHero(storedDirection);

        _scrollCooldownCoroutine = StartCoroutine(ScrollCooldown());
    }

    private IEnumerator ScrollCooldown()
    {
        yield return new WaitForSeconds(_scrollCooldown);
        _scrollCooldownCoroutine = null;
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
        UPIA.GameplayActions.MouseScroll.performed += MouseScroll;
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
        UPIA.GameplayActions.MouseScroll.started -= MouseScroll;
        UPIA.GameplayActions.MouseScroll.performed -= MouseScroll;
        UPIA.GameplayActions.EscapePress.started -= EscapePress;

        UPIA.Disable();

        _subscribedToInput = false;
    }
    #endregion



    #region BaseManager
    public override void SubscribeToEvents()
    {
        //Prevents the player from performing any actions after the game ends
        GameplayManagers.Instance.GetGameStateManager().GetBattleWonOrLostEvent().AddListener(UnsubscribeToPlayerInput);
    }
    #endregion

    #region Getters
    #endregion
}
