using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossTargetZoneOutline : MonoBehaviour
{
    [SerializeField] private Outline _targetZoneOutline;

    private Color outlineColor;
    
    private void Start()
    {
        SetTargetZoneOutlineAlpha();
    }

    public void SetTargetZoneOutlineColor(Color newColor, bool doesSetAlpha)
    {
        outlineColor.r = newColor.r;
        outlineColor.g = newColor.g;
        outlineColor.b = newColor.b;
        if (doesSetAlpha)
        {
            outlineColor.a = newColor.a;
        }
        _targetZoneOutline.OutlineColor = outlineColor;
    }

    public void SetTargetZoneOutlineAlpha()
    {
        outlineColor = _targetZoneOutline.OutlineColor;
        outlineColor.a = SaveManager.Instance.GetBossTargetZoneOutlineStrengthScaled();
        _targetZoneOutline.OutlineColor = outlineColor;
    }
}
