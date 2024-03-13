using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "PlayerStats", menuName = "ScriptableObjects/PlayerStats", order = 1)]
public class PlayerStats : ScriptableObject
{
    [Header("Modifyable Stats")]
    [Space]
    public Stat dashTimeStat;
    public Stat dashCooldownStat;
    public Stat dashMaxMagnitudeStat;
    public Stat dashMoveSpeedStat;
    public Stat walkMoveSpeedStat;
    public Stat jumpPowerStat;
    public Stat walkMaxMagnitudeStat;


    //Dash variables
    [HideInInspector] public float dashTime { get { return dashTimeStat.Value; } set { dashTimeStat.SetBaseValue(value); } } //How long does the dash last
    [HideInInspector] public float dashTimeLeft; //A tracker for dash time.
    [HideInInspector] public float dashCooldown { get { return dashCooldownStat.Value; } set { dashCooldownStat.SetBaseValue(value); } } //How long after a dash before the player can dash again
    [HideInInspector] public float dashCooldownTimeLeft; //A tracker for dash cooldown
    [Header("Unmodifyable Stats")]
    [Space]
    public float dashForce; //How much force is applied during the dash.
    [HideInInspector] public float dashMaxMagnitude { get { return dashMaxMagnitudeStat.Value; } set { dashMaxMagnitudeStat.SetBaseValue(value); } } //The max magnitude of the player during the dash.
    [HideInInspector] public float dashMoveSpeed { get { return dashMoveSpeedStat.Value; } set { dashMoveSpeedStat.SetBaseValue(value); } } //Control the player has when dashing
    public float dashDeccelRate = 1.05f; //DeccelRate during dash
    public float dashFOV; //Camera FOV during dash

    //Movement Variables
    public float moveSpeed; //The speed at which the player accelerates (modified by walk and dash move speed)
    [HideInInspector] public float walkMoveSpeed { get { return walkMoveSpeedStat.Value; } set { walkMoveSpeedStat.SetBaseValue(value); } } //Control the player has when walking
    public float rotateSpeed; //Camera sensitivity
    [HideInInspector] public float jumpPower { get { return jumpPowerStat.Value; } set { jumpPowerStat.SetBaseValue(value); } } //How much force is applied during a jump
    [HideInInspector] public float walkMaxMagnitude { get { return walkMaxMagnitudeStat.Value; } set { walkMaxMagnitudeStat.SetBaseValue(value); } } //Max magnitude while not dashing
    public float maxMagnitude; //The max speed the player can move at (modified by walk max magnitude and dash max magnitude)
    public float deccelRate = 1.1f; //Essentially simulated friction, the rate the player deccelerates when not moving in a given direction
    public float walkDeccelRate = 1.1f; //Deccel rate during walk
    public float walkFOV = 60;
}
