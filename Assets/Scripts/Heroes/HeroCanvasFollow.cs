using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Provides the functionality for the hero ui world canvas to follow it's associated hero
/// </summary>
public class HeroCanvasFollow : MonoBehaviour
{
    [SerializeField] private float _destructionDelay;

    [Space]
    [SerializeField] private GameObject _followHero;
    private const float xRotation = 35;

    private void Start()
    {
        transform.SetParent(null, false);
        transform.eulerAngles = new Vector3(xRotation, 0, 0);
        StartCoroutine(FollowAssociatedHero());
    }

    /// <summary>
    /// Sets the location of the ui world canvas to be the location of the hero
    /// </summary>
    /// <returns></returns>
    private IEnumerator FollowAssociatedHero()
    {
        while(_followHero != null)
        {
            transform.position = _followHero.transform.position;
            yield return null;
        }

        //Provides a delay between the hero not being found and the ui being destroyed
        yield return new WaitForSeconds(_destructionDelay);

        //Destroy the ui canvas after the hero is destroyed
        Destroy(gameObject);
    }
}
