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
    private EquipmentSlot rootSlot = null;
    [SerializeField]
    private List<EquipmentSlot> slots = null;
    private GearLoadout activeLoadout;

    [SerializeField]
    private EquipmentSelectionMenu selectionMenu = null;
    [SerializeField]
    private DeckCardList deckList = null;
    [SerializeField]
    private InventoryList inventoryList = null;
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

    private InventoryCollection playerInventory = null;


    void Start()
    {
        CardDatabase.LoadAllCards();
        GearDatabase.LoadAllGear();

        if (GameplayContext.CurrentInventory is null || GameplayContext.RequestReset)
        {
            playerInventory = new InventoryCollection();
            //two legs
            playerInventory.AddItem(GearDatabase.GetGearDataByID(0));
            playerInventory.AddItem(GearDatabase.GetGearDataByID(0));
            //two melee arm
            playerInventory.AddItem(GearDatabase.GetGearDataByID(1), 2);
            //two rifle arm
            playerInventory.AddItem(GearDatabase.GetGearDataByID(3), 2);
            GameplayContext.CurrentInventory = playerInventory;
            Debug.Log("made new inventory");
        }
        else
        {
            playerInventory = GameplayContext.CurrentInventory;
            Debug.Log("copied inventory");
        } 
        
        
        
        Debug.Log("Current loadout: " + GameplayContext.CurrentLoadout);
        if (GameplayContext.CurrentLoadout != null && !GameplayContext.RequestReset)
        {
            activeLoadout = GameplayContext.CurrentLoadout;
            
            CopyFromLoadout(activeLoadout);
        }
        else
        {
            activeLoadout = new GearLoadout();
            GearLoadout template = new GearLoadout();
            template.EquipIntoSlot(GearDatabase.GetGearDataByID(2), LoadoutSlots.Body);
            CopyFromLoadout(template);
        }

        Invoke("RefreshVisuals", 0.01f);

        //activeLoadout = new GearLoadout();
        //GearLoadout template = new GearLoadout();
        //template.EquipIntoSlot(GearDatabase.GetGearDataByID(2), LoadoutSlots.Body);
        //CopyFromLoadout(template);

        SlotTitleText.enabled = false;
        GearTitleText.enabled = false;
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
        DeckTemplate template = activeLoadout.ToDeckTemplate();
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

    private void UpdateInventoryList()
    {
        List<string> items = new List<string>();
        foreach (GearData gear in playerInventory.GetAllGearTypes())
        {
            int count = playerInventory.GetCountOf(gear);
            string countstr = count <= 1 ? "" : "(x" + count + ")";
            string line = string.Format("{0} {1}", gear.gearName, countstr);
            items.Add(line);
        }
        inventoryList.UpdateList(items);
    }

    private void RefreshVisuals()
    {
        rootSlot.Refresh();
        UpdateInventoryList();
    }

    public void CopyFromLoadout(GearLoadout loadout)
    {
        foreach (EquipmentSlot slot in slots)
        {
            LoadoutSlots slotid = slot.SlotID;
            slot.ForceSetGear(loadout.slots[slotid].contains);
            slot.Refresh();
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
        List<GearData> availableGear = playerInventory.GetAllGearTypesOfSlot(type);
        selectionMenu.PresentMenu(availableGear, showUnequip: !slot.Empty);
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
            pendingSlot.SetEquippedGear(gear, playerInventory);
            if (gear != null)
            {
                playerInventory.RemoveItem(gear);
            }
            
        }
        pendingSlot = null;
        UpdateHeaderText(null);
        UpdateLoadout();
        UpdateInventoryList();
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
        SceneManager.LoadScene("OverworldScreen");
    }

    private void EmitSound(AudioClip sound)
    {
        audioPlayer.clip = sound;
        audioPlayer.pitch = Random.Range(0.9f, 1.1f);
        audioPlayer.Play();
    }

    public void SandboxCheat()
    {
        playerInventory = new InventoryCollection();
        GameplayContext.CurrentInventory = playerInventory;

        foreach (GearData gear in GearDatabase.GetAllGearData())
        {
            playerInventory.AddItem(gear, n: 3);
        }
        UpdateInventoryList();
    }
}
