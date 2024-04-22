using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputGameplayManager : BaseGameplayManager
{
    private UniveralPlayerInputActions UPIA;

    // Start is called before the first frame update
    void Start()
    {
        SubscribeToEvents();
        SubscribeToPlayerInput();
    }

    private void OnDestroy()
    {
        UnsubscribeToPlayerInput();
    }

    private void ClickOnPoint()
    {

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
