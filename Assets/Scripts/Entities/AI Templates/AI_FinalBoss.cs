using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_FinalBoss : IAiTemplate
{
    public int MaxHealth = 100;
    public int moveSpeed = 3;
    public int attackRange = 5;
    public int hookDamage = 10;
    public int attackDamage = 10;


    public void ApplyTo(Entity entity)
    {
        entity.entityName = "Final Boss";
        entity.Health.SetMaxHealth(MaxHealth, updateHealth: true);
        entity.EnableStatusEffects(false);

        entity.AIController.AddAction(new BasicMelee(hookDamage), 20);
        entity.AIController.AddAction(new ThrowHook(attackRange, moveSpeed), 10);
        entity.AIController.AddAction(new MoveWithinRangeOfPlayer(moveSpeed, attackRange), 1);
        entity.AIController.AddAction(new BasicMelee(attackDamage), 20);
        entity.AIController.AddAction(new BasicRanged(attackDamage, attackRange), 10);
        entity.AIController.AddAction(new MoveTowardsPlayer(hookDamage), 1);

    }

}

class HookThrow : IAIAction
{
    public string ActionName { get { return "HookThrow"; } }
    public int range;
    public int movementRange;
    public event Action OnActionFinish;

    public HookThrow(int range, int speed)
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
        //shouldn't attack, within melee range
        List<Entity> adjacent = GridHelper.GetAdjacentEntities(with.Grid, with.Position);
        if (adjacent.Contains(GameplayContext.Player))
        {
            return false;
        }

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
