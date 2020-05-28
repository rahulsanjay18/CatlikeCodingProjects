using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTransformation : Transformation
{
	public focal_length = 1f;
    public override Matrix4x4 Matrix{
		get{
			Matrix4x4 matrix = new Matrix4x4();
			matrix.SetRow(0, new Vector4(focal_length, 0f, 0f, 0f));
			matrix.SetRow(1, new Vector4(0f, focal_length, 0f, 0f));
			matrix.SetRow(2, new Vector4(0f, 0f, 1f, 0f));
			matrix.SetRow(3, new Vector4(0f, 0f, 0f, 1f));
			return matrix;
		}
	}
}
