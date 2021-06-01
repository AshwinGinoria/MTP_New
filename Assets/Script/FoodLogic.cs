using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodLogic : MonoBehaviour
{
    public void OnEaten()
    {           
        Destroy(gameObject);
    }
}
