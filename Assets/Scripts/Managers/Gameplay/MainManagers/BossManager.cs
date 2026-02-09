using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Performs general management of the boss and provides accessibility to other scripts to certain functionality
/// </summary>
public class BossManager : MainGameplayManagerFramework
{
    public static BossManager Instance;
    
    [SerializeField] private BossBase _bossBase;

    private void SetUpBoss()
    {
        // Sets up the boss base by giving it the BossSO
        _bossBase.SetUp(SelectionManager.Instance.GetSelectedBoss());
    }
    
    #region BaseManager
    /// <summary>
    /// Establishes the instance for the BossManager
    /// </summary>
    public override void SetUpInstance()
    {
        base.SetUpInstance();
        Instance = this;
    }

    protected override void SubscribeToEvents()
    {
        base.SubscribeToEvents();
        GameStateManager.Instance.GetStartOfCharacterSpawningEvent().AddListener(SetUpBoss);
    }

    #endregion

    #region Events
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
