using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CoreManagersFramework : MonoBehaviour
{
    /// <summary>
    /// Tries to instance the manager, if it fails it destroys itself
    /// </summary>
    protected virtual void Awake()
    {
        //Attempts to establish the instance
        if (EstablishInstance())
        {
            //Gets all managers
            GetAllManagers();
            //If successful setup all main managers
            SetupMainManagers();
        }
        else
        {
            //If it fails, it destroys itself
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Gets all managers under itself
    /// </summary>
    protected abstract void GetAllManagers();

    /// <summary>
    /// Attempts to establish the singleton
    /// Returns if it succeeded
    /// </summary>
    /// <returns></returns>
    protected abstract bool EstablishInstance();

    /// <summary>
    /// Sets up all managers that can be accessed from this one
    /// </summary>
    protected abstract void SetupMainManagers();
}
