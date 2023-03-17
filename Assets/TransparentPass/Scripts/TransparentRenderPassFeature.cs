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
        public int passIndex = -1;

        //public LayerMask LayerMask;
        public RenderTexture colorTexture;
        public RenderTexture renderTexture;
        public RenderTexture midTexture;

        //public RenderQueueType renderQueueType;
        //public ClearFlag clearFlag;
    }

    public class TransparentRenderPassFeature : ScriptableRendererFeature
    {
        [Range(0f, 1f)]
        public float opacity = 0.65f;
        [FormerlySerializedAs("settings")]
        [SerializeField]
        private TransparentPassSettings m_TransparentPassSettings;

        private TransparentRenderPass transparentPass;

        public override void Create()
        {
            transparentPass = new TransparentRenderPass(m_TransparentPassSettings, opacity);
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            transparentPass.ConfigureInput(ScriptableRenderPassInput.Color);
            transparentPass.ConfigureInput(ScriptableRenderPassInput.Depth);
            renderer.EnqueuePass(transparentPass);
        }
    }
}
