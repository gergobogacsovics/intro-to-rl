using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class SubmarineController : Agent {
    Rigidbody rigidBody;
    public float speed = 10;

    public Transform TargetTransform;

    private GameObject goal;

    private const float MAX_DISTANCE = 28.28427f;

    // Start is called before the first frame update
    void Start()
    {
    }

    public override void OnEpisodeBegin() {
        transform.localPosition = new Vector3(0, 0.5f, 0);

        // Generate a random position for the treasure prefab 
        float xPosition = UnityEngine.Random.Range(-9, 9);
        float zPosition = UnityEngine.Random.Range(-9, 9);

        // Assign the randomly generated position to the treasure prefab
        //TargetTransform.localPosition = new Vector3(xPosition, 0.5f, zPosition);
    }

    public override void CollectObservations(VectorSensor sensor) {
        // The position of the agent
        sensor.AddObservation(transform.localPosition.x);
        sensor.AddObservation(transform.localPosition.y);

        // The position of the treasure prefab
        sensor.AddObservation(TargetTransform.localPosition.x);
        sensor.AddObservation(TargetTransform.localPosition.y);

        // The distance between the agent and the treasure
        //sensor.AddObservation(Vector3.Distance(TargetTransform.localPosition, transform.localPosition));
    }

    public override void OnActionReceived(ActionBuffers actions) {
        var actionTaken = actions.ContinuousActions;

        float actionSpeed = actionTaken[0];//(actionTaken[0] + 1) / 2; // [0, +1]
        float actionSteering = actionTaken[1]; // [-1, +1]

        transform.Translate(actionSpeed * Vector3.forward * speed * Time.fixedDeltaTime);
        transform.rotation = Quaternion.Euler(new Vector3(0, actionSteering * 180, 0));

        float distance_scaled = Vector3.Distance(TargetTransform.localPosition, transform.localPosition) / MAX_DISTANCE;
        //Debug.Log(distance_scaled);

        AddReward(-distance_scaled / 10); // [0, 0.1]
    }

    public override void Heuristic(in ActionBuffers actionsOut) {
        ActionSegment<float> actions = actionsOut.ContinuousActions;

        actions[0] = -1;
        actions[1] = 0;

        if (Input.GetKey("w"))
            actions[0] = 1;
        
        if (Input.GetKey("d"))
            actions[1] = +0.5f;

        if (Input.GetKey("a"))
            actions[1] = -0.5f;
    }

    private void OnCollisionEnter(Collision collision) {
        if (collision.collider.tag == "Wall") {
            AddReward(-1);
            EndEpisode();
        }
        else if (collision.collider.tag == "Treasure") {
            AddReward(1);
            EndEpisode();
        }
    }
}
