using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTowardsEnemy : IAIAction
{
    public string ActionName { get { return "MoveTowardsEnemy";} }

    public int speed;
    public event Action OnActionFinish;
    private Entity navTarget = null;

    public MoveTowardsEnemy(int speed)
    {
        this.speed = speed;
    }

    public void Do(Entity with)
    {
        List<Hex> path = GridHelper.GetPathToHex(GameplayContext.Grid, with.Position,
            navTarget.Position);

        path.Remove(navTarget.Position);

        with.MoveAlong(path, maxSteps: speed, callback: () => {
            OnActionFinish?.Invoke();
            OnActionFinish = null;
            });
    }

    public bool IsDoable(Entity with)
    {
        navTarget = null;
        if (GameplayContext.Manager.allEnemies.Count == 1)
        {
            return false; //there's noone else to path to
        }

        //find the nearest other entity
        List<Entity> otherEnts = new List<Entity>(GameplayContext.Manager.allEnemies);
        otherEnts.Remove(with);
        List<Hex> otherEntPositions = new List<Hex>();
        foreach (Entity other in otherEnts)
        {
            otherEntPositions.Add(other.Position);
        }
        Hex nearestPos = GridHelper.GetNearestHexInList(otherEntPositions, with.Position);

        //find path
        List<Hex> path = GridHelper.GetPathToHex(GameplayContext.Grid, with.Position,nearestPos);

        //if path is valid (and long enough)
        if (path != null && path.Count > 1)
        {
            navTarget = GameplayContext.Grid.GetEntityAtHex(nearestPos);
            return true;
        }

        return false;
    }
}
