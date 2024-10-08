using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Creature : MonoBehaviour
{
    [HideInInspector]
    public Vector3 position;
    [HideInInspector]
    public Vector3 forward;
    private Vector3 velocity;
    private Vector3 acceleration;

    [HideInInspector]
    public Vector3 avgFlockHeading = new Vector3();
    [HideInInspector]
    public Vector3 avgAvoidanceHeading = new Vector3();
    [HideInInspector]
    public Vector3 centreOfFlockmates = new Vector3();
    [HideInInspector]
    public int numPerceivedFlockmates = 0;

    Transform target;
    BoidSettings settings;
    Material material;
    Transform cachedTransform;

    public Color creatureColor;
    public string creatureCharge;

    private Color initialColor;

    public Swarm swarm;

    public GameObject explosion;

    public GameObject explosionPS;

    public GameObject bigExplosion;


    // Start is called before the first frame update
    void Awake()
    {
        /*velocity = new Vector3(Random.Range(-1.0f, 1.0f), 0, Random.Range(-1.0f, 1.0f));*/
        var creatureRenderer = gameObject.GetComponent<Renderer>();
        cachedTransform = transform;
        initialColor = creatureRenderer.material.GetColor("_Color");
    }

    public void ChangeColor(Color color)
    {
        var creatureRenderer = gameObject.GetComponent<Renderer>();
        creatureRenderer.material.SetColor("_Color", color);
        creatureColor = color;
    }

    public void DestroySelf()
    {
        swarm.DestroyCreature(gameObject);
    }

    public void Initialize(Swarm swarm, BoidSettings settings, Transform target)
    {
        this.swarm = swarm;
        this.target = target;
        this.settings = settings;

        position = cachedTransform.position;
        forward = cachedTransform.forward;

        float startSpeed = (settings.minSpeed + settings.maxSpeed) / 2;
        velocity = transform.forward * startSpeed;
    }
    private void OnCollisionEnter(Collision col)
    {
        Creature creature = col.gameObject.GetComponent<Creature>();
        if(creature && creatureCharge == "Yellow" && creature.creatureCharge == "Red")
        {
            Instantiate(explosion, transform.position, Quaternion.identity);
            Instantiate(bigExplosion, transform.position, Quaternion.identity);
            DestroySelf();
        }
        if(creature && creatureCharge == "Red" && creature.creatureCharge == "Blue")
        {
            creature.creatureCharge = "White";
            creature.creatureColor = initialColor;
            creature.ChangeColor(initialColor);

            creature.swarm.GenerateRandomCreature(creature.transform.position);

            swarm.GenerateRandomCreature(gameObject.transform.position);

        }
    }

    /*Vector3 CalculateAcceleration()
    {
        //get all sibs
        //sum velocity V3
        //
        Vector3 swarmMidpoint = GetComponentInParent<Swarm>().swarmMidpoint;

        return new Vector3(swarmMidpoint.x, 0, swarmMidpoint.z) - transform.position;
    }*/

    // Update is called once per frame
    void Update()
    {
        if (SceneManager.GetActiveScene().buildIndex != 0 && transform.position.y < -3.0f)
        {
            Instantiate(explosionPS, transform.position, Quaternion.identity);
            swarm.DestroyCreature(gameObject);
        }
    }

    public void UpdateCreature()
    {
        Vector3 acceleration = Vector3.zero;

        if (target != null)
        {
            Vector3 offsetToTarget = (target.position - position);
            acceleration = SteerTowards(offsetToTarget) * settings.targetWeight;
        }

        if (numPerceivedFlockmates != 0)
        {
            centreOfFlockmates /= numPerceivedFlockmates;

            Vector3 offsetToFlockmatesCentre = (centreOfFlockmates - position);

            var alignmentForce = SteerTowards(avgFlockHeading) * settings.alignWeight;
            var cohesionForce = SteerTowards(offsetToFlockmatesCentre) * settings.cohesionWeight;
            var seperationForce = SteerTowards(avgAvoidanceHeading) * settings.seperateWeight;

            acceleration += alignmentForce;
            acceleration += cohesionForce;
            acceleration += seperationForce;
        }

        if (IsHeadingForCollision())
        {
            Vector3 collisionAvoidDir = ObstacleRays();
            Vector3 collisionAvoidForce = SteerTowards(collisionAvoidDir) * settings.avoidCollisionWeight;
            acceleration += collisionAvoidForce;
        }


        velocity += acceleration * Time.deltaTime;
        float speed = velocity.magnitude;
        Vector3 dir = velocity / speed;
        speed = Mathf.Clamp(speed, settings.minSpeed, settings.maxSpeed);
        velocity = dir * speed;

        cachedTransform.position += velocity * Time.deltaTime;
        cachedTransform.forward = dir;
        position = cachedTransform.position;
        forward = dir;
    }

    bool IsHeadingForCollision()
    {
        RaycastHit hit;
        if (Physics.SphereCast(position, settings.boundsRadius, forward, out hit, settings.collisionAvoidDst, settings.obstacleMask))
        {
            return true;
        }
        else { }
        return false;
    }

    Vector3 ObstacleRays()
    {
        Vector3[] rayDirections = BoidHelper.directions;

        for (int i = 0; i < rayDirections.Length; i++)
        {
            Vector3 dir = cachedTransform.TransformDirection(rayDirections[i]);
            Ray ray = new Ray(position, dir);
            if (!Physics.SphereCast(ray, settings.boundsRadius, settings.collisionAvoidDst, settings.obstacleMask))
            {
                return dir;
            }
        }

        return forward;
    }

    Vector3 SteerTowards(Vector3 vector)
    {
        Vector3 v = vector.normalized * settings.maxSpeed - velocity;
        return Vector3.ClampMagnitude(v, settings.maxSteerForce);
    }
}
