using static UnityEngine.Rendering.Universal.ControllableGlitchUrp;

namespace UnityEngine.Rendering.Universal {
    internal class ControllableGlitchUrpPass : ScriptableRenderPass {
        public Material material;
        private readonly ControllableGlitchSettings settings;
        private RenderTargetIdentifier source;
        private RenderTargetIdentifier tempCopy = new RenderTargetIdentifier(SHADER_TEMP);

        private readonly string tag;

        static readonly int SHADER_AMOUNT = Shader.PropertyToID("_Amount");
        static readonly int SHADER_FRAME = Shader.PropertyToID("_Frame");
        static readonly int SHADER_PIXEL = Shader.PropertyToID("_Pixel");
        static readonly int SHADER_TEMP = Shader.PropertyToID("_TempCopy");
        static readonly string FRAME = "FRAME";
        static readonly string PIXEL = "PIXEL";

        private Vector2 rand1 = new Vector2(5.0f, 1.0f);
        private Vector2 rand2 = new Vector2(31.0f, 1.0f);
        private Vector2 randMul2 = new Vector2(127.1f, 311.7f);
        private float chromaticGlitch, frameGlitch, pixelGlitch;
        private float x, y;

        public ControllableGlitchUrpPass(RenderPassEvent renderPassEvent, Material material, ControllableGlitchSettings settings, string tag) {
            this.renderPassEvent = renderPassEvent;
            this.tag = tag;
            this.material = material;
            this.settings = settings;
        }

        public void Setup(RenderTargetIdentifier source) {
            this.source = source;
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData) {
            var opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            opaqueDesc.depthBufferBits = 0;

            CommandBuffer cmd = CommandBufferPool.Get(tag);
            cmd.GetTemporaryRT(SHADER_TEMP, opaqueDesc, FilterMode.Bilinear);
            cmd.CopyTexture(source, tempCopy);

            chromaticGlitch = settings.ChromaticGlitch;
            if (chromaticGlitch != 0) {
                material.SetFloat(SHADER_AMOUNT, Mathf.Lerp(0, 0.35f, Mathf.Pow(chromaticGlitch, 3f)));
            } else {
                material.SetFloat(SHADER_AMOUNT, 0f);
            }

            frameGlitch = settings.FrameGlitch;
            if (frameGlitch != 0) {
                material.SetFloat(SHADER_FRAME, Mathf.Lerp(0, 0.05f, Mathf.Pow(frameGlitch, 3f)));
                material.EnableKeyword(FRAME);
            } else {
                material.DisableKeyword(FRAME);
            }

            pixelGlitch = settings.PixelGlitch;
            if (pixelGlitch != 0) {
                x = Mathf.Sin(Vector2.Dot(rand1 * Mathf.Floor(Time.realtimeSinceStartup * 12.0f), randMul2)) * 43758.5453123f;
                y = Mathf.Sin(Vector2.Dot(rand2 * Mathf.Floor(Time.realtimeSinceStartup * 12.0f), randMul2)) * 43758.5453123f;

                material.SetVector(SHADER_PIXEL, new Vector2((x - Mathf.Floor(x)) * pixelGlitch * 0.1f, (y - Mathf.Floor(y)) * pixelGlitch * 0.1f));
                material.EnableKeyword(PIXEL);
            } else {
                material.DisableKeyword(PIXEL);
            }

            cmd.Blit(tempCopy, source, material);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public override void FrameCleanup(CommandBuffer cmd) {
            cmd.ReleaseTemporaryRT(SHADER_TEMP);
        }
    }
}