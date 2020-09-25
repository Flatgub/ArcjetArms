using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentSelectionMenu : MonoBehaviour
{
    private Button buttonTemplate;
    private List<Button> activeButtons;
    public EquipmentScreenManager manager;
    private float animationtime = 0.1f;

    private void Start()
    {
        buttonTemplate = GetComponentInChildren<Button>(true);
        buttonTemplate.gameObject.SetActive(false);
        activeButtons = new List<Button>();
        transform.localScale = new Vector3(1, 0, 1);
    }

    public void PresentMenu(List<GearData> candidates, bool showUnequip = true)
    {
        if (activeButtons.Count > 0)
        {
            if (candidates.Count == 0 && !showUnequip)
            {
                CloseMenu();
                return;
            }
            else
            {
                ClearAllButtons();
            } 
        }

        if (showUnequip)
        {
            CreateButton("Uninstall", null, null);
        }
        foreach (GearData gear in candidates)
        {
            CreateButton(gear);
        }
        transform.LeanScaleY(1, animationtime).setEaseOutCirc();
    } 

    public void CreateButton(GearData gear)
    {
        CreateButton(gear.gearName, gear, gear.art);
    }

    public void CreateButton(string label, GearData gear, Sprite icon)
    {
        Button newButton = Instantiate(buttonTemplate, transform);
        newButton.GetComponentInChildren<Text>().text = label;
        if (icon != null)
        {
            Image img = newButton.transform.Find("GearIcon").GetComponent<Image>();
            img.enabled = true;
            img.sprite = icon;
        }
        newButton.onClick.AddListener(() => { manager.OnEquipmentSelectionMade(gear); });
        newButton.gameObject.SetActive(true);
        newButton.gameObject.name = label;
        activeButtons.Add(newButton);
    } 

    public void ClearAllButtons()
    {
        while (activeButtons.Count > 0)
        {
            Destroy(activeButtons[activeButtons.Count - 1].gameObject);
            activeButtons.RemoveAt(activeButtons.Count - 1);
        }
    }

    public void OnButtonClicked(Button button)
    {
        CloseMenu();
    }

    public void CloseMenu()
    {
        transform.LeanScaleY(0, animationtime).setEaseInCirc().setOnComplete(ClearAllButtons);
    }
}
