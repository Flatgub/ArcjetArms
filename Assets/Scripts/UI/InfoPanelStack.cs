using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoPanelStack : MonoBehaviour
{
    [SerializeField]
    private InfoPanelRenderer panelPrefab;

    private List<InfoPanelRenderer> panels;

    public void AddPanel(string title, string body)
    {
        InfoPanelRenderer panel = Instantiate(panelPrefab, transform);
        panel.Title = title;
        panel.Body = body;
        panels.Add(panel);
    }

    public void Clear()
    {
        while (panels.Count > 0)
        {
            InfoPanelRenderer panel = panels[0];
            Destroy(panel);
            panels.RemoveAt(0);
        }
    }



}
