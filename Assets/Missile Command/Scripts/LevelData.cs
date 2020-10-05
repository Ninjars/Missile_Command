using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "Missile Command/Level Data")]
public class LevelData : ScriptableObject {
    public string message = "";
    public List<WeaponData> attacks;
}
