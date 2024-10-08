using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    private void Start()
    {
        Destroy(gameObject, 0.5f);
    }

    private void OnTriggerEnter(Collider col)
    {
        Creature creature = col.gameObject.GetComponent<Creature>();
        if (creature != null)
        {
            creature.DestroySelf();
        } 
        else if (col.gameObject.CompareTag("Explodable"))
        {
            Destroy(col.gameObject,.2f);
        }
    }
}
