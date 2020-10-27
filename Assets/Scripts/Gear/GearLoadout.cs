using System;
using System.Collections.Generic;
using UnityEngine;
using static GearData;

public class GearLoadout
{
    public enum GearSlotTypes
    {
        Head,
        Shoulder,
        Arm,
        Hand,
        Body,
        Leg,
        Core
    }

    public enum LoadoutSlots
    {
        Head,
        LeftShoulder,
        RightShoulder,
        LeftArm,
        RightArm,
        LeftHand,
        RightHand,
        Body,
        Core,
        LeftLeg,
        RightLeg
    }

    private static Dictionary<LoadoutSlots, GearSlotTypes> _slotTypes =
        new Dictionary<LoadoutSlots, GearSlotTypes> {
            { LoadoutSlots.Head, GearSlotTypes.Head },
            { LoadoutSlots.LeftShoulder, GearSlotTypes.Shoulder },
            { LoadoutSlots.RightShoulder, GearSlotTypes.Shoulder },
            { LoadoutSlots.LeftArm, GearSlotTypes.Arm },
            { LoadoutSlots.RightArm, GearSlotTypes.Arm },
            { LoadoutSlots.LeftHand, GearSlotTypes.Hand },
            { LoadoutSlots.RightHand, GearSlotTypes.Hand },
            { LoadoutSlots.Body, GearSlotTypes.Body },
            { LoadoutSlots.Core, GearSlotTypes.Core },
            { LoadoutSlots.LeftLeg, GearSlotTypes.Leg },
            { LoadoutSlots.RightLeg, GearSlotTypes.Leg }
        };

    public Dictionary<LoadoutSlots, LoadoutSlot> slots;

    public GearLoadout()
    {
        slots = new Dictionary<LoadoutSlots, LoadoutSlot>
        {
            [LoadoutSlots.Head] = new LoadoutSlot("Head", GearSlotTypes.Head, false),

            [LoadoutSlots.LeftShoulder] = new LoadoutSlot("Left Shoulder", GearSlotTypes.Shoulder, false),
            [LoadoutSlots.LeftArm] = new LoadoutSlot("Left Arm", GearSlotTypes.Arm, false),
            [LoadoutSlots.LeftHand] = new LoadoutSlot("Left Hand", GearSlotTypes.Hand, false),

            [LoadoutSlots.RightShoulder] = new LoadoutSlot("Right Shoulder", GearSlotTypes.Shoulder, false),
            [LoadoutSlots.RightArm] = new LoadoutSlot("Right Arm", GearSlotTypes.Arm, false),
            [LoadoutSlots.RightHand] = new LoadoutSlot("Right Hand", GearSlotTypes.Hand, false),

            [LoadoutSlots.Core] = new LoadoutSlot("Core", GearSlotTypes.Core, false),
            [LoadoutSlots.Body] = new LoadoutSlot("Body", GearSlotTypes.Body, false),

            [LoadoutSlots.LeftLeg] = new LoadoutSlot("Left Leg", GearSlotTypes.Leg, false),
            [LoadoutSlots.RightLeg] = new LoadoutSlot("Right Leg", GearSlotTypes.Leg, false)
        };
    }

    public bool SlotIsFree(LoadoutSlots slot)
    {
        LoadoutSlot slotcontainer = slots[slot];
        return (!slotcontainer.hidden && slotcontainer.contains == null);
    }

    public bool CanEquipIntoSlot(GearData gear, LoadoutSlots slot)
    {
        if (!SlotIsFree(slot))
        {
            return false;
        }

        LoadoutSlot slotcontainer = slots[slot];
        if (slotcontainer.type != gear.requiredSlot)
        {
            return false;
        }

        return true;
    }

    public void EquipIntoSlot(GearData gear, LoadoutSlots slot)
    {
        if (!CanEquipIntoSlot(gear, slot))
        {
            throw new ArgumentException("Cannot equip " + gear + " into " + slot.ToString());
        }

        LoadoutSlot slotcontainer = slots[slot];
        slotcontainer.contains = gear;
    }

    public void UnequipFromSlot(LoadoutSlots slot)
    {
        LoadoutSlot slotcontainer = slots[slot];
        slotcontainer.contains = null;
    }

    public DeckTemplate ToDeckTemplate()
    {
        DeckTemplate deck = new DeckTemplate();

        foreach (LoadoutSlot slot in slots.Values)
        {
            if (slot.contains is GearData data)
            {
                foreach (CardBundle bundle in data.Cards)
                {
                    deck.AddCardID(bundle.card.cardID, numberOf: bundle.amount);
                }
            } 
        }

        return deck;
    }

    public List<GearData> ToList()
    {
        List<GearData> list = new List<GearData>();

        foreach (LoadoutSlot slot in slots.Values)
        {
            if (slot.contains is GearData data)
            {
                list.Add(data);
            }
        }

        return list;
    }

    public static GearSlotTypes GetSlotType(LoadoutSlots slot)
    {
        if (_slotTypes.TryGetValue(slot, out GearSlotTypes result))
        {
            return result;
        }
        throw new ArgumentException(slot.ToString() + " is not a valid slot");
    }
}
