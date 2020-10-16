using System;
using UnityEngine;

[Serializable]
public class TrailSettings {
    public LinearTrail prefab;
    public Color color;
    public float fadeDuration = 2f;
    public float zPos = 6;
}
