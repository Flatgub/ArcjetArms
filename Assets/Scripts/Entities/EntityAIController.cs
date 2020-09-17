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

    public void DoRandomAction()
    {
        List<IAIAction> candidates = GetPossibleActions();
        if (candidates.Count != 0)
        {
            IAIAction action = candidates.GetRandom();
            action.Do(controlling);
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
