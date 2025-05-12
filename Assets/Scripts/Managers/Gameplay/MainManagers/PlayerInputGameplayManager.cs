using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

/// <summary>
/// Handles player input while in a gameplay scene
/// </summary>
public class PlayerInputGameplayManager : MainGameplayManagerFramework
{
    public static PlayerInputGameplayManager Instance;
    
    [Tooltip("The time between mouse scrolling switching heroes")]
    [SerializeField] private float _scrollCooldown;
    private bool _clickAndDragEnabled;

    private Coroutine _scrollCooldownCoroutine;

    [Space]
    [SerializeField] private LayerMask _selectClickLayerMask;
    [SerializeField] private LayerMask _directClickLayerMask;

    [Tooltip("The range the raycast from the camera checks for heroes")]
    private const float PLAYER_CLICK_RANGE = 50;

    private List<HeroBase> _controlledHeroes = new List<HeroBase>();

    [Space]
    [SerializeField] private GameObject _heroDirectIcon;

    private UniversalPlayerInputActions _universalPlayerInputActions;

    private bool _isSubscribedToInput = false;
    
    /// <summary>
    /// Sets the click and drag based on what it is set in the save manager
    /// </summary>
    private void SetClickAndDragFromSave()
    {
        _clickAndDragEnabled = SaveManager.Instance.GetClickAndDragEnabled();
    }

