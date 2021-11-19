using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.UIElements;

public class TreeGenerator : MonoBehaviour
{
    float maxAge;
    float age;
    float maxHeight;
    float height;
    Vector3[] brancheDirection;
    public GameObject branchObject;
    public GameObject testLeaves;
    GameObject[] testPoints;
    Vector3[] treeStemPoints;
    GameObject[,] branches;
    int[] branchesLength;
    int nextPoint;
    int nextBranch;
    float branchAngle;
    float radius;
    float radiusOffset;
    Vector3 pos;
    float branchRadius;
    int vertLoc;
    int triangleLoc;
    Mesh mesh;
    int numTrianglesPerChunk;
    Vector3[] vertices;
    int[] triangles;
    Vector3 topPoint;
    Vector3 offSet;

    void Start()
    {
        Random.InitState(System.DateTime.Now.Millisecond * this.GetInstanceID());

        maxAge = 50;
        maxHeight = 15;
        age = Random.Range(10.0f, maxAge);
        height = maxHeight * (age / maxAge);

        nextPoint = 0;
        nextBranch = 0;
        testPoints = new GameObject[(int)(height / 0.1f) + 1];
        branches = new GameObject[10, 15];
        branchesLength = new int[10];
        brancheDirection = new Vector3[10];
        radius = 0.4f;
        radiusOffset = 0.01f;
        treeStemPoints = new Vector3[32];
        offSet = new Vector3();

        mesh = new Mesh();
        vertLoc = 0;
        triangleLoc = 0;
        vertices = new Vector3[16384];
        numTrianglesPerChunk = 8192;
        triangles = new int[3 * numTrianglesPerChunk];
        topPoint = new Vector3();//transform.position;

        for (float i = 0; i < height; i += 0.5f)
        {
            if (i != 0) pos = new Vector3(topPoint.x + offSet.x, 0.5f + topPoint.y, topPoint.z + offSet.z);
            else pos = new Vector3(topPoint.x + offSet.x, topPoint.y, topPoint.z + offSet.z);
            //treeStemPoints[(int)i] = pos;

            if (Random.Range(0.0f, 100.0f) > 80.0f && i >= height / 3)
            {
                offSet = new Vector3(Random.Range(-0.1f, 0.1f), 0, Random.Range(-0.1f, 0.1f));
                if (Random.Range(0.0f, 100.0f) > 85.0f)
                {
                    CreateBranch(pos, radius, new Vector3(Random.Range(-0.3f, 0.3f), 0, Random.Range(-0.3f, 0.3f)));
                }
            }
            if (Random.Range(0.0f, 100.0f) > 70.0f && i >= height / 2)
            {
                CreateBranch(pos, radius, new Vector3(Random.Range(-0.3f, 0.3f), Random.Range(-0.3f, 0.3f), Random.Range(-0.3f, 0.3f)));
            }

            vertices[vertLoc] = new Vector3(pos.x, pos.y, pos.z - radius - radiusOffset);
            vertices[vertLoc + 1] = new Vector3(pos.x + offSet.x, pos.y + 0.50f, pos.z - radius + offSet.z);
            vertices[vertLoc + 2] = new Vector3(pos.x + radius + offSet.x, pos.y + 0.50f, pos.z + offSet.z);
            vertices[vertLoc + 3] = new Vector3(pos.x + radius + radiusOffset, pos.y, pos.z);

            triangles[triangleLoc] = vertLoc;
            triangles[triangleLoc + 1] = vertLoc + 1;
            triangles[triangleLoc + 2] = vertLoc + 2;
            triangles[triangleLoc + 3] = vertLoc;
            triangles[triangleLoc + 4] = vertLoc + 2;
            triangles[triangleLoc + 5] = vertLoc + 3;

            triangleLoc += 6;
            vertLoc += 4;
            
            vertices[vertLoc] = new Vector3(pos.x, pos.y, pos.z + radius + radiusOffset);
            vertices[vertLoc + 1] = new Vector3(pos.x + offSet.x, pos.y + 0.50f, pos.z + radius + offSet.z);
            vertices[vertLoc + 2] = new Vector3(pos.x + radius + offSet.x, pos.y + 0.50f, pos.z + offSet.z);
            vertices[vertLoc + 3] = new Vector3(pos.x + radius + radiusOffset, pos.y, pos.z);

            triangles[triangleLoc] = vertLoc + 2;
            triangles[triangleLoc + 1] = vertLoc + 1;
            triangles[triangleLoc + 2] = vertLoc;
            triangles[triangleLoc + 3] = vertLoc + 3;
            triangles[triangleLoc + 4] = vertLoc + 2;
            triangles[triangleLoc + 5] = vertLoc;

            triangleLoc += 6;
            vertLoc += 4;

            vertices[vertLoc] = new Vector3(pos.x, pos.y, pos.z + radius + radiusOffset);
            vertices[vertLoc + 1] = new Vector3(pos.x + offSet.x, pos.y + 0.50f, pos.z + radius + offSet.z);
            vertices[vertLoc + 2] = new Vector3(pos.x - radius + offSet.x, pos.y + 0.50f, pos.z + offSet.z);
            vertices[vertLoc + 3] = new Vector3(pos.x - radius - radiusOffset, pos.y, pos.z);

            triangles[triangleLoc] = vertLoc;
            triangles[triangleLoc + 1] = vertLoc + 1;
            triangles[triangleLoc + 2] = vertLoc + 2;
            triangles[triangleLoc + 3] = vertLoc;
            triangles[triangleLoc + 4] = vertLoc + 2;
            triangles[triangleLoc + 5] = vertLoc + 3;

            triangleLoc += 6;
            vertLoc += 4;

            vertices[vertLoc] = new Vector3(pos.x, pos.y, pos.z - radius - radiusOffset);
            vertices[vertLoc + 1] = new Vector3(pos.x + offSet.x, pos.y + 0.50f, pos.z - radius + offSet.z);
            vertices[vertLoc + 2] = new Vector3(pos.x - radius + offSet.x, pos.y + 0.50f, pos.z + offSet.z);
            vertices[vertLoc + 3] = new Vector3(pos.x - radius - radiusOffset, pos.y, pos.z);

            triangles[triangleLoc] = vertLoc + 2;
            triangles[triangleLoc + 1] = vertLoc + 1;
            triangles[triangleLoc + 2] = vertLoc;
            triangles[triangleLoc + 3] = vertLoc + 3;
            triangles[triangleLoc + 4] = vertLoc + 2;
            triangles[triangleLoc + 5] = vertLoc;

            triangleLoc += 6;
            vertLoc += 4;

            topPoint = pos;
            radius -= radiusOffset;
        }
        mesh.Clear();
        mesh.subMeshCount = 2;
        mesh.vertices = vertices;
        mesh.SetTriangles(triangles, 0);
        mesh.RecalculateNormals();

        GetComponent<MeshFilter>().mesh = mesh;
    }
    void CreateBranch(Vector3 branchStartPos, float r, Vector3 dir)
    {
        Vector3 branchPos;
        float length = Random.Range(0.5f, 2.5f);
        Mesh branchMesh = new Mesh();
        int branchVertLoc = 0;
        int branchTriangleLoc = 0;
        Vector3[] branchVertices = new Vector3[16384];
        int numTrianglesPerChunk = 8192;
        int[] branchTriangles = new int[3 * numTrianglesPerChunk];
        Vector3 branchTopPoint = branchStartPos;
        float branchRadius = r;
        Vector3 branchOffSet = dir;

        for (float i = 0; branchRadius > 0; i += 0.10f)
        {
            if (i > length) break;
            branchPos = new Vector3(branchTopPoint.x + branchOffSet.x, branchTopPoint.y + branchOffSet.y, branchTopPoint.z + branchOffSet.z);
            //treeStemPoints[(int)i] = pos;

            if (Random.Range(0.0f, 100.0f) > 90.0f)
            {
                branchOffSet.x += Random.Range(-0.3f, 0.3f);
                branchOffSet.z += Random.Range(-0.3f, 0.3f);
            }
            if (Random.Range(0.0f, 100.0f) > 95.0f)
            {
                CreateBranch(branchPos, branchRadius, new Vector3(Random.Range(-0.3f, 0.3f), Random.Range(-0.3f, 0.3f), Random.Range(-0.3f, 0.3f)));
            }

            branchVertices[branchVertLoc] = new Vector3(branchPos.x, branchPos.y, branchPos.z - branchOffSet.z * branchRadius);
            branchVertices[branchVertLoc + 1] = new Vector3(branchPos.x + branchOffSet.x, branchPos.y + 0.10f, branchPos.z - branchRadius + branchOffSet.z);
            branchVertices[branchVertLoc + 2] = new Vector3(branchPos.x + branchRadius + branchOffSet.x, branchPos.y + 0.10f, branchPos.z + branchOffSet.z);
            branchVertices[branchVertLoc + 3] = new Vector3(branchPos.x + branchRadius, branchPos.y, branchPos.z);

            branchTriangles[branchTriangleLoc] = branchVertLoc;
            branchTriangles[branchTriangleLoc + 1] = branchVertLoc + 1;
            branchTriangles[branchTriangleLoc + 2] = branchVertLoc + 2;
            branchTriangles[branchTriangleLoc + 3] = branchVertLoc;
            branchTriangles[branchTriangleLoc + 4] = branchVertLoc + 2;
            branchTriangles[branchTriangleLoc + 5] = branchVertLoc + 3;

            branchTriangleLoc += 6;
            branchVertLoc += 4;

            branchVertices[branchVertLoc] = new Vector3(branchPos.x, branchPos.y, branchPos.z + branchRadius);
            branchVertices[branchVertLoc + 1] = new Vector3(branchPos.x + branchOffSet.x, branchPos.y + 0.10f, branchPos.z + branchRadius + branchOffSet.z);
            branchVertices[branchVertLoc + 2] = new Vector3(branchPos.x + branchRadius + branchOffSet.x, branchPos.y + 0.10f, branchPos.z + branchOffSet.z);
            branchVertices[branchVertLoc + 3] = new Vector3(branchPos.x + branchRadius, branchPos.y, branchPos.z);

            branchTriangles[branchTriangleLoc] = branchVertLoc + 2;
            branchTriangles[branchTriangleLoc + 1] = branchVertLoc + 1;
            branchTriangles[branchTriangleLoc + 2] = branchVertLoc;
            branchTriangles[branchTriangleLoc + 3] = branchVertLoc + 3;
            branchTriangles[branchTriangleLoc + 4] = branchVertLoc + 2;
            branchTriangles[branchTriangleLoc + 5] = branchVertLoc;

            branchTriangleLoc += 6;
            branchVertLoc += 4;

            branchVertices[branchVertLoc] = new Vector3(branchPos.x, branchPos.y, branchPos.z + branchRadius);
            branchVertices[branchVertLoc + 1] = new Vector3(branchPos.x + branchOffSet.x, branchPos.y + 0.10f, branchPos.z + branchRadius + branchOffSet.z);
            branchVertices[branchVertLoc + 2] = new Vector3(branchPos.x - branchRadius + branchOffSet.x, branchPos.y + 0.10f, branchPos.z + branchOffSet.z);
            branchVertices[branchVertLoc + 3] = new Vector3(branchPos.x - branchRadius, branchPos.y, branchPos.z);

            branchTriangles[branchTriangleLoc] = branchVertLoc;
            branchTriangles[branchTriangleLoc + 1] = branchVertLoc + 1;
            branchTriangles[branchTriangleLoc + 2] = branchVertLoc + 2;
            branchTriangles[branchTriangleLoc + 3] = branchVertLoc;
            branchTriangles[branchTriangleLoc + 4] = branchVertLoc + 2;
            branchTriangles[branchTriangleLoc + 5] = branchVertLoc + 3;

            branchTriangleLoc += 6;
            branchVertLoc += 4;

            branchVertices[branchVertLoc] = new Vector3(branchPos.x, branchPos.y, branchPos.z - branchRadius + 0.0049f);
            branchVertices[branchVertLoc + 1] = new Vector3(branchPos.x + branchOffSet.x, branchPos.y + 0.10f, branchPos.z - branchRadius + 0.0049f + branchOffSet.z);
            branchVertices[branchVertLoc + 2] = new Vector3(branchPos.x - branchRadius + 0.0049f + branchOffSet.x, branchPos.y + 0.10f, branchPos.z + branchOffSet.z);
            branchVertices[branchVertLoc + 3] = new Vector3(branchPos.x - branchRadius + 0.0049f, branchPos.y, branchPos.z);

            branchTriangles[branchTriangleLoc] = branchVertLoc + 2;
            branchTriangles[branchTriangleLoc + 1] = branchVertLoc + 1;
            branchTriangles[branchTriangleLoc + 2] = branchVertLoc;
            branchTriangles[branchTriangleLoc + 3] = branchVertLoc + 3;
            branchTriangles[branchTriangleLoc + 4] = branchVertLoc + 2;
            branchTriangles[branchTriangleLoc + 5] = branchVertLoc;

            branchTriangleLoc += 6;
            branchVertLoc += 4;

            branchTopPoint = branchPos;
            branchRadius -= 0.01f;
        }
        branchMesh.Clear();
        branchMesh.subMeshCount = 2;
        branchMesh.vertices = branchVertices;
        branchMesh.SetTriangles(branchTriangles, 0);
        branchMesh.RecalculateNormals();

        GameObject temp = Instantiate(branchObject, this.gameObject.transform.position, new Quaternion());
        temp.GetComponent<MeshFilter>().mesh = branchMesh;
        temp.transform.parent = this.gameObject.transform;
    }
}