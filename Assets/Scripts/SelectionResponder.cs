using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// SelectionResponder is a small helper class used for triggering onClick events in the owner
/// <see cref="InterfaceManager"/> 
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class SelectionResponder : MonoBehaviour
{
    public Hex position;
    public SpriteRenderer appearance;

    /// <summary>
    /// The event that's invoked when this SelectionResponder is clicked on
    /// </summary>
    public event Action<Hex> OnSelect;

    /// <summary>
    /// Initializes the SelectionResponder and <see cref=">InterfaceManager"/> <c>owner</c> to its
    /// <c>OnSelect</c> event
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="onSelect"></param>
    public void Initialize(Hex pos, Action<Hex> onSelect)
    {
        position = pos;
        OnSelect += onSelect;
    }

    public void OnMouseDown()
    {
        OnSelect?.Invoke(position);
    }
}
