using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 网格处理类库
/// </summary>
public static class MeshUtils {

    public static Mesh CopyMesh(this Mesh mesh) {
        Mesh changeMesh = new Mesh();
        changeMesh.vertices = mesh.vertices;// mesh.vertices;
        changeMesh.triangles = mesh.triangles;
        changeMesh.tangents = mesh.tangents;
        changeMesh.normals = mesh.normals;
        changeMesh.uv = mesh.uv;
        changeMesh.uv2 = mesh.uv2;
        changeMesh.uv3 = mesh.uv3;
        changeMesh.uv4 = mesh.uv4;
        changeMesh.bindposes = mesh.bindposes;
        changeMesh.boneWeights = mesh.boneWeights;
        changeMesh.bounds = mesh.bounds;
        changeMesh.colors = mesh.colors;
        changeMesh.colors32 = mesh.colors32;
        changeMesh.hideFlags = mesh.hideFlags;
        changeMesh.indexFormat = mesh.indexFormat;
        changeMesh.name = mesh.name + "_copy";
        return changeMesh;
    }
   
}
