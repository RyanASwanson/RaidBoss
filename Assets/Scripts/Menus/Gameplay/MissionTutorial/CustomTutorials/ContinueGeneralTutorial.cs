using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContinueGeneralTutorial : MonoBehaviour
{
    public void ContinueTutorial()
    {
        BaseCustomTutorial.Instance.CloseCurrentTutorialStepSection();
    }
}
