using UnityEngine;
using FuzzyLogicSystem;

public class FuzzyLogicTest : MonoBehaviour
{
    public Transform target = null;

    public Transform source = null;

    public TextAsset fuzzyLogicData = null;

    private FuzzyLogic fuzzyLogic = null;

    private void Start()
    {
        fuzzyLogic = FuzzyLogic.Deserialize(fuzzyLogicData.bytes, null);
    }

    public void EvaluateTime(float time)
    {
        fuzzyLogic.evaluate = true;
        fuzzyLogic.GetFuzzificationByName("timeTaken").value = time;
        float rating = fuzzyLogic.Output();
        Debug.Log("Fuzzy Logic Rating: " + rating);
    }
}


 
