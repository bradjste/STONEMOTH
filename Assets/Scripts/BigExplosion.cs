using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigExplosion : MonoBehaviour

{
    public GameObject explosionAnim;
    private void Start()
    {
        Destroy(gameObject, 0.5f);
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.CompareTag("Explodable"))
        {
            Instantiate(explosionAnim, col.ClosestPoint(transform.position), Quaternion.identity);
            Destroy(col.gameObject, .2f);
        }
    }
}
