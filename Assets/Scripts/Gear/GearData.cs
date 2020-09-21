using System;
using UnityEngine;
using static GearLoadout;

[CreateAssetMenu(fileName = "NewGear", menuName = "Gear/Generic", order = 1)]
public class GearData : ScriptableObject
{
    public string gearName;
    public string faction;
    public GearSlotTypes[] requiredSlots;
    public GearData upgradesTo;

    [Serializable]
    public struct CardBundle
    {
        public CardData card;
        public int amount;
    }

    [SerializeField]
    public CardBundle[] Cards;
}
