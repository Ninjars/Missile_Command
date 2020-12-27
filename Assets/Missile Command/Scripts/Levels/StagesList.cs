using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StagesList", menuName = "Missile Command/Stages List")]
public class StagesList : ScriptableObject {
    public string title;
    public List<StageData> stages;
}
