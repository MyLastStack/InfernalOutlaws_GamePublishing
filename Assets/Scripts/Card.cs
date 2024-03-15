using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EventManager;

[System.Serializable]
public abstract class Card
{
    public CardStats stats; //scriptable object
    public int stacks;

    public virtual void CallCard(PlayerController player, int stacks)
    {

    }

    public virtual void SubscribeEvent()
    {

    }
}


#region Items 

public class TestCard : Card
{
    public void CallCard(GameObject player)
    {
        PlayerController controller = player.GetComponent<PlayerController>();
        StatModifier modifier = new StatModifier(0.15f + (0.8f * stacks), ModifierType.PercentAdd);
        controller.ps.walkMaxMagnitudeStat.AddModifier(modifier);
    }

    public override void SubscribeEvent()
    {
        Jump.AddListener(CallCard);
    }
}

#endregion