using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static EventManager;

public class CardPickup : MonoBehaviour
{
    public GameObject manager;
    public CardStats[] cardPool;
    CardStats card;

    [SerializeField] TextMeshProUGUI titleText;
    [SerializeField] TextMeshProUGUI descriptionText;
    [SerializeField] Image image;

    Card cardScript;

    private void OnEnable() //This will re-roll the card every time it's enabled, meaning you don't have to re-instantiate
    {
        Time.timeScale = 0;
        MouseLocker.Unlock();
        card = cardPool[Random.Range(0, cardPool.Length)];

        titleText.text = card.name;
        descriptionText.text = card.description;
        image.sprite = card.sprite;
        AssignCard();
    }

    void AssignCard() //This needs to be updated whenever new cards are added
    {
        switch (card.cardScript)
        {
            case Cards.TestCard:
                cardScript = new TestCard();
                break;
            case Cards.TwoOfBullets:
                cardScript = new TwoOfBullets();
                break;
            case Cards.AceOfBoots:
                cardScript = new AceOfBoots();
                break;
            case Cards.OutlawOfBullets:
                cardScript = new OutlawOfBullets();
                break;
            case Cards.ThreeOfBadges:
                cardScript = new ThreeOfBadges();
                break;
        }
    }

    public void GetCard()
    {
        AddCard.Invoke(cardScript);
        manager.SetActive(false);
    }
}
