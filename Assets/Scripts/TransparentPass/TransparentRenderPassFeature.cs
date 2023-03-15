using System;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.Universal.Internal;
using UnityEngine.Serialization;

namespace TransparentPass
{
    [Serializable]
    public class TransparentPassSettings
    {
        public Material renderMaterial;
        public Material blitMaterial;
        public LayerMask LayerMask;
        public RenderTexture colorTexture;
        public RenderTexture renderTexture;
        public RenderQueueType renderQueueType;
        public ClearFlag clearFlag;
    }
    
    public class TransparentRenderPassFeature : ScriptableRendererFeature
    {
        
        [SerializeField]
        private Shader m_CopyDepthPS;
        [SerializeField]
        private Shader m_CopyColorPS;
        [SerializeField]
        private Shader m_BlitPS;

        [FormerlySerializedAs("settings")]
        [SerializeField]
        private TransparentPassSettings m_TransparentPassSettings;
        
        private TransparentRenderPass transparentPass;
        private CopyDepthPass copyDepthPass;
        private CopyColorPass copyColorPass;


        public override void Create()
        {
            transparentPass = new TransparentRenderPass(m_TransparentPassSettings);
            var copyDepthMaterial = CoreUtils.CreateEngineMaterial(m_CopyDepthPS);
            copyDepthPass = new CopyDepthPass(RenderPassEvent.AfterRendering, copyDepthMaterial);
            var copyColorMaterial = CoreUtils.CreateEngineMaterial(m_CopyColorPS);
            var blitMaterial = CoreUtils.CreateEngineMaterial(m_BlitPS);
            copyColorPass = new CopyColorPass(RenderPassEvent.AfterRendering, copyColorMaterial, blitMaterial);
        }


        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            transparentPass.ConfigureInput(ScriptableRenderPassInput.Color);
            transparentPass.ConfigureInput(ScriptableRenderPassInput.Depth);
           // copyDepthPass.Setup(new RenderTargetHandle(renderer.cameraDepthTarget), new RenderTargetHandle(m_TransparentPassSettings.renderTexture.colorBuffer));
           // copyColorPass.Setup(renderer.cameraColorTarget, new RenderTargetHandle(m_TransparentPassSettings.renderTexture.colorBuffer), Downsampling.None);
            //renderer.EnqueuePass(copyColorPass);
            //renderer.EnqueuePass(copyDepthPass);
            renderer.EnqueuePass(transparentPass);
        }
    }
}