    /// <summary>
    /// Finds the object in 3D space at the location in which you clicked
    /// </summary>
    /// <param name="detectionLayerMask"> The layer/layers that can be clicked on </param> 
    /// <param name="rayHit"> Out variable for the object the ray hit </param> 
    /// <returns> Returns if something was clicked </returns> 
    private bool ClickOnPoint(LayerMask detectionLayerMask, out RaycastHit rayHit)
    {
        Camera mainCam = CameraGameManager.Instance.GetGameplayCamera();

        Ray clickRay = mainCam.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(clickRay, out rayHit, PLAYER_CLICK_RANGE,detectionLayerMask))
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
        NewControlledHero(HeroesManager.Instance.GetCurrentHeroes()[id]);
    }

    private void ScrollControlledHero(int direction)
    {
        if(_controlledHeroes.Count == 0)
        {
            HeroBase heroBase = HeroesManager.Instance.GetCurrentLivingHeroes()[0];
            if (!heroBase.IsUnityNull())
            {
                NewControlledHero(heroBase);
                return;
            }
        }

        HeroBase heroToControl;
        do
        {
            direction += _controlledHeroes[0].GetHeroID();

            if (direction > HeroesManager.Instance.GetCurrentHeroes().Count - 1)
                direction = 0;
            else if (direction < 0)
                direction = HeroesManager.Instance.GetCurrentHeroes().Count - 1;

            heroToControl = HeroesManager.Instance.GetCurrentHeroes()[direction];
        }
        while (heroToControl.GetHeroStats().IsHeroDead());

        NewControlledHeroByID(direction);
        
    }

    //TODO function for if we ever want to select multiple heroes
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

    /// <summary>
    /// Activates the manual abilities of all controlled heroes
    /// </summary>
    private void ActivateAllManualAbilities()
    {
        if(ClickOnPoint(_directClickLayerMask, out RaycastHit clickedOn))
        {
            Vector3 targetLoc = EnvironmentManager.Instance.GetClosestPointToFloor(clickedOn.point);

            foreach (HeroBase currentHero in _controlledHeroes)
            {
                currentHero.GetSpecificHeroScript().AttemptActivationOfManualAbility(targetLoc);
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

    /// <summary>
    /// Determines where the icon for the direct click should appear on the ground
    /// </summary>
    /// <param name="location"></param>
    /// <returns></returns>
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
        
        if (IsInvalidHeroPress(pressNumVal))
            return;

        NewControlledHero(HeroesManager.Instance.GetCurrentHeroes()[pressNumVal]);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="context"> The context of the button pressed</param>
    private void SpecificHeroAbilityPress(InputAction.CallbackContext context)
    {
        int pressNumVal = (int)context.ReadValue<float>();

        if(IsInvalidHeroPress(pressNumVal))
            return;

        if (ClickOnPoint(_directClickLayerMask, out RaycastHit clickedOn))
        {
            Vector3 targetLoc = GameplayManagers.Instance.GetEnvironmentManager()
                .GetClosestPointToFloor(clickedOn.point);

            HeroesManager.Instance.GetCurrentHeroes()
                [pressNumVal].GetSpecificHeroScript().AttemptActivationOfManualAbility(targetLoc);
        }
    }

    /// <summary>
    /// Returns true if the number is more than the number of heroes,
    /// or if the hero in that slot doesn't exist
    /// </summary>
    /// <param name="pressNumVal"></param>
    /// <returns></returns>
    private bool IsInvalidHeroPress(int pressNumVal)
    {
        return (HeroesManager.Instance.GetCurrentHeroes().Count <= pressNumVal
            || HeroesManager.Instance.GetCurrentHeroes()[pressNumVal] == null);
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
        TimeManager.Instance.PressGamePauseButton();
    }

    private void SubscribeToPlayerInput()
    {
        _universalPlayerInputActions = new UniversalPlayerInputActions();
        _universalPlayerInputActions.GameplayActions.Enable();

        _universalPlayerInputActions.GameplayActions.SelectClick.started += PlayerSelectClicked;
        _universalPlayerInputActions.GameplayActions.DirectClick.started += PlayerDirectClicked;
        _universalPlayerInputActions.GameplayActions.ActiveAbility.started += HeroActiveButton;
        _universalPlayerInputActions.GameplayActions.NumberPress.started += HeroNumberPress;
        _universalPlayerInputActions.GameplayActions.SpecificAbilityPress.started += SpecificHeroAbilityPress;
        _universalPlayerInputActions.GameplayActions.MouseScroll.performed += MouseScroll;
        _universalPlayerInputActions.GameplayActions.EscapePress.started += EscapePress;

        _isSubscribedToInput = true;
    }
    
    private void UnsubscribeToPlayerInput()
    {
        if (!_isSubscribedToInput) return;

        _universalPlayerInputActions.GameplayActions.SelectClick.started -= PlayerSelectClicked;
        _universalPlayerInputActions.GameplayActions.DirectClick.started -= PlayerDirectClicked;
        _universalPlayerInputActions.GameplayActions.ActiveAbility.started -= HeroActiveButton;
        _universalPlayerInputActions.GameplayActions.NumberPress.started -= HeroNumberPress;
        _universalPlayerInputActions.GameplayActions.SpecificAbilityPress.started -= SpecificHeroAbilityPress;
        _universalPlayerInputActions.GameplayActions.MouseScroll.performed -= MouseScroll;
        _universalPlayerInputActions.GameplayActions.EscapePress.started -= EscapePress;

        _universalPlayerInputActions.Disable();

        _isSubscribedToInput = false;
    }
    #endregion

    #region BaseManager
    /// <summary>
    /// Establishes the Instance for the PlayerInputGameplayManager
    /// </summary>
    public override void SetUpInstance()
    {
        base.SetUpInstance();
        Instance = this;
    }
    
    /// <summary>
    /// Performs any needed set up on the PlayerInputGameplayManager
    /// </summary>
    public override void SetUpMainManager()
    {
        base.SetUpMainManager();
        SetClickAndDragFromSave();
        SubscribeToPlayerInput();
    }

    /// <summary>
    /// Subscribes to all needed events for the PlayerInputGameplayManager
    /// </summary>
    protected override void SubscribeToEvents()
    {
        //Prevents the player from performing any actions after the game ends
        GameStateManager.Instance.GetBattleWonOrLostEvent().AddListener(UnsubscribeToPlayerInput);
    }
    
    /// <summary>
    /// Unsubscribes player input and any other needed clean up
    /// </summary>
    protected override void OnDestroy()
    {
        base.OnDestroy();
        UnsubscribeToPlayerInput();
    }
    #endregion
}
