using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryList : MonoBehaviour
{
    private DeckListMember memberTemplate;
    private List<DeckListMember> activeMembers;

    private void Start()
    {
        memberTemplate = GetComponentInChildren<DeckListMember>(true);
        memberTemplate.gameObject.SetActive(false);
    }

    void Awake()
    {
        activeMembers = new List<DeckListMember>();
    }

    public void UpdateList(List<string> entries)
    {
        ClearList();
        foreach (string item in entries)
        {
            CreateGearListing(item);
        }
    }

    private void CreateGearListing(string text)
    {
        DeckListMember newMember = Instantiate(memberTemplate, transform);
        newMember.GetComponent<Text>().text = text;
        newMember.gameObject.SetActive(true);
        newMember.gameObject.name = text;
        //newMember.represents = card;
        //newMember.OnMouseOver += OnMouseOver;
        //newMember.OnMouseLeave += OnMouseLeave;
        activeMembers.Add(newMember);
    }

    private void ClearList()
    {
        while (activeMembers.Count > 0)
        {
            Destroy(activeMembers[activeMembers.Count - 1].gameObject);
            activeMembers.RemoveAt(activeMembers.Count - 1);
        }
    }
}
