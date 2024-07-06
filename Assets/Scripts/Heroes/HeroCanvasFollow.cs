using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroCanvasFollow : MonoBehaviour
{
    [SerializeField] private GameObject _followHero;
    private const float xRotation = 35;

    private void Start()
    {
        transform.SetParent(null, false);
        transform.eulerAngles = new Vector3(35, 0, 0);
        StartCoroutine(FollowAssociatedHero());
    }
    private IEnumerator FollowAssociatedHero()
    {
        while(_followHero != null)
        {
            transform.position = _followHero.transform.position;
            yield return null;
        }
        Destroy(gameObject);
    }
}
