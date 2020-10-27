using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTowardsEnemy : IAIAction
{
    public string ActionName { get { return "MoveTowardsEnemy";} }

    public int speed;
    public event Action OnActionFinish;

    public MoveTowardsEnemy(int speed)
    {
        this.speed = speed;
    }

    public void Do(Entity with)
    {
        List<Hex> path = GridHelper.GetPathToHex(GameplayContext.Grid, with.Position,
            GameplayContext.ActiveEntity.Position);

        path.Remove(GameplayContext.Player.Position);

        with.MoveAlong(path, maxSteps: speed, callback: () => {
           // OnActionFinish?.Invoke();
            OnActionFinish = null;
            });
    }

    public bool IsDoable(Entity with)
    {
        List<Hex> path = GridHelper.GetPathToHex(GameplayContext.Grid, with.Position,
            GameplayContext.ActiveEntity.Position);

        return (path != null && path.Count > 1); //count 1 is adjacent
    }
}
