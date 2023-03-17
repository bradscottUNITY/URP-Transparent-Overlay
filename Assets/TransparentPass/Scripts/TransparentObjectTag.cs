using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class TransparentObjectTag : MonoBehaviour
{
    private TransparentObject request;

    public bool useVisibility;

    private void OnEnable()
    {
        request = new TransparentObject(gameObject);
        Add();
    }

    private void OnDisable()
    {
        Remove();
    }

    private void OnBecameVisible()
    {
        if(!useVisibility) 
            return;
        Add();
    }

    private void OnBecameInvisible()
    {
        if (!useVisibility)
            return;
        Remove();
    }

    private void Add()
    {
        TransparentRenderPass.ActiveRenderers.Add(request);
    }

    private void Remove()
    {
        if (TransparentRenderPass.ActiveRenderers.Contains(request))
        {
            TransparentRenderPass.ActiveRenderers.Remove(request);
        }
    }
}

[Serializable]
public class TransparentObject
{
    public GameObject gameObject { get; private set; }
    
    public List<DrawRendererCommand> submeshes { get; private set; }
    
    public TransparentObject(GameObject gameObject)
    {
        this.gameObject = gameObject;
        submeshes = new List<DrawRendererCommand>();
        
        foreach (var meshRenderer in gameObject.GetComponentsInChildren<MeshRenderer>())
        {
            int materialCount = meshRenderer.sharedMaterials.Length;
            for (int i = 0; i < materialCount; i++)
            {
                DrawRendererCommand command = new DrawRendererCommand()
                {
                    renderer = meshRenderer,
                    material = meshRenderer.sharedMaterials[i],
                    submeshIndex = i,
                    shaderPass = 0
                };

                submeshes.Add(command);
            }
        }
    }
}

[Serializable]
public class DrawRendererCommand
{
    public Renderer renderer;
    public Material material;
    public int submeshIndex;
    public int shaderPass;
}
