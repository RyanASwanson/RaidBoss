using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Provides the functionality for displaying stats of heroes
/// </summary>
public class StatCounter : MonoBehaviour
{
    [SerializeField] private float _delayBetweenNodes;

    [SerializeField] private List<Animator> _statNodes;

    private const string SHOW_NODE_FILL_ANIM_TRIGGER = "ShowNodeFill";
    private const string HIGHLIGHT_NODE_FILL_ANIM_TRIGGER = "HighlightNodeFill";
    private const string SHOW_NODE_EMPTY_ANIM_TRIGGER = "ShowNodeEmpty";

    private const string RESET_NODE_ANIM_TRIGGER = "ResetNode";

    private Coroutine _showNodeCoroutine;

    /// <summary>
    /// Starts the process of showing stat nodes
    /// </summary>
    /// <param name="statNumber"></param>
    public void ShowStatNodes(int statNumber)
    {
        _showNodeCoroutine = StartCoroutine(ShowNodeProcess(statNumber));
    }

    /// <summary>
    /// Iterates through all stat nodes to determine which animation to play
    /// </summary>
    /// <param name="statNumber"></param>
    /// <returns></returns>
    private IEnumerator ShowNodeProcess(int statNumber)
    {
        for (int i = 0; i < _statNodes.Count; i++)
        {
            SpecificNodeAction(statNumber, i);
            yield return new WaitForSeconds(_delayBetweenNodes);
            
        }

        _showNodeCoroutine = null;
    }


    /// <summary>
    /// Stops the process of showing nodes
    /// </summary>
    public void StopShowNodeProcess()
    {
        if(_showNodeCoroutine != null)
            StopCoroutine(_showNodeCoroutine);
    }

    /// <summary>
    /// Performs animations based on comparing the heroes stat and current position in loop
    /// </summary>
    /// <param name="statNumber"></param> How much the hero has in the current stat
    /// <param name="currentPos"></param> What position we are in the for loop
    private void SpecificNodeAction(int statNumber, int currentPos)
    {
        _statNodes[currentPos].SetTrigger(RESET_NODE_ANIM_TRIGGER);

        if (currentPos == statNumber-1)
            _statNodes[currentPos].SetTrigger(HIGHLIGHT_NODE_FILL_ANIM_TRIGGER);
        else if (currentPos < statNumber)
            _statNodes[currentPos].SetTrigger(SHOW_NODE_FILL_ANIM_TRIGGER);
        else
            _statNodes[currentPos].SetTrigger(SHOW_NODE_EMPTY_ANIM_TRIGGER);
    }
}
