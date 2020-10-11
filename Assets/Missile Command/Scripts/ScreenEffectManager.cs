using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ScreenEffectManager : MonoBehaviour {
    
    public ControllableGlitchUrp glitchEffectController;

    public float distortion;
    public float distortionMaxEffect = 5f;
    public float distortionDecayRate = 2f;
    public float glitch;
    public float glitchMaxEffect = 1;
    public float glitchDecayRate = 0.1f;

    private void Update() {
        if (glitch > 0) {
            glitch = Mathf.Max(0, glitch - glitchDecayRate * Time.deltaTime);
        }
        if (distortion > 0) {
            distortion = Mathf.Max(0, distortion - distortionDecayRate * Time.deltaTime);
        }
        glitchEffectController.settings.ChromaticGlitch = distortion / distortionMaxEffect;
        glitchEffectController.settings.FrameGlitch = distortion / distortionMaxEffect;
        glitchEffectController.settings.PixelGlitch = glitch / glitchMaxEffect;
    }

    public void onNukeExplosion(float yPosition) {
        distortion += 1 - Mathf.Clamp01(yPosition / 1000f);
    }

    public void onCityNukeHit(float impactStrength) {
        distortion = Mathf.Clamp(distortion + impactStrength, 0, distortionMaxEffect);
    }

    public void onBatteryNukeHit() {
        glitch = Mathf.Clamp(glitch + 1, 0, glitchMaxEffect);
    }
}
