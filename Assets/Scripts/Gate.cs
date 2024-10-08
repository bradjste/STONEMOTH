using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : MonoBehaviour
{
    public Color gateChargeColor;
    public string gateChargeName;
    // Start is called before the first frame update
    void Start()
    {
        var chargeRenderer = gameObject.GetComponent<Renderer>();
        chargeRenderer.material.SetColor("_Color", gateChargeColor);
    }

    private void OnTriggerEnter(Collider other)
    {
        Creature creature = other.gameObject.GetComponent<Creature>();
        if(creature && gateChargeName != creature.creatureCharge)
        {
            creature.DestroySelf();
        }
    }
}
