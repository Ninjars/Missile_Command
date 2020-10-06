﻿using System;
using UnityEngine;

[Serializable]
public class RangeData {
    public float from;
    public float to;

    public float evaluate(float progress) {
        return from + ((to - from) * progress);
    }
}

[Serializable]
public class RangeIntData {
    public int from;
    public int to;

    public int evaluate(float progress) {
        return Mathf.RoundToInt(from + ((to - from) * progress));
    }
}
