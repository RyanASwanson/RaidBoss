using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralVFXFunctionality : MonoBehaviour
{
    [SerializeField] private bool _hasLifeTime;
    [SerializeField] private float _lifeTime;

    // Start is called before the first frame update
    void Start()
    {
        if (_hasLifeTime)
            Destroy(gameObject, _lifeTime);
    }

}
