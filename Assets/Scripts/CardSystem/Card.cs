using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEditor;
using UnityEngine;
using static EventManager;

/// <summary>
/// A class that handles the functionality of cards in the game
/// </summary>
[Serializable]
public abstract class Card
{
    public const string CARD_ASSET_PATH = "ScriptableObjects/Cards/";
    public virtual string cardName { get; }
    
    public int stacks = 1;

    public virtual CardStats GetStats()
    {
        return Resources.Load<CardStats>(CARD_ASSET_PATH + cardName);
    }

    public virtual void CallCard(PlayerController player)
    {
        //This method is for when the card actually triggers its effect
    }

    public virtual void SubscribeEvent()
    {
        //This method is for subscribing a method to trigger on an event
    }

    public virtual void Update()
    {
        //This method is for any card that needs an update, such as cards that might have a cooldown for example
    }
}

#region Templates

public class PassiveTemplate : Card
{
    public override string cardName => "EnterNameHere"; //Make sure there is a card asset that corresponds to this in Resources/ScriptableObjects/Cards

    public override void CallCard(PlayerController player)
    {
        //Perform your functionality here
    }
}

public class TriggeredTemplate : Card
{
    public override string cardName => "EnterNameHere"; //Make sure there is a card asset that corresponds to this in Resources/ScriptableObjects/Cards

    public void CallCard(/*Enter Parameters for whatever event you're listening to*/)
    {
        //Perform your functionality here
    }

    public override void SubscribeEvent()
    {
        //Write the AddListener for your event here and hook it up to CallCard
    }
}

#endregion

#region Passive Cards

public class TwoOfBullets : Card
{
    public override string cardName => "Two of Bullets";

    public override void CallCard(PlayerController player)
    {
        StatModifier modifier = new StatModifier(0.15f + (0.1f * (stacks - 1)), ModifierType.PercentAdd, cardName);
        player.gun.stats.fireRate.AddModifier(modifier);
    }
}

public class AceOfBoots : Card
{
    public override string cardName => "Ace of Boots"; //Make sure there is a card asset that corresponds to this in Resources/ScriptableObjects/Cards

    public override void CallCard(PlayerController player)
    {
        StatModifier modifier = new StatModifier(0.15f + (0.08f * (stacks - 1)), ModifierType.PercentAdd, cardName);
        player.ps.jumpPowerStat.AddModifier(modifier);
    }
}

#endregion

#region Triggered Cards

public class TestCard : Card
{
    public override string cardName => "TestCard";

    public void CallCard(GameObject player)
    {
        //Apply modifiers when the event is triggered
        PlayerController controller = player.GetComponent<PlayerController>();
        StatModifier modifier = new StatModifier(0.15f + (0.08f * (stacks - 1)), ModifierType.PercentAdd, cardName);
        controller.ps.walkMaxMagnitudeStat.AddModifier(modifier);
        controller.ps.walkMoveSpeedStat.AddModifier(modifier);
    }

    public override void SubscribeEvent()
    {
        //Register the event to listen for
        Jump.AddListener(CallCard);
    }
}

public class OutlawOfBullets : Card
{
    public override string cardName => "Outlaw of Bullets"; //Make sure there is a card asset that corresponds to this in Resources/ScriptableObjects/Cards

    public void CallCard(GameObject obj)
    {
        PlayerController player = obj.GetComponent<PlayerController>();
        StatModifier modifier = new StatModifier(0.30f + (0.15f * (stacks - 1)), ModifierType.PercentAdd, cardName);
        player.gun.stats.fireRate.AddModifier(modifier);
    }

    public void EndEffect(GameObject obj)
    {
        PlayerController player = obj.GetComponent<PlayerController>();
        player.gun.stats.fireRate.RemoveModifier(cardName);
    }

    public override void SubscribeEvent()
    {
        PlayerDash.AddListener(CallCard);
        PlayerDashEnd.AddListener(EndEffect);
    }
}

#endregion

public enum Cards //After making a card, make sure to add its name to this list
{
    TestCard,
    TwoOfBullets,
    AceOfBoots,
    OutlawOfBullets
}