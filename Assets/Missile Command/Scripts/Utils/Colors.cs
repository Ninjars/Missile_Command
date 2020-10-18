using UnityEngine;

public class Colors : MonoBehaviour {
    public ColorPalette colorPalette;

    private static Colors _instance;

    public static Colors Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
    }

    public Color cityColor { get { return colorPalette.city; }}
    public Color batteryColor { get { return colorPalette.battery; }}
    public Color buildingExplodeColor { get { return colorPalette.buildingExplode; }}
    public Color deadBuildingColor { get { return colorPalette.deadBuilding; }}
    public Color targetMarkerColor { get { return colorPalette.targetMarker; }}
    public Color missileColor { get { return colorPalette.missile; }}
    public Color missileTrailColor { get { return colorPalette.missileTrail; }}
    public Color missileExplodeColor { get { return colorPalette.missileExplode; }}
    public Color attackColor { get { return colorPalette.attack; }}
    public Color attackTrailColor { get { return colorPalette.attackTrail; }}
    public Color attackExplodeColor { get { return colorPalette.attackExplode; }}
    public RangeColorData skyColor { get { return colorPalette.sky; }}
    public RangeColorData backgroundColor { get { return colorPalette.background; }}
}
