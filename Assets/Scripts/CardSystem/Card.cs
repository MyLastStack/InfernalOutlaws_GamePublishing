using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;
using static EventManager;
using Object = UnityEngine.Object;

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
public class ThreeOfBullets : Card
{
    public override string cardName => "Three of Bullets";

    public override void CallCard(PlayerController player)
    {
        StatModifier modifier = new StatModifier(0.15f + (0.1f * (stacks - 1)), ModifierType.PercentAdd, cardName);
        player.gun.stats.damage.AddModifier(modifier);
    }
}
public class FourOfBullets : Card
{
    public override string cardName => "Four of Bullets";

    public override void CallCard(PlayerController player)
    {
        StatModifier modifier = new StatModifier(1f + (1f * (stacks - 1)), ModifierType.Flat, cardName);
        player.gun.stats.damage.AddModifier(modifier);
    }
}
public class AceOfBoots : Card
{
    public override string cardName => "Ace of Boots";

    public override void CallCard(PlayerController player)
    {
        StatModifier modifier = new StatModifier(0.15f + (0.08f * (stacks - 1)), ModifierType.PercentAdd, cardName);
        player.ps.jumpPowerStat.AddModifier(modifier);
    }
}
public class FourOfLassos : Card
{
    public override string cardName => "Four of Lassos";
    private const float MAX_VAL = 1f;
    public override void CallCard(PlayerController player)
    {
        StatModifier modifier1 = new StatModifier(0.25f + (0.12f * (stacks - 1)), ModifierType.PercentAdd, cardName);

        float modifierVal = 0;
        for (int i = 0; i < stacks; i++)
        {
            modifierVal += (MAX_VAL - modifierVal + 0.001f) / 10;
            modifierVal = Mathf.Min(modifierVal, MAX_VAL);
        }
        StatModifier modifier2 = new StatModifier(-modifierVal, ModifierType.PercentAdd, cardName);

        player.gun.stats.damage.AddModifier(modifier1);
        player.gun.stats.fireRate.AddModifier(modifier2);
    }
}
public class EightOfBadges : Card
{
    public override string cardName => "Eight of Badges";

    public override void CallCard(PlayerController player)
    {
        float baseVal = 1f;
        for (int i = 0; i < stacks; i++)
        {
            baseVal *= 0.9f;
        } //This applies a stacking multiplicative -10%
        StatModifier modifier = new StatModifier(baseVal - 1f, ModifierType.PercentMult, cardName);
        player.ps.shieldCooldownStat.AddModifier(modifier);
    }
}
public class SevenOfLassos : Card
{
    public override string cardName => "Seven of Lassos";

    public override void CallCard(PlayerController player)
    {
        StatModifier modifier = new StatModifier(10 * stacks, ModifierType.Flat, cardName);
        player.ps.maxHealthStat.AddModifier(modifier);
        player.ps.health += 10;
    }
}
public class SixOfBadges : Card
{
    public override string cardName => "Six of Badges";

    public override void CallCard(PlayerController player)
    {
        StatModifier modifier = new StatModifier(5 * stacks, ModifierType.Flat, cardName);
        player.ps.maxShieldStat.AddModifier(modifier);
    }
}
public class ThreeOfLassos : Card
{
    public override string cardName => "Three of Lassos";

    public override void CallCard(PlayerController player)
    {
        StatModifier modifier = new StatModifier(0.4f * stacks, ModifierType.PercentAdd, cardName);
        player.ps.shieldRegenSpeedStat.AddModifier(modifier);
    }
}
public class DeputyOfBullets : Card
{
    public override string cardName => "Deputy of Bullets";

    public override void CallCard(PlayerController player)
    {
        float calculatedVal = player.ps.maxHealth;
        calculatedVal = (calculatedVal - calculatedVal % 10) / 10;
        StatModifier modifier = new StatModifier(0.01f * calculatedVal * stacks, ModifierType.PercentAdd, cardName);
        player.gun.stats.damage.AddModifier(modifier);
    }
}
public class ThreeOfBoots : Card
{
    public override string cardName => "Three of Boots";

    public override void CallCard(PlayerController player)
    {
        StatModifier modifier = new StatModifier(0.15f * stacks, ModifierType.PercentAdd, cardName);
        player.ps.walkMoveSpeedStat.AddModifier(modifier);
        player.ps.dashMoveSpeedStat.AddModifier(modifier);
        player.ps.walkMaxMagnitudeStat.AddModifier(modifier);
        player.ps.dashMaxMagnitudeStat.AddModifier(modifier);
    }
}
public class TenOfBadges : Card
{
    public override string cardName => "Ten of Badges";

