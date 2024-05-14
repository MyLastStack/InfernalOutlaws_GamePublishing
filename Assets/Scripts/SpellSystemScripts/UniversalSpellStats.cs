using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UniversalSpellStats : MonoBehaviour
{
    public Stat SpellCooldown = new Stat(2);
    public Stat SpellDamage = new Stat(1);
    public Stat Duration = new Stat(1);
    public Stat TickRate = new Stat(1);
    public Stat SpellSize = new Stat(1);
    public Stat SpellPotency = new Stat(1);
    public Stat ProjectileSpeed = new Stat(1);
    public Stat MaxCount = new Stat(1);
    public Stat Charges = new Stat(1);
}
