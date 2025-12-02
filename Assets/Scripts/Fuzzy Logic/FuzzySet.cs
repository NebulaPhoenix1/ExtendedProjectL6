using UnityEngine;

public class FuzzySet : MonoBehaviour
{
    [Tooltip("Name of fuzzy set e.g. 'Hot', 'Cold', 'Fast', 'Slow'")]
    public string fuzzySetName; //Name of fuzzy set
    public AnimationCurve membershipCurve; //Curve defining membership function
    public float EvaluateMembership(float input)
    {
        return membershipCurve.Evaluate(input);
    }
}
