using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Card", menuName = "ScriptableObjects/Card", order = 2)]
public class CardStats : ScriptableObject
{
    public string cardName;
    [TextArea] public string description;
    public Sprite sprite;

    public CardType cardType;
}

public enum CardType
{
    Passive,
    Triggered
}
