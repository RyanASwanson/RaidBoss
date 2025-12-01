using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralRandomRotation : MonoBehaviour
{
    [SerializeField] private bool _setRandomRotationOnStart;

    [SerializeField] private Vector3 _randomEulerDeviation;
    
    // Start is called before the first frame update
    void Start()
    {
        if (_setRandomRotationOnStart)
        {
            SetRandomRotation();
        }
    }

    public void SetRandomRotation()
    {
        Vector3 randomEuler = transform.localEulerAngles;
        randomEuler.x += RandomAxisValue(_randomEulerDeviation.x);
        randomEuler.y += RandomAxisValue(_randomEulerDeviation.y);
        randomEuler.z += RandomAxisValue(_randomEulerDeviation.z);
        transform.localEulerAngles = randomEuler;
    }

    private float RandomAxisValue(float value)
    {
        if (value == 0)
        {
            return 0;
        }

        return Random.Range(-value, value);
    }
}
