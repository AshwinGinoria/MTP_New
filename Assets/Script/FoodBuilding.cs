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
    public Transform xMinWall;
    public Transform xMaxWall;
    public Transform zMinWall;
    public Transform zMaxWall;
    public Transform env;

    public int numFood;
    public int numBadFood;
    void CreateFood(int num, GameObject type) {
        CollectableObjectsCreator(chotiGun, num * 2 / 5);
        CollectableObjectsCreator(bomb, num * 2 / 5);
        CollectableObjectsCreator(badiGun, num / 5);
    }
 
     void CreateBadFood(int num, GameObject type) {
        float xRangeStart = xMinWall.localPosition.x + 1f;
        float xRangeEnd = xMaxWall.localPosition.x - 1f;
        float zRangeStart = zMinWall.localPosition.z + 1.0f;
        float zRangeEnd = zMaxWall.localPosition.z - 1.0f;

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
        float xRangeStart = xMinWall.localPosition.x + 1f;
        float xRangeEnd = xMaxWall.localPosition.x - 1f;
        float zRangeStart = zMinWall.localPosition.z + 1.0f;
        float zRangeEnd = zMaxWall.localPosition.z - 1.0f;

        for(int i = 0 ; i < num ; i++) {
            GameObject a = Instantiate(type, env);
            a.transform.localPosition = new Vector3(Random.Range(xRangeStart, xRangeEnd), 12f, Random.Range(zRangeStart, zRangeEnd));
            a.transform.rotation = Quaternion.Euler(new Vector3(0f, Random.Range(0, 180), 90f));
            GameObject b = Instantiate(type, env);
            b.transform.localPosition = new Vector3(Random.Range(xRangeStart, xRangeEnd), 2f, Random.Range(zRangeStart, zRangeEnd));
            b.transform.rotation =  Quaternion.Euler(new Vector3(0f, Random.Range(0, 180), 90f));
        }
    }

    void ObstacleObjectsCreator(float yIndex) {
        HashSet<float> store = new HashSet<float>();
        float xRangeStart = xMinWall.localPosition.x + 1.0f;
        float xRangeSize = 9f;
        float zRangeStart = zMinWall.localPosition.z - 1.0f;
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
            GameObject chr = Instantiate(chair, env);
            chr.transform.localPosition = new Vector3(xIndex, yIndex, zIndex);
            chr.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
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
            GameObject tbl = Instantiate(table, env); 
            tbl.transform.localPosition = new Vector3(xIndex, yIndex, zIndex);
            tbl.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
        }
    }


    public void ResetFoodArea(GameObject[] agents)
    {
        foreach (GameObject agent in agents)
        {
            // need to reset agent position -- as of now not needed
        }

        CreateFood(numFood, food);
        CreateBadFood(numBadFood, badFood);
    }

}
