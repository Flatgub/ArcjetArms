using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DeckListMember : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public CardData represents;
    public event Action<DeckListMember> OnMouseOver;
    public event Action OnMouseLeave;

    public void OnPointerEnter(PointerEventData eventData)
    {
        OnMouseOver?.Invoke(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        OnMouseLeave?.Invoke();
    }
}
