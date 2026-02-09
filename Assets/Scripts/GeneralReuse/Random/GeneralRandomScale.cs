using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralRandomScale : MonoBehaviour
{
    [SerializeField] private bool _setRandomScaleOnStart;
    
    [Space]
    [SerializeField] private bool _doesScaleGlobally;

    [Space] 
    [SerializeField] private bool _doesScaleAxesIndividually;
    
    [Space] 
    [SerializeField] private float _minimumUniversalScale;
    [SerializeField] private float _maximumUniversalScale;
    
    [Space]
    [SerializeField] private Vector3 _minimumScale;
    [SerializeField] private Vector3 _maximumScale;
    
    // Start is called before the first frame update
    void Start()
    {
        if (_setRandomScaleOnStart)
        {
            SetRandomScale();
        }
    }

    public void SetRandomScale()
    {
        Vector3 randomScale = transform.localEulerAngles;

        if (_doesScaleAxesIndividually)
        {
            randomScale.Set(
                Random.Range(_minimumScale.x,_maximumScale.x),
                Random.Range(_minimumScale.y,_maximumScale.y),
                Random.Range(_minimumScale.z,_maximumScale.z));
        }
        else
        {
            float randomValue = Random.Range(_minimumUniversalScale, _maximumUniversalScale);
            
            randomScale.Set(randomValue,randomValue,randomValue);
        }
        
        
        if (_doesScaleGlobally)
        {
            transform.lossyScale.Set(randomScale.x, randomScale.y, randomScale.z);
        }
        else
        {
            transform.localScale = randomScale;
        }
    }
}
