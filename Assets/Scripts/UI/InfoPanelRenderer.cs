using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoPanelRenderer : MonoBehaviour
{
    [SerializeField]
    private Text titleText = null;
    [SerializeField]
    private Text bodyText = null;

    public string Title
    {
        get
        {
            return titleText.text;
        }
        set
        {
            titleText.text = value;
        }
    }

    public string Body
    {
        get
        {
            return bodyText.text;
        }
        set
        {
            bodyText.text = value;
        }
    }
}


