using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatCounter : MonoBehaviour
{
    [SerializeField] private float _delayBetweenNodes;

    [SerializeField] private List<Animator> _statNodes;

    private const string _showNodeFillAnimTrigger = "ShowNodeFill";
    private const string _highlightNodeFillAnimTrigger = "HighlightNodeFill";
    private const string _showNodeEmptyAnimTrigger = "ShowNodeEmpty";

    private const string _resetNodeAnimTrigger = "ResetNode";

    private Coroutine _showNodeCoroutine;

    public void ShowStatNodes(int statNumber)
    {
        _showNodeCoroutine = StartCoroutine(ShowNodeProcess(statNumber));
    }

    private IEnumerator ShowNodeProcess(int statNumber)
    {
        for (int i = 0; i < _statNodes.Count; i++)
        {
            SpecificNodeAction(statNumber, i);
            yield return new WaitForSeconds(_delayBetweenNodes);
            
        }

        _showNodeCoroutine = null;
    }

    public void StopShowNodeProcess()
    {
        if(_showNodeCoroutine != null)
            StopCoroutine(_showNodeCoroutine);
    }

    private void SpecificNodeAction(int statNumber, int currentPos)
    {
        _statNodes[currentPos].SetTrigger(_resetNodeAnimTrigger);

        if (currentPos == statNumber-1)
            _statNodes[currentPos].SetTrigger(_highlightNodeFillAnimTrigger);
        else if (currentPos < statNumber)
            _statNodes[currentPos].SetTrigger(_showNodeFillAnimTrigger);
        else
            _statNodes[currentPos].SetTrigger(_showNodeEmptyAnimTrigger);
    }
}
