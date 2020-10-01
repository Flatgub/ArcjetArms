using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Entity))]
public class EntityAIController : MonoBehaviour
{
    private Entity controlling;
    private List<IAIAction> actions;

    private void Awake()
    {
        controlling = GetComponent<Entity>();
        controlling.SetAIController(this);
        actions = new List<IAIAction>();
    }

    public void AddAction(IAIAction newAction)
    {
        if (!actions.Contains(newAction))
        {
            actions.Add(newAction);
        }
    }

    public void DoRandomAction(Action callback)
    {
        List<IAIAction> candidates = GetPossibleActions();
        if (candidates.Count != 0)
        {
            IAIAction action = candidates.GetRandom();
            action.OnActionFinish += callback;
            action.Do(controlling);
        }
        else
        {
            Debug.LogWarning(controlling.entityName + " had no valid actions this turn");
            callback?.Invoke();
        }
    }

    public List<IAIAction> GetPossibleActions()
    {
        List<IAIAction> possibleActions = new List<IAIAction>();
        foreach (IAIAction action in actions)
        {
            if (action.IsDoable(controlling))
            {
                possibleActions.Add(action);
            } 
        }
        return possibleActions;
    }
}
