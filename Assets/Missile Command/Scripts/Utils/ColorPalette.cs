using UnityEngine;

[CreateAssetMenu(fileName = "ColorPalette", menuName = "Missile Command/Color Palette")]
public class ColorPalette : ScriptableObject {
    public Color city;
    public Color battery;
    public Color buildingExplode;
    public Color deadBuilding;
    public Color targetMarker;
    public Color missile;
    public Color missileTrail;
    public Color missileExplode;
    public Color attack;
    public Color attackTrail;
    public Color attackExplode;
    public RangeColorData sky;
    public RangeColorData background;
}
