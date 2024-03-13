using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static EventManager;

public class CardSystem : MonoBehaviour
{
    public Card card;
    // Start is called before the first frame update
    void Start()
    {
        Jump.AddListener(CallCard);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CallCard(GameObject test)
    {
        card.TriggeredFunction.Invoke();
    }

    #region Card Effects

    public void Heal()
    {
        for (int i = 0; i < card.Quantity; i++)
        {
            Debug.Log("Waow you healedededed");
        }
    }

    #endregion
}

[Serializable]
public struct Card
{
    public string Name;
    public int Quantity;
    public CardType Type;
    public UnityEvent TriggeredFunction;
}

public enum CardType
{
    Passive,
    Triggered
}

public enum Event
{

}
