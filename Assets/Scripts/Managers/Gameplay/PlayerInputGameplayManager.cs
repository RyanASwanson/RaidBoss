using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputGameplayManager : BaseGameplayManager
{
    [SerializeField] private LayerMask _clickLayerMask;

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

    private void ClickOnPoint()
    {
        Camera mainCam = GameplayManagers.Instance.GetCameraManager.GetGameplayCamera;

        Ray clickRay = mainCam.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit rayHit;

        if (Physics.Raycast(clickRay, out rayHit, _clickLayerMask))
        {
            Vector3 targetLocation = rayHit.point;
            Debug.Log(targetLocation);
        }
        
    }

    #region InputActions
    public void PlayerClicked(InputAction.CallbackContext context)
    {
        ClickOnPoint();
    }

    private void SubscribeToPlayerInput()
    {
        UPIA = new UniveralPlayerInputActions();
        UPIA.GameplayActions.Enable();

        UPIA.GameplayActions.Click.started += PlayerClicked;
    }
    private void UnsubscribeToPlayerInput()
    {
        UPIA.GameplayActions.Click.started -= PlayerClicked;

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
