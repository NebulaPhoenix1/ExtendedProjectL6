using UnityEngine;
using FuzzyLogicSystem;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class FuzzyLogicTest : MonoBehaviour
{
    public Transform target = null;
    public Transform source = null;
    public TextAsset fuzzyLogicData = null;

    // 1. Add a field to type in the time you want to test
    [Header("Debug Controls")]
    [Tooltip("Enter the time value here to test with the button below.")]
    public float testTimeInput = 0f;

    private FuzzyLogic fuzzyLogic = null;

    private void Start()
    {
        // Added a check to prevent errors if the button is pressed before Play Mode
        if (fuzzyLogicData != null)
        {
            fuzzyLogic = FuzzyLogic.Deserialize(fuzzyLogicData.bytes, null);
        }
    }

    public void EvaluateTime(float time)
    {
        // Ensure logic exists (useful if testing in Edit Mode without Play Mode)
        if (fuzzyLogic == null) 
        {
            if(fuzzyLogicData != null)
                fuzzyLogic = FuzzyLogic.Deserialize(fuzzyLogicData.bytes, null);
            else
            {
                Debug.LogError("Cannot evaluate: FuzzyLogicData is missing.");
                return;
            }
        }

        fuzzyLogic.evaluate = true;
        
        // Safety check in case the Fuzzification name is wrong
        var fuzzification = fuzzyLogic.GetFuzzificationByName("timeTaken");
        if (fuzzification != null)
        {
            fuzzification.value = time;
            float rating = fuzzyLogic.Output() * 100; //100 is the max value so we multiply by it
            Debug.Log($"<color=cyan>Fuzzy Logic Rating (Time: {time}):</color> <b>{rating}</b>");
            float goodRating = fuzzyLogic.GetInferenceByName("Good Output").Output();
            float badRating = fuzzyLogic.GetInferenceByName("Bad Output").Output();
            Debug.Log($"<color=green>Good Rating:</color> <b>{goodRating}</b>");
            Debug.Log($"<color=red>Bad Rating:</color> <b>{badRating}</b>");
        }
        else
        {
            Debug.LogError("Could not find Fuzzification parameter named 'timeTaken'");
        }
    }
}

// -------------------------------------------------------------------------
// 2. The Custom Editor Script
// This tells Unity how to draw the inspector for this specific script.
// -------------------------------------------------------------------------
#if UNITY_EDITOR
[CustomEditor(typeof(FuzzyLogicTest))]
public class FuzzyLogicTestEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Draw the default inspector (Fields: target, source, testTimeInput, etc.)
        DrawDefaultInspector();

        // Get a reference to the actual script
        FuzzyLogicTest script = (FuzzyLogicTest)target;

        // Add some spacing
        GUILayout.Space(10);

        // Draw the Button
        if (GUILayout.Button("Evaluate Time & Debug"))
        {
            // Call the method using the variable set in the inspector
            script.EvaluateTime(script.testTimeInput);
        }
    }
}
#endif