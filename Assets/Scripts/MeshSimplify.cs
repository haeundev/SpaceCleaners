using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

public class MeshSimplify : MonoBehaviour
{
    public float quality = 0.5f;
    
    [Button("Simplify Mesh")]
    public void Simplify()
    {
        foreach (var mf in GetComponentsInChildren<MeshFilter>())
        {
            var originalMesh = mf.sharedMesh;
            var meshSimplifier = new UnityMeshSimplifier.MeshSimplifier();
            meshSimplifier.Initialize(originalMesh);
            meshSimplifier.SimplifyMesh(quality);
            var destMesh = meshSimplifier.ToMesh();
            mf.sharedMesh = destMesh;
        }
    }
}