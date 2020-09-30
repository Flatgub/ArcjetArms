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
    private List<EquipmentSlot> slots = null;
    private GearLoadout activeLoadout;

    [SerializeField]
    private EquipmentSelectionMenu selectionMenu = null;
    [SerializeField]
    private DeckCardList deckList = null;
    [SerializeField]
    private Text SlotTitleText = null;
    [SerializeField]
    private Text GearTitleText = null;

    [SerializeField]
    private AudioClip partAttachNoise = null;
    [SerializeField]
    private AudioClip partChangeRemoveNoise = null;
    //[SerializeField]
    //private AudioClip menuOpenNoise = null;
    [SerializeField]
    private AudioSource audioPlayer = null;

    private EquipmentSlot pendingSlot = null;
    
    void Start()
    {
        CardDatabase.LoadAllCards();
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

    private void UpdateLoadout()
    {
        foreach (EquipmentSlot slot in slots)
        {
            LoadoutSlots slotID = slot.SlotID;
            activeLoadout.UnequipFromSlot(slotID);
            if (!slot.Empty && slot.isActiveAndEnabled)
            {
                GearData gear = slot.GetEquippedGear();
                activeLoadout.EquipIntoSlot(gear, slotID);
            }
        }

        UpdateDeckList();
    }

    private void UpdateDeckList()
    {
        DeckTemplate template = activeLoadout.LoadoutToDeckTemplate();
        Dictionary<string, CardData> cards = new Dictionary<string, CardData>();
        foreach (KeyValuePair<int, int> pair in template)
        {
            CardData card = CardDatabase.GetCardDataByID(pair.Key);
            string line = string.Format("{0} (x{1})", card.title, pair.Value);
            cards.Add(line, card);
        }
        if (cards.Count == 0)
        {
            cards.Add("Nothing", null);
        }
        deckList.UpdateList(cards);
    }

    public void CopyFromLoadout(GearLoadout loadout)
    {
        foreach (EquipmentSlot slot in slots)
        {
            LoadoutSlots slotid = slot.SlotID;
            slot.SetEquippedGear(loadout.slots[slotid].contains);
        }

        UpdateLoadout();
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
        //EmitSound(menuOpenNoise);
        pendingSlot = slot;

        UpdateHeaderText(slot);
    }

    public void OnEquipmentSelectionMade(GearData gear)
    {
        if (pendingSlot != null)
        {
            if (pendingSlot.Empty && gear != null)
            {
                EmitSound(partAttachNoise);
            } 
            else 
            {
                EmitSound(partChangeRemoveNoise);
            }
            pendingSlot.SetEquippedGear(gear);
        }
        pendingSlot = null;
        UpdateHeaderText(null);
        UpdateLoadout();
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

    private void EmitSound(AudioClip sound)
    {
        audioPlayer.clip = sound;
        audioPlayer.pitch = Random.Range(0.9f, 1.1f);
        audioPlayer.Play();
    }
}
