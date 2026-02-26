using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwapTextures : MonoBehaviour
{
    [SerializeField] private TextureSwap[] _swapGroup;

    public void SwapTexture(int textureID)
    {
        _swapGroup[textureID].SwapToMaterial();
    }
    
    public void SwapAllTextures()
    {
        for (int i = 0; i < _swapGroup.Length; i++)
        {
            SwapTexture(i);
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
