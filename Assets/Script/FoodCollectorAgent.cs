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
    private float startingFrom = -1; // 0 ground , 1 first floor
    private float isReachedUppar = 0;
    private float isReachedNiche = 0;

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

        if(action != 3)
            this.numSteps += 1;
        if(this.numSteps >= 20000) {
            this.numSteps = 0;
            foodCollectorSetting.EnvironmentReset();
            EndEpisode();
            this.OnEpisodeBegin();
        }
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        // AddReward(-0.001f);

        if(agentRb.position.y >= 10 && groundNoGoodBallsCollected / (numGroundGoodBalls + 1e-6) >= 0.5) {
            AddReward(2f);
            // Debug.Log("+5 added");
            numGroundGoodBalls -= groundNoGoodBallsCollected;
            groundNoGoodBallsCollected = 0;
        }  
        if(agentRb.position.y < 8 && !CheckonRamp() && firstFloorNoGoodBallsCollected / (numFirstGoodBalls + 1e-5) >= 0.5) {
            AddReward(6f);
            // Debug.Log("+6 added down");
            numFirstGoodBalls -= firstFloorNoGoodBallsCollected;
            firstFloorNoGoodBallsCollected = 0;
        }


        if(CheckonRamp() ) {
            if(startingFrom == -1) {
                if(prevYPosition <= 1) {
                    startingFrom = 0;

                } else {
                    startingFrom = 1;
                }
            }
            if(startingFrom == 0 && isReachedUppar == 0) {
                if(agentRb.position.y > prevYPosition) {
                    // Debug.Log("rawrd added ");
                    AddReward(0.2f);
                } else if(agentRb.position.y < prevYPosition) {
                    AddReward(-0.2f);
                    // Debug.Log("penalt aded");
                }
            } else if(isReachedNiche == 0) {
                if(agentRb.position.y < prevYPosition) {
                    AddReward(0.2f);
                } else if(agentRb.position.y > prevYPosition) {
                    AddReward(-0.2f);
                }
            }

        } else {
            if(startingFrom == 0 && agentRb.position.y > 1) {
                isReachedUppar = 1;
            }
            if(startingFrom == 1 && agentRb.position.y < 1) {
                isReachedNiche = 1;
            }
            startingFrom = -1;
        }
        prevYPosition = agentRb.position.y;
        if(!isGrounded && !CheckonRamp()) {
            
            agentRb.AddForce(transform.up * -3000, ForceMode.Force);
        }
        var dirToGo = Vector3.zero;
        var rotateDir = Vector3.zero;

        
        switch (action)
        {   
            case 0:
                dirToGo = transform.forward;
                break;
            // case 1:
            //     dirToGo = -transform.forward;
            //     break;
            case 1:
                rotateDir = -transform.up;
                break;
            case 2:
                rotateDir = transform.up;
                break;
        }
        agentRb.AddForce(dirToGo * moveSpeed, ForceMode.VelocityChange);
        transform.Rotate(rotateDir, Time.fixedDeltaTime * turnSpeed);

        if (agentRb.velocity.sqrMagnitude > 25f) // slow it down
        {
            agentRb.velocity *= 0.95f;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
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
        if (collision.gameObject.CompareTag("badFood"))
        {
            AddReward(-1f);
            collision.gameObject.GetComponent<FoodLogic>().OnEaten();
            badBallCount += 1;
        }
        if (collision.gameObject.CompareTag("wall")) 
        {
            AddReward(-0.2f);
        }
        // Debug.Log("badBallCount = "+ badBallCount);
        // Debug.Log("goodBallCount = "+ goodBallCount);
    }
    void OnCollisionStay(Collision collision) {
        if (collision.gameObject.CompareTag("wall")) 
        {
            AddReward(-0.2f);
        }
    }


    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActionsOut = actionsOut.DiscreteActions;
        discreteActionsOut[0] = 3;
        if (Input.GetKey(KeyCode.W))
        {
            discreteActionsOut[0] = 0;
        }
        // if (Input.GetKey(KeyCode.S))
        // {
        //     discreteActionsOut[0] = 1;
        // }
        if (Input.GetKey(KeyCode.A))
        {
            discreteActionsOut[0] = 1;
        }
        if (Input.GetKey(KeyCode.D))
        {
            discreteActionsOut[0] = 2;
        }
    }

    public override void OnEpisodeBegin()
    {
        agentRb.velocity = Vector3.zero;
        this.transform.position = initialPos;
        this.transform.rotation= initialRot;
        goodBallCount=0;
        badBallCount=0;
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        MoveAgent(actionBuffers.DiscreteActions);
    }


}
