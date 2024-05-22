using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossVisuals : BossChildrenFunctionality
{
    [Header("Rotation")]
    [SerializeField] private float _rotateSpeed;

    private GameObject _visualObjectBase;

    public void BossLookAt(Vector3 lookLocation)
    {
        /*transform.LookAt(lookLocation);
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);*/
        StartCoroutine(LookAtProcess(lookLocation));
    }

    private IEnumerator LookAtProcess(Vector3 lookLocation)
    {
        Vector3 lookDir = lookLocation - _visualObjectBase.transform.position;

        Quaternion toRotation = Quaternion.FromToRotation(_visualObjectBase.transform.forward, lookDir);
        _visualObjectBase.transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, _rotateSpeed * Time.time);
        yield return null;
    }


    public override void SubscribeToEvents()
    {
        
    }

    #region Getters

    #endregion

    #region Setters

    #endregion
}
