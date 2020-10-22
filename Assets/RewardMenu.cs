using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static GearData;

public class RewardMenu : MonoBehaviour
{
    public Transform rewardContainer;
    public Transform previewContainer;

    private RewardScreenItem rewardScreenTemplate;
    private CardRenderer cardPreviewTemplate;

    private List<RewardScreenItem> activeOptions;
    private List<CardRenderer> activePreviews;
    private RewardScreenItem selectedOption = null;

    [SerializeField]
    private Button confirmButton = null;

    // Start is called before the first frame update
    void Start()
    {
        activePreviews = new List<CardRenderer>();
        activeOptions = new List<RewardScreenItem>();

        rewardScreenTemplate = rewardContainer.GetComponentInChildren<RewardScreenItem>();
        cardPreviewTemplate = previewContainer.GetComponentInChildren<CardRenderer>();

        rewardScreenTemplate.gameObject.SetActive(false);
        cardPreviewTemplate.gameObject.SetActive(false);

        GearDatabase.LoadAllGear();
        AddRewardOption(GearDatabase.GetGearDataByID(7));
        AddRewardOption(GearDatabase.GetGearDataByID(10));
        AddRewardOption(GearDatabase.GetGearDataByID(106));
        confirmButton.interactable = false;
    }

    public void AddRewardOption(GearData newGear)
    {
        RewardScreenItem option = Instantiate(rewardScreenTemplate, rewardContainer);
        option.SetGear(newGear);
        option.gameObject.SetActive(true);
        option.OnMouseEnter += () => { MouseOverOption(newGear); };
        option.OnMouseExit += () => { MouseLeaveOption(); };
        option.OnMouseClick += () => { MouseSelectOption(option); };
        activeOptions.Add(option);
    }

    public void MouseOverOption(GearData gear)
    {
        if (activePreviews.Count != 0)
        {
            ClearPreviews();
        }

        foreach (CardBundle card in gear.Cards)
        {
            CreateCardPreview(card.card, card.amount);
        }
        
    }

    public void MouseLeaveOption()
    {
        ClearPreviews();
        if (selectedOption != null)
        {
            MouseOverOption(selectedOption.gear);
        }
    }

    private void ClearPreviews()
    {
        foreach (CardRenderer preview in activePreviews)
        {
            Destroy(preview.gameObject);
        }
        activePreviews.Clear();
    }

    public void MouseSelectOption(RewardScreenItem item)
    {
        item.SetSelected(true);
        foreach (RewardScreenItem other in activeOptions)
        {
            if (other != item)
            {
                other.SetSelected(false);
            } 
        }
        selectedOption = item;
        confirmButton.interactable = true;
    }

    private void CreateCardPreview(CardData card, int count)
    {
        CardRenderer preview = Instantiate(cardPreviewTemplate, previewContainer);
        preview.ShowCardData(card);
        preview.gameObject.SetActive(true);
        Text counttext = preview.transform.Find("CountText")?.GetComponent<Text>();
        counttext.text = string.Format("(x{0})", count);
        activePreviews.Add(preview);
    }

    public void ConfirmButtonClick()
    {
        GameplayContext.CurrentInventory?.AddItem(selectedOption.gear);
        GameplayContext.Manager.ReturnToLoadoutScreen();
    }
}

