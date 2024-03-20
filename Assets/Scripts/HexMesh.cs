using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class HexMesh : MonoBehaviour
{
    Mesh hexMesh;
    MeshCollider meshCollider;

    List<Vector3> vertices;
    List<int> triangles;
    List<Color> colors;

    private void Awake()
    {
        GetComponent<MeshFilter>().mesh = hexMesh = new Mesh();
        meshCollider = gameObject.AddComponent<MeshCollider>();
        hexMesh.name = "Hex Mesh";
        vertices = new List<Vector3>();
        triangles = new List<int>();
        colors = new List<Color>();
    }

    public void Triangulate(HexCell[] cells)
    {
        hexMesh.Clear();
        vertices.Clear();
        triangles.Clear();
        colors.Clear();

        for (int i = 0; i < cells.Length; i++)
        {
            Triangulate(cells[i]);
        }
        hexMesh.vertices = vertices.ToArray();
        hexMesh.triangles = triangles.ToArray();
        hexMesh.RecalculateNormals();
        meshCollider.sharedMesh = hexMesh;
        hexMesh.colors = colors.ToArray();
    }

    void Triangulate(HexCell cell)
    {
        for(HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
        {
            Triangulate(d, cell);
        }
        
    }
    void Triangulate(HexDirection direction, HexCell cell)
    {
        Vector3 center = cell.transform.localPosition;
        Vector3 v1 = center + HexMetrics.GetFirstSolidCorner(direction);
        Vector3 v2 = center + HexMetrics.GetSecondSolidCorner(direction);

        AddTriangle(center, v1, v2);
        AddTriangleColor(cell.color);

        if(direction == HexDirection.NE)
        {
            TriangulateConnection(direction, cell, v1, v2);
        }
        if(direction <= HexDirection.SE)
        {
            TriangulateConnection(direction, cell, v1, v2);
        }

        //Vector3 v3 = center + HexMetrics.GetFirstCorner(direction);
        //Vector3 v4 = center + HexMetrics.GetSecondCorner(direction);

        //Vector3 bridge = HexMetrics.GetBridge(direction);
        //Vector3 v3 = v1 + bridge;
        //Vector3 v4 = v2 + bridge;

        //AddQuad(v1, v2, v3, v4);

        //HexCell prevNeighbor = cell.GetNeighbor(direction.Previous()) ?? cell;
        //HexCell neighbor = cell.GetNeighbor(direction) ?? cell;
        //HexCell nextNeighbor = cell.GetNeighbor(direction.Next()) ?? cell;

        //AddQuadColor(cell.color,(cell.color + neighbor.color) * 0.5f);

        //Color bridgeColor = (cell.color + neighbor.color) * 0.5f;
        //AddQuadColor(cell.color, bridgeColor);

        //AddTriangle(v1, center + HexMetrics.GetFirstCorner(direction), v3);
        //AddTriangleColor(
        //    cell.color,
        //    (cell.color + prevNeighbor.color + neighbor.color) / 3f,
        //    bridgeColor
        //    );
        //AddTriangle(v2, v4, center + HexMetrics.GetSecondCorner(direction));
        //AddTriangleColor(
        //    cell.color,
        //    bridgeColor,
        //    (cell.color + neighbor.color + nextNeighbor.color) / 3f
        //    );
        //AddQuadColor(
        //    cell.color,
        //    cell.color,
        //    (cell.color + prevNeighbor.color + neighbor.color) / 3f,
        //    (cell.color + neighbor.color + nextNeighbor.color) / 3f
        //);

    }
    void TriangulateConnection(HexDirection direction, HexCell cell, Vector3 v1, Vector3 v2)
    {
        HexCell neighbor = cell.GetNeighbor(direction);
        if (neighbor == null) { return; }
        Vector3 bridge = HexMetrics.GetBridge(direction);
        Vector3 v3 = v1 + bridge;
        Vector3 v4 = v2 + bridge;
        v3.y = v4.y = neighbor.Elevation * HexMetrics.elevationStep;

        TriangulateEdgeTerraces(v1, v2, cell, v3, v4, neighbor);

        //AddQuad(v1, v2, v3, v4);
        //AddQuadColor(cell.color, neighbor.color);

        HexCell nextNeighbor = cell.GetNeighbor(direction.Next());
        if (direction <= HexDirection.E && nextNeighbor != null)
        {
            Vector3 v5 = v2 + HexMetrics.GetBridge(direction.Next());
            v5.y = nextNeighbor.Elevation * HexMetrics.elevationStep;
            AddTriangle(v2, v4, v5);
            AddTriangleColor(cell.color, neighbor.color, nextNeighbor.color);
        }
    }
    void TriangulateEdgeTerraces(Vector3 beginLeft, Vector3 beginRight, HexCell beginCell,Vector3 endLeft, Vector3 endRight, HexCell endCell)
    {
        Vector3 v3 = HexMetrics.TerraceLerp(beginLeft, endLeft, 1);
        Vector3 v4 = HexMetrics.TerraceLerp(beginRight, endRight, 1);
        Color c2 = HexMetrics.TerraceLerp(beginCell.color, endCell.color, 1);

        AddQuad(beginLeft, beginRight, v3, v4);
        AddQuadColor(beginCell.color, c2);

        for(int i = 2; i < HexMetrics.terracesSteps; i++)
        {
            Vector3 v1 = v3;
            Vector3 v2 = v4;
            Color c1 = c2;
            v3 = HexMetrics.TerraceLerp(beginLeft, endLeft, i);
            v4 = HexMetrics.TerraceLerp(beginRight, endRight, i);
            c2 = HexMetrics.TerraceLerp(beginCell.color, endCell.color, i);
            AddQuad(v1, v2 , v3, v4);
            AddQuadColor(c1 , c2);
        }
        AddQuad(v3, v4, endLeft, endRight);
        AddQuadColor(c2, endCell.color);
    }
    void AddTriangle(Vector3 v1, Vector3 v2, Vector3 v3)
    {
        int vertexIndex = vertices.Count;
        vertices.Add(v1);
        vertices.Add(v2); 
        vertices.Add(v3);
        triangles.Add(vertexIndex);
        triangles.Add(vertexIndex + 1);
        triangles.Add(vertexIndex + 2);
    }
    void AddTriangleColor(Color color)
    {
        colors.Add(color);
        colors.Add(color);
        colors.Add(color);
    }
    void AddTriangleColor(Color c1, Color c2, Color c3)
    {
        colors.Add(c1);
        colors.Add(c2);
        colors.Add(c3);
    }
    void AddQuad(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4)
    {
        int vertexIndex = vertices.Count;
        vertices.Add((v1));
        vertices.Add((v2));
        vertices.Add((v3));
        vertices.Add((v4));
        triangles.Add(vertexIndex);
        triangles.Add(vertexIndex + 2);
        triangles.Add(vertexIndex + 1);
        triangles.Add(vertexIndex + 1);
        triangles.Add(vertexIndex + 2);
        triangles.Add(vertexIndex + 3);
    }
    void AddQuadColor(Color c1, Color c2, Color c3, Color c4)
    {
        colors.Add(c1);
        colors.Add(c2);
        colors.Add(c3);
        colors.Add(c4);
    }
    void AddQuadColor(Color c1, Color c2)
    {
        colors.Add(c1);
        colors.Add(c1);
        colors.Add(c2);
        colors.Add(c2);
    }
}
