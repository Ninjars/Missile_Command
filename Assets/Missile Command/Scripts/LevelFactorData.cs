using UnityEngine;

[CreateAssetMenu(fileName = "LevelFactorData", menuName = "Missile Command/Level Factor Data")]
public class LevelFactorData : ScriptableObject {
    public int levelForMin;
    public int levelForMax;
    public float minValue;
    public float maxValue;

    public float evaluate(int level) {
        var factor = Mathf.Clamp01((levelForMax - level) / (float) (levelForMax - levelForMin));
        return minValue + ((maxValue - minValue) * factor);
    }
}
