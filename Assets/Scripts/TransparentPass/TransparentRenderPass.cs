using System.Collections;
using System.Collections.Generic;
using TransparentPass;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class TransparentRenderPass : ScriptableRenderPass
{
    private TransparentPassSettings settings;
   // private RenderTexture renderTexure;

    private FilteringSettings m_FilteringSettings;
    private List<ShaderTagId> m_ShaderTagIdList = new List<ShaderTagId>();
    private RenderStateBlock m_RenderStateBlock;

    
    private RenderTargetIdentifier source { get; set; }
    private RenderTargetIdentifier destination { get; set; }


    public TransparentRenderPass(TransparentPassSettings settings)
    {
        this.settings = settings;
        renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;

        RenderQueueRange renderQueueRange = RenderQueueRange.opaque;
        m_FilteringSettings = new FilteringSettings(renderQueueRange, settings.LayerMask);
        
        m_ShaderTagIdList.Add(new ShaderTagId("SRPDefaultUnlit"));
        m_ShaderTagIdList.Add(new ShaderTagId("UniversalForward"));
        m_ShaderTagIdList.Add(new ShaderTagId("UniversalForwardOnly"));
        m_RenderStateBlock = new RenderStateBlock(RenderStateMask.Nothing);

        //this.renderTexure = settings.renderTexture;
    }
    
    public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
    {
        base.OnCameraSetup(cmd, ref renderingData);
        
        //Clear the previous results in our target texture
        ConfigureTarget(settings.renderTexture);
        ConfigureClear(settings.clearFlag, Color.clear);
    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        RenderVehiclesPass(context, ref renderingData);
        CopyDepthPass(context, ref renderingData);
        BlitPass(context, ref renderingData);
    }

    private void CopyDepthPass(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        CommandBuffer cmd = CommandBufferPool.Get(name: "Copy Color Pass");
        cmd.Clear();
        cmd.Blit(renderingData.cameraData.renderer.cameraColorTarget, settings.colorTexture);
        context.ExecuteCommandBuffer(cmd);
        cmd.Clear();
        CommandBufferPool.Release(cmd);
    }

    private void RenderVehiclesPass(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        CommandBuffer cmd = CommandBufferPool.Get(name: "Vehicle Prepass");

        SortingCriteria sortingCriteria = renderingData.cameraData.defaultOpaqueSortFlags;
        
        DrawingSettings drawingSettings = CreateDrawingSettings(m_ShaderTagIdList, ref renderingData, sortingCriteria);
        drawingSettings.overrideMaterial = settings.renderMaterial;
        
        //cmd.SetRenderTarget(settings.renderTexture);
        context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref m_FilteringSettings, ref m_RenderStateBlock);
        
        context.ExecuteCommandBuffer(cmd);
        cmd.Clear();
        CommandBufferPool.Release(cmd);
    }

    private void BlitPass(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        CommandBuffer cmd = CommandBufferPool.Get(name: "TransparentPass");
        // RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
        // opaqueDesc.depthBufferBits = 0;

        var renderer = renderingData.cameraData.renderer;

        source = new RenderTargetIdentifier(settings.renderTexture);
        destination = renderer.cameraColorTarget;
        
        Blit(cmd, source, destination, settings.blitMaterial);
        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
    }
}
