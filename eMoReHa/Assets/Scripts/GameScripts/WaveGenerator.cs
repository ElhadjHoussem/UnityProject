using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveGenerator : MonoBehaviour
{
    public int Dimension = 1000;
    public Octave[] Octaves;

    protected MeshFilter MeshFilter;
    protected Mesh Mesh;
    // Start is called before the first frame update
    void Start()
    {
        Mesh = new Mesh();
        Mesh.name = gameObject.name;
        Mesh.vertices = GenerateVertices();
        Mesh.triangles = GenerateTriangles();
        Mesh.RecalculateBounds();
        MeshFilter = gameObject.AddComponent<MeshFilter>();
        MeshFilter.mesh = Mesh;
    }

    private Vector3[] GenerateVertices()
    {
        var verts = new Vector3[(Dimension + 1)*(Dimension + 1)];
        
        //equally Dist vertices
        for(int x =0; x <= Dimension; x++)
            for(int z =0; z<= Dimension; z++)   
                verts[getIndex(x, z)] = new Vector3(x, 0, z);
           
        return verts;
    }

    private int getIndex(int x, int z)
    {
        return x * (Dimension + 1) + z;
    }

    private int[] GenerateTriangles()
    {
        var triangles = new int[Mesh.vertices.Length * 6];
        for(int x=0; x < Dimension; x++)
        {
            for (int z = 0; z < Dimension; z++)
            {
                triangles[getIndex(x, z) * 6 + 0] = getIndex(x, z);
                triangles[getIndex(x, z) * 6 + 1] = getIndex(x+1, z+1);
                triangles[getIndex(x, z) * 6 + 2] = getIndex(x+1, z);

                triangles[getIndex(x, z) * 6 + 3] = getIndex(x, z);
                triangles[getIndex(x, z) * 6 + 4] = getIndex(x, z+1);
                triangles[getIndex(x, z) * 6 + 5] = getIndex(x+1, z+1);

            }
        }
        return triangles;
    }
    [Serializable]
    public struct Octave
    {
        public Vector2 speed;
        public Vector2 scale;
        public float height;
        public bool alternate;

    }
    // Update is called once per frame
    void Update()
    {
        var verts = Mesh.vertices;

        //equally Dist vertices
        for (int x = 0; x <= Dimension; x++)
            for (int z = 0; z <= Dimension; z++)
            {
                var y = 0f;
                for (int oct = 0; oct < Octaves.Length; oct++)
                {
                    if (Octaves[oct].alternate)
                    {
                        var perlNoise = Mathf.PerlinNoise((x*Octaves[oct].scale.x)/Dimension, (z * Octaves[oct].scale.y) / Dimension) * Math.PI * 2f;
                        y += Mathf.Cos((float)(perlNoise + Octaves[0].speed.magnitude * Time.time)) * Octaves[0].height;
                    }
                }
                verts[getIndex(x, z)] = new Vector3(x, y, z);

            }

        Mesh.vertices= verts;
    }
}
