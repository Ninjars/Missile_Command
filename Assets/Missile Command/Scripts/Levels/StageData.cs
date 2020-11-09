using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "Missile Command/Level Data")]
public class StageData : ScriptableObject {
    public string title;
    public int levels = 10;
    public RangeData levelDuration;
    public List<WeaponData> weaponData;
}
