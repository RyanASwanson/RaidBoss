using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenLink : MonoBehaviour
{
    [SerializeField] private string _linkURL;

    public void OpenAssociatedLink()
    {
        Application.OpenURL(_linkURL);
    }
}
