using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject creaturePrefab;
    public float spawnTime = 3f;
    public GameObject swarmGameObject;

    int creaturesSpawned = 0;
    int spawnCount = int.MaxValue;

    // Start is called before the first frame update
    /*void Start()
    {
        InvokeRepeating("AddCreatureToSwarm", spawnTime, spawnTime);
    }*/

    public void AddCreatureToSwarm()
    {
        Swarm swarm = swarmGameObject.GetComponent<Swarm>();
        Vector2 randomDirection = Random.insideUnitCircle;
        swarm.AddCreature(new Vector3(randomDirection.x, 0.5f, randomDirection.y), transform.localPosition);

        creaturesSpawned++;
        if (creaturesSpawned == spawnCount)
        {
            CancelInvoke();
        }
    }

    public void SpawnCreatures(int count, float totalTime)
    {
        creaturesSpawned = 0;
        spawnCount = count;

        InvokeRepeating("AddCreatureToSwarm", 0, totalTime / count);
    }
}
