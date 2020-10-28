using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApproachMelee : IAIAction
{
    public string ActionName { get { return "ApproachMelee";} }

    public int range;
    public int baseDamage;
    public event Action OnActionFinish;

    public ApproachMelee(int range, int damage)
    {
        this.range = range;
        this.baseDamage = damage;
    }

    public void Do(Entity with)
    {
        List<Hex> path = GridHelper.GetPathToHex(GameplayContext.Grid, with.Position,
            GameplayContext.Player.Position);

        path.Remove(GameplayContext.Player.Position);

        //this is gross, it works but make it cleaner later
        with.MoveAlong(path, maxSteps: range, callback: () => {
            with.DealDamageTo(GameplayContext.Player, baseDamage);
            with.TriggerAttackEvent(GameplayContext.Player);
            LeanTween.moveLocal(with.gameObject, GameplayContext.Player.transform.position, 0.1f)
                .setEaseInCubic().setLoopPingPong(1);

            FXHelper.PlaySound("MetalHit");

            LeanTween.delayedCall(0.2f, () =>
            {
                OnActionFinish?.Invoke();
                OnActionFinish = null;
            });
        });
    }

    public bool IsDoable(Entity with)
    {
        List<Hex> path = GridHelper.GetPathToHex(GameplayContext.Grid, with.Position,
            GameplayContext.Player.Position, maxLength: range+1);

        return (path != null && path.Count > 1); //count 1 is adjacent
    }
}
