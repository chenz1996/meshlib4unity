using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MemLib;

public class ViewableVertex : MonoBehaviour {

	public static ViewableVertex Create(int id,Vector3 position,Transform parent){
		GameObject ball=new GameObject("id");
		ball.layer = parent.gameObject.layer;
		ball.transform.position=position;
		ball.transform.parent=parent;
		ball.transform.localScale = new Vector3 (0.05f,0.05f,0.05f);

		ball.AddComponent<MeshFilter>().sharedMesh=MeshLib.Cube;
		ball.AddComponent<MeshRenderer>().material=MaterialLib.VertexMaterial;
		ball.AddComponent<ViewableVertex> ();
		return ball.GetComponent<ViewableVertex>();
	}

	public void setName(string name){
		gameObject.name = name;

	}

	public void setScale(float size){
		transform.localScale = new Vector3 (0.01f*size,0.01f*size,0.01f*size);
	}

	public void setPosition(Vector3 position){
		transform.position = position;
	}
	// Use this for initialization
	// void Start () {
		
	// }
	
	// // Update is called once per frame
	// void Update () {
		
	// }
}
