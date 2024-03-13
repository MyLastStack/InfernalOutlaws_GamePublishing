using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Stat
{
    public float BaseValue; //The starting value of the stat

    private readonly List<StatModifier> modifiers; //The list of all modifiers of the stat (both flat and multiplicative)

    public float Value
    {
        get
        {
            if (hasChanged)
            {
                _lastVal = CalculateValue();
                hasChanged = false;
            }
            return _lastVal;
        }
    }

    private bool hasChanged = true;
    private float _lastVal;

    public Stat(float baseValue)
    {
        BaseValue = baseValue;
        modifiers = new List<StatModifier>();
    }

    public void SetBaseValue(float val)
    {
        hasChanged = true;
        BaseValue = val;
    }

    public void AddModifier(StatModifier mod)
    {
        hasChanged = true;
        modifiers.Add(mod);
        modifiers.Sort(CompareOrder);
    }

    public bool RemoveModifier(StatModifier mod)
    {
        hasChanged = true;
        return modifiers.Remove(mod);
    }

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

    public float CalculateValue()
    {
        float tempValue = BaseValue;
        float tempPercentAdd = 0;

        for (int i = 0; i < modifiers.Count; i++)
        {
            var curMod = modifiers[i];

            if (curMod.Type == ModifierType.Flat)
            {
                tempValue += curMod.Value;
            }
            else if(curMod.Type == ModifierType.PercentMult)
            {
                tempValue *= curMod.Value;
            } else
            {
                tempPercentAdd += curMod.Value;

                if (i + 1 > modifiers.Count || modifiers[i + 1].Type != ModifierType.PercentAdd)
                {
                    tempValue *= 1 + tempPercentAdd;
                    tempPercentAdd = 0;
                }
            }
        }

        return (float)Math.Round(tempValue, 4);
    }
}

public class StatModifier
{
    public readonly float Value;
    public ModifierType Type;
    public int Order;

    public StatModifier(float value, ModifierType type, int order)
    {
        Value = value;
        Type = type;
        Order = order;
    }

    public StatModifier(float value, ModifierType type) : this(value, type, (int)type) { }
}

public enum ModifierType
{
    Flat,
    PercentAdd,
    PercentMult
}
