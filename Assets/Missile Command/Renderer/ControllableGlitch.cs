using UnityEngine;

public class ControllableGlitch : MonoBehaviour {
    public Material material;
    [Range(0, 1)]
    public float chromaticGlitch;
    [Range(0, 1)]
    public float frameGlitch;
    [Range(0, 2)]
    public float pixelGlitch;
    public bool FastMode;

    private Vector2 rand1 = new Vector2(5.0f, 1.0f);
    private Vector2 rand2 = new Vector2(31.0f, 1.0f);
    private Vector2 randMul2 = new Vector2(127.1f, 311.7f);
    static readonly int amountString = Shader.PropertyToID("_Amount");
    static readonly int frameString = Shader.PropertyToID("_Frame");
    static readonly int pixelString = Shader.PropertyToID("_Pixel");
    static readonly string frameKeyword = "FRAME";
    static readonly string pixelKeyword = "PIXEL";
    private float x, y;

    private void OnRenderImage(RenderTexture source, RenderTexture destination) {
        if (chromaticGlitch != 0) {
            material.SetFloat(amountString, Mathf.Lerp(0, 0.35f, Mathf.Pow(chromaticGlitch, 3f)));
        } else
            material.SetFloat(amountString, 0f);

        if (frameGlitch != 0) {
            material.SetFloat(frameString,  Mathf.Lerp(0, 0.05f, Mathf.Pow(frameGlitch, 3f)));
            material.EnableKeyword(frameKeyword);
        } else
            material.DisableKeyword(frameKeyword);

        if (pixelGlitch != 0) {
            x = Mathf.Sin(Vector2.Dot(rand1 * Mathf.Floor(Time.realtimeSinceStartup * 12.0f), randMul2)) * 43758.5453123f;
            y = Mathf.Sin(Vector2.Dot(rand2 * Mathf.Floor(Time.realtimeSinceStartup * 12.0f), randMul2)) * 43758.5453123f;

            material.SetVector(pixelString, new Vector2((x - Mathf.Floor(x)) * pixelGlitch * 0.1f, (y - Mathf.Floor(y)) * pixelGlitch * 0.1f));
            material.EnableKeyword(pixelKeyword);
        } else {
            material.DisableKeyword(pixelKeyword);
        }

        Graphics.Blit(source, destination, material);
    }
}
