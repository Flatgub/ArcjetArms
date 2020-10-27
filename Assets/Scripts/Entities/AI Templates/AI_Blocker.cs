using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Blocker : IAiTemplate
{
    public int MaxHealth = 20;
    public int moveSpeed = 4;
    public int attackRange = 1;
    public int damage = 5;

    public void ApplyTo(Entity entity)
    {
        entity.entityName = "Hook Thrower";
        entity.Health.SetMaxHealth(MaxHealth, updateHealth: true);

        entity.AIController.AddAction(new BasicMelee(damage), 20);
        entity.AIController.AddAction(new DeployBlock(attackRange, moveSpeed), 10);
        entity.AIController.AddAction(new MoveTowardsPlayer(damage), 1);

    }

}

class DeployBlock : IAIAction
{
    public string ActionName { get { return "DeployBlock"; } }
    public int range;
    public int movementRange;
    public event Action OnActionFinish;

    public DeployBlock(int range, int speed)
    {
        this.range = range;
        this.movementRange = speed;
    }

    void IAIAction.Do(Entity with)
    {
        HexGrid grid = GameplayContext.Grid;
        if (InlineWithPlayer(with))
        {
            List<Hex> adjacents = with.Position.GetAllNeighbours();
            GridHelper.RemoveOccupiedHexes(grid, adjacents);
            GameplayContext.Player.MoveTo(GridHelper.GetNearestHexInList(adjacents, GameplayContext.Player.Position));

            LeanTween.delayedCall(0.2f, () =>
            {
                OnActionFinish?.Invoke();
                OnActionFinish = null;
            });
        }
        else
        {
            Hex nearest = GetNearestInlinePosition(with);
            List<Hex> path = GridHelper.GetPathToHex(GameplayContext.Grid, with.Position,
            nearest);

            with.MoveAlong(path, maxSteps: movementRange, callback: () => {
                OnActionFinish?.Invoke();
                OnActionFinish = null;
            });

        }
    }

    bool IAIAction.IsDoable(Entity with)
    {
        //ready to attack
        if (InlineWithPlayer(with))
        {
            return true;
        }

        //can move to a spot to attack
        if (GetNearestInlinePosition(with) != null)
        {
            return true;
        }

        return false;
    }

    private Hex GetNearestInlinePosition(Entity with)
    {
        List<Hex> inlinePositions = new List<Hex>();
        Entity player = GameplayContext.Player;
        HexGrid grid = GameplayContext.Grid;

        foreach (Hex dir in Hex.Directions)
        {
            List<Hex> line =
                GridHelper.CastLineInDirection(grid, player.Position, dir, range, includeStart: false);

            inlinePositions.AddRange(line);
        }

        return GridHelper.GetNearestHexInList(inlinePositions, with.Position);
    }

    private bool InlineWithPlayer(Entity with)
    {
        HexGrid grid = GameplayContext.Grid;

        bool canhit = false;
        foreach (Hex dir in Hex.Directions)
        {
            List<Hex> line = GridHelper.CastLineInDirection(grid, with.Position, dir, range,
                includeHit: true);
            Hex end = line[line.Count - 1];
            if (grid.GetEntityAtHex(end) == GameplayContext.Player)
            {
                canhit = true;
                break;
            }
        }

        return canhit;
    }
}
