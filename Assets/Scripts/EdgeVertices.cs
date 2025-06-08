using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public struct EdgeVertices
{
    public Vector3 v1, v2, v3, v4, v5;

    public EdgeVertices(Vector3 corner1, Vector3 corner2)
    {
        v1 = corner1;
        v2 = Vector3.Lerp(corner1, corner2, 0.25f);
        v3 = Vector3.Lerp(corner1, corner2, 0.5f);
        v4 = Vector3.Lerp(corner1, corner2, 0.75f);
        v5 = corner2;
    }
    public static EdgeVertices TerraceLepr(EdgeVertices a, EdgeVertices b, int atep)
    {
        EdgeVertices result;
        result.v1 = HexMetrics.TerraceLerp(a.v1, b.v1, atep);
        result.v2 = HexMetrics.TerraceLerp(a.v2, b.v2, atep);
        result.v3 = HexMetrics.TerraceLerp(a.v3, b.v3, atep);
        result.v4 = HexMetrics.TerraceLerp(a.v4, b.v4, atep);
        result.v5 = HexMetrics.TerraceLerp(a.v5, b.v5, atep);
        return result;
    }
}

