using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AssignMaterial : MonoBehaviour
{
    public Material material;

    private void OnValidate()
    {
        if (material)
        {
            var renderers = GetComponentsInChildren<MeshRenderer>();
            foreach (var meshRenderer in renderers)
            {
                Material[] matList = Enumerable.Repeat(material, meshRenderer.sharedMaterials.Length).ToArray();
                meshRenderer.sharedMaterials = matList;
            }
        }
    }
}
