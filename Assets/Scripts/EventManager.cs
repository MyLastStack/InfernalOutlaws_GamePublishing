using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public static class EventManager
{
    #region Enemy Events

    public static UnityEvent<GameObject, float> GenericHitEnemy = new UnityEvent<GameObject, float>(); //Gameobject is the enemy that took damage.
    public static UnityEvent<GameObject> EnemyDeath = new UnityEvent<GameObject>(); //Gameobject is the enemy that died. Make sure to use this before the enemy is disabled.
    public static UnityEvent<GameObject> EnemySpawn = new UnityEvent<GameObject>(); //Gameobject is the enemy that spawned.

    #endregion

    #region Gun Events

    public static UnityEvent<GunScript> GunFired = new UnityEvent<GunScript>(); //Called when a gun is fired
    public static UnityEvent<GunScript, GameObject, float> BulletHitEnemy = new UnityEvent<GunScript, GameObject, float>(); //Called when a bullet hits an enemy
    public static UnityEvent<GunScript> Reload = new UnityEvent<GunScript>(); //Called when a gun reloads

    #endregion

    #region Spell Events

    //Update these once the spell system has been implemented
    public static UnityEvent SpellFired = new UnityEvent();
    public static UnityEvent SpellHitEnemy = new UnityEvent();
    public static UnityEvent SpellOffCooldown = new UnityEvent();
    public static UnityEvent DrawCard = new UnityEvent();
    public static UnityEvent ShuffleDeck = new UnityEvent();

    #endregion

    #region Player Events

    public static UnityEvent<GameObject, float> GenericHitPlayer = new UnityEvent<GameObject, float>(); //Called when the player takes any damage
    public static UnityEvent<GameObject> Jump = new UnityEvent<GameObject>(); //Called when the player jumps
    public static UnityEvent<GameObject> Land = new UnityEvent<GameObject>(); //Called when the player lands on the ground
    public static UnityEvent<GameObject> PlayerDeath = new UnityEvent<GameObject>(); //Called when the player would die
    public static UnityEvent<GameObject> PlayerDash = new UnityEvent<GameObject>(); //Called when the player dashes
    public static UnityEvent<GameObject> PlayerDashEnd = new UnityEvent<GameObject>();
    public static UnityEvent<GameObject> ShieldBreak = new UnityEvent<GameObject>();
    public static UnityEvent<GameObject, float> GenericHitShield = new UnityEvent<GameObject, float>();
    public static UnityEvent<GameObject, float> GenericHitHealth = new UnityEvent<GameObject, float>();

    #endregion

    #region Miscellaneous Events

    public static UnityEvent<GameObject, float> GenericHitEntity = new UnityEvent<GameObject, float>(); //Called when anything takes damage
    public static UnityEvent<GameObject> EntityDeath = new UnityEvent<GameObject>(); //Called when anything dies
    public static UnityEvent Purchase = new UnityEvent(); //Called when the player purchases anything from the shop. Update later.
    public static UnityEvent<int> WaveEnd = new UnityEvent<int>(); //Called when the final enemy in a wave has been defeated. Int is wave num.
    public static UnityEvent<int> WaveStart = new UnityEvent<int>(); //Called when a wave starts. Int is wave num.

    #endregion



    //Non-card related
    public static UnityEvent<Card> AddCard = new UnityEvent<Card>();


    public static void ResetListeners() //Call this when you restart a run to prevent triggered cards from previous runs from triggering
    {
        GenericHitEnemy.RemoveAllListeners();
        EnemyDeath.RemoveAllListeners();
        EnemySpawn.RemoveAllListeners();
        GunFired.RemoveAllListeners();
        BulletHitEnemy.RemoveAllListeners();
        Reload.RemoveAllListeners();
        SpellFired.RemoveAllListeners();
        SpellHitEnemy.RemoveAllListeners();
        SpellOffCooldown.RemoveAllListeners();
        DrawCard.RemoveAllListeners();
        ShuffleDeck.RemoveAllListeners();
        GenericHitPlayer.RemoveAllListeners();
        Jump.RemoveAllListeners();
        Land.RemoveAllListeners();
        PlayerDeath.RemoveAllListeners();
        PlayerDash.RemoveAllListeners();
        PlayerDashEnd.RemoveAllListeners();
        ShieldBreak.RemoveAllListeners();
        GenericHitShield.RemoveAllListeners();
        GenericHitHealth.RemoveAllListeners();
        GenericHitEntity.RemoveAllListeners();
        EntityDeath.RemoveAllListeners();
        Purchase.RemoveAllListeners();
        WaveEnd.RemoveAllListeners();
        WaveStart.RemoveAllListeners();
    }
}
