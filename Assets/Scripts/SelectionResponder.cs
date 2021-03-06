﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// SelectionResponder is a small helper class used for triggering onClick events.
/// </summary>
/// <seealso cref="SingleHexSelection"/> 
/// <seealso cref="SingleEntitySelection"/> 
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
    /// Initializes the SelectionResponder
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
