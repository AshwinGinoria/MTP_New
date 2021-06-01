using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodBuilding : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject food;
    public GameObject badFood;
    public int numFood;
    public int numBadFood;

    void CreateFood(int num, GameObject type) {
        
        float xRangeStart = -9f;
        float xRangeEnd = 17f;
        float zRangeStart = -17f;
        float zRangeEnd = 17f;


        for(int i = 0 ; i < num ; i++) {
            GameObject a = Instantiate(type, new Vector3(Random.Range(xRangeStart, xRangeEnd), 12f,
                Random.Range(zRangeStart, zRangeEnd)) , Quaternion.Euler(new Vector3(0f, 0f, 0f)) );
            GameObject b = Instantiate(type, new Vector3(Random.Range(xRangeStart, xRangeEnd), 2f,
                Random.Range(zRangeStart, zRangeEnd)) , Quaternion.Euler(new Vector3(0f, 0f, 0f)));
        }
    }
 
     void CreateBadFood(int num, GameObject type) {
        
        float xRangeStart = -9f;
        float xRangeEnd = 17f;
        float zRangeStart = -17f;
        float zRangeEnd = 17f;


        for(int i = 0 ; i < num ; i++) {
            GameObject a = Instantiate(type, new Vector3(Random.Range(xRangeStart, xRangeEnd), 12f,
                Random.Range(zRangeStart, zRangeEnd)) , Quaternion.Euler(new Vector3(0f, 0f, 0f)) );
            GameObject b = Instantiate(type, new Vector3(Random.Range(xRangeStart, xRangeEnd), 2f,
                Random.Range(zRangeStart, zRangeEnd)) , Quaternion.Euler(new Vector3(0f, 0f, 0f)));
        }
    }


    public void ResetFoodArea(GameObject[] agents)
    {
        foreach (GameObject agent in agents)
        {
            // need to reset agent position -- as of now noot needed
        }

        CreateFood(numFood, food);
        CreateBadFood(numBadFood, badFood);
    }

}
