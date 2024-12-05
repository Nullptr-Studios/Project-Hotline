#pragma warning disable 0618

using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace URPGlitch.Runtime.DigitalGlitch
{
    [Serializable]
    public sealed class DigitalGlitchFeature : ScriptableRendererFeature
    {
        [SerializeField] Shader shader;
        DigitalGlitchRenderPass _scriptablePass;

        public override void Create()
        {
            _scriptablePass = new DigitalGlitchRenderPass(shader);
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