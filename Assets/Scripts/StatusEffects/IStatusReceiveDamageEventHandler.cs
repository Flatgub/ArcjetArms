using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IStatusReceiveDamageEventHandler
{
    int OnReceivingDamage(int baseDamage);
}
