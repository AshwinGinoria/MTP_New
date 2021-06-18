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

    public List<GameObject> CreateStairs() {
        List<GameObject> item = new List<GameObject>();
        foreach (var fa in foodArea) {
            item = fa.CreateStairs();
        }
        return item;
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
