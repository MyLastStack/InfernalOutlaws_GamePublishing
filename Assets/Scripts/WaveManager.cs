using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WaveManager : MonoBehaviour
{
    int currentWave = 1;
    public GameObject enemy;
    Timer timer;
    public float spawnCooldown;
    public float spawnRadius;

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
        timer.Reset();
        Vector3 pos = RandomNavmeshLocation(spawnRadius);
        Instantiate(enemy, pos, Quaternion.Euler(Vector3.zero), transform);
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
