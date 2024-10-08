using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Swarm : MonoBehaviour
{
    const int threadGroupSize = 1024;

    [SerializeField]
    GameObject creaturePrefab;
    [SerializeField]
    GameObject targetObject;
    [SerializeField]
    GameObject spawnerObject;

    [SerializeField]
    BoidSettings settings;
    [SerializeField]
    ComputeShader compute;

    [SerializeField] 
    float spawnRadius = 5;
    [SerializeField]
    public int spawnCount = 30;
    [SerializeField]
    public int maxSwarmSize = 300;

    public List<GameObject> creatures = new List<GameObject>();

    [HideInInspector]
    public GameManager gameManager;

    GameObject spawnPoint;
    public TMP_Text swarmSizeText;

    public GameObject explosionPS;
    public List<AudioClip> explosionAudioClips;

    [SerializeField] bool generateCreaturesOnCreate = false;

    private void Awake()
    {
        spawnPoint = GameObject.FindGameObjectWithTag("Respawn");
        SetTargetPosition();
        SetSpawnerPosition();

        if (generateCreaturesOnCreate)
        {
            GenerateInitialCreatures();
        }
    }

    public void GenerateRandomCreature(Vector3 center = new Vector3())
    {
        Vector2 randomDir = Random.insideUnitCircle;
        Vector2 randomPos = new Vector2(center.x, center.z) + Random.insideUnitCircle * spawnRadius;
        AddCreature(
            new Vector3(randomDir.x, 1.0f, randomDir.y), 
            new Vector3(randomPos.x, 1.0f, randomPos.y)
        );
    }

    public void AddCreature(Vector3 direction, Vector3 position)
    {
        if (creatures.Count >= maxSwarmSize) return;
        Vector3 pos = transform.position + position + new Vector3(0, 3f, 0);
        Transform creaturesContainer = transform.Find("Creatures");

        GameObject creature = Instantiate(creaturePrefab, pos, Quaternion.identity, creaturesContainer);
        creature.transform.position = pos;
        creature.transform.forward = direction;

        creature.GetComponent<Creature>().Initialize(this, settings, targetObject.transform);

        /*creature.GetComponent<MeshRenderer>().material.color = targetObject.GetComponent<MeshRenderer>().material.color;*/

        creatures.Add(creature);  

        UpdateCountText();
    }  

    public void DestroyCreature(GameObject creature)
    {
        AudioClip selectedAudioClip = explosionAudioClips[Mathf.FloorToInt(Random.Range(0, explosionAudioClips.Count))];
        gameObject.GetComponent<AudioSource>().PlayOneShot(selectedAudioClip);
        Instantiate(explosionPS, creature.transform.position, Quaternion.identity);

        creatures.Remove(creature);
        Destroy(creature);

        UpdateCountText();
        gameManager.CheckIfLostGame();
    }

    void UpdateCountText()
    {
        if (swarmSizeText != null)
        {
            swarmSizeText.text = "" + creatures.Count;
        }
    }

    public void DestroyAllCreatures()
    {
        foreach (GameObject creature in creatures)
        {
            Destroy(creature);
        }
        creatures.Clear();

        UpdateCountText();
    }

    // Update is called once per frame
    void Update()
    {
        //calculate
        UpdateCreaturePositions();
    }

    void UpdateCreaturePositions()
    {
        if (creatures.Count > 0)
        {
            var boidData = new BoidData[creatures.Count];

            for (int i = 0; i < creatures.Count; i++)
            {
                boidData[i].position = creatures[i].transform.position;
                boidData[i].direction = creatures[i].transform.forward;
            }

            var boidBuffer = new ComputeBuffer(creatures.Count, BoidData.Size);
            boidBuffer.SetData(boidData);

            compute.SetBuffer(0, "boids", boidBuffer);
            compute.SetInt("numBoids", creatures.Count);
            compute.SetFloat("viewRadius", settings.perceptionRadius);
            compute.SetFloat("avoidRadius", settings.avoidanceRadius);

            int threadGroups = Mathf.CeilToInt(creatures.Count / (float)threadGroupSize);
            compute.Dispatch(0, threadGroups, 1, 1);

            boidBuffer.GetData(boidData);

            for (int i = 0; i < creatures.Count; i++)
            {
                Creature creature = creatures[i].GetComponent<Creature>();
                creature.avgFlockHeading = boidData[i].flockHeading;
                creature.centreOfFlockmates = boidData[i].flockCentre;
                creature.avgAvoidanceHeading = boidData[i].avoidanceHeading;
                creature.numPerceivedFlockmates = boidData[i].numFlockmates;

                creature.UpdateCreature();
            }

            boidBuffer.Release();
        }
    }

    public void SetTargetPosition()
    {
        SetTargetPosition(spawnPoint.transform.position + new Vector3(0, 1f, 0));
    }

    public void SetTargetColor(Color color)
    {
        targetObject.GetComponent<MeshRenderer>().material.color = color;
    }

    public void SetTargetPosition(Vector3 position)
    {
        targetObject.transform.position = position;
    }

    void SetSpawnerPosition()
    {
        spawnerObject.transform.position = spawnPoint.transform.position + new Vector3(0, 1f, 0);
    }

    public void GenerateInitialCreatures()
    {
        Spawner spawner = spawnerObject.GetComponent<Spawner>();
        spawner.SpawnCreatures(spawnCount, 3f);

        /*for (int i = 0; i < spawnCount; i++)
        {
            GenerateRandomCreature(spawnPoint.transform.position);
        }*/
    }

    /*
        void CalculateSwarmMidpoint()
        {
            Vector3 midpoint = new Vector3(0, 0, 0);

            foreach (GameObject creature in creatures) {
                midpoint += creature.transform.position;
            }
            midpoint /= creatures.Count;

            swarmMidpoint = new Vector3(midpoint.x,0.5f,midpoint.z);
        }
    */

    public struct BoidData
    {
        public Vector3 position;
        public Vector3 direction;

        public Vector3 flockHeading;
        public Vector3 flockCentre;
        public Vector3 avoidanceHeading;
        public int numFlockmates;

        public static int Size
        {
            get
            {
                return sizeof(float) * 3 * 5 + sizeof(int);
            }
        }
    }
}