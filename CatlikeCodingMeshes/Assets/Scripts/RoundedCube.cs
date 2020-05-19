using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class RoundedCube : MonoBehaviour
{	
	public int x_size, y_size, z_size;
	public int roundness;
	
	private Mesh mesh;
	private Vector3[] vertices;
	private Vector3[] normals;
	
	private Color32[] cubeUV;
	
	private void Awake(){
		Generate();
	}
	
	private void Generate(){
		GetComponent<MeshFilter>().mesh = mesh = new Mesh();
		mesh.name = "Procedural Rounded Cube";
		
		CreateVertices();
		CreateTriangles();
		CreateColliders();
	}
	
	private void CreateVertices(){
		int corner_vertices = 8;
		int edge_vertices = (x_size + y_size + z_size - 3) * 4;
		int face_vertices = (
			(x_size - 1) * (y_size - 1) +
			(x_size - 1) * (z_size - 1) +
			(y_size - 1) * (z_size - 1)) * 2;
			
		vertices = new Vector3[corner_vertices + edge_vertices + face_vertices];
		normals = new Vector3[vertices.Length];
		cubeUV = new Color32[vertices.Length];
		
		int v = 0;
		for(int y = 0; y <= y_size; y++){
			for(int x = 0; x <= x_size; x++){
				SetVertex(v++, x, y, 0);
			}
		
			for(int z = 1; z <= z_size; z++){
				SetVertex(v++, x_size, y, z);
			}
		
			for(int x = x_size - 1; x >= 0; x--){
				SetVertex(v++, x, y, z_size);
			}
		
			for(int z = z_size - 1; z > 0; z--){
				SetVertex(v++, 0, y, z);
			}
		}
		
		for(int z = 1; z < z_size; z++){
			for(int x = 1; x < x_size; x++){
				SetVertex(v++, x, y_size, z);
			}
		}
		
		for(int z = 1; z < z_size; z++){
			for(int x = 1; x < x_size; x++){
				SetVertex(v++, x, 0, z);
			}
		}
		
		mesh.vertices = vertices;
		mesh.normals = normals;
		mesh.colors32 = cubeUV;
	}
	
	private void SetVertex(int i, int x, int y, int z){
		Vector3 inner = vertices[i] = new Vector3(x, y, z);
		
		if(x < roundness){
			inner.x = roundness;
		}else if(x > x_size - roundness){
			inner.x = x_size - roundness;
		}
		if(y < roundness){
			inner.y = roundness;
		}else if(y > y_size - roundness){
			inner.y = y_size - roundness;
		}
		
		if(z < roundness){
			inner.z = roundness;
		}else if(z > z_size - roundness){
			inner.z = z_size - roundness;
		}
		
		normals[i] = (vertices[i] - inner).normalized;
		vertices[i] = inner + normals[i] * roundness;
		cubeUV[i] = new Color32((byte) x, (byte) y, (byte) z, 0);
	}
	
	private void CreateTriangles(){
		int[] trianglesZ = new int[(x_size * y_size) * 12];
		int[] trianglesX = new int[(y_size * z_size) * 12];
		int[] trianglesY = new int[(x_size * z_size) * 12];
		int ring = (x_size + z_size) * 2;
		int tZ = 0, tX = 0, tY = 0, v = 0;
		
		for(int y = 0; y < y_size; y++, v++){
			for(int q = 0; q < x_size; q++, v++){
				tZ = SetQuad(trianglesZ, tZ, v, v + 1, v + ring, v + ring + 1);
			}
			
			for(int q = 0; q < z_size; q++, v++){
				tX = SetQuad(trianglesX, tX, v, v + 1, v + ring, v + ring + 1);
			}
			
			for(int q = 0; q < x_size; q++, v++){
				tZ = SetQuad(trianglesZ, tZ, v, v + 1, v + ring, v + ring + 1);
			}
			
			for(int q = 0; q < z_size - 1; q++, v++){
				tX = SetQuad(trianglesX, tX, v, v + 1, v + ring, v + ring + 1);
			}
		
			tX = SetQuad(trianglesX, tX, v, v - ring + 1, v + ring, v + 1);
		}
		
		tY = CreateTopFace(trianglesY, tY, ring);
		tY = CreateBottomFace(trianglesY, tY, ring);
		
		mesh.subMeshCount = 3;
		mesh.SetTriangles(trianglesZ, 0);
		mesh.SetTriangles(trianglesX, 1);
		mesh.SetTriangles(trianglesY, 2);
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
		
		for(int i = 0; i < vertices.Length; i++){
			Gizmos.color = Color.black;
			Gizmos.DrawSphere(vertices[i], .1f);
			Gizmos.color = Color.yellow;
			Gizmos.DrawRay(vertices[i], normals[i]);
		}
	}
	
	private void CreateColliders(){
		AddBoxCollider(x_size, y_size - roundness * 2, z_size - roundness * 2);
		AddBoxCollider(x_size - roundness * 2, y_size, z_size - roundness * 2);
		AddBoxCollider(x_size - roundness * 2, y_size - roundness * 2, z_size);
		
		Vector3 min = Vector3.one * roundness;
		Vector3 half = new Vector3(x_size, y_size, z_size) * .5f;
		Vector3 max = new Vector3(x_size, y_size, z_size) - min;
		
		AddCapsuleCollider(0, half.x, min.y, min.z);
		AddCapsuleCollider(0, half.x, min.y, max.z);
		AddCapsuleCollider(0, half.x, max.y, min.z);
		AddCapsuleCollider(0, half.x, max.y, max.z);
		
		AddCapsuleCollider(1, min.x, half.y, min.z);
		AddCapsuleCollider(1, min.x, half.y, max.z);
		AddCapsuleCollider(1, max.x, half.y, min.z);
		AddCapsuleCollider(1, max.x, half.y, max.z);
		
		AddCapsuleCollider(2, min.x, min.y, half.z);
		AddCapsuleCollider(2, min.x, max.y, half.z);
		AddCapsuleCollider(2, max.x, min.y, half.z);
		AddCapsuleCollider(2, max.x, max.y, half.z);
	}
	
	private void AddBoxCollider(float x, float y, float z){
		BoxCollider c = gameObject.AddComponent<BoxCollider>();
		c.size = new Vector3(x, y, z);
	}
	
	private void AddCapsuleCollider(int direction, float x, float y, float z){
		CapsuleCollider c = gameObject.AddComponent<CapsuleCollider>();
		c.center = new Vector3(x, y, z);
		c.direction = direction;
		c.radius = roundness;
		c.height = c.center[direction] * 2f;
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
