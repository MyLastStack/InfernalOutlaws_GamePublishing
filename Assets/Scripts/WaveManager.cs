using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class WaveManager : MonoBehaviour
{
    public int currentWave = 1;
    public GameObject[] enemyPrefabs;
    Timer timer;
    public float spawnCooldown;
    public float spawnRadius;
    int currentEnemy = 0;
    public GameObject CardMenu;

    public EnemyTypeWrapper[] waves; //Fill this with the enemies in each waves

    private void Start()
    {
        timer = new Timer(spawnCooldown);
        timer.timerComplete.AddListener(SpawnEnemy);
    }

    private void Update()
    {
        timer.Tick(Time.deltaTime);
    }

    public void SpawnEnemy()
    {
        currentEnemy++;
        if (currentEnemy >= waves[currentWave - 1].enemies.Length) //If the player is on the last enemy of the wave, go to the next wave
        {
            currentWave = Mathf.Clamp(currentWave + 1, 1, waves.Length);
            currentEnemy = 1;
            CardMenu.SetActive(true);
        }
        else
        {
            timer.Reset();
            Vector3 pos = RandomNavmeshLocation(spawnRadius);
            GameObject selectedPrefab = enemyPrefabs[(int)(waves[currentWave - 1].enemies[currentEnemy - 1])]; //Selects an enemy prefab based on the enum

            Instantiate(selectedPrefab, pos, Quaternion.Euler(Vector3.zero), transform);

        }
    }

    public Vector3 RandomNavmeshLocation(float radius)
    {
        Vector3 randomDirection = Random.insideUnitSphere * radius; //Pick a random point within a sphere
        randomDirection += transform.position; //apply that sphere to the gameobject's location
        NavMeshHit hit;
        Vector3 finalPosition = Vector3.zero;
        if (NavMesh.SamplePosition(randomDirection, out hit, radius, 1))
        {
            finalPosition = hit.position;
        }
        return finalPosition;
    }
}

[Serializable]
public enum EnemyTypes
{
    Standard,
    Projectile
}

[Serializable]
public struct EnemyTypeWrapper
{
    public EnemyTypes[] enemies;
}
