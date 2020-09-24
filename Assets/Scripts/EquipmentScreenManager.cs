using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static GearLoadout;

public class EquipmentScreenManager : MonoBehaviour
{
    [SerializeField]
    private List<EquipmentSlot> slots;
    private GearLoadout activeLoadout;

    [SerializeField]
    private Text SlotTitleText = null;
    [SerializeField]
    private Text GearTitleText = null;

    void Start()
    {
        GearDatabase.LoadAllGear();
        activeLoadout = new GearLoadout();
        SlotTitleText.enabled = false;
        GearTitleText.enabled = false;
    }

    // Update is called once per frame
    private void UpdateLoadout()
    {
        
    }
    public void UpdateHeaderText(EquipmentSlot slot)
    {
        if (slot != null)
        {
            SlotTitleText.enabled = true;
            SlotTitleText.text = slot.SlotID.ToString();
            if (!slot.Empty)
            {
                GearTitleText.enabled = true;
                GearTitleText.text = slot.GetEquippedGear().gearName;
            }
        }
        else
        {
            SlotTitleText.enabled = false;
            SlotTitleText.text = "";
            GearTitleText.enabled = false;
            GearTitleText.text = "";
        }
    }

    public void OnSlotClicked(EquipmentSlot slot)
    {
        if (slot.Empty)
        {
            GearSlotTypes type = GearLoadout.GetSlotType(slot.SlotID);
            switch (type)
            {
                case GearSlotTypes.Arm:
                    slot.SetEquippedGear(GearDatabase.GetGearDataByID(1));
                    break;

                case GearSlotTypes.Leg:
                    slot.SetEquippedGear(GearDatabase.GetGearDataByID(0));
                    break;
            }
            
        }
        else
        {
            slot.SetEquippedGear(null);
        }

        UpdateHeaderText(slot);
    }

    public void OnSlotMousedOver(EquipmentSlot slot)
    {
        UpdateHeaderText(slot);
    }

    public void OnSlotMouseLeave()
    {
        UpdateHeaderText(null);
    }
}
