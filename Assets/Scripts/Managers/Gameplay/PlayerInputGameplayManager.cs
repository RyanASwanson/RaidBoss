using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputGameplayManager : BaseGameplayManager
{
    [SerializeField] private LayerMask _selectClickLayerMask;
    [SerializeField] private LayerMask _directClickLayerMask;

    private List<HeroBase> _controlledHeroes;

    private UniveralPlayerInputActions UPIA;

    // Start is called before the first frame update
    void Start()
    {
        //SubscribeToEvents();
        SubscribeToPlayerInput();
    }

    private void OnDestroy()
    {
        UnsubscribeToPlayerInput();
    }

    private bool ClickOnPoint(LayerMask detectionLayerMask, out Vector3 hitPoint)
    {
        Camera mainCam = GameplayManagers.Instance.GetCameraManager().GetGameplayCamera();

        Ray clickRay = mainCam.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(clickRay, out RaycastHit rayHit, 100,detectionLayerMask))
        {
            hitPoint = rayHit.point;
            return true;
        }
        hitPoint = Vector3.zero;
        return false;
    }

    #region Controlling Heroes
    private void NewControlledHero(HeroBase newHero)
    {
        _controlledHeroes.Add(newHero);
    }

    private void NewControlledHeroes()
    {

    }

    private void ClearControlledHeroes()
    {
        _controlledHeroes.Clear();
    }
    #endregion

    #region InputActions
    private void PlayerSelectClicked(InputAction.CallbackContext context)
    {
        if(ClickOnPoint(_selectClickLayerMask, out Vector3 hitPoint))
        {
            
        }
    }


    private void PlayerDirectClicked(InputAction.CallbackContext context)
    {
        if (ClickOnPoint(_directClickLayerMask, out Vector3 hitPoint))
        {

        }
    }

    private void SubscribeToPlayerInput()
    {
        UPIA = new UniveralPlayerInputActions();
        UPIA.GameplayActions.Enable();

        UPIA.GameplayActions.SelectClick.started += PlayerSelectClicked;
    }
    private void UnsubscribeToPlayerInput()
    {
        UPIA.GameplayActions.SelectClick.started -= PlayerSelectClicked;

        UPIA.Disable();
    }
    #endregion

    #region BaseManager
    public override void SubscribeToEvents()
    {
        throw new System.NotImplementedException();
    }
    #endregion
}
