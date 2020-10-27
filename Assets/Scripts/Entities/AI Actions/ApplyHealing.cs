using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplyHealing : IAIAction
{
    public string ActionName { get { return "ApplyHealing";} }

    public int healAmount;
    public event Action OnActionFinish;
    private Entity healTarget = null;

    public ApplyHealing(int heal)
    {
        healAmount = heal;
    }

    public void Do(Entity with)
    {
        //define how we heal ahead of time, to avoid code repetition
        Action healAction = () =>
        {
            healTarget.Health.ApplyHealing(healAmount);
            LeanTween.moveLocal(with.gameObject, GameplayContext.ActiveEntity.transform.position,
                0.1f).setEaseInCubic().setLoopPingPong(1);

            LeanTween.delayedCall(0.2f, () =>
            {
                OnActionFinish?.Invoke();
                OnActionFinish = null;
                healTarget = null;
            });
        };

        //if we're not adjacent
        if (with.Position.DistanceTo(healTarget.Position) > 1)
        {
            List<Hex> path = GridHelper.GetPathToHex(GameplayContext.Grid, with.Position,
                healTarget.Position);

            path.Remove(GameplayContext.Player.Position);

            //step forward first, and THEN do the heal action once we're adjacent
            with.MoveAlong(path, maxSteps: 2, callback: healAction); 
        }
        else 
        {
            //we're already adjacent, just do the healing action
            healAction.Invoke();
        } 
    }

    public bool IsDoable(Entity with)
    {
        healTarget = null;
        if (GameplayContext.Manager.allEnemies.Count == 1)
        {
            return false; //there's noone else to heal
        }

        //find the nearest other entity
        List<Entity> otherEnts = new List<Entity>(GameplayContext.Manager.allEnemies);
        otherEnts.Remove(with);
        List<Hex> otherEntPositions = new List<Hex>();
        foreach (Entity other in otherEnts)
        {
            //only consider entities that aren't on full health
            if (other.Health.HealthAsFraction() <= 0.9f)
            {
                otherEntPositions.Add(other.Position);
            }
        }

        if (otherEntPositions.Count == 0)
        {
            return false; //there's no one who needs healing
        }

        Hex nearestPos = GridHelper.GetNearestHexInList(otherEntPositions, with.Position);
        Entity targetEnt = GameplayContext.Grid.GetEntityAtHex(nearestPos);

        //if we're adjacent to our heal target, we can heal 'em
        if (GridHelper.GetAdjacentEntities(with.Grid, with.Position).Contains(targetEnt))
        {
            healTarget = targetEnt;
            return true;
        }

        //find path
        List<Hex> path = GridHelper.GetPathToHex(GameplayContext.Grid, with.Position, nearestPos);

        //if it only takes one step to approach the entity, we'll walk to them
        if (path != null && path.Count == 2)
        {
            healTarget = targetEnt;
            return true;
        }


        //either their too far or we can't make it to them or something
        return false;
    }
}
