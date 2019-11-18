using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MemLib;
using MemLib.HalfEdgeStructure;

[RequireComponent(typeof(MeshFilter))]
[ExecuteInEditMode]
public class CMeshViewer : MonoBehaviour {
	Mesh mesh;
	Mesh vertMesh;
	Vector3[] verts;
	[Header("渲染选项")]
	public bool VertexRender = false;
	public bool EdgeRender = false;
	[Header("尺寸")]
	[Range(0.1f,10f)]
	public float vertSize = 1f;

	ViewableVertex[] vBalls;
	// Use this for initialization
	void Start () {
		mesh = GetComponent<MeshFilter> ().sharedMesh;
		verts = mesh.vertices;

		vertMesh = new Mesh ();
		vBalls = GetComponentsInChildren<ViewableVertex> ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnRenderObject(){
		//渲染顶点
		/*
		if (VertexRender) {
			MaterialLib.VertexMaterial.SetPass(0);
			if (vertMesh != null) {
				vertMesh.Clear();
				List <CombineInstance> combineInstances = new List<CombineInstance>();
				//Graphics.DrawMeshNow(vertMesh, transform.localToWorldMatrix);
				float scale = 0.005f*vertSize;
				foreach (Vector3 position in verts) {
					//平移矩阵
					Matrix4x4 translateMatrix = new Matrix4x4(
						new Vector4(scale, 0, 0, 0),
						new Vector4(0, scale, 0, 0),
						new Vector4(0, 0, scale, 0),
						new Vector4(position.x, position.y, position.z, 1)
					);
					CombineInstance instance = new CombineInstance();
					instance.mesh = MeshLib.Cube;
					instance.transform = translateMatrix;//transform.localToWorldMatrix *
					combineInstances.Add(instance);
					//Graphics.DrawMeshNow(vertMesh, transform.localToWorldMatrix * translateMatrix);
				}
				vertMesh.CombineMeshes(combineInstances.ToArray(), false);
			}

			Graphics.DrawMeshNow(vertMesh, transform.localToWorldMatrix);
		}*/
		if (VertexRender) {
			if (vBalls == null) {
				vBalls = new ViewableVertex[verts.Length];
			} else if (vBalls.Length < verts.Length) {
				ViewableVertex[] temp = new ViewableVertex[verts.Length];
				for (int i = 0; i < vBalls.Length; i++) {
					temp [i] = vBalls [i];
				}
//				for (int i = vBalls.Count; i < verts.Count; i++) {
//					temp [i] = ViewableVertex.Create (0,Vector3.zero,transform);
//				}
				vBalls = temp;
			} else if (vBalls.Length > verts.Length) {
				ViewableVertex[] temp = new ViewableVertex[verts.Length];
				for (int i = 0; i < verts.Length; i++) {
					temp [i] = vBalls [i];
				}
				vBalls = temp;
			}

			for (int i = 0; i < verts.Length; i++) {
				if (vBalls [i] != null) {
					vBalls [i].gameObject.SetActive (true);

					vBalls [i].setName (i + "");

					float scale = 0.05f * vertSize;
					vBalls [i].setScale (vertSize);
					Vector3 position = verts [i];
//					vBalls [i].transform=transform.localToWorldMatrix*
//						new Matrix4x4(
//						new Vector4(scale, 0, 0, 0),
//						new Vector4(0, scale, 0, 0),
//						new Vector4(0, 0, scale, 0),
//						new Vector4(position.x, position.y, position.z, 1)
//					);
					vBalls [i].transform.localPosition = position;
				} else {
					vBalls [i] = ViewableVertex.Create (i, verts [i], transform);
				}
			}
		} else {
			for (int i = 0; i < verts.Length; i++) {
				if (vBalls [i] != null) {
					vBalls [i].gameObject.SetActive (false);
				}
			}
		}
		//渲染边
		if(EdgeRender){
			MaterialLib.EdgeMaterial.SetPass(0);
			// GL.DrawLine();
		}
	}
}
