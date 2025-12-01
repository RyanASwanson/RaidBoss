using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameVFXManager : MainUniversalManagerFramework
{
    public static GameVFXManager Instance;
    
    //Currently not in use, though I'm keeping it around as I have a feeling I may need it later.

    /*
[SerializeField] private GameObject _customCanvasDecalObject;


public void CreateCustomCanvasDecal(Sprite decalSprite, Transform transform)
{
    GameObject newestCustomDecal = Instantiate(_customCanvasDecalObject, transform.position, transform.rotation);

    if (newestCustomDecal.TryGetComponent(out CustomCanvasDecal newCustomDecalFunctionality))
    {
        newCustomDecalFunctionality.ShowCustomCanvasDecal(decalSprite);
    }
}*/
    
    #region BaseManager
    public override void SetUpInstance()
    {
        base.SetUpInstance();
        Instance = this;
    }
    #endregion
}
