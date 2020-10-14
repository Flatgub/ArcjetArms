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
        //UpdateDependants(false, null);
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

    public void Refresh()
    {
        if (equippedGear != null)
        {
            icon.enabled = true;
            icon.sprite = equippedGear.art;
        }
        else
        {
            icon.enabled = false;
        }

        foreach (EquipmentSlot slot in dependants)
        {
            if (equippedGear != null)
            {
                //check if the slot is of a type that the equipped gear doesn't provide
                if (Array.IndexOf(equippedGear.DoesntProvide, GearLoadout.GetSlotType(slot.SlotID)) > -1)
                {
                    slot.gameObject.SetActive(false);
                }
                else
                {
                    slot.gameObject.SetActive(gameObject.activeSelf);
                }
            }
            else
            {
                slot.gameObject.SetActive(false);
            }
            slot.Refresh();
        }
    }

    private void UpdateDependants(bool enabled, InventoryCollection inventory)
    {
        Debug.Log("updating " + SlotID.ToString());
        foreach (EquipmentSlot slot in dependants)
        {
            bool setgear = false;
            if (enabled)
            {
                if (equippedGear != null)
                {
                    //check if the slot is of a type that the equipped gear doesn't provide
                    if (Array.IndexOf(equippedGear.DoesntProvide, GearLoadout.GetSlotType(slot.SlotID)) > -1)
                    {
                        slot.SetEquippedGear(null, inventory);
                        setgear = true;
                        slot.gameObject.SetActive(false);
                    }
                }
                else
                {
                    slot.SetEquippedGear(null, inventory);
                    setgear = true;
                }
            }
            else
            {
                slot.SetEquippedGear(null, inventory);
                setgear = true;
            }
            slot.gameObject.SetActive(enabled);
            if (!setgear)
            {
                slot.UpdateDependants(enabled, inventory);
            }
        }
    }

    public void ForceSetGear(GearData gear)
    {
        equippedGear = gear;
        if (gear != null)
        {
            icon.enabled = true;
            icon.sprite = gear.art;
        }
        else
        {
            icon.enabled = false;
        }
    }


    private void OnEnable()
    {
        Refresh();
    }

    private void OnDisable()
    {
        Refresh();
    }

}
