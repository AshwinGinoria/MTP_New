
using UnityEngine;
using UnityEngine.UI;

public class signal : MonoBehaviour
{
    // Start is called before the first frame update
    public FoodCollectorAgent Agent;
    public Text score;

    // Update is called once per frame
    void Update()
    {
        score.text = Agent.goodBallCount.ToString();
    }
}
