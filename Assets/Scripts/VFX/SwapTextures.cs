using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwapTextures : MonoBehaviour
{
    [SerializeField] private TextureSwap[] _swapGroup;
    
    public void SwapAllTextures()
    {
        foreach(TextureSwap textureSwap in _swapGroup)
        {
            textureSwap.SwapToMaterial();
        }
    }

}

[System.Serializable]
public class TextureSwap
{
    [SerializeField] private Renderer _renderer;
    [SerializeField] private Material _swapMaterial;

    public void SwapToMaterial()
    {
        _renderer.material = _swapMaterial;
    }
}
