using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IStatusCalculateDamageEventHandler
{
    int OnCalculateDamageAdditive(int damage);

    float OnCalculateDamageMultiplicative(float damage);
}
