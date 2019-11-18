using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// author:MemoryC
/// date:2018.09.16
/// description:ObjReader by MemoryC
/// support:load .obj file from external storage and convert it to mesh file with uvCoord and vertice normals
/// </summary>
public class ObjReader{
    /// <summary>
    /// 通过文件路径读取obj文件
    /// </summary>
    /// <param name="filename">最好用绝对路径</param>
    /// <param name="scale">比例，默认1</param>
    /// <a href="http://www.manew.com/forum-47-453-1.html"></a>
    /// <returns></returns>
    public static Mesh ReadMeshByObj(string filename,float scale=1f) {

        Mesh mesh = new Mesh();
        mesh.name = filename.Substring((int)Mathf.Max(filename.LastIndexOf("/"), filename.LastIndexOf("\\"))+1);

        List<Vector3> verticies = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<Vector3> normals = new List<Vector3>();
        List<int> triangles = new List<int>();
        Vector2[] uUV=null;
        Vector3[] uNorm = null;

        System.IO.StreamReader sr = new System.IO.StreamReader(filename);
        string line;
        int lineNum = 0;

        float left=1f, right=0f, up=0f, down=1f;
        while ((line = sr.ReadLine()) != null) {
            lineNum++;
            if (line.StartsWith("v ")) {//顶点
                line = line.Substring(2).Trim();
                string[] pos = line.Split(' ');
                if (pos.Length == 3) {
                    verticies.Add(new Vector3(float.Parse(pos[0]) / scale, float.Parse(pos[1]) / scale, float.Parse(pos[2]) / scale));
                } else {
                    Debug.Log("第" + lineNum + "行数据故障：" + line);
                }
            } else if (line.StartsWith("vt ")) {//纹理坐标
                line = line.Substring(3).Trim();
                string[] pos = line.Split(' '); 
                if (pos.Length == 2) {
                    uvs.Add(new Vector2(float.Parse(pos[0]), float.Parse(pos[1])));
                    
                } else {
                    Debug.Log("第" + lineNum + "行数据故障：" + line);
                }
            } else if(line .StartsWith("vn ")) {
                line = line.Substring(3).Trim();
                string[] pos = line.Split(' ');
                if (pos.Length == 3) {
                    normals.Add(new Vector3(float.Parse(pos[0]), float.Parse(pos[1]), float.Parse(pos[2])));
                } else {
                    Debug.Log("第" + lineNum + "行数据故障：" + line);
                }
            } else if (line.StartsWith("f ")){
                line = line.Substring(2).Trim();
                string[] pos = line.Split(' ');
                if (uUV == null) {
                    uUV = new Vector2[verticies.Count];
                }
                if (uNorm == null) {
                    uNorm = new Vector3[verticies.Count];
                }
                if (pos.Length == 3|| pos.Length == 4) {
                    if (pos[0].Contains("/")) {
                        //预处理
                        for (int i = 0; i < pos.Length; i++) {
                            pos[i].Replace("//", "/-/");
                            if (pos[i].EndsWith("/")) {
                                pos[i] = pos[i] + "-";
                            }
                        }
                        int[] vIndexes = new int[pos.Length];
                        
                        //解析
                        for (int i = 0; i < pos.Length; i++) {
                            string[] v_vt_vn = pos[i].Split('/');
                            int vIndex = int.Parse(v_vt_vn[0]);
                            if (!v_vt_vn[1].Equals("-")) {
                                int vtIndex = int.Parse(v_vt_vn[1]);
                                if(uUV[vIndex - 1].magnitude==0) {
                                    uUV[vIndex - 1] = uvs[vtIndex - 1];
                                }
                                
                            }
                            if (!v_vt_vn[2].Equals("-")) {
                                int vnIndex = int.Parse(v_vt_vn[2]);
                                if ( uNorm[vIndex - 1].magnitude == 0) {
                                    uNorm[vIndex - 1] = normals[vnIndex - 1];
                                }
                            }
                            vIndexes[i] = vIndex;
                            

                        }
                        //加入三角形
                        for (int i = 0; i < 3; i++) {
                            triangles.Add(vIndexes[i] - 1);
                        }
                        if (vIndexes.Length == 4) {
                            triangles.Add(vIndexes[0] - 1);
                            triangles.Add(vIndexes[2] - 1);
                            triangles.Add(vIndexes[3] - 1);
                        } 

                    } else {
                        triangles.Add(int.Parse(pos[0]) - 1);
                        triangles.Add(int.Parse(pos[1]) - 1);
                        triangles.Add(int.Parse(pos[2]) - 1);
                        if (pos.Length == 4) {
                            triangles.Add(int.Parse(pos[0]) - 1);
                            triangles.Add(int.Parse(pos[2]) - 1);
                            triangles.Add(int.Parse(pos[3]) - 1);
                        }
                    }
                } else {
                    Debug.Log("第" + lineNum + "行数据故障：" + line);
                }
            }
        }
        mesh.vertices = verticies.ToArray();
        mesh.normals = uNorm;
        mesh.triangles = triangles.ToArray();
        mesh.uv = uUV;//uvs.ToArray();
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();
        mesh.RecalculateBounds();
        Debug.Log(string.Format("模型加载完成,面数：{0}点数：{1}",triangles.Count/3,verticies.Count));
        Debug.LogFormat("UV:\nleft:{0}\tright:{1}\tup:{2}\tdown:{3}", left, right, up, down);
        return mesh;
    }

    public static Vector3[] readVerticiesByObj(string filename) {
        List<Vector3> verticies = new List<Vector3>();

        System.IO.StreamReader sr = new System.IO.StreamReader(filename);
        string line;
        int lineNum = 0;
        while ((line = sr.ReadLine()) != null) {
            lineNum++;
            if (line.StartsWith("v ")) {
                line = line.Substring(2).Trim();
                string[] pos = line.Split(' ');
                if (pos.Length == 3) {
                    verticies.Add(new Vector3(float.Parse(pos[0]) / 100f, float.Parse(pos[1]) / 100f, float.Parse(pos[2]) / 100f));
                } else {
                    Debug.Log("第" + lineNum + "行数据故障：" + line);
                }
            } else if (line.StartsWith("vt ") || line.StartsWith("vn ")|| line.StartsWith("f ")) {
                break;
            }
        }

        return verticies.ToArray();
    }
}
