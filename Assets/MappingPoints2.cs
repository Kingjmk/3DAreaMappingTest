using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MappingPoints2 : MonoBehaviour
{

	public List<List<Vector3>> PointstoRender = new List<List<Vector3>>();
	public Vector3[,] PointArray;
	public GameObject OriginPointObject;
	public Vector3 OriginZero;
	public Material material;
	void Awake()
	{
		if (OriginPointObject != null) OriginZero = OriginPointObject.transform.position;
		init();
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			init();
		}
	}

	private void init()
	{
		MapPointsUsingRayCast(OriginZero, 10, 180, 10);
		//RenderPointsIntoQuads(PointstoRender);
	}


	void MapPointsUsingRayCast(Vector3 VZero, int rounds = 1, int range = 360, int accuarcy = 1)
	{
		Vector3 RayDir;
		RaycastHit hit;
		int ListIndex = -1;
		int jAlt = Mathf.CeilToInt(rounds/2);
		if (rounds % 2 != 0) jAlt++;
		for (int j = -Mathf.CeilToInt(rounds/2); j < jAlt; j++)
		{
			//PointList of Lists Stuff
			ListIndex++;
			PointstoRender.Add(new List<Vector3>());
			float Elevation = Mathf.Tan(j * Mathf.Deg2Rad);
			//Debug.Log("J = " + j + "/" + Elevation);
			for (int i = 0; i < range; ++i)
			{

				//if (j % accuarcy != 0) break;
				if (i % accuarcy != 0) continue;
				float x = Mathf.Sin(i * Mathf.Deg2Rad);
				float y = Mathf.Cos(i * Mathf.Deg2Rad);
				RayDir = new Vector3(x, Elevation, y);
				
				if (Physics.Raycast(VZero, RayDir, out hit, Mathf.Infinity))
				{
					//NEED TO GET THE POSITION ONLY FROM DISTANCE AND DIRECTION
					//Calc New Pos from Distance and Dir
					Vector3 newPos = RayDir * hit.distance + VZero;
					Debug.DrawLine(VZero, newPos, Color.green, 60f);
					//Debug.DrawRay(VZero,RayDir*hit.distance,Color.red,60f);
					PointstoRender[ListIndex].Add(hit.point); //Add To List of Lists
					//Debug.Log(PointstoRender[ListIndex].Last());
				}
			}
		}

	}

	private Mesh mesh;
	public Vector3[] vertices;

	void RenderPointsIntoQuads(List<List<Vector3>> Points,int x =1,int y=1)
	{
		int xSize = x;
		int ySize = y;
		int tSize = (xSize + 1) * (ySize + 1);
		Debug.Log("xSize : " + xSize);

		int yHeight = 15;
		GetComponent<MeshFilter>().mesh = mesh = new Mesh();
		mesh.name = "GeneratedMesh";
		vertices = new Vector3[tSize];
		Vector2[] uv = new Vector2[vertices.Length];
		Debug.Log("ySize : " + ySize);

		for (int i = 0; i <= xSize; i++)
		{
			Vector3 a;
			if (i == xSize)
			{
				a = Points[i - 1];
			}
			else
			{
				a = Points[i];
			}

			Debug.DrawRay(a, Vector3.Normalize(OriginZero - a) * Vector3.Distance(OriginZero, a) / 10, Color.red, 60f);

			vertices[i] = new Vector3(a.x, yHeight, a.z);
			//Need for loop for this
			vertices[i + (xSize + 1)] = new Vector3(a.x, yHeight + 1, a.z);
		}

		mesh.vertices = vertices;

		//MAGIC HERE
		//int[] triangles = new int[xSize * ySize * 6];
		//for (int ti = 0, vi = 0, x = 0; x < xSize; x++, ti += 6, vi++)
		//{
		//	triangles[ti] = vi;
		//	triangles[ti + 3] = triangles[ti + 2] = vi + 1;
		//	triangles[ti + 4] = triangles[ti + 1] = vi + xSize + 1;
		//	triangles[ti + 5] = vi + xSize + 2;
		//}
		int[] triangles = new int[xSize * ySize * 6];
		for (int ti = 0, vi = 0, y = 0; y < ySize; y++, vi++)
		{
			for (int x = 0; x < xSize; x++, ti += 6, vi++)
			{
				triangles[ti] = vi;
				triangles[ti + 3] = triangles[ti + 2] = vi + 1;
				triangles[ti + 4] = triangles[ti + 1] = vi + xSize + 1;
				triangles[ti + 5] = vi + xSize + 2;
			}
		}

		mesh.triangles = triangles;
		mesh.RecalculateNormals();

	}

}
