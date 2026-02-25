using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private CurveProgression _extrasPageScaleCurveProgression;
    
    private UniversalPlayerInputActions _universalPlayerInputActions;

    private void OnEnable()
    {
        SubscribeToPlayerInput();
    }

    private void OnDisable()
    {
        UnsubscribeToPlayerInput();
    }
    
    private void PlayerEscapePressed(InputAction.CallbackContext context)
    {
        _extrasPageScaleCurveProgression.StartMovingDownOnCurve();
    }

    private void SubscribeToPlayerInput()
    {
        _universalPlayerInputActions = new UniversalPlayerInputActions();
        _universalPlayerInputActions.GameplayActions.Enable();
        
        _universalPlayerInputActions.GameplayActions.EscapePress.started += PlayerEscapePressed;
    }

    private void UnsubscribeToPlayerInput()
    {
        _universalPlayerInputActions.GameplayActions.EscapePress.started -= PlayerEscapePressed;
        
        _universalPlayerInputActions.GameplayActions.Disable();
    }
    

    #region Buttons
    public void QuitButtonPressed()
    {
        Application.Quit();
    }
    #endregion
}
