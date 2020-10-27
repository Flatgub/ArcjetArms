using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RewardScreenItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    private Text gearTitle = null;

    [SerializeField]
    private Image gearSprite = null;

    [SerializeField]
    private Image frameSprite = null;

    public Color frameIdleColor;
    public Color frameSelectedColor;

    public GearData gear;

    public event Action OnMouseEnter;
    public event Action OnMouseExit;
    public event Action OnMouseClick;

    public void Start()
    {
        frameSprite.color = Color.white;
        frameSprite.CrossFadeColor(frameIdleColor, 0f, false, false);
    }

    public void SetGear(GearData gear)
    {
        this.gear = gear;
        gearTitle.text = gear.gearName;
        gearSprite.sprite = gear.art;
    }


    public void SetSelected(bool selected)
    {
        Color col = selected ? frameSelectedColor : frameIdleColor;
        frameSprite.CrossFadeColor(col, 0.07f, false, true);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        OnMouseEnter?.Invoke();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        OnMouseExit?.Invoke();
    }

    public void ClickButton()
    {
        OnMouseClick?.Invoke();
    }
}
