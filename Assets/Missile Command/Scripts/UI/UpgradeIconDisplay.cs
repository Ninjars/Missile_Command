using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shapes;

public class UpgradeIconDisplay : MonoBehaviour {

    public List<ShapeRenderer> primaryShapes;
    public List<ShapeRenderer> secondaryShapes;
    private Colors colors { get { return Colors.Instance; } }

    private void OnEnable() {
        foreach (var shape in primaryShapes) {
            shape.Color = colors.upgradeUiNormalColor;
        }
        foreach (var shape in secondaryShapes) {
            shape.Color = colors.upgradeUiSecondaryColor;
        }
    }
}
