using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class MyAgent : Agent
{
    public float RayCastLength = 20;

    Rigidbody m_rigidbody;
    float m_speed = 20;

    public GameObject Spawner;

    private Vector3 startingPosition = new Vector3(0.0f, 1.59f, -5.0f);

    private float boundXLeft = -20f;
    private float boundXRight = 20f;

    private enum ACTIONS {
        LEFT = 0,
        NOTHING = 1,
        RIGHT = 2
    }
    
    void Start()
    {

    }

    public override void OnEpisodeBegin() {
        m_rigidbody = GetComponent<Rigidbody>();
        transform.localPosition = startingPosition;
    }

    private int DoARaycast(Vector3 direction) {
        RaycastHit hit;
        Ray landingRay = new Ray(transform.localPosition, direction);

        if(Physics.Raycast(landingRay, out hit, RayCastLength) && hit.collider.tag == "Ball") {
            Debug.DrawRay(transform.localPosition, transform.TransformDirection(direction) * hit.distance, Color.red);
            //Debug.DrawLine(transform.localPosition, hit.point, Color.red);
            return 1;
        }
        else {
            //Debug.DrawLine(transform.localPosition, direction * RayCastLength, Color.green);
            return 0;
        }


    }

    public override void CollectObservations(VectorSensor sensor) {
        // We do a raycast in each direction
        for(int i = 0; i < 6; i++) {
            var direction = new Vector3(0.5f - (i * 2 / 10.0f), 0.3f, 1.0f);
            
            sensor.AddObservation(DoARaycast(direction));
        }

    }


    public override void Heuristic(in ActionBuffers actionsOut) {
        ActionSegment<int> actions = actionsOut.DiscreteActions;

        var horizontal = Input.GetAxisRaw("Horizontal");
        var vertical = Input.GetAxisRaw("Vertical");

        if (horizontal == -1) {
            actions[0] = (int)ACTIONS.LEFT;
        }
        else if (horizontal == +1) {
            actions[0] = (int)ACTIONS.RIGHT;
        }
        else {
            actions[0] = (int)ACTIONS.NOTHING;
        }

    }


    public override void OnActionReceived(ActionBuffers actions) {
        var actionTaken = actions.DiscreteActions[0];

        switch (actionTaken) {
            case (int)ACTIONS.NOTHING:
                break;
            case (int)ACTIONS.LEFT:
                // We translate the agent's body to the left if it can move left
                if (transform.localPosition.x > boundXLeft)
                    transform.Translate(-Vector3.right * m_speed * Time.deltaTime);
                break;
            case (int)ACTIONS.RIGHT:
                // We translate the agent's body to the right if it can move right
                if (transform.localPosition.x < boundXRight)
                    transform.Translate(Vector3.right * m_speed * Time.deltaTime);
                break;
        }

        AddReward(0.1f);
    }

    private void OnTriggerEnter(Collider other) {
        // If the agent collided with a ball, we delete the Balls & end the episode
        if (other.tag == "Ball") {
            // We delete each Ball object that we have spawned so far 
            var parent = Spawner.transform;
            int numberOfChildren = parent.childCount;

            for (int i = 0; i < numberOfChildren; i++) {
                if (parent.GetChild(i).tag == "Ball") {
                    Destroy(parent.GetChild(i).gameObject);
                }
            }

            EndEpisode();
        }
    }

}
