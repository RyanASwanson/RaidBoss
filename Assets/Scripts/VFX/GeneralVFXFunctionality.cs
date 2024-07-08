using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralVFXFunctionality : MonoBehaviour
{
    [SerializeField] private bool _hasLifeTime;
    [SerializeField] private float _lifeTime;

    [Space]
    [SerializeField] private bool _hasDetachTime;
    [SerializeField] private float _detachTime;

    // Start is called before the first frame update
    void Start()
    {
        if (_hasLifeTime)
            Destroy(gameObject, _lifeTime);

        if (_hasDetachTime)
            StartCoroutine(DetachProcess());
    }

    private IEnumerator DetachProcess()
    {
        yield return new WaitForSeconds(_detachTime);
        transform.SetParent(null);
    }
}
