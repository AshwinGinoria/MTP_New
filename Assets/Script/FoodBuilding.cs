using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodBuilding : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject food;
    public GameObject badFood;
    public GameObject chair;
    public GameObject table;
    public GameObject chotiGun;
    public GameObject badiGun;
    public GameObject bomb;
    public int numFood;
    public int numBadFood;

    void CreateFood(int num, GameObject type) {
        CollectableObjectsCreator(chotiGun, 15);
        CollectableObjectsCreator(bomb, 25);
        CollectableObjectsCreator(badiGun, 10);        
    }
 
     void CreateBadFood(int num, GameObject type) {
        
        float xRangeStart = -9f;
        float xRangeEnd = 17f;
        float zRangeStart = -17f;
        float zRangeEnd = 17f;


        // for(int i = 0 ; i < num ; i++) {
        //     GameObject a = Instantiate(type, new Vector3(Random.Range(xRangeStart, xRangeEnd), 12f,
        //         Random.Range(zRangeStart, zRangeEnd)) , Quaternion.Euler(new Vector3(0f, 0f, 0f)) );
        //     GameObject b = Instantiate(type, new Vector3(Random.Range(xRangeStart, xRangeEnd), 2f,
        //         Random.Range(zRangeStart, zRangeEnd)) , Quaternion.Euler(new Vector3(0f, 0f, 0f)));
        // }
        ObstacleObjectsCreator(14f);
        ObstacleObjectsCreator(4f);
        
        
    }

    void CollectableObjectsCreator(GameObject type, int num) {
        float xRangeStart = -9f;
        float xRangeEnd = 17f;
        float zRangeStart = -17f;
        float zRangeEnd = 17f;
        for(int i = 0 ; i < num ; i++) {
            GameObject a = Instantiate(type, new Vector3(Random.Range(xRangeStart, xRangeEnd), 12f,
                Random.Range(zRangeStart, zRangeEnd)) , Quaternion.Euler(new Vector3(0f, Random.Range(0, 180), 90f)) );
            GameObject b = Instantiate(type, new Vector3(Random.Range(xRangeStart, xRangeEnd), 2f,
                Random.Range(zRangeStart, zRangeEnd)) , Quaternion.Euler(new Vector3(0f, Random.Range(0, 180), 90f)));
        }
    }

    void ObstacleObjectsCreator(float yIndex) {
        HashSet<float> store = new HashSet<float>();
        float xRangeStart = -10f;
        float xRangeSize = 9f;
        float zRangeStart = -17f;
        float zRangeSize = 11f;            


        for(int i = 0; i < 20; i++) {
            float xIndex = Random.Range(0, xRangeSize); // -7f --> 14f
            float zIndex = Random.Range(0, zRangeSize); // -14f --> 13f
            float index = zIndex*xRangeSize + xIndex;
            if(store.Contains(index)) {
                i--;
                continue;
            } 
            store.Add(index);
            xIndex = 3 * xIndex + xRangeStart + Random.Range(0.5f, 2.5f);
            zIndex = 3 * zIndex + zRangeStart + Random.Range(0.5f, 2.5f);
            Instantiate(chair, new Vector3(xIndex, yIndex, zIndex), Quaternion.Euler(new Vector3(0f, 0f, 0f)));
        }
        for(int i = 0; i < 10; i++) {
            float xIndex = Random.Range(0, xRangeSize); // -7f --> 14f
            float zIndex = Random.Range(0, zRangeSize); // -14f --> 13f
            float index = zIndex*xRangeSize + xIndex;
            if(store.Contains(index)) {
                i--;
                continue;
            } 
            store.Add(index);
            xIndex = 3 * xIndex + xRangeStart + 1.5f;
            zIndex = 3 * zIndex + zRangeStart + 1.5f;
            Instantiate(table, new Vector3(xIndex, yIndex, zIndex), Quaternion.Euler(new Vector3(0f, 0f, 0f)));
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
