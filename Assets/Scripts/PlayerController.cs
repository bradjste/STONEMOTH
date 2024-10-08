using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    GameManager gameManager;
    static GameObject goCopy;

    void Start()
    {
        gameManager = GameManager.Instance;
        goCopy = gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        GameObject reticule = gameManager.userInterface.reticuleObject;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit = new RaycastHit();

        if (Physics.Raycast(ray, out hit))
        {
            GameObject hitObject = hit.collider.gameObject;

            if (hitObject.CompareTag("Ground"))
            {
                // Move reticule
                reticule.SetActive(true);
                reticule.transform.position = new Vector3(hit.point.x, 0, hit.point.z);

                bool moveSwarmA = Input.GetMouseButtonDown(0) && gameManager.swarmCount > 0;
                bool moveSwarmB = Input.GetMouseButtonDown(1) && gameManager.swarmCount > 1;
                int swarmNum = 0;
                if (moveSwarmA)
                {
                    swarmNum = 0;
                }
                else if (moveSwarmB)
                {
                    swarmNum = 1;
                }

                if (hitObject.CompareTag("Ground") && (moveSwarmA || moveSwarmB))
                {
                    Swarm swarm = gameManager.swarms[swarmNum];
                    swarm.SetTargetPosition(hit.point + new Vector3(0, 1, 0));
                }
            } else
            {
                // Hide reticule
                reticule.SetActive(false);
            }
        }
        else
        {
            // Hide reticule
            reticule.SetActive(false);
        }
    }

    public static void ResetCamera()
    {
        goCopy.transform.position = new Vector3(0, 80, 0);
    }
}
