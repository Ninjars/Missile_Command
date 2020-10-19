using System.Collections.Generic;
using Shapes;
using UnityEngine;

public class BackdropGenerator : MonoBehaviour {
    public int backdropZ;
    public float yOffset;

    [Tooltip("Number of backdrop layers")]
    public RangeIntData layers;
    [Tooltip("Max height of layers, from closest to furthest")]
    public RangeData heights;
    [Tooltip("Density of points making up a backdrop layer")]
    public RangeData pointsFrequency;

    private Colors colors { get { return Colors.Instance; }}
    private GameObject container;

    public void generateBackground(WorldCoords worldCoords) {
        if (container != null) {
            GameObject.Destroy(container);
        }
        container = new GameObject("background container");
        container.transform.position = Vector3.forward * backdropZ;

        int numberOfLayers = layers.evaluate(Random.value);
        float pointFrequency = pointsFrequency.evaluate(Random.value);
        for (int i = 0; i < numberOfLayers; i++) {
            float layerFactor = i / (float) numberOfLayers;
            Polygon layer = createLayer(
                worldCoords, 
                Mathf.RoundToInt(pointFrequency * i), 
                colors.backgroundColor.evaluate(layerFactor), 
                heights.evaluate(layerFactor), 
                i);
            layer.transform.parent = container.transform;
            layer.transform.position = container.transform.position + (Vector3.forward * i);
        }
    }

    private Polygon createLayer(WorldCoords worldCoords, int pointCount, Color color, float maxHeight, int layer) {
        Polygon polygon = new GameObject($"bg {layer}").AddComponent<Polygon>();
        polygon.FillType = FillType.LinearGradient;

        float originY = worldCoords.groundY + yOffset;
        List<Vector2> points = new List<Vector2>();
        points.Add(new Vector2(worldCoords.worldRight, originY));
        points.Add(new Vector2(worldCoords.worldLeft, originY));

        float minY = maxHeight * 0.1f;
        PerlinProvider perlin = new PerlinProvider(maxHeight - minY, Random.value * 100, 0.8f, 0.2f, 2f);

        points.Add(new Vector2(worldCoords.worldLeft, originY + minY + perlin.get(worldCoords.worldLeft)));

        float averagePointGap = (worldCoords.worldRight - worldCoords.worldLeft) / (float) (pointCount);
        for (int i = 1; i < pointCount - 1; i++) {
            var x = worldCoords.worldLeft + (i * averagePointGap);
            points.Add(new Vector2(x, originY + minY + perlin.get(x)));
        }

        points.Add(new Vector2(worldCoords.worldRight, originY + minY + perlin.get(worldCoords.worldRight)));

        polygon.points = points;
        polygon.FillColorStart = color;
        polygon.FillColorEnd = getMistedColor(color);
        polygon.FillLinearStart = new Vector2(0, originY + maxHeight * 0.75f);
        polygon.FillLinearEnd = new Vector2(0, originY);
        polygon.meshOutOfDate = true;

        return polygon;
    }

    private Color getMistedColor(Color color) {
        float h, s, v;
        Color.RGBToHSV(color, out h, out s, out v);
        h *= 0.98f;
        s *= 0.8f;
        return Color.HSVToRGB(h, s, v);
    }
}
