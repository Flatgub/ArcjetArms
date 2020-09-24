using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static GearLoadout;

public class EquipmentSlot : MonoBehaviour
{
    [SerializeField]
    private Button button = null;
    [SerializeField]
    private Image icon = null;
    [SerializeField]
    private List<EquipmentSlot> dependants = null;

    [SerializeField]
    private LoadoutSlots loadoutSlot = LoadoutSlots.Head;
    public LoadoutSlots SlotID
    {
        get
        {
            return loadoutSlot;
        }
    }

    private GearData equippedGear = null;
    public bool Empty
    {
        get
        {
            return equippedGear == null;
        }
    }

    public GearData GetEquippedGear()
    {
        return equippedGear;
    }

    public void Start()
    {
        icon.enabled = false;
        UpdateDependants(false);
    }

    public void SetEquippedGear(GearData gear)
    {
        equippedGear = gear;
        if (gear != null)
        {
            icon.enabled = true;
            icon.sprite = gear.art;
            UpdateDependants(true);
        }
        else
        {
            icon.enabled = false;
            UpdateDependants(false);
        }
    }

    private void UpdateDependants(bool enabled)
    {
        foreach (EquipmentSlot slot in dependants)
        {
            if (enabled && equippedGear != null)
            {
                //check if the slot is of a type that the equipped gear doesn't provide
                if (Array.IndexOf(equippedGear.DoesntProvide, GearLoadout.GetSlotType(slot.SlotID)) > -1)
                {
                    slot.gameObject.SetActive(false);
                    continue;
                }
            }
            slot.gameObject.SetActive(enabled);
        }
    }

    private void OnEnable()
    {
        UpdateDependants(equippedGear != null);
    }

    private void OnDisable()
    {
        UpdateDependants(false);
    }

}
