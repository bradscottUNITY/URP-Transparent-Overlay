using System.Collections.Generic;
using TransparentPass;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class TransparentRenderPass : ScriptableRenderPass
{
    public static HashSet<TransparentObject> ActiveRenderers = new ();
    
    private static readonly int k_Opacity = Shader.PropertyToID("_Opacity");
    
    private TransparentPassSettings settings;

    private Material blitMaterial;

    public TransparentRenderPass(TransparentPassSettings settings, float opacity)
    {
        this.settings = settings;
        renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
        
        blitMaterial = CoreUtils.CreateEngineMaterial(Shader.Find("Blit/Blit Overlay"));
        blitMaterial.SetFloat(k_Opacity, opacity);
        blitMaterial.SetTexture("_Dest", settings.renderTexture);
    }
    
    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        //Create a new command buffer for this custom pass
        CommandBuffer cmd = CommandBufferPool.Get(name: "Transparent Overlay Pass");
        cmd.Clear();
        
        //Copy the camera color texture into a RenderTexture to overlay onto later
        cmd.Blit(renderingData.cameraData.renderer.cameraColorTarget, settings.colorTexture);
        context.ExecuteCommandBuffer(cmd);
        cmd.Clear();
        
        
        //Clear the previous frame results
        cmd.SetRenderTarget(settings.renderTexture);
        cmd.ClearRenderTarget(true, true, Color.clear);
        context.ExecuteCommandBuffer(cmd);
        cmd.Clear();
        
        //Render each requested GameObject into the transparent pass
        foreach (var transparentObject in ActiveRenderers)
        {
            //Schedule a render for each submesh
            foreach (var submesh in transparentObject.submeshes)
            {
                cmd.DrawRenderer(submesh.renderer, settings.renderMaterial, submesh.submeshIndex, settings.passIndex);
            }
            
            blitMaterial.SetTexture("_BaseTex", settings.midTexture);
            blitMaterial.SetTexture("_OverlayTex", settings.renderTexture);
            cmd.Blit(settings.renderTexture, settings.midTexture, blitMaterial);
            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
        }
        
        
        // //RenderPass(context, ref renderingData);
        // blitMaterial.SetTexture("_BaseTex", settings.colorTexture);
        // blitMaterial.SetTexture("_OverlayTex", settings.midTexture);
        // context.ExecuteCommandBuffer(cmd);
        // cmd.Clear();
        CommandBufferPool.Release(cmd);
    }
}
