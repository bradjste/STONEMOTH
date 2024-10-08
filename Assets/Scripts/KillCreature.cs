using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillCreature : MonoBehaviour
{
    void OnTriggerEnter(Collider col)
    {
        Creature creature = col.gameObject.GetComponent<Creature>();
        Swarm swarm = creature.swarm;
        swarm.DestroyCreature(col.gameObject);
    }
}
