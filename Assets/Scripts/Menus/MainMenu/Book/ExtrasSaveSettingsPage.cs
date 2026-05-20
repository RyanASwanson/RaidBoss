using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtrasSaveSettingsPage : MonoBehaviour
{
    /// <summary>
    /// Called by Reset Save Button when fully held down
    /// </summary>
    public void ResetSaveData()
    {
        SaveManager.Instance.ResetGameplaySaveData();
    }
}
