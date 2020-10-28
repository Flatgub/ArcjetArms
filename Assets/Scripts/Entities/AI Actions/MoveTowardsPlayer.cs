using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTowardsPlayer : IAIAction
{
    public string ActionName { get { return "MoveTowardsPlayer";} }

    public int speed;
    public event Action OnActionFinish;
    public TerrainType rockTerrain;

    public MoveTowardsPlayer(int speed)
    {
        this.speed = speed;
    }

    public void Do(Entity with)
    {
        List<Hex> path = GridHelper.GetPathToHex(GameplayContext.Grid, with.Position,
            GameplayContext.Player.Position);

        path.Remove(GameplayContext.Player.Position);

        with.MoveAlong(path, maxSteps: speed, callback: () => {
            OnActionFinish?.Invoke();
            OnActionFinish = null;
            });
    }

    public bool IsDoable(Entity with)
    {
        List<Hex> path = GridHelper.GetPathToHex(GameplayContext.Grid, with.Position,
            GameplayContext.Player.Position);

        return (path != null && path.Count > 1); //count 1 is adjacent
    }
}
