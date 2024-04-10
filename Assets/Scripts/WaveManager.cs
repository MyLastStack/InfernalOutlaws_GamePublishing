using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using Random = UnityEngine.Random;
using static EventManager;
using TMPro;

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
    private List<EnemyTypes> currentWaveEnemies = new List<EnemyTypes>();
    private List<GameObject> activeEnemies = new List<GameObject>();

    public float enemyStatModifer = 1; //Public for debugging & inspector purposes but should generally always start at 1

    public TextMeshProUGUI waveCounterTMP;
 

    private void Start()
    {
        timer = new Timer(spawnCooldown);
        timer.timerComplete.AddListener(SpawnEnemy);
        SetWaveEnemeies(currentWave);
        WaveStart.AddListener(SetWaveEnemeies); //Set enemies at the start of a wave
        AddCard.AddListener(StartWave);
    }

    private void Update()
    {
        timer.Tick(Time.deltaTime);
        waveCounterTMP.text = "Wave: " + currentWave;
    }

    public void SpawnEnemy()
    {
        currentEnemy++;
        if (currentEnemy > currentWaveEnemies.Count) //If the player is on the last enemy of the wave, go to the next wave
        {
            activeEnemies.RemoveAll(x => x == null || !x.activeSelf);
            if (activeEnemies.Count == 0)
            {
                WaveEnd.Invoke(currentWave);
                CardMenu.SetActive(true);
            }
        }
        else
        {
            timer.Reset();
            Vector3 pos = RandomNavmeshLocation(spawnRadius);
            GameObject selectedPrefab = enemyPrefabs[(int)(currentWaveEnemies[currentEnemy - 1])]; //Selects an enemy prefab based on the enum

            GameObject spawnedEnemy = Instantiate(selectedPrefab, pos, Quaternion.Euler(Vector3.zero), transform);

            //Update enemy stats
            EnemyBehaviour enemyScript = spawnedEnemy.GetComponent<EnemyBehaviour>();
            if(enemyScript != null)
            {
                enemyScript.damage *= enemyStatModifer;
            }
            RangedEnemyBehaviour rangedEnemyScript = spawnedEnemy.GetComponent<RangedEnemyBehaviour>();
            if(rangedEnemyScript != null)
            {
                rangedEnemyScript.damage *= enemyStatModifer;
            }
            HealthScript health = spawnedEnemy.GetComponent<HealthScript>();
            if(health != null)
            {
                health.health *= enemyStatModifer;
            }

            activeEnemies.Add(spawnedEnemy);
        }
    }

    void SetWaveEnemeies(int wave)
    {
        currentWaveEnemies.Clear();
        for (int i = 0; i < wave; i++)
        {
            currentWaveEnemies.AddRange(waves[Random.Range(0, waves.Length)].enemies); //Add a selected group of enemies to the wave
        }
    }

    void StartWave(Card eventData)
    {
        currentWave++;
        currentEnemy = 1;
        spawnCooldown -= (spawnCooldown / 20); //5% timer reduction per wave
        enemyStatModifer *= 1.05f;
        timer.SetMaxTime(spawnCooldown);
        WaveStart.Invoke(currentWave);
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
