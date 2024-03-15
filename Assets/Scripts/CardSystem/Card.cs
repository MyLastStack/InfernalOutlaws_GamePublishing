using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEditor;
using UnityEngine;
using static EventManager;

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

    public virtual void CallCard(PlayerController player, int stacks)
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

    public override void CallCard(PlayerController player, int stacks)
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


#region Cards

public class TestCard : Card
{
    public override string cardName => "TestCard";

    public void CallCard(GameObject player)
    {
        PlayerController controller = player.GetComponent<PlayerController>();
        StatModifier modifier = new StatModifier(0.15f + (0.08f * (stacks - 1)), ModifierType.PercentAdd, cardName);
        controller.ps.walkMaxMagnitudeStat.AddModifier(modifier);
        controller.ps.walkMoveSpeedStat.AddModifier(modifier);
    }

    public override void SubscribeEvent()
    {
        Jump.AddListener(CallCard);
    }
}

#endregion