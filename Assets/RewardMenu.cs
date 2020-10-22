using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static GearData;

public class RewardMenu : MonoBehaviour
{
    private bool initialized = false;
    public float appearSpeed = 0.25f;

    [SerializeField]
    private CanvasGroup selfGroup = null;

    public Transform rewardContainer;
    public Transform previewContainer;

    [SerializeField]
    private RewardScreenItem rewardScreenTemplate = null;
    [SerializeField]
    private CardRenderer cardPreviewTemplate = null;

    private List<RewardScreenItem> activeOptions;
    private List<CardRenderer> activePreviews;
    private RewardScreenItem selectedOption = null;

    [SerializeField]
    private Button confirmButton = null;

    // Start is called before the first frame update
    void Start()
    {
        if (!initialized)
        {
            Init();
        }
    }

    public void Init()
    {
        activePreviews = new List<CardRenderer>();
        activeOptions = new List<RewardScreenItem>();

        rewardScreenTemplate.gameObject.SetActive(false);
        cardPreviewTemplate.gameObject.SetActive(false);

        confirmButton.interactable = false;
        initialized = true;
    }

    public void ShowRewardMenu()
    {
        gameObject.SetActive(true);
        selfGroup.interactable = true;
        selfGroup.blocksRaycasts = true;
        selfGroup.LeanAlpha(1.0f, appearSpeed);
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

