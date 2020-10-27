using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Entity))]
public class EntityAIController : MonoBehaviour
{
    private Entity controlling;

    private struct ActionChoice
    {
        public IAIAction action;
        public int priority;
    }
    
    private List<ActionChoice> actions;

    private void Awake()
    {
        controlling = GetComponent<Entity>();
        controlling.SetAIController(this);
        actions = new List<ActionChoice>();
    }

    public void AddAction(IAIAction newAction, int priority)
    {
        ActionChoice pair = new ActionChoice();
        pair.action = newAction;
        pair.priority = Mathf.Max(priority, 1);

        actions.Add(pair);
    }

    public void DoRandomAction(Action callback)
    {
        if (controlling.isStunned)
        {
            callback.Invoke();
            return;
        }

        List<ActionChoice> candidates = GetPossibleActions();

        candidates.Sort((a, b) => {return Mathf.Clamp(a.priority - b.priority, -1, 1); });

        int prioritySum = 0;
        foreach (ActionChoice pair in candidates)
        {
            prioritySum += pair.priority;
        }

        int budget = UnityEngine.Random.Range(0, prioritySum);

        Debug.Log("candidatsize: " + candidates.Count + ", budget: " + budget);

        if (candidates.Count != 0)
        {
            //select from weighted set
            IAIAction action = null;
            int index = 0;
            while (budget >= 0 && index < candidates.Count)
            {
                ActionChoice pair = candidates[index];
                action = pair.action;
                budget -= pair.priority;
                index++;
            }

            Debug.Log("chose " + action?.ActionName);

            action.OnActionFinish += callback;
            action.Do(controlling);
        }
        else
        {
            Debug.LogWarning(controlling.entityName + " had no valid actions this turn");
            callback?.Invoke();
        }
    }

    private List<ActionChoice> GetPossibleActions()
    {
        List<ActionChoice> possibleActions = new List<ActionChoice>();
        foreach (ActionChoice pair in actions)
        {
            if (pair.action.IsDoable(controlling))
            {
                possibleActions.Add(pair);
            } 
        }
        return possibleActions;
    }
}
