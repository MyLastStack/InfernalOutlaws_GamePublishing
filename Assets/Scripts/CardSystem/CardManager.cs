using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static EventManager;

public class CardManager : MonoBehaviour
{
    public List<Card> cards = new List<Card>();
    public PlayerController player;

    private void Start()
    {
        AddCard.AddListener(AddCardToList);
    }

    void AddCardToList(Card card)
    {
        //Increase stacks if card is already owned, otherwise add it to the list
        if (cards.Where(x => x.cardName == card.cardName).Any())
        {
            var cardInList = cards.Find(x => x.cardName == card.cardName);
            cardInList.stacks++;
            CallCard(cardInList);
        }
        else
        {
            cards.Add(card);
            CallCard(cards[cards.Count - 1]);
        }
    }

    void CallCard(Card card)
    {
        if (card.GetStats().cardType == CardType.Passive)
        {
            card.CallCard(player);
        }
        else
        {
            card.SubscribeEvent();
        }
    }
}
