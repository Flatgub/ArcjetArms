using System;
using UnityEngine;
using static GearLoadout;
using static LootPool;

[CreateAssetMenu(fileName = "NewGear", menuName = "Gear/Generic", order = 1)]
public class GearData : ScriptableObject
{
    public int gearID;
    public string gearName;
    public string faction;
    public GearSlotTypes requiredSlot;
    public GearSlotTypes[] DoesntProvide;
    //public GearSlotTypes[] requiredSlots;
    public GearData upgradesTo;
    public Sprite art;

    [Serializable]
    public struct CardBundle
    {
        public CardData card;
        public int amount;
    }

    [SerializeField]
    public CardBundle[] Cards;

    public LootRarity rarity = LootRarity.Common;
}
