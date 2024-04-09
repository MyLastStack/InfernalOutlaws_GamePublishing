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
            case Cards.FourOfLassos:
                cardScript = new FourOfLassos();
                break;
            case Cards.ThreeOfBullets:
                cardScript = new ThreeOfBullets();
                break;
            case Cards.EightOfBadges:
                cardScript = new EightOfBadges();
                break;
            case Cards.FourOfBullets:
                cardScript = new FourOfBullets();
                break;
            case Cards.OutlawOfBadges:
                cardScript = new OutlawOfBadges();
                break;
            case Cards.SheriffOfBullets:
                cardScript = new SheriffOfBullets();
                break;
            case Cards.SevenOfLassos:
                cardScript = new SevenOfLassos();
                break;
            case Cards.SixOfBadges:
                cardScript = new SixOfBadges();
                break;
            case Cards.ThreeOfLassos:
                cardScript = new ThreeOfLassos();
                break;
            case Cards.DeputyOfBullets:
                cardScript = new DeputyOfBullets();
                break;
            case Cards.NineOfLassos:
                cardScript = new NineOfLassos();
                break;
            case Cards.SheriffOfBoots:
                cardScript = new SheriffOfBoots();
                break;
            case Cards.ThreeOfBoots:
                cardScript = new ThreeOfBoots();
                break;
            case Cards.FiveOfBadges:
                cardScript = new FiveOfBadges();
                break;
            case Cards.FourOfBadges:
                cardScript = new FourOfBadges();
                break;
            case Cards.JesterOfBullets:
                cardScript = new JesterOfBullets();
                break;
            case Cards.TenOfBadges:
                cardScript = new TenOfBadges();
                break;
        }
    }

    public void GetCard()
    {
        AddCard.Invoke(cardScript);
        manager.SetActive(false);
    }
}
