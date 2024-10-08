using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
    GameManager gameManager;
    [SerializeField] GameObject creaturesContainer;
    [SerializeField] GameObject collectedCreaturePrefab;

    void Start()
    {
        gameManager = GameManager.Instance;
        gameManager.goals.Add(this);
    }

    void OnTriggerEnter(Collider col)
    {
        Creature creature = col.gameObject.GetComponent<Creature>();
        Swarm swarm = creature.swarm;
        Color creatureColor = creature.GetComponent<Renderer>().material.color;
        swarm.DestroyCreature(col.gameObject);

        CreateCollectedCreature(gameManager.score, creatureColor);
        gameManager.AddToScore(1);
    }

    void CreateCollectedCreature(int score, Color color)
    {
        float[] map = new float[3];
        map[0] = -1f;
        map[1] = 0;
        map[2] = 1f;

        Vector3 position = transform.position + new Vector3(map[score % 3], Mathf.Floor(score / 3), 0);
        GameObject creature = Instantiate(collectedCreaturePrefab, position, Quaternion.identity, creaturesContainer.transform);
        creature.GetComponent<Renderer>().material.SetColor("_Color", color);
        creature.transform.localScale *= 0.5f;
    }

    public void ResetGoal()
    {
        for (int i = 0; i < creaturesContainer.transform.childCount; i++)
        {
            Destroy(creaturesContainer.transform.GetChild(i).gameObject);
        }
    }
}
