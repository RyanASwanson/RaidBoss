using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Performs general management of the boss and provides accessibility to other scripts to certain functionality
/// </summary>
public class BossManager : MainGameplayManagerFramework
{
    public static BossManager Instance;
    
    [SerializeField] private BossBase _bossBase;
    
    #region BaseManager
    /// <summary>
    /// Establishes the instance for the BossManager
    /// </summary>
    public override void SetUpInstance()
    {
        base.SetUpInstance();
        Instance = this;
    }

    /// <summary>
    /// Sets up the manager then sets up the specific boss
    /// </summary>
    public override void SetUpMainManager()
    {
        base.SetUpMainManager();

        // Sets up the boss base by giving it the BossSO
        _bossBase.SetUp(SelectionManager.Instance.GetSelectedBoss());
    }
    #endregion

    #region Getters
    /// <summary>
    /// Returns the direction from the input vector to the boss normalized
    /// </summary>
    /// <param name="startLocation"> The location we are checking the direction to boss from </param>
    /// <returns></returns>
    public Vector3 GetDirectionToBoss(Vector3 startLocation)
    {
        Vector3 returnVector = _bossBase.transform.position - startLocation;
        returnVector = new Vector3(returnVector.x, 0, returnVector.z);
        returnVector.Normalize();

        return returnVector;
    }
    #endregion
}
