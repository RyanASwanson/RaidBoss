using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroCanvasFollow : MonoBehaviour
{
    [SerializeField] private GameObject _followHero;
    private const float xRotation = 35;

    private void Start()
    {
        transform.parent = null;
        transform.eulerAngles = new Vector3(35, 0, 0);
        StartCoroutine(FollowAssociatedHero());
    }
    private IEnumerator FollowAssociatedHero()
    {
        while(true)
        {
            transform.position = _followHero.transform.position;
            yield return null;
        }
        
    }
}
