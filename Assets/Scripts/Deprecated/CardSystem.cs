using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static EventManager;

public class CardSystem : MonoBehaviour
{
    public Card card;
    public EventType eventType;


    #region Potential Event Data

    GameObject obj; //For things like the player or the enemy, depending on the event
    float f; //Usually damage
    GunScript gun; //The gun
    int i; //Usually waves

    #endregion

    void Start()
    {
        //if (card.Type == CardType.Triggered)
        //{
        //    switch (eventType) //Hell
        //    {
        //        case EventType.GenericHitEnemy:
        //            GenericHitEnemy.AddListener(CallCard);
        //            break;
        //        case EventType.EnemyDeath:
        //            EnemyDeath.AddListener(CallCard);
        //            break;
        //        case EventType.EnemySpawn:
        //            EnemySpawn.AddListener(CallCard);
        //            break;
        //        case EventType.GunFired:
        //            GunFired.AddListener(CallCard);
        //            break;
        //        case EventType.BulletHitEnemy:
        //            BulletHitEnemy.AddListener(CallCard);
        //            break;
        //        case EventType.Reload:
        //            Reload.AddListener(CallCard);
        //            break;
        //        case EventType.GenericHitPlayer:
        //            GenericHitPlayer.AddListener(CallCard);
        //            break;
        //        case EventType.Jump:
        //            Jump.AddListener(CallCard);
        //            break;
        //        case EventType.Land:
        //            Land.AddListener(CallCard);
        //            break;
        //        case EventType.PlayerDeath:
        //            PlayerDeath.AddListener(CallCard);
        //            break;
        //        case EventType.PlayerDash:
        //            PlayerDash.AddListener(CallCard);
        //            break;
        //        case EventType.GenericHitEntity:
        //            GenericHitEntity.AddListener(CallCard);
        //            break;
        //        case EventType.EntityDeath:
        //            EntityDeath.AddListener(CallCard);
        //            break;
        //        case EventType.Purchase:
        //            Purchase.AddListener(CallCard);
        //            break;
        //        case EventType.WaveEnd:
        //            WaveEnd.AddListener(CallCard);
        //            break;
        //        case EventType.WaveStart:
        //            WaveStart.AddListener(CallCard);
        //            break;
        //    }
        //}
    }

    // Update is called once per frame
    void Update()
    {
        //if(card.Type == CardType.Passive)
        //{
        //    GameObject player = GameObject.FindGameObjectWithTag("Player");
        //    CallCard(player); //Passive effects will always be passed the player as their data
        //}
    }

    public void CallCard()
    {
       // card.TriggeredFunction.Invoke();
    }

    #region CallCard Overloads

    public void CallCard(GameObject obj)
    {
        this.obj = obj;
        //card.TriggeredFunction.Invoke();
    }

    public void CallCard(GameObject obj, float f)
    {
        this.obj = obj;
        this.f = f;
        //card.TriggeredFunction.Invoke();
    }

    public void CallCard(GunScript gun)
    {
        this.gun = gun;
        //card.TriggeredFunction.Invoke();
    }

    public void CallCard(GunScript gun, float f)
    {
        this.gun = gun;
        this.f = f;
        //card.TriggeredFunction.Invoke();
    }

    public void CallCard(GunScript gun, GameObject obj, float f)
    {
        this.gun = gun;
        this.obj = obj;
        this.f = f;
        //card.TriggeredFunction.Invoke();
    }

    public void CallCard(int i)
    {
        this.i = i;
        //card.TriggeredFunction.Invoke();
    }

    #endregion

    #region Card Effects

    

    #endregion
}

[Serializable]
//public struct Card
//{
//    public string Name;
//    public int Quantity;
//    public CardType Type;
//    public UnityEvent TriggeredFunction;
//}

public enum EventType
{
    GenericHitEnemy,
    EnemyDeath,
    EnemySpawn,
    GunFired,
    BulletHitEnemy,
    Reload,
    GenericHitPlayer,
    Jump,
    Land,
    PlayerDeath,
    PlayerDash,
    GenericHitEntity,
    EntityDeath,
    Purchase,
    WaveEnd,
    WaveStart
}
