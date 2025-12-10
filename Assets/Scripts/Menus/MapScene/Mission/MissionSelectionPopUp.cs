using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionSelectionPopUp : MonoBehaviour
{
    [SerializeField] private ScrollUISelection _popUpScrollUI;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MissionSelected()
    {
        _popUpScrollUI.ShowNewScroll(90f);
    }

    public void MissionDeselected()
    {
        _popUpScrollUI.HideScroll();
    }
}
