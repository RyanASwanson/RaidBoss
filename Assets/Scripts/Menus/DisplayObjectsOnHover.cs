using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayObjectsOnHover : MonoBehaviour
{
    [Header("ObjectsToShow")]
    [SerializeField] private List<GameObject> _displayObjects;

    public void DisplayObjects(bool display)
    {
        foreach (GameObject disObj in _displayObjects)
        {
            disObj.SetActive(display);
        }
    }
}
