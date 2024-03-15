using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardStats : ScriptableObject
{
    public string cardName;
    public string description;
    public Sprite sprite;

    public CardType cardType;
}

public enum CardType
{
    Passive,
    Triggered
}
