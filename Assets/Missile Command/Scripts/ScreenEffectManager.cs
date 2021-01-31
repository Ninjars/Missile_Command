using UnityEngine;

public class ScreenEffectManager : MonoBehaviour {
    
    public ControllableGlitch glitchEffectController;

    public float distortion;
    public float distortionMaxEffect = 5f;
    public float distortionDecayRate = 2f;
    public float glitch;
    public float glitchMaxEffect = 1;
    public float glitchDecayRate = 0.2f;

    private void Update() {
        if (glitch > 0) {
            glitch = Mathf.Max(0, glitch - glitchDecayRate * Time.deltaTime);
        }
        if (distortion > 0) {
            distortion = Mathf.Max(0, distortion - distortionDecayRate * Time.deltaTime);
        }
        glitchEffectController.chromaticGlitch = distortion / distortionMaxEffect;
        glitchEffectController.frameGlitch = distortion / distortionMaxEffect;
        glitchEffectController.pixelGlitch = glitch / glitchMaxEffect;
    }

    private void increaseGlitch(float value, float max) {
        glitch = Mathf.Clamp(glitch + value, 0, Mathf.Min(max, glitchMaxEffect));
    }

    private void increaseDistort(float value, float max) {
        distortion = Mathf.Clamp(distortion + value, 0, Mathf.Min(max, distortionMaxEffect));
    }

    public void onEMP(float scale, float yPosition) {
        var factor = (1 - Mathf.Clamp01(yPosition / 10f)) * scale;
        increaseDistort(factor, distortionMaxEffect * 0.5f);
        increaseGlitch(factor * 0.2f, distortionMaxEffect * 0.6f);
    }

    public void onCityNukeHit(float impactStrength) {
        increaseDistort(impactStrength, distortionMaxEffect);
        increaseGlitch(impactStrength * 0.4f, distortionMaxEffect * 0.6f);
    }

    public void onBatteryDestroyed() {
        increaseGlitch(1, glitchMaxEffect);
    }
}
