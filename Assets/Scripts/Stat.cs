using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class Stat
{
    public float BaseValue; //The starting value of the stat

    private List<StatModifier> modifiers = new List<StatModifier>(); //The list of all modifiers of the stat (both flat and multiplicative)

    private bool hasNotChanged = false;
    private float _lastVal;

    public float Value //The final value after modifiers
    {
        get
        {
            if (!hasNotChanged) //Check if the item has a new modifier or not
            {
                _lastVal = CalculateValue();
                hasNotChanged = true;
            }
            return _lastVal;
        }
    }

    public int iValue
    {
        get
        {
            return Mathf.RoundToInt(Value);
        }
    }

    public Stat(float baseValue) //Constructor
    {
        BaseValue = baseValue;
        modifiers = new List<StatModifier>();
        hasNotChanged = false;
    }

    //Changes base value of the stat. DO NOT use this to add modifiers to the stat!
    public void SetBaseValue(float val)
    {
        hasNotChanged = false;
        BaseValue = val;
    }

    //Add a new modifier to the stat, additive, multiplicative, or exponential
    public void AddModifier(StatModifier mod)
    {
        if (modifiers == null) //If the list hasn't been initialized yet
        {
            modifiers = new List<StatModifier>();
        }

        hasNotChanged = false;

        modifiers.RemoveAll(x => x.Source == mod.Source); //Remove any existing modifiers from the same source first

        modifiers.Add(mod);
        modifiers.Sort(CompareOrder);
    }

    //Removes a modifier
    public bool RemoveModifier(StatModifier mod)
    {
        if (modifiers == null) //If the list hasn't been initialized yet
        {
            modifiers = new List<StatModifier>();
        }

        hasNotChanged = true;
        return modifiers.Remove(mod);
    }

    public void ClearModifiers()
    {
        hasNotChanged = false;
        if (modifiers != null)
        {
            modifiers.Clear();
        }
    }

    //Sets the order based on the StatModifier.Order variable
    private int CompareOrder(StatModifier a, StatModifier b)
    {
        if (a.Order > b.Order)
        {
            return 1;
        }
        else if (b.Order > a.Order)
        {
            return -1;
        }
        else
        {
            return 0;
        }
    }

    //Calculate the final value
    public float CalculateValue()
    {
        if (modifiers == null) //If the list hasn't been initialized yet
        {
            modifiers = new List<StatModifier>();
        }

        float tempValue = BaseValue;
        float tempPercentAdd = 0;

        for (int i = 0; i < modifiers.Count; i++) //Go through the list and add the modifiers
        {
            var curMod = modifiers[i];

            if (curMod.Type == ModifierType.Flat) //For flat additive modifiers (1 + 1 = 2)
            {
                tempValue += curMod.Value;
            }
            else if (curMod.Type == ModifierType.PercentMult) //For exponential percent multiplication (100% + 100% = 400%)
            {
                tempValue *= (1 + curMod.Value);
            }
            else //For additive percent multiplication (100% + 100% = 200%)
            {
                //Add to a temporary variable for additive multiplication
                tempPercentAdd += curMod.Value;

                //Check if the value is at the end of the array or the next value is not percent add
                //If so, apply the final calculation
                if (i + 1 > modifiers.Count - 1 || modifiers[i + 1].Type != ModifierType.PercentAdd)
                {
                    tempValue *= (1 + tempPercentAdd);
                    tempPercentAdd = 0;
                }
            }
        }

        return (float)Math.Round(tempValue, 4);
    }
}

public class StatModifier
{
    public readonly float Value; //The value to add/multiply by
    public ModifierType Type; //What type of operation is it?
    public int Order; //What order will the operation be applied in the list? (By default: 1: Flat 2: PercentAdd 3: PercentMult)
    public string Source; //What created this modifier?

    public StatModifier(float value, ModifierType type, int order, string source) //Constructor
    {
        Value = value;
        Type = type;
        Order = order;
        Source = source;
    }

    public StatModifier(float value, ModifierType type, string source) : this(value, type, (int)type, source) { } //Overload constructor that defaults the order
}

public enum ModifierType
{
    Flat,
    PercentAdd,
    PercentMult
}
