using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeroBuffDebuffFunctionality : MonoBehaviour
{
    [SerializeField] private Image _buffIcon;

    public void ChangeHeroBuffDebuffIcon(Sprite buffDebuffSprite)
    {
        _buffIcon.sprite = buffDebuffSprite;
    }
}
