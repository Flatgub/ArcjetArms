using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class HealthBar : MonoBehaviour
{
    [SerializeField]
    public Slider slideController;
    [SerializeField]
    private Image backFill = null;
    [SerializeField]
    private Image frontFill = null;

    private Color colour;
    public Color Colour
    {
        get
        {
            return colour;
        }
        set
        {
            colour = value;
            UpdateColours();
        }
    }


    private float maxVal = 1;
    private float curVal = 1;
    public float Value
    {
        get
        {
            return curVal;
        }
        set
        {
            curVal = value;
            UpdateSlider();
        }
    }
    public float MaxValue
    {
        get
        {
            return maxVal;
        }
        set
        {
            UpdateMaxValue(value);
        }
    }

    private void UpdateMaxValue(float newMax)
    {
        float frac = curVal / maxVal;
        float newfrac = frac * newMax;
        maxVal = newMax;
        curVal = newfrac;
        UpdateSlider();
    }

    private void UpdateSlider()
    {
        slideController.value = curVal / maxVal;
    }

    private void UpdateColours()
    {
        backFill.color = colour;
        frontFill.color = colour;
    }
}
