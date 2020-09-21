using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GearLoadout;

[CreateAssetMenu(fileName = "NewArm", menuName = "Gear/Arm", order = 1)]
public class ArmGearData : GearData
{
    [Range(0,2)]
    public int handProvided = 1;
}