    public override void CallCard(PlayerController player)
    {
        float baseVal = 1;
        for (int i = 0; i < stacks; i++)
        {
            baseVal *= 0.75f;
        }
        StatModifier mod = new StatModifier(baseVal - 1, ModifierType.PercentAdd, cardName);
        player.gun.stats.spread.AddModifier(mod);
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
    public override string cardName => "Outlaw of Bullets";

    public void CallCard(GameObject obj)
    {
        PlayerController player = obj.GetComponent<PlayerController>();
        StatModifier modifier = new StatModifier(0.50f * stacks, ModifierType.PercentAdd, cardName);
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
public class ThreeOfBadges : Card
{

    Timer abilityDurationTimer = new Timer(5);
    PlayerController player;
    public override string cardName => "Three of Badges";

    public void CallCard(GameObject obj)
    {
        abilityDurationTimer.Unpause();
        abilityDurationTimer.Reset();
        player = obj.GetComponent<PlayerController>();
        StatModifier modifier = new StatModifier(0.5f + (0.25f * (stacks - 1)), ModifierType.PercentAdd, cardName);
        player.gun.stats.damage.AddModifier(modifier);
    }

    public override void SubscribeEvent()
    {
        abilityDurationTimer.Pause();
        ShieldBreak.AddListener(CallCard);
    }

    public override void Update()
    {
        abilityDurationTimer.Tick(Time.deltaTime);

        if (abilityDurationTimer.IsDone() && !abilityDurationTimer.isPaused)
        {
            player.gun.stats.damage.RemoveModifier(cardName);
            abilityDurationTimer.Pause();
        }
    }
}
public class OutlawOfBadges : Card
{
    public override string cardName => "Outlaw of Badges";

    public void CallCard(GameObject enemy)
    {
        PlayerController player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        player.ps.health += 1 * stacks;
    }

    public override void SubscribeEvent()
    {
        EnemyDeath.AddListener(CallCard);
    }
}
public class SheriffOfBullets : Card
{
    public override string cardName => "Sheriff of Bullets";
    int killCount;
    PlayerController player;

    public void CallCard(GameObject enemy)
    {
        killCount++;
        StatModifier mod = new StatModifier(killCount * 0.01f * stacks, ModifierType.PercentMult, cardName);
        player.gun.stats.fireRate.AddModifier(mod);
    }

    void ResetCount(GameObject player, float damage)
    {
        killCount = 0;
        StatModifier mod = new StatModifier(0, ModifierType.PercentMult, cardName);
        this.player.gun.stats.fireRate.AddModifier(mod);
    }

    public override void SubscribeEvent()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        EnemyDeath.AddListener(CallCard);
        GenericHitPlayer.AddListener(ResetCount);
    }
}
public class NineOfLassos : Card
{
    public override string cardName => "Nine of Lassos";
    PlayerController player;
    int killCount = 0;
    int modifierVal = 0;

    public void CallCard(GameObject enemy)
    {
        killCount++;
        if (killCount >= 3)
        {
            modifierVal += stacks;
            killCount = 0;
            StatModifier modifier = new StatModifier(modifierVal, ModifierType.Flat, cardName);
            player.ps.maxHealthStat.AddModifier(modifier);
            player.ps.health += stacks;
        }
    }

    public override void SubscribeEvent()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        EnemyDeath.AddListener(CallCard);
    }
}
public class SheriffOfBoots : Card
{
    public override string cardName => "Sheriff of Boots";

    public void CallCard(GameObject player)
    {
        PlayerController playerScript = player.GetComponent<PlayerController>();
        List<HealthScript> nearbyDamageables = Physics.OverlapSphere(player.transform.position, 3)
            .Where(x => x.gameObject.GetComponent<HealthScript>() != null)
            .Select(x => x.gameObject.GetComponent<HealthScript>()).ToList();
        //To explain this horrid line of code, basically it's casting a sphere around the player with a radius of 3 units. From there, it finds all colliders that contain a health script.
        //After that, it grabs the health scripts of those objects. Finally, it returns it as a list. This should find all enemies within a 3 unit radius of the player.
        foreach (HealthScript health in nearbyDamageables)
        {
            health.health -= (6 + (4 * (stacks - 1))) * (playerScript.gun.stats.damage.Value / playerScript.gun.stats.damage.BaseValue);
            //What this equation does is take the base damage that the card does (6) and adds 4 for every stack past one.
            //After that, it applies the modifiers that the gun has by dividing the modified value by the base value
            //(assuming the modified value is 10 and the base value is 5, that means the modifier is 2)
        }
    }

