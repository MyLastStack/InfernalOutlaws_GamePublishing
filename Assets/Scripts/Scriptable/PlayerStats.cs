using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "PlayerStats", menuName = "ScriptableObjects/PlayerStats", order = 1)]
public class PlayerStats : ScriptableObject
{
    public Stat dashTimeStat;
    public Stat dashCooldownStat;
    public Stat dashForceStat;

    //Dash variables
    public float dashTime { get { return dashTimeStat.Value; } set { dashTimeStat.SetBaseValue(value); } } //How long does the dash last
    [HideInInspector] public float dashTimeLeft; //A tracker for dash time.
    public float dashCooldown { get { return dashCooldownStat.Value; } set { dashCooldownStat.SetBaseValue(value); } } //How long after a dash before the player can dash again
    [HideInInspector] public float dashCooldownTimeLeft; //A tracker for dash cooldown
    public float dashForce { get { return dashForceStat.Value; } set { dashForceStat.SetBaseValue(value); } } //How much force is applied during the dash.
    public float dashMaxMagnitude; //The max magnitude of the player during the dash.
    public float dashMoveSpeed; //Control the player has when dashing
    public float dashDeccelRate = 1.05f; //DeccelRate during dash
    public float dashFOV; //Camera FOV during dash

    //Movement Variables
    public float moveSpeed; //The speed at which the player accelerates
    public float walkMoveSpeed; //Control the player has when walking
    public float rotateSpeed; //Camera sensitivity
    public float jumpPower; //How much force is applied during a jump
    public float walkMaxMagnitude; //Max magnitude while not dashing
    public float maxMagnitude; //The max speed the player can move at
    public float deccelRate = 1.1f; //Essentially simulated friction, the rate the player deccelerates when not moving in a given direction
    public float walkDeccelRate = 1.1f; //Deccel rate during walk
    public float walkFOV = 60;
}
