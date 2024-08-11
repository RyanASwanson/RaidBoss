using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeroBuffDebuffFunctionality : MonoBehaviour
{
    [SerializeField] private Image _upArrow;
    [SerializeField] private Image _downArrow;
    [SerializeField] private Image _buffIcon;

    [SerializeField] private Animator _animator;

    private const string _buffAnimTrigger = "Buff";
    private const string _debuffAnimTrigger = "Debuff";

    public void ChangeHeroBuffDebuffIcon(Sprite buffDebuffSprite, bool isBuff)
    {
        _buffIcon.sprite = buffDebuffSprite;

        _upArrow.enabled = isBuff;
        _downArrow.enabled = !isBuff;

        if (isBuff)
            _animator.SetTrigger(_buffAnimTrigger);
        else
            _animator.SetTrigger(_debuffAnimTrigger);

    }
}