    public override void SubscribeEvent()
    {
        Land.AddListener(CallCard);
    }
}
public class FiveOfBadges : Card
{
    public override string cardName => "Five of Badges";
    private const float MAX_VAL = 0.75f;

    public void CallCard(GameObject player, float damage)
    {
        PlayerController playerScript = player.GetComponent<PlayerController>();

        float damageReduction = 0;
        for (int i = 0; i < stacks; i++)
        {
            damageReduction += (MAX_VAL - damageReduction + 0.001f) / 10;
            damageReduction = Mathf.Min(damageReduction, MAX_VAL);
        }

        player.GetComponent<PlayerController>().damageRecieved *= (1 - damageReduction);
    }

    public override void SubscribeEvent()
    {
        GenericHitPlayer.AddListener(CallCard);
    }
}
public class FourOfBadges : Card
{
    public override string cardName => "Four of Badges";
    private const float MAX_VAL = 0.75f;

    public void CallCard(GameObject player, float damage)
    {
        PlayerController playerScript = player.GetComponent<PlayerController>();

        float chance = 0;
        for (int i = 0; i < stacks; i++)
        {
            chance += (MAX_VAL - chance + 0.001f) / 10;
            chance = Mathf.Min(chance, MAX_VAL);
        }

        bool preventDamage = Random.Range(0f, 1f) < chance;

        if (preventDamage)
        {
            player.GetComponent<PlayerController>().damageRecieved = 0;
        }
    }

    public override void SubscribeEvent()
    {
        GenericHitPlayer.AddListener(CallCard);
    }
}
public class JesterOfBullets : Card
{
    public override string cardName => "Jester of Bullets";

    public void CallCard(GameObject player, float damage)
    {
        float baseModifier = 1;
        for (int i = 0; i < stacks; i++)
        {
            baseModifier *= 2;
            damage *= 2;
        }

        StatModifier mod = new StatModifier(baseModifier, ModifierType.PercentMult, cardName);
        player.GetComponent<PlayerController>().gun.stats.damage.AddModifier(mod);
        player.GetComponent<PlayerController>().damageRecieved = damage;
    }

    public override void SubscribeEvent()
    {
        StatModifier mod = new StatModifier(2, ModifierType.PercentMult, cardName);
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().gun.stats.damage.AddModifier(mod);
        GenericHitPlayer.AddListener(CallCard);
    }
}
public class NineOfBadges : Card
{
    public override string cardName => "Nine of Badges";

    private Timer invFrames;

    public void CallCard(GameObject player, float damage)
    {
        PlayerController playerScript = player.GetComponent<PlayerController>();
        if (invFrames.IsDone())
        {
            invFrames.SetMaxTime(0.5f * stacks);
            invFrames.Reset();
        }
        else
        {
            player.GetComponent<PlayerController>().damageRecieved = 0;
        }
    }

    public override void SubscribeEvent()
    {
        GenericHitPlayer.AddListener(CallCard);
        invFrames = new Timer(0.5f);
        invFrames.SetTime(0);
    }

    public override void Update()
    {
        if (invFrames != null)
        {
            invFrames.Tick(Time.deltaTime);
        }
    }
}
public class SheriffOfLassos : Card
{
    public override string cardName => "Sheriff of Lassos";
    private const float MAX_VAL = 0.85f;
    public PlayerController playerScript;
    private GameObject prefab;

    public void CallCard(GameObject enemy)
    {
        float chance = 0;
        for (int i = 0; i < stacks; i++)
        {
            chance += (MAX_VAL - chance + 0.001f) / 10;
            chance = Mathf.Min(chance, MAX_VAL);
        }

        float value = Random.Range(0f, 1f);
        if (value < chance)
        {
            List<HealthScript> nearbyDamageables = Physics.OverlapSphere(enemy.transform.position, 5)
            .Where(x => x.gameObject.GetComponent<HealthScript>() != null)
            .Select(x => x.gameObject.GetComponent<HealthScript>()).ToList();

            foreach (HealthScript health in nearbyDamageables)
            {
                health.health -= (10 + (7 * (stacks - 1))) * (playerScript.gun.stats.damage.Value / playerScript.gun.stats.damage.BaseValue);
            }

            GameObject instance = GameObject.Instantiate(prefab);
            instance.transform.position = enemy.transform.position;
        }
    }

