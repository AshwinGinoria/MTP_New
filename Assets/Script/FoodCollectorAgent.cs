using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Random = UnityEngine.Random;
public class FoodCollectorAgent : Agent
{

    FoodCollectorSetting foodCollectorSetting;
    public GameObject area;
    FoodBuilding foodBuilding;
    Rigidbody agentRb;


    // speed of agent rotation 
    public float turnSpeed = 300;
    public float moveSpeed = 2;

    public Material badMaterial;
    public Material goodMaterial;

    //
    public Transform groundCheck;
    public float groundDistance = 0;
    public LayerMask groundMask;

    // ramp 
    public Transform rampCheck;
    public float rampDistance = 0;
    public LayerMask rampMask;


    bool isGrounded;
    bool isAgentOnRamp;
    public float goodBallCount = 0;
    public float badBallCount = 0;
    
    private bool isMoved = false;
    private Vector3 initialPos;
    private Quaternion initialRot;
    private float numSteps;
    private float groundNoGoodBallsCollected = 0;
    private float firstFloorNoGoodBallsCollected = 0;
    private float numGroundGoodBalls;
    private float numFirstGoodBalls;

    private float prevYPosition = 0;
    private float startingFrom = 0; // 0 ground , 1 first floor
    private float isReachedUppar = 0;
    private float isReachedNiche = 0;
    private bool isCollectedAction = false;
    private bool isStairsCreated = false;
    private List<GameObject> stairs;
    private int currentIndex = -1;

    EnvironmentParameters resetParameters;

    public override void Initialize() {
        agentRb = GetComponent<Rigidbody>();
        foodBuilding = area.GetComponent<FoodBuilding>();
        foodCollectorSetting = FindObjectOfType<FoodCollectorSetting>();
        resetParameters = Academy.Instance.EnvironmentParameters;
        initialPos=this.transform.position;
        initialRot = this.transform.rotation;
        numGroundGoodBalls = foodBuilding.numFood;
        numFirstGoodBalls = foodBuilding.numFood;        
    }

    public override void CollectObservations(VectorSensor sensor) {
        var localVelocity = transform.InverseTransformDirection(agentRb.velocity);
        sensor.AddObservation(localVelocity.x);
        sensor.AddObservation(localVelocity.z);
        if(CheckonRamp()) {
            sensor.AddObservation(1);
        } else {
            sensor.AddObservation(0);
        }
        sensor.AddObservation(this.transform.localPosition);
    }

    public bool CheckonRamp() {
        bool isOnRamp = Physics.CheckSphere(rampCheck.position, rampDistance, rampMask);
        return isOnRamp;
    }

    public void MoveAgent(ActionSegment<int> act)
    {   
        var action = (int)act[0];
        this.isCollectedAction = false;
        // 'C' for collecting objects
        if(action == 4) {
            this.isCollectedAction = true;
        }

        // 'Increase Number of steps'
        if(action != 6)
            this.numSteps += 1;
        // unnecessary backward steps;
        if(action == 3) {
            AddReward(-0.02f);
        }
        if(this.numSteps >= 25000) {
            this.numSteps = 0;
            this.startingFrom = 0;
            this.currentIndex = -1;
            foodCollectorSetting.EnvironmentReset();
            EndEpisode();
            this.OnEpisodeBegin();
        }

        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if(!isGrounded && !CheckonRamp()) {
            agentRb.AddForce(-transform.up * 0.2f, ForceMode.VelocityChange);
        }
        var dirToGo = Vector3.zero;
        var rotateDir = Vector3.zero;

        switch (action)
        {   
            case 0:
                dirToGo = transform.forward;
                break;
            case 3:
                dirToGo = -transform.forward;
                break;
            case 1:
                rotateDir = -transform.up;
                break;
            case 2:
                rotateDir = transform.up;
                break;
        }

        // jump on ground without any reason.
        if(action == 5 && isGrounded) {
            AddReward(-0.05f);
        }

        agentRb.AddForce(dirToGo * moveSpeed, ForceMode.VelocityChange);
        transform.Rotate(rotateDir, Time.fixedDeltaTime * turnSpeed);
        if(action == 5 && CheckonRamp()) {
            agentRb.AddForce(transform.up * 5f , ForceMode.VelocityChange);
        }
        if(action == 5 && isGrounded) {
            agentRb.AddForce(transform.up * 1.2f , ForceMode.VelocityChange);
        }
        if (agentRb.velocity.sqrMagnitude > 25f) // slow it down
        {
            agentRb.velocity *= 0.95f;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("badFood"))
        {
            AddReward(-1f);
            // collision.gameObject.GetComponent<FoodLogic>().OnEaten();
            badBallCount += 1;
        }
        if (collision.gameObject.CompareTag("ramp")) {
            int index = -1;
            for(int i = 0; i < 10 ; i++) {
                if(collision.gameObject.GetInstanceID() == this.stairs[i].GetInstanceID())
                    index = i;
            }
            if(index != -1) {
                if(startingFrom == 0) {
                    if(index > currentIndex) {
                        AddReward(+0.5f);
                    } else if(index < currentIndex) {
                        AddReward(-0.5f);
                    }
                    currentIndex = index;
                    if(currentIndex == 9) {
                        startingFrom = 1;
                    }
                } else if(startingFrom == 1) {
                    if(index < currentIndex) {
                        AddReward(0.5f);
                    } else if (index > currentIndex) {
                        AddReward(-0.5f);
                    }
                    currentIndex = index;
                    if(currentIndex == 0) {
                        startingFrom = -1;
                    }
                }
            }

        }
    }

    void OnTriggerStay(Collider collision) {
        // Debug.Log(this.isCollectedAction);
        if(this.isCollectedAction) {
            if (collision.gameObject.CompareTag("goodFood"))
            {
                AddReward(1f);
                collision.gameObject.GetComponent<FoodLogic>().OnEaten();
                goodBallCount += 1;
                if(agentRb.position.y < 5) {
                    groundNoGoodBallsCollected+= 1;
                } else {
                    firstFloorNoGoodBallsCollected += 1;
                }
            }
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActionsOut = actionsOut.DiscreteActions;
        discreteActionsOut[0] = 6;
        if (Input.GetKey(KeyCode.W))
        {
            discreteActionsOut[0] = 0;
        }
        if (Input.GetKey(KeyCode.S))
        {
            discreteActionsOut[0] = 3;
        }
        if (Input.GetKey(KeyCode.A))
        {
            discreteActionsOut[0] = 1;
        }
        if (Input.GetKey(KeyCode.D))
        {
            discreteActionsOut[0] = 2;
        }
        if (Input.GetKey(KeyCode.C)) {
            discreteActionsOut[0] = 4;
        }
        if (Input.GetKey(KeyCode.Space)) {
            discreteActionsOut[0] = 5;
        }
    }

    public override void OnEpisodeBegin()
    {
        if(isStairsCreated == false) {
            this.stairs = foodCollectorSetting.CreateStairs();
            isStairsCreated = true;
        }
        agentRb.velocity = Vector3.zero;
        this.transform.position = initialPos;
        this.transform.rotation= initialRot;
        this.startingFrom = 0;
        this.currentIndex = -1;
        goodBallCount=0;
        badBallCount=0;
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        MoveAgent(actionBuffers.DiscreteActions);
    }


}
