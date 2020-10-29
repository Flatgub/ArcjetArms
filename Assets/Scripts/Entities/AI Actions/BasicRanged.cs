using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicRanged : IAIAction
{
    public string ActionName { get { return "BasicRanged"; } }

    public int baseDamage;
    public int range;
    public event Action OnActionFinish;
    public string attackSound;

    public BasicRanged(int damage, int range, string attackSound = "rifleShot")
    {
        this.baseDamage = damage;
        this.range = range;
        this.attackSound = attackSound;
    }

    public void Do(Entity with)
    {
        with.DealDamageTo(GameplayContext.Player, baseDamage);
        with.TriggerAttackEvent(GameplayContext.Player);

        FXHelper.FireTracerBetween(with, GameplayContext.Player);
        FXHelper.PlaySound(attackSound);
        FXHelper.PlaySound(GameplayContext.Player.rangedHitSoundName, 0.1f);

        //LeanTween.moveLocal(with.gameObject, GameplayContext.Player.transform.position, 0.1f)
        //.setEaseInCubic().setLoopPingPong(1);

        LeanTween.delayedCall(0.2f, () =>
        {
            OnActionFinish?.Invoke();
            OnActionFinish = null;
        });
    }

    public bool IsDoable(Entity with)
    {
        return (with.Position.DistanceTo(GameplayContext.Player.Position) <= range);
    }
}
