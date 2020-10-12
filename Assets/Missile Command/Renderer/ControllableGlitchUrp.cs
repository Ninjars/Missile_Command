namespace UnityEngine.Rendering.Universal
{
    public class ControllableGlitchUrp : ScriptableRendererFeature
    {
        [System.Serializable]
        public class ControllableGlitchSettings
        {
            public RenderPassEvent Event = RenderPassEvent.AfterRenderingTransparents;

            public Material blitMaterial = null;

            [Range(0, 1)]
            public float ChromaticGlitch;

            [Range(0, 1)]
            public float FrameGlitch;

            [Range(0, 1)]
            public float PixelGlitch;
        }

        public ControllableGlitchSettings settings = new ControllableGlitchSettings();

        ControllableGlitchUrpPass mobilePostProcessLwrpPass;

        public override void Create()
        {
            mobilePostProcessLwrpPass = new ControllableGlitchUrpPass(settings.Event, settings.blitMaterial, settings, this.name);
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            mobilePostProcessLwrpPass.Setup(renderer.cameraColorTarget);
            renderer.EnqueuePass(mobilePostProcessLwrpPass);
        }
    }
}
