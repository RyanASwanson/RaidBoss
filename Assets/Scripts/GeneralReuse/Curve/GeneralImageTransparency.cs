using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GeneralImageTransparency : MonoBehaviour
{
    [Space]
    [SerializeField] private CurveProgression _curveProgression;
    
    private Image _image;

    void OnEnable()
    {
        _image = GetComponent<Image>();
        SubscribeToEvents();
    }

    void OnDisable()
    {
        UnsubscribeFromEvents();
    }
    

    public void UpdateMaterialProperty(float progress)
    {
        Color tempColor = _image.color;
        tempColor.a = progress;
        _image.color = tempColor;
    }
    
    private void SubscribeToEvents()
    {
        if (_curveProgression)
        {
            _curveProgression.OnCurveValueChanged.AddListener(UpdateMaterialProperty);
        }
    }

    private void UnsubscribeFromEvents()
    {
        if (_curveProgression)
        {
            _curveProgression.OnCurveValueChanged.RemoveListener(UpdateMaterialProperty);
        }
    }
}
