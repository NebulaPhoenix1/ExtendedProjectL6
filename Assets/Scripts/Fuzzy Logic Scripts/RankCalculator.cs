using UnityEngine;
using FuzzyLogicSystem;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class RankCalculator : MonoBehaviour
{
    //Debug Testing Controls
    [SerializeField] private float averageHitsTaken;
    [SerializeField] private float averageTrapHitsTaken;
    private FuzzyLogic fuzzyLogic = null;
    public TextAsset fuzzyLogicData = null;


    [SerializeField] private float playerRank = 50; //Between 0 - 100 (100 is perfect player, 0 is awful)
    [SerializeField] private int maxRankSwingPerRoom = 20; //The maximum rank is allowed to change per room

    //Returns a rank between 0-1 (0 is the worst a player can be, 1 is the best)
    public void CalculateRankDelta()
    {
        //Clear memory if not play mode, no idea why but this fixed the evaluations outside play mode lmao
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            fuzzyLogic = null;
        }
#endif


        if (fuzzyLogic == null)
        { 
            if (fuzzyLogicData != null)
                fuzzyLogic = FuzzyLogic.Deserialize(fuzzyLogicData.bytes, null);
            else
            {
                Debug.LogError("Cannot evaluate: FuzzyLogicData is missing.");
                return;
            }
        }

        fuzzyLogic.evaluate = true;

        //Pass values in 
        var hitsInput = fuzzyLogic.GetFuzzificationByName("averageHitsTaken");
        if (hitsInput != null)
        {
            hitsInput.value = averageHitsTaken;
        }
        else
        {
            Debug.LogError("Could not find Fuzzification parameter 'averageHitsTaken'");
        }

        var trapsInput = fuzzyLogic.GetFuzzificationByName("averageTrapDamageTaken");
        if (trapsInput != null)
        {
            trapsInput.value = averageTrapHitsTaken;
        }
        else
        {
            Debug.LogError("Could not find Fuzzification parameter 'averageTrapDamageTaken'");
        }

        float rawRankDelta = fuzzyLogic.Output();

        /*

        float slightIncreaseWeight;
        float massiveIncreaseWeight;
        float slightDecreaseWeight;
        float massiveDecreaseWeight;

        //Takes the tracked room data gets and calculates that sets membership value
        //averageHitsTaken
        slightIncreaseWeight = fuzzyLogic.GetInferenceByName("Increase Slight").Output();
        massiveIncreaseWeight = fuzzyLogic.GetInferenceByName("Increase Massive").Output();
        slightDecreaseWeight = fuzzyLogic.GetInferenceByName("Slight Decrease").Output();
        massiveDecreaseWeight = fuzzyLogic.GetInferenceByName("Decrease Massive").Output();


        //Debug log everything
        Debug.Log("Slight Increase: " + slightIncreaseWeight + " ,Massive Increase: " + massiveIncreaseWeight + "\n Slight Decrease: " + slightDecreaseWeight + " ,Massive Decrease: " + massiveDecreaseWeight);

        //Debug.Log("Average Hits: " + averageHitsTaken + " , Average Trap Hits: " + averageTrapHitsTaken);
        //Debug.Log("Actual average hits: " + hitsInput.value + " , Actual average trap hits: " + trapsInput.value);

        */

        //This line of code takes the rankDelta from being between 0 and 1 to between -1 and 1
        //Where -1 = massive decrease of rank, 1 = massive increase, and 0 = stay the same
        float rankDelta = (rawRankDelta - 0.5f) * 2f * maxRankSwingPerRoom;
        playerRank = Mathf.Clamp(playerRank + rankDelta, 0f, 100f);
        Debug.Log($"Fuzzy Output: {rawRankDelta}. Rank Change: {rankDelta}. New Player Rank: {playerRank}");

        return;
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Added a check to prevent errors if the button is pressed before Play Mode
        if (fuzzyLogicData != null)
        {
            fuzzyLogic = FuzzyLogic.Deserialize(fuzzyLogicData.bytes, null);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(RankCalculator))]
public class RankCalculatorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Draw the default inspector (Fields: target, source, testTimeInput, etc.)
        DrawDefaultInspector();

        // Get a reference to the actual script
        RankCalculator script = (RankCalculator)target;

        // Add some spacing
        GUILayout.Space(10);

        // Draw the Button
        if (GUILayout.Button("Evaluate Time & Debug"))
        {
            // Call the method using the variable set in the inspector
            script.CalculateRankDelta();
        }
    }
}
#endif
