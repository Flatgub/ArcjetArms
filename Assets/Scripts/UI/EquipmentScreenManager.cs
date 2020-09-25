using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static GearLoadout;

public class EquipmentScreenManager : MonoBehaviour
{
    [SerializeField]
    private List<EquipmentSlot> slots;
    private GearLoadout activeLoadout;

    [SerializeField]
    private EquipmentSelectionMenu selectionMenu = null;
    [SerializeField]
    private Text SlotTitleText = null;
    [SerializeField]
    private Text GearTitleText = null;

    private EquipmentSlot pendingSlot = null;
    
    void Start()
    {
        GearDatabase.LoadAllGear();
        activeLoadout = new GearLoadout();
        SlotTitleText.enabled = false;
        GearTitleText.enabled = false;

        GearLoadout template = new GearLoadout();
        template.EquipIntoSlot(GearDatabase.GetGearDataByID(2), LoadoutSlots.Body);
        CopyFromLoadout(template);
    }

    private void Update()
    {
        if (Input.GetMouseButtonUp(0) && !EventSystem.current.IsPointerOverGameObject() 
            && pendingSlot != null)
        {
            selectionMenu.CloseMenu();
            pendingSlot = null;
        }
    }

    // Update is called once per frame
    private void UpdateLoadout()
    {
        foreach (EquipmentSlot slot in slots)
        {
            LoadoutSlots slotID = slot.SlotID;
            activeLoadout.UnequipFromSlot(slotID);
            if (!slot.Empty)
            {
                GearData gear = slot.GetEquippedGear();
                activeLoadout.EquipIntoSlot(gear, slotID);
            }
        }
    }

    public void CopyFromLoadout(GearLoadout loadout)
    {
        foreach (EquipmentSlot slot in slots)
        {
            LoadoutSlots slotid = slot.SlotID;
            slot.SetEquippedGear(loadout.slots[slotid].contains);
        }
    }

    private void SetHeaderText(string slotname, string gearname)
    {
        SlotTitleText.enabled = (slotname.Length > 0);
        SlotTitleText.text = slotname;
        GearTitleText.enabled = (gearname.Length > 0);
        GearTitleText.text = gearname;
    }

    public void UpdateHeaderText(EquipmentSlot slot)
    {
        if (slot != null)
        {
            string slotname = slot.SlotID.ToString();
            string gearname = slot.GetEquippedGear()?.gearName ?? "Empty";
            SetHeaderText(slotname, gearname);
        }
        else if (pendingSlot != null)
        {
            string slotname = pendingSlot.SlotID.ToString();
            string gearname = pendingSlot.GetEquippedGear()?.gearName ?? "Empty";
            SetHeaderText(slotname, gearname);
        }
        else
        {
            SetHeaderText("", "");
        }
    }

    public void OnSlotClicked(EquipmentSlot slot)
    {

        GearSlotTypes type = GearLoadout.GetSlotType(slot.SlotID);
        selectionMenu.PresentMenu(GearDatabase.GetAllGearBySlotType(type), showUnequip: !slot.Empty);
        pendingSlot = slot;

        UpdateHeaderText(slot);
    }

    public void OnEquipmentSelectionMade(GearData gear)
    {
        if (pendingSlot != null)
        {
            pendingSlot.SetEquippedGear(gear);
        }
        pendingSlot = null;
        UpdateHeaderText(null);
    }

    public void OnSlotMousedOver(EquipmentSlot slot)
    {
        UpdateHeaderText(slot);
    }

    public void OnSlotMouseLeave()
    {
        UpdateHeaderText(null);
    }

    public void OnMenuLeave()
    {
        UpdateLoadout();
        GameplayContext.CurrentLoadout = activeLoadout;
        SceneManager.LoadScene("CombatEncounter");
    }
}
