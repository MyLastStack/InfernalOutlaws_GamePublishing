using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Stat
{
    public float BaseValue; //The starting value of the stat

    private List<StatModifier> modifiers = new List<StatModifier>(); //The list of all modifiers of the stat (both flat and multiplicative)

    private bool hasNotChanged = false;
    private float _lastVal;

    public float Value
    {
        get
        {
            Debug.Log(hasNotChanged);
            if (!hasNotChanged)
            {
                _lastVal = CalculateValue();
                Debug.Log("Yo");
                hasNotChanged = true;
            }
            return _lastVal;
        }
    }

    public Stat(float baseValue)
    {
        BaseValue = baseValue;
        modifiers = new List<StatModifier>();
        hasNotChanged = false;
    }

    public void SetBaseValue(float val)
    {
        hasNotChanged = false;
        BaseValue = val;
    }

    public void AddModifier(StatModifier mod)
    {
        hasNotChanged = false;
        modifiers.Add(mod);
        modifiers.Sort(CompareOrder);
    }

    public bool RemoveModifier(StatModifier mod)
    {
        hasNotChanged = true;
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
        if(modifiers == null)
        {
            modifiers = new List<StatModifier>();
        }

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
                    tempValue *= (1 + tempPercentAdd);
                    tempPercentAdd = 0;
                }
            }
        }

        Debug.Log("tempValue: " + tempValue);
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
