using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Charge : MonoBehaviour
{
    public Color chargeColor;
    public string chargeName;

    // Start is called before the first frame update
    void Start()
    {
        var chargeRenderer = gameObject.GetComponent<Renderer>();
        chargeRenderer.material.SetColor("_Color", chargeColor);
    }

    void OnTriggerEnter(Collider other)
    {
        Creature creature = other.gameObject.GetComponent<Creature>();
        //creature.creatureColor = ChargeColor;
            creature.ChangeColor(chargeColor);
        creature.creatureCharge = chargeName;
    }
}
