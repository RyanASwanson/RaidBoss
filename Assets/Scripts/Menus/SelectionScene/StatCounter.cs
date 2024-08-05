using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatCounter : MonoBehaviour
{
    [SerializeField] private float _delayBetweenNodes;

    [SerializeField] private List<Animator> _statNodes;

    private const string _showNodeFillAnimTrigger = "ShowNodeFill";
    private const string _showNodeEmptyAnimTrigger = "ShowNodeEmpty";

    public void ShowStatNodes(int statNumber)
    {
        StartCoroutine(ShowNodeProcess(statNumber));
    }

    private IEnumerator ShowNodeProcess(int statNumber)
    {
        for (int i = 0; i < _statNodes.Count; i++)
        {
            SpecificNodeAction(statNumber, i);
            yield return new WaitForSeconds(_delayBetweenNodes);
            
        }
    }

    private void SpecificNodeAction(int statNumber, int currentPos)
    {
        if (currentPos < statNumber)
            _statNodes[currentPos].SetTrigger(_showNodeFillAnimTrigger);
        else
            _statNodes[currentPos].SetTrigger(_showNodeEmptyAnimTrigger);
    }
}
