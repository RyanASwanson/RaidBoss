using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContinueGeneralTutorial : MonoBehaviour
{
    public void ProgressTutorial()
    {
        BaseCustomTutorial.Instance.ProgressToNextTutorialStep();
    }
    
    public void CloseCurrentTutorial()
    {
        BaseCustomTutorial.Instance.CloseCurrentTutorialStepSection();
    }
}
