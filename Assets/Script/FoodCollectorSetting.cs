using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.MLAgents;

public class FoodCollectorSetting : MonoBehaviour
{

    [HideInInspector]
    public GameObject[] agents;
    [HideInInspector]
    public FoodBuilding[] foodArea;

    public int totalScore;
    public Text scoreText;

    StatsRecorder m_Recorder;
    
    public void Awake()
    {
        Academy.Instance.OnEnvironmentReset += EnvironmentReset;
        m_Recorder = Academy.Instance.StatsRecorder;
    }

    public void EnvironmentReset()
    {
        ClearObjects(GameObject.FindGameObjectsWithTag("goodFood"));
        ClearObjects(GameObject.FindGameObjectsWithTag("badFood"));

        agents = GameObject.FindGameObjectsWithTag("agent");
        foodArea = FindObjectsOfType<FoodBuilding>();
        foreach (var fa in foodArea)
        {
            fa.ResetFoodArea(agents);
        }

        totalScore = 0;
    }

    void ClearObjects(GameObject[] objects)
    {
        foreach (var food in objects)
        {
            Destroy(food);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
