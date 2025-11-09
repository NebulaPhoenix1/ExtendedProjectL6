using UnityEngine;
using System.IO;
using System.Linq;

//Events for keeping track of healing done in game for DDA later
[System.Serializable]
public class HealEvent
{
    private uint healAmount;
    private GameObject source;
    private GameObject target;
    private Vector3 distanceBetweenSourceAndTarget;

    public HealEvent(uint healAmount, GameObject source, GameObject target)
    {
        this.healAmount = healAmount;
        this.source = source;
        this.target = target;
        distanceBetweenSourceAndTarget = target.transform.position - source.transform.position;
    }
}
