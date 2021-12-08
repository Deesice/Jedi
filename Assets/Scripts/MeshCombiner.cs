using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshCombiner : MonoBehaviour
{
    public Mesh Combine(Material finalMaterial)
    {
        var meshFilters = GetComponentsInChildren<MeshFilter>();
        var meshRenderers = GetComponentsInChildren<MeshRenderer>();
        var combine = new CombineInstance[meshFilters.Length];

        int i = 0;
        RandomColor randomColor;
        Color[] colors;
        Color color;
        while (i < meshFilters.Length)
        {
            combine[i].mesh = meshFilters[i].mesh;
            combine[i].transform = transform.worldToLocalMatrix * meshFilters[i].transform.localToWorldMatrix;

            if (randomColor = meshFilters[i].GetComponent<RandomColor>())
                color = randomColor.GetRandomColor();
            else
                color = meshRenderers[i].sharedMaterial.color;

            colors = new Color[combine[i].mesh.vertexCount];
            for (int j = 0; j < colors.Length; j++)
                colors[j] = color;

            combine[i].mesh.colors = colors;
            i++;
        }

        var newMesh = new Mesh();
        //newMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        newMesh.CombineMeshes(combine);

        i = 0;
        while (i < meshFilters.Length)
        {
            DestroyImmediate(meshFilters[i]);
            DestroyImmediate(meshRenderers[i]);
            i++;
        }
        gameObject.AddComponent<MeshFilter>().mesh = newMesh;
        gameObject.AddComponent<MeshRenderer>().sharedMaterial = finalMaterial;
        return newMesh;
    }
}
