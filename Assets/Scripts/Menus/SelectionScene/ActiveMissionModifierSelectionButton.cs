using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActiveMissionModifierSelectionButton : MonoBehaviour
{
    [SerializeField] private int _activeMissionModifierID;
    
    [SerializeField] private Image _associatedModifierImage;

    private void Start()
    {
        SetButtonModifierIconVisuals();
    }

    private void SetButtonModifierIconVisuals()
    {
        _associatedModifierImage.enabled = false;
    }
    
    public void UpdateModifierImage()
    {
        Debug.Log(SelectionManager.Instance.GetMissionModifierCount() + " " + _activeMissionModifierID);
        if (SelectionManager.Instance.GetMissionModifierCount() <= _activeMissionModifierID)
        {
            Debug.Log("Returning");
            RemoveModifierImage();
            return;
        }

        _associatedModifierImage.enabled = true;
        _associatedModifierImage.sprite =
            SelectionManager.Instance.GetCurrentMissionModifiers()[_activeMissionModifierID].GetModifierSprite();
    }

    public void RemoveModifierImage()
    {
        _associatedModifierImage.enabled = false;
    }

}
