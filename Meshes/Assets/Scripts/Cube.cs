using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class Cube : MonoBehaviour
{	
	public int x_size, y_size, z_size;
	
	private Mesh mesh;
	private Vector3[] vertices;
	
	private void Awake(){
		Generate();
	}
	
	private void Generate(){
		GetComponent<MeshFilter>().mesh = mesh = new Mesh();
		mesh.name = "Procedural Cube";
		
		CreateVertices();
		CreateTriangles();
		
	}
	
	private void CreateVertices(){
		int corner_vertices = 8;
		int edge_vertices = (x_size + y_size + z_size - 3) * 4;
		int face_vertices = (
			(x_size - 1) * (y_size - 1) +
			(x_size - 1) * (z_size - 1) +
			(y_size - 1) * (z_size - 1)) * 2;
			
		vertices = new Vector3[corner_vertices + edge_vertices + face_vertices];
		
		int v = 0;
		for(int y = 0; y <= y_size; y++){
			for(int x = 0; x <= x_size; x++){
				vertices[v++] = new Vector3(x, y, 0);
			}
		
			for(int z = 1; z <= z_size; z++){
				vertices[v++] = new Vector3(x_size, y, z);
			}
		
			for(int x = x_size - 1; x >= 0; x--){
				vertices[v++] = new Vector3(x, y, z_size);
			}
		
			for(int z = z_size - 1; z > 0; z--){
				vertices[v++] = new Vector3(0, y, z);
			}
		}
		
		for(int z = 1; z < z_size; z++){
			for(int x = 1; x < x_size; x++){
				vertices[v++] = new Vector3(x, y_size, z);
			}
		}
		
		for(int z = 1; z < z_size; z++){
			for(int x = 1; x < x_size; x++){
				vertices[v++] = new Vector3(x, 0, z);
			}
		}
		
		mesh.vertices = vertices;
	}
	
	private void CreateTriangles(){
		int quads = (x_size * y_size + x_size * z_size + y_size * z_size) * 2;
		int[] triangles = new int[quads * 6];
		int ring = (x_size + z_size) * 2;
		int t = 0, v = 0;
		
		for(int y = 0; y < y_size; y++, v++){
			for(int q = 0; q < ring - 1; q++, v++){
				t = SetQuad(triangles, t, v, v + 1, v + ring, v + ring + 1);
			}
		
			t = SetQuad(triangles, t, v, v - ring + 1, v + ring, v + 1);
		}
		
		t = CreateTopFace(triangles, t, ring);
		t = CreateBottomFace(triangles, t, ring);
		
		mesh.triangles = triangles;
	}
	
	private static int SetQuad(int[] triangles, int i, int v00, int v10, int v01, int v11){
		triangles[i] = v00;
		triangles[i+1] = triangles[i+4] = v01;
		triangles[i+2] = triangles[i+3] = v10;
		triangles[i+5] = v11;
		
		return i+6;
	}
	
	private int CreateTopFace(int[] triangles, int t, int ring){
		int v = ring * y_size;
		for(int x = 0; x < x_size - 1; x++, v++){
			t = SetQuad(triangles, t, v, v + 1, v + ring - 1, v + ring);
		}
		t = SetQuad(triangles, t, v, v + 1, v + ring - 1, v + 2);
		
		int v_min = ring * (y_size + 1) - 1;
		int v_mid = v_min + 1;
		int v_max = v + 2;
		
		for(int z = 1; z < z_size - 1; z++, v_min--, v_mid++, v_max++){
			t = SetQuad(triangles, t, v_min, v_mid, v_min - 1, v_mid + x_size - 1);
		
			for(int x = 1; x < x_size - 1; x++, v_mid++){
				t = SetQuad(triangles, t, v_mid, v_mid + 1, v_mid + x_size - 1, v_mid + x_size);
			}
			t = SetQuad(triangles, t, v_mid, v_max, v_mid + x_size - 1, v_max + 1);
		}
		
		int v_top = v_min - 2;
		t = SetQuad(triangles, t, v_min, v_mid, v_top + 1, v_top);
		for(int x = 1; x < x_size - 1; x++, v_top--, v_mid++){
			t = SetQuad(triangles, t, v_mid, v_mid + 1, v_top, v_top - 1);
		}

		t = SetQuad(triangles, t, v_mid, v_top - 2, v_top, v_top - 1);
		return t;
	}
	
	private int CreateBottomFace(int[] triangles, int t, int ring){
		int v = 1;
		int v_mid = vertices.Length - (x_size - 1) * (z_size - 1);
		
		t = SetQuad(triangles, t, ring - 1, v_mid, 0, 1);
		for(int x = 1; x < x_size - 1; x++, v++, v_mid++){
			t = SetQuad(triangles, t, v_mid, v_mid + 1, v, v + 1);
		}
		t = SetQuad(triangles, t, v_mid, v + 2, v, v + 1);
		
		int v_min = ring - 2;
		v_mid -= x_size - 2;
		int v_max = v + 2;
		
		for(int z = 1; z < z_size - 1; z++, v_min--, v_mid++, v_max++){
			t = SetQuad(triangles, t, v_min, v_mid + x_size - 1, v_min + 1, v_mid);
		
			for(int x = 1; x < x_size - 1; x++, v_mid++){
				t = SetQuad(triangles, t, v_mid + x_size - 1, v_mid + x_size, v_mid, v_mid + 1);
			}
			t = SetQuad(triangles, t, v_mid + x_size - 1, v_max + 1, v_mid, v_max);
		}
		
		int v_top = v_min - 1;
		t = SetQuad(triangles, t, v_top + 1, v_top, v_top + 2, v_mid);
		for(int x = 1; x < x_size - 1; x++, v_top--, v_mid++){
			t = SetQuad(triangles, t, v_top, v_top - 1, v_mid, v_mid + 1);
		}

		t = SetQuad(triangles, t, v_top, v_top - 1, v_mid, v_top - 2);
		return t;
	}
	
	private void OnDrawGizmos(){
		if(vertices == null){
			return;
		}
		
		Gizmos.color = Color.black;
		
		for(int i = 0; i < vertices.Length; i++){
			Gizmos.DrawSphere(vertices[i], .1f);
		}
	}

	
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
