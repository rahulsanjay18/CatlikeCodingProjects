using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class Grid : MonoBehaviour
{
    public int x_size, y_size;
	private Vector3[] vertices;
	private Mesh mesh;
	
	private void Awake(){
		Generate();
	}
	
	private void Generate(){
		
		GetComponent<MeshFilter>().mesh = mesh = new Mesh();
		mesh.name = "Procedural Grid";
		
		vertices = new Vector3[(x_size + 1) * (y_size +1)];
		Vector2[] uv = new Vector2[vertices.Length];
		Vector4[] tangents = new Vector4[vertices.Length];
		Vector4 tangent = new Vector4(1f, 0f, 0f, -1f);
		
		for(int i=0, y = 0; y <= y_size; y++){
			for(int x = 0; x <= x_size; x++, i++){
				vertices[i] = new Vector3(x, y);
				uv[i] = new Vector2((float)x / x_size, (float)y / y_size);
				tangents[i] = tangent;
			}
		}
		
		mesh.vertices = vertices;
		mesh.uv = uv;
		mesh.tangents = tangents;
		
		int[] triangles = new int[x_size * y_size * 6];
		for (int ti = 0, vi = 0, y = 0; y < y_size; y++, vi++) {
			for(int x = 0; x < x_size; x++, ti += 6, vi++){
				triangles[ti] = vi;
				triangles[ti + 3] = triangles[ti + 2] = vi + 1;
				triangles[ti + 4] = triangles[ti + 1] = vi + x_size + 1;
				triangles[ti + 5] = vi + x_size + 2;
				//mesh.triangles = triangles;
			}
		}
		mesh.triangles = triangles;
		mesh.RecalculateNormals();
	}
	
	private void OnDrawGizmos(){
		if(vertices == null){
			return;
		}
		Gizmos.color = Color.black;
		for(int i = 0; i < vertices.Length; i++){
			Gizmos.DrawSphere(vertices[i], 0.1f);
		}
	}
}
