using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralEscapeInputPress : GeneralInputPress
{
    protected override void OnEnable()
    {
        base.OnEnable();
        _universalPlayerInputActions.GameplayActions.EscapePress.started += ButtonPressStart;
        _universalPlayerInputActions.GameplayActions.EscapePress.canceled += ButtonPressEnd;
    }

    protected override void OnDisable()
    {
        _universalPlayerInputActions.GameplayActions.EscapePress.started -= ButtonPressStart;
        _universalPlayerInputActions.GameplayActions.EscapePress.canceled -= ButtonPressEnd;
        base.OnDisable();
    }
}
