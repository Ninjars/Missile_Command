using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shapes;

public class UpgradeElement : MonoBehaviour {
    public float elementSeparation = 0.5f;

    internal void display(UpgradeState upgradeState, Action upgradeAction) {
        // throw new NotImplementedException();
    }

    internal void setMeasurements(float baseElementOffset, float baseElementRadius, int index) {
        var radius = getRadius();
        transform.localPosition = Vector3.up * (baseElementOffset + (elementSeparation + 2 * radius) * index);
        var line = GetComponentInChildren<Line>();
        line.Start = new Vector3(0, -radius, 0);
        var lineLength = index == 0 ? baseElementOffset - baseElementRadius : elementSeparation;
        line.End = new Vector3(0, line.Start.y - lineLength, 0);
    }

    private float getRadius() {
        var disk = GetComponentInChildren<Disc>();
        return disk.Radius;
    }
}
