using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardPickupManager : MonoBehaviour
{
    public CardPickup[] cards;

    public CardStats[] cardSet1;
    public CardStats[] cardSet2;
    public CardStats[] cardSet3;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Q))
        {
            cards[0].card = cardSet1[0];
            cards[1].card = cardSet1[1];
            cards[2].card = cardSet1[2];
            foreach (var card in cards)
            {
                card.UpdateText();
            }
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            cards[0].card = cardSet2[0];
            cards[1].card = cardSet2[1];
            cards[2].card = cardSet2[2];
            foreach (var card in cards)
            {
                card.UpdateText();
            }
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            cards[0].card = cardSet3[0];
            cards[1].card = cardSet3[1];
            cards[2].card = cardSet3[2];
            foreach(var card in cards)
            {
                card.UpdateText();
            }
        }

        if(Input.GetKeyDown(KeyCode.F))
        {
            gameObject.SetActive(false);
        }
    }
}
