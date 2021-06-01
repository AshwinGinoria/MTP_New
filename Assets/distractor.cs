
using UnityEngine;
using UnityEngine.UI;

public class distractor : MonoBehaviour
{
    public FoodCollectorAgent Agent;
    public Text score;

    // Update is called once per frame
    void Update()
    {
        score.text = Agent.badBallCount.ToString();
    }
}
