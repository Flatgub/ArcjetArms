using System;
using System.Collections.Generic;
using UnityEngine;

public class MoveWithinRangeOfPlayer : IAIAction
{
    public string ActionName { get { return "MoveWithinRangeOfPlayer"; } }

    public int speed;
    public int desiredRange;
    public event Action OnActionFinish;

    private Hex targetLocation = null;

    public MoveWithinRangeOfPlayer(int speed, int range)
    {
        this.speed = speed;
        this.desiredRange = range;
    }

    public void Do(Entity with)
    {
        List<Hex> path = GridHelper.GetPathToHex(GameplayContext.Grid, with.Position,
            targetLocation);

        path.Remove(GameplayContext.Player.Position);

        with.MoveAlong(path, maxSteps: speed, callback: () => {
            OnActionFinish?.Invoke();
            OnActionFinish = null;
        });
    }

    public bool IsDoable(Entity with)
    {
        List<Hex> withinRange =
            GridHelper.GetHexesInRange(GameplayContext.Grid, GameplayContext.Player.Position,
            desiredRange, false);

        //abort if already in range
        if (withinRange.Contains(with.Position))
        {
            return false;
        }

        GridHelper.RemoveOccupiedHexes(GameplayContext.Grid, withinRange);

        //abort if nowhere to go
        if (withinRange.Count == 0)
        {
            return false;
        }

        Hex nearest = GridHelper.GetNearestHexInList(withinRange, with.Position);
        targetLocation = nearest;

        List<Hex> path = GridHelper.GetPathToHex(GameplayContext.Grid, with.Position, nearest);

        return (path != null); 
    }

}
