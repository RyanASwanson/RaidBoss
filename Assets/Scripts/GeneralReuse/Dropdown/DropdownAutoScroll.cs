using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DropdownAutoScroll : MonoBehaviour
{
    [SerializeField] private RectTransform _contentTransform;
    [SerializeField] private TMP_Dropdown _dropdown;
    
    private ScrollRect _scrollRect;
    
    void Start()
    {
        _scrollRect = GetComponent<ScrollRect>();

        ScrollToSelected();
    }

    void ScrollToSelected()
    {
        int selectedIndex = _dropdown.value;
        
        if (selectedIndex > -1)
        {
            // Stores the current X position of the content
            Vector2 contentPosition = _contentTransform.anchoredPosition;
            
            // Scrolls down to target
            _scrollRect.normalizedPosition = new Vector2(-5, 1 - (selectedIndex / ((float)_dropdown.options.Count - 1)));
            
            // Ensures the X position doesn't move after scrolling down
            contentPosition.Set(contentPosition.x, _contentTransform.anchoredPosition.y);
            _contentTransform.anchoredPosition = contentPosition;
        }
    }
}
