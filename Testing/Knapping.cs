using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knapping : MonoBehaviour
{
    /* This script is an attempt to use similar math from the terrain generation
     * to create a system for players to chip away at procedurally shaped rocks
     * in real world space in order to create stone tools for use in the game world
     */
    Mesh mesh;
    Vector3[] vertices;
    int[,,] stone;
    int[] triangles;
    void Start()
    {
        stone = new int[5, 5, 5];
        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                for (int k = 0; k < 5; k++)
                {
                    stone[i, j, k] = 1;
                }
            }
        }
        DrawMesh();
        this.gameObject.GetComponent<MeshFilter>().mesh = mesh;
        this.gameObject.GetComponent<MeshCollider>().sharedMesh = mesh;
        this.gameObject.GetComponent<Transform>().localScale = new Vector3(.1f, .1f, .1f);
    }
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Y))
        {
            //Knapp(GameObject.Find("Main Camera").GetComponent<CameraMover>().GetHitPosition());
        }
    }
    void DrawMesh()
    {
        int vertLoc = 0;
        int triangleLoc = 0;
        mesh = new Mesh();
        int numTriangles = 1024;

        vertices = new Vector3[1024];
        triangles = new int[numTriangles * 3];

        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                for (int k = 0; k < 5; k++)
                {
                    if (stone[k, i, j] != 0)
                    {
                        if (j == 0 || stone[k, i, j - 1] == 0)
                        {
                            vertices[vertLoc] = new Vector3(k - 2.5f, i - 2.5f, j - 2.5f);
                            vertices[vertLoc + 1] = new Vector3(k - 2.5f, i - 1.5f, j - 2.5f);
                            vertices[vertLoc + 2] = new Vector3(k - 1.5f, i - 1.5f, j - 2.5f);
                            vertices[vertLoc + 3] = new Vector3(k - 1.5f, i - 2.5f, j - 2.5f);

                            triangles[triangleLoc] = vertLoc;
                            triangles[triangleLoc + 1] = vertLoc + 1;
                            triangles[triangleLoc + 2] = vertLoc + 2;
                            triangles[triangleLoc + 3] = vertLoc;
                            triangles[triangleLoc + 4] = vertLoc + 2;
                            triangles[triangleLoc + 5] = vertLoc + 3;

                            triangleLoc += 6;
                            vertLoc += 4;
                        }
                        if (i == 0 || stone[k, i - 1, j] == 0)
                        {
                            vertices[vertLoc] = new Vector3(k - 2.5f, i - 2.5f, j - 2.5f);
                            vertices[vertLoc + 1] = new Vector3(k - 2.5f, i - 2.5f, j - 1.5f);
                            vertices[vertLoc + 2] = new Vector3(k - 1.5f, i - 2.5f, j - 1.5f);
                            vertices[vertLoc + 3] = new Vector3(k - 1.5f, i - 2.5f, j - 2.5f);

                            triangles[triangleLoc] = vertLoc + 2;
                            triangles[triangleLoc + 1] = vertLoc + 1;
                            triangles[triangleLoc + 2] = vertLoc;
                            triangles[triangleLoc + 3] = vertLoc + 3;
                            triangles[triangleLoc + 4] = vertLoc + 2;
                            triangles[triangleLoc + 5] = vertLoc;

                            triangleLoc += 6;
                            vertLoc += 4;
                        }
                        if (k == 0 || stone[k - 1, i, j] == 0)
                        {
                            vertices[vertLoc] = new Vector3(k - 2.5f, i - 2.5f, j - 2.5f);
                            vertices[vertLoc + 1] = new Vector3(k - 2.5f, i - 2.5f, j - 1.5f);
                            vertices[vertLoc + 2] = new Vector3(k - 2.5f, i - 1.5f, j - 1.5f);
                            vertices[vertLoc + 3] = new Vector3(k - 2.5f, i - 1.5f, j - 2.5f);

                            triangles[triangleLoc] = vertLoc;
                            triangles[triangleLoc + 1] = vertLoc + 1;
                            triangles[triangleLoc + 2] = vertLoc + 2;
                            triangles[triangleLoc + 3] = vertLoc;
                            triangles[triangleLoc + 4] = vertLoc + 2;
                            triangles[triangleLoc + 5] = vertLoc + 3;

                            triangleLoc += 6;
                            vertLoc += 4;
                        }
                        if (j == 4 || stone[k, i, j + 1] == 0)
                        {
                            vertices[vertLoc] = new Vector3(k - 2.5f, i - 2.5f, j - 1.5f);
                            vertices[vertLoc + 1] = new Vector3(k - 2.5f, i - 1.5f, j - 1.5f);
                            vertices[vertLoc + 2] = new Vector3(k - 1.5f, i - 1.5f, j - 1.5f);
                            vertices[vertLoc + 3] = new Vector3(k - 1.5f, i - 2.5f, j - 1.5f);

                            triangles[triangleLoc] = vertLoc + 2;
                            triangles[triangleLoc + 1] = vertLoc + 1;
                            triangles[triangleLoc + 2] = vertLoc;
                            triangles[triangleLoc + 3] = vertLoc + 3;
                            triangles[triangleLoc + 4] = vertLoc + 2;
                            triangles[triangleLoc + 5] = vertLoc;

                            triangleLoc += 6;
                            vertLoc += 4;
                        }
                        if (i == 4 || stone[k, i + 1, j] == 0)
                        {
                            vertices[vertLoc] = new Vector3(k - 2.5f, i - 1.5f, j - 2.5f);
                            vertices[vertLoc + 1] = new Vector3(k - 2.5f, i - 1.5f, j - 1.5f);
                            vertices[vertLoc + 2] = new Vector3(k - 1.5f, i - 1.5f, j - 1.5f);
                            vertices[vertLoc + 3] = new Vector3(k - 1.5f, i - 1.5f, j - 2.5f);

                            triangles[triangleLoc] = vertLoc;
                            triangles[triangleLoc + 1] = vertLoc + 1;
                            triangles[triangleLoc + 2] = vertLoc + 2;
                            triangles[triangleLoc + 3] = vertLoc;
                            triangles[triangleLoc + 4] = vertLoc + 2;
                            triangles[triangleLoc + 5] = vertLoc + 3;

                            triangleLoc += 6;
                            vertLoc += 4;
                        }
                        if (k == 4 || stone[k + 1, i, j] == 0)
                        {
                            vertices[vertLoc] = new Vector3(k - 1.5f, i - 2.5f, j - 2.5f);
                            vertices[vertLoc + 1] = new Vector3(k - 1.5f, i - 2.5f, j - 1.5f);
                            vertices[vertLoc + 2] = new Vector3(k - 1.5f, i - 1.5f, j - 1.5f);
                            vertices[vertLoc + 3] = new Vector3(k - 1.5f, i - 1.5f, j - 2.5f);

                            triangles[triangleLoc] = vertLoc + 2;
                            triangles[triangleLoc + 1] = vertLoc + 1;
                            triangles[triangleLoc + 2] = vertLoc;
                            triangles[triangleLoc + 3] = vertLoc + 3;
                            triangles[triangleLoc + 4] = vertLoc + 2;
                            triangles[triangleLoc + 5] = vertLoc;

                            triangleLoc += 6;
                            vertLoc += 4;
                        }
                    }
                }
            }
        }
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }
    void Knapp(Vector3 pos)
    {
        Debug.Log(((int)(pos.x * 10) + 2) + ":" + ((int)(pos.y * 10) - 8) + ":" + ((int)(pos.z * 10) - 3));
        stone[((int)(pos.x * 10) + 2), ((int)(pos.y * 10) - 8), ((int)(pos.z * 10) - 3)] = 0;
        DrawMesh();
    }
}