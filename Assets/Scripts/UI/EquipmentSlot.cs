using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static GearLoadout;

public class EquipmentSlot : MonoBehaviour
{
    //[SerializeField]
    //private Button button = null;
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
        UpdateDependants(false, null);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="gear">the gear to equip</param>
    /// <param name="inventory">the inventory to return unequipped items into</param>
    public void SetEquippedGear(GearData gear, InventoryCollection inventory)
    {
        if (equippedGear != null && inventory != null)
        {
            inventory.AddItem(equippedGear);
        }

        equippedGear = gear;
        if (gear != null)
        {
            icon.enabled = true;
            icon.sprite = gear.art;
            UpdateDependants(true, inventory);
        }
        else
        {
            icon.enabled = false;
            UpdateDependants(false, inventory);
        }
    }

    private void UpdateDependants(bool enabled, InventoryCollection inventory)
    {
        foreach (EquipmentSlot slot in dependants)
        {
            if (enabled )
            {
                if (equippedGear != null)
                {
                    //check if the slot is of a type that the equipped gear doesn't provide
                    if (Array.IndexOf(equippedGear.DoesntProvide, GearLoadout.GetSlotType(slot.SlotID)) > -1)
                    {
                        slot.SetEquippedGear(null, inventory);
                        slot.gameObject.SetActive(false);
                    }
                }
                else
                {
                    slot.SetEquippedGear(null, inventory);
                }
            }
            else
            {
                slot.SetEquippedGear(null, inventory);
            }
            slot.gameObject.SetActive(enabled);
        }
    }

    private void OnEnable()
    {
        UpdateDependants(equippedGear != null, null);
    }

    private void OnDisable()
    {
        UpdateDependants(false, null);
    }

}
