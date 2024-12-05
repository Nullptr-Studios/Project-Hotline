#pragma warning disable 0618

using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace URPGlitch.Runtime.AnalogGlitch
{
    public sealed class AnalogGlitchFeature : ScriptableRendererFeature
    {
        [SerializeField] Shader shader;
        AnalogGlitchRenderPass _scriptablePass;

        public override void Create()
        {
            _scriptablePass = new AnalogGlitchRenderPass(shader);
        }
        
        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            renderer.EnqueuePass(_scriptablePass);
        }

        protected override void Dispose(bool disposing)
        {
            _scriptablePass.Dispose();
        }
    }
}