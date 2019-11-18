using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
public class ObjExporter {
    //
    // Static Methods
    //
    public static void MeshToFile(MeshFilter mf, string filename, float scale) {
        using (StreamWriter streamWriter = new StreamWriter(filename)) {
            streamWriter.Write(ObjExporter.MeshToString(mf, scale));
        }
    }

    public static void MeshToFile(SkinnedMeshRenderer sm, string filename, float scale) {
        using (StreamWriter streamWriter = new StreamWriter(filename)) {
            streamWriter.Write(ObjExporter.MeshToString(sm, scale));
        }
    }

    public static void MeshToFile(Mesh mesh, string filename, float scale) {
        using (StreamWriter streamWriter = new StreamWriter(filename)) {
            string meshstring = "";
            meshstring += "mtllib design.mtl\n";
            meshstring += "g "+ filename.Substring(filename.LastIndexOf('/')+1)+ "\n";

            meshstring += MeshToString_vertivies(mesh, scale);
            meshstring += MeshToString_vns(mesh);
            meshstring += MeshToString_uvs(mesh,1f,Vector2.zero,Vector2.one);
            meshstring += MeshToString_faces(mesh);

            streamWriter.Write(meshstring, scale);
        }
    }

    public static void MeshToFile(Vector3[]verticies,int[]triangles, string filename, float scale) {

        StringBuilder stringBuilder = new StringBuilder();

        stringBuilder.Append("mtllib design.mtl\n");
        stringBuilder.Append("g " + filename.Substring(filename.LastIndexOf('/') + 1) + "\n");

        for (int i = 0; i < verticies.Length; i++) {
            Vector3 vector = verticies[i];
            stringBuilder.Append(string.Format("v {0} {1} {2}\n", vector.x * scale, vector.y * scale, vector.z * scale));//TODO
        }
        stringBuilder.Append("\n");

        for (int triIndex = 0; triIndex < triangles.Length; triIndex += 3) {
            stringBuilder.Append(string.Format("f {0} {1} {2}\n", triangles[triIndex] + 1, triangles[triIndex + 1] + 1, triangles[triIndex + 2] + 1));
        }

        using (StreamWriter streamWriter = new StreamWriter(filename)) {
            streamWriter.Write(stringBuilder.ToString());
        }
    }

    /// <summary>
    /// 写顶点
    /// </summary>
    /// <param name="mesh"></param>
    /// <param name="scale"></param>
    /// <returns></returns>
    private static string MeshToString_vertivies(Mesh mesh, float scale) {

        StringBuilder stringBuilder = new StringBuilder();

        Vector3[] vertices = mesh.vertices;
        for (int i = 0; i < vertices.Length; i++) {
            Vector3 vector = vertices[i];
            stringBuilder.Append(string.Format("v {0} {1} {2}\n", vector.x * scale, vector.y * scale, vector.z * scale));//TODO
        }
        stringBuilder.Append("\n");
        return stringBuilder.ToString();
    }

    /// <summary>
    /// 写切线
    /// </summary>
    /// <param name="mesh"></param>
    /// <param name="scale"></param>
    /// <param name="textureOffset"></param>
    /// <param name="textureScale"></param>
    /// <returns></returns>
    private static string MeshToString_uvs(Mesh mesh, float scale, Vector2 textureOffset, Vector2 textureScale) {
        
        StringBuilder stringBuilder = new StringBuilder();
        
        Dictionary<int, int> dictionary = new Dictionary<int, int>();
        if (mesh.subMeshCount > 1) {
            int[] triangles = mesh.GetTriangles(1);
            for (int j = 0; j < triangles.Length; j += 3) {
                if (!dictionary.ContainsKey(triangles[j])) {
                    dictionary.Add(triangles[j], 1);
                }
                if (!dictionary.ContainsKey(triangles[j + 1])) {
                    dictionary.Add(triangles[j + 1], 1);
                }
                if (!dictionary.ContainsKey(triangles[j + 2])) {
                    dictionary.Add(triangles[j + 2], 1);
                }
            }
        }
        for (int num = 0; num != mesh.uv.Length; num++) {
            Vector2 vector2 = Vector2.Scale(mesh.uv[num], textureScale) + textureOffset;
            if (dictionary.ContainsKey(num)) {
                stringBuilder.Append(string.Format("vt {0} {1}\n", mesh.uv[num].x, mesh.uv[num].y));
            } else {
                stringBuilder.Append(string.Format("vt {0} {1}\n", vector2.x, vector2.y));
            }
        }
        
        return stringBuilder.ToString();
    }
    /// <summary>
    /// 写法线
    /// </summary>
    /// <param name="mesh"></param>
    /// <returns></returns>
    private static string MeshToString_vns(Mesh mesh) {

        StringBuilder stringBuilder = new StringBuilder();

        for (int num = 0; num != mesh.normals.Length; num++) {
            Vector3 normal = mesh.normals[num];
            stringBuilder.Append(string.Format("vn {0} {1} {2}\n", normal.x, normal.y, normal.z));
        }

        return stringBuilder.ToString();
    }

    /// <summary>
    /// 写面片
    /// </summary>
    /// <param name="mesh"></param>
    /// <returns></returns>
    private static string MeshToString_faces(Mesh mesh) {

        StringBuilder stringBuilder = new StringBuilder();

        for (int k = 0; k < mesh.subMeshCount; k++) {
            stringBuilder.Append("\n");
            if (k == 0) {
                stringBuilder.Append("usemtl ").Append("Material_design").Append("\n");
            }
            if (k == 1) {
                stringBuilder.Append("usemtl ").Append("Material_logo").Append("\n");
            }
            int[] triangles2 = mesh.GetTriangles(k);
            for (int l = 0; l < triangles2.Length; l += 3) {
                stringBuilder.Append(string.Format("f {0}/{0}/{0} {1}/{1}/{1} {2}/{2}/{2}\n", triangles2[l] + 1, triangles2[l + 1] + 1, triangles2[l + 2] + 1));
            }
        }

        return stringBuilder.ToString();
    }

    private static string MeshToString(MeshFilter mf, float scale) {

        Vector2 textureOffset = mf.GetComponent<Renderer>().material.GetTextureOffset("_MainTex");
        Vector2 textureScale = mf.GetComponent<Renderer>().material.GetTextureScale("_MainTex");

        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append("mtllib design.mtl").Append("\n");
        stringBuilder.Append("g ").Append(mf.name).Append("\n");

        string meshstring = stringBuilder.ToString();
        meshstring += MeshToString_vertivies(mf.mesh, scale);
        meshstring += MeshToString_uvs(mf.mesh,scale, textureOffset, textureScale);
        meshstring += MeshToString_vns(mf.mesh);
        meshstring += MeshToString_faces(mf.mesh);

        return meshstring;
    }

    private static string MeshToString(SkinnedMeshRenderer smr, float scale) {

        Vector2 textureOffset = smr.material.GetTextureOffset("_MainTex");
        Vector2 textureScale = smr.material.GetTextureScale("_MainTex");

        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append("mtllib design.mtl").Append("\n");
        stringBuilder.Append("g ").Append(smr.gameObject.name).Append("\n");

        string meshstring = stringBuilder.ToString();
        meshstring += MeshToString_vertivies(smr.sharedMesh, scale);
        meshstring += MeshToString_uvs(smr.sharedMesh, scale, textureOffset, textureScale);
        meshstring += MeshToString_faces(smr.sharedMesh);

        return meshstring;
    }

}
