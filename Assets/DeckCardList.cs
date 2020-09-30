using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DeckCardList : MonoBehaviour
{
    private DeckListMember memberTemplate;
    private List<DeckListMember> activeMembers;
    [SerializeField]
    private CardRenderer visualizer = null;

    // Start is called before the first frame update
    void Start()
    {
        memberTemplate = GetComponentInChildren<DeckListMember>(true);
        memberTemplate.gameObject.SetActive(false);
        activeMembers = new List<DeckListMember>();
        visualizer.gameObject.SetActive(false);
    }

    public void UpdateList(Dictionary<string, CardData> cards)
    {
        ClearList();
        foreach (KeyValuePair<string, CardData> card in cards)
        {
            CreateCardListing(card.Key, card.Value);
        }
    }

    private void CreateCardListing(string text, CardData card)
    {
        DeckListMember newMember = Instantiate(memberTemplate, transform);
        newMember.GetComponent<Text>().text = text;
        newMember.gameObject.SetActive(true);
        newMember.gameObject.name = text;
        newMember.represents = card;
        newMember.OnMouseOver += OnMouseOver;
        newMember.OnMouseLeave += OnMouseLeave;
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

    public void OnMouseOver(DeckListMember member)
    {
        if (member.represents != null)
        {
            visualizer.gameObject.SetActive(true);
            visualizer.ShowCardData(member.represents);
            visualizer.transform.position = member.transform.position - new Vector3(0, 12f, 0);
        }
    }
    public void OnMouseLeave()
    {
        visualizer.gameObject.SetActive(false);
    }
}

