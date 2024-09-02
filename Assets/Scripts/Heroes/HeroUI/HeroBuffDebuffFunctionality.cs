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

    private const string BUFF_ANIM_TRIGGER = "Buff";
    private const string DEBUFF_ANIM_TRIGGER = "Debuff";

    public void ChangeHeroBuffDebuffIcon(Sprite buffDebuffSprite, bool isBuff)
    {
        _buffIcon.sprite = buffDebuffSprite;

        _upArrow.enabled = isBuff;
        _downArrow.enabled = !isBuff;

        if (isBuff)
            _animator.SetTrigger(BUFF_ANIM_TRIGGER);
        else
            _animator.SetTrigger(DEBUFF_ANIM_TRIGGER);

    }
}
