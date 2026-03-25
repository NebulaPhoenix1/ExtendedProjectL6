using UnityEngine;
using FuzzyLogicSystem;
using TMPro;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class RankCalculator : MonoBehaviour
{
    //Debug Testing Controls
    //[SerializeField] private float averageHitsTaken; //How much dmg on average was taken in each room
    //[SerializeField] private float averageTrapHitsTaken; //How much trap dmg was taken in each room
    //[SerializeField] private float averageTimePerEnemyKilled; //Enemies per room/total time for room
    //[SerializeField] private float accuracy; //hits landed/hits taken as a percentage 

    [SerializeField] private bool showRankInUI;
    [SerializeField] private TMP_Text rankDisplayText;

    private FuzzyLogic fuzzyLogic = null;
    public TextAsset fuzzyLogicData = null;


    [SerializeField] private float newPlayerRank; //Between 0 - 100 (100 is perfect player, 0 is awful)
    [SerializeField] private int maxRankSwingPerRoom = 20; //The maximum rank is allowed to change per room
    

    //Returns a rank between 0-1 (0 is the worst a player can be, 1 is the best)
    public void CalculateRankDelta(float averageHitsTaken, float averageTrapHitsTaken, float averageTimePerEnemy, float accuracy)
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

        var timePerEnemyInput = fuzzyLogic.GetFuzzificationByName("averageTimePerEnemyKilled");
        if (timePerEnemyInput != null)
        {
            timePerEnemyInput.value = averageTimePerEnemy;
        }
        else
        {
            Debug.LogError("COuld not find Fuzzification parameter 'averageTimePerEnemyKilled'");
        }

        var accuracyInput = fuzzyLogic.GetFuzzificationByName("accuracy");
        if (accuracyInput != null)
        {
            accuracyInput.value = accuracy;
        }
        else
        {
            Debug.LogError("COuld not find Fuzzification parameter 'accuracy'");
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

        float currentPlayerRank = StatTracker.Instance.GetRank();

        float rankDelta = (rawRankDelta - 0.5f) * 2f * maxRankSwingPerRoom;
        newPlayerRank = Mathf.Clamp(currentPlayerRank + rankDelta, 0f, 100f);
        Debug.Log($"Fuzzy Output: {rawRankDelta}. Rank Change: {rankDelta}. New Player Rank: {newPlayerRank}");
        Debug.Log($"Avg Hits Taken: {averageHitsTaken} , AvgTrapHitsTaken {averageTrapHitsTaken} , AvgTimePerEnemy {averageTimePerEnemy} , Accuracy {accuracy}");

        if(showRankInUI && rankDisplayText != null )
        {
            rankDisplayText.text = "Rank :" + newPlayerRank.ToString();
        }
        StatTracker.Instance.SetRank( newPlayerRank );
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
        //Get Player Rank saved to JSON
        StatTracker statTracker = StatTracker.Instance;
        if (statTracker != null)
        {
            newPlayerRank = statTracker.GetRank();
        }
        else
        {
            Debug.LogWarning("Could not get player rank from StatTracker, using default value of 50");
        }
        if(showRankInUI && rankDisplayText != null) { rankDisplayText.text = "Rank: " + newPlayerRank.ToString(); }
        
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
            //script.CalculateRankDelta( averageHitsTaken, averageTrapHitsTaken, averageTimePerEnemy, acuracy);
        }
    }
}
#endif