    public override void SubscribeEvent()
    {
        EnemyDeath.AddListener(CallCard);
        playerScript = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        prefab = (GameObject)Resources.Load("Prefabs/Explosion Fire", typeof(GameObject));
    }
}
public class DeputyOfLassos : Card
{
    public override string cardName => "Deputy of Lassos";

    List<EnemyDamageHolder> enemies = new List<EnemyDamageHolder>();
    List<float> damageLeftList = new List<float>();
    Timer timer;

    GunScript gun;

    public void CallCard(GameObject enemy, float damage)
    {
        gun.damageDealt = 0; //Disable the damage dealt first

        if (enemies.Where(x => x.enemy == enemy).Count() == 0)
        {
            enemies.Add(new EnemyDamageHolder(enemy, damage * (1 + (0.75f * stacks))));
        }
        else
        {
            EnemyDamageHolder hitEnemy = enemies.Where(x => x.enemy == enemy).FirstOrDefault();
            hitEnemy.damageLeft += (damage * (1 + 0.75f * stacks));

            enemies[enemies.IndexOf(enemies.Where(x => x.enemy == enemy).FirstOrDefault())] = hitEnemy;
        }
    }

    public override void Update()
    {
        enemies.RemoveAll(x => x.enemy == null);
        timer.Tick(Time.deltaTime);
    }

    public void ApplyDamage()
    {
        Debug.Log("Attempting to apply damage");
        timer.Reset();
        for (int i = 0; i < enemies.Count; i++)
        {
            EnemyDamageHolder enemyHolder = enemies[i];
            float damageDealt = 5 / (2.5f + (stacks * 0.5f));
            enemyHolder.damageLeft -= damageDealt;
            if(enemyHolder.damageLeft < 0)
            {
                damageDealt -= Mathf.Abs(enemyHolder.damageLeft);
                enemyHolder.damageLeft = 0;
            }

            enemies[i] = enemyHolder;

            enemies[i].enemy.GetComponent<HealthScript>().health -= damageDealt;
        }
    }

    public override void SubscribeEvent()
    {
        GenericHitEnemy.AddListener(CallCard);
        timer = new Timer(0.25f);
        timer.timerComplete.AddListener(ApplyDamage);
        gun = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().gun;
    }

    private struct EnemyDamageHolder
    {
        public GameObject enemy;
        public float damageLeft;

        public EnemyDamageHolder(GameObject enemy, float damageLeft)
        {
            this.enemy = enemy;
            this.damageLeft = damageLeft;
        }
    }
}
public class DeputyOfBoots : Card
{
    public override string cardName => "Deputy of Boots";
    Timer timer;
    Timer abilityCooldown;

    public void CallCard(GameObject player, float damage)
    {
        timer.Reset();
        timer.Pause();
        abilityCooldown.Reset();
    }

    public override void SubscribeEvent()
    {
        GenericHitPlayer.AddListener(CallCard);
        GenericHitEnemy.AddListener(CallCard);
        timer = new Timer(1f);
        timer.Pause();
        abilityCooldown = new Timer(10f);
    }

    public override void Update()
    {
        abilityCooldown.Tick(Time.deltaTime);
        if(abilityCooldown.IsDone())
        {
            timer.Unpause();
        }
        timer.Tick(Time.deltaTime);
        if(timer.IsDone())
        {
            timer.Reset();
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().ps.health += (1 * stacks);
        }
    }
}

#endregion

public enum Cards //After making a card, make sure to add its name to this list
{
    TestCard,
    TwoOfBullets,
    AceOfBoots,
    OutlawOfBullets,
    ThreeOfBadges,
    FourOfLassos,
    ThreeOfBullets,
    EightOfBadges,
    FourOfBullets,
    OutlawOfBadges,
    SheriffOfBullets,
    SevenOfLassos,
    SixOfBadges,
    ThreeOfLassos,
    DeputyOfBullets,
    NineOfLassos,
    SheriffOfBoots,
    ThreeOfBoots,
    FiveOfBadges,
    FourOfBadges,
    JesterOfBullets,
    TenOfBadges,
    NineOfBadges,
    SheriffOfLassos,
    DeputyOfLassos,
    DeputyOfBoots
}