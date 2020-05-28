using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformationGrid : MonoBehaviour
{
    public Transform prefab;
	Matrix4x4 transformation;
	public int grid_resolution = 10;
	
	Transform[] grid;
	List<Transformation> transformations;
	
	void Awake(){
		grid = new Transform[grid_resolution * grid_resolution * grid_resolution];
		for (int i = 0, z = 0; z < grid_resolution; z++){
			for(int y = 0; y < grid_resolution; y++){
				for(int x = 0; x < grid_resolution; x++, i++){
					grid[i] = CreateGridPoint(x, y, z);
				}
			}
		}
		
		transformations = new List<Transformation>();
	}
	
	Transform CreateGridPoint(int x, int y, int z){
		Transform point = Instantiate<Transform>(prefab);
		point.localPosition = GetCoordinates(x, y, z);
		point.GetComponent<MeshRenderer>().material.color = new Color(
			(float)x / grid_resolution,
			(float)y / grid_resolution,
			(float)z / grid_resolution);
		return point;
	}
	
	Vector3 GetCoordinates(int x, int y, int z){
		return new Vector3(
			x - (grid_resolution - 1) * .5f,
			y - (grid_resolution - 1) * .5f,
			z - (grid_resolution - 1) * .5f);
	}
	
	void Update(){
		UpdateTransformation();
		for(int i = 0, z = 0; z < grid_resolution; z++){
			for(int y = 0; y < grid_resolution; y++){
				for(int x = 0; x < grid_resolution; x++, i++){
					grid[i].localPosition = TransformPoint(x, y, z);
				}
			}
		}
	}
	
	void UpdateTransformation(){
		GetComponents<Transformation>(transformations);
		if(transformations.Count > 0){
			transformation = transformations[0].Matrix;
			for (int i = 1; i < transformations.Count; i++){
				transformation = transformations[i].Matrix * transformation;
			}
		}
	}
	
	Vector3 TransformPoint(int x, int y, int z){
		Vector3 coordinates = GetCoordinates(x, y, z);
		return transformation.MultiplyPoint(coordinates);
	}
	
	
}
