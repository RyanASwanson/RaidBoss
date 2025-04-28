using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Performs general management of the boss
/// </summary>
public class BossManager : MainGameplayManagerFramework
{
    [SerializeField] private BossBase _bossBase;

    private GameObject _bossGameObject;



    #region BaseManager
    /// <summary>
    /// Sets up the manager then sets up the specific boss
    /// </summary>
    public override void SetUpMainManager()
    {
        base.SetUpMainManager();
        _bossGameObject = _bossBase.gameObject;

        //Sets up the boss base by giving it the BossSO
        _bossBase.Setup(UniversalManagers.Instance.GetSelectionManager().GetSelectedBoss());
    }
    #endregion

    #region Getters
    public BossBase GetBossBase() => _bossBase;
    public GameObject GetBossBaseGameObject() => _bossGameObject;

    /// <summary>
    /// Returns the direction from the input vector to the boss normalized
    /// </summary>
    /// <param name="startLocation"></param>
    /// <returns></returns>
    public Vector3 GetDirectionToBoss(Vector3 startLocation)
    {
        Vector3 returnVector = _bossGameObject.transform.position - startLocation;
        returnVector = new Vector3(returnVector.x, 0, returnVector.z);
        returnVector.Normalize();

        return returnVector;
    }

    #endregion

    #region Setters

    #endregion
}
