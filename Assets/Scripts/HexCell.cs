using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexCell : MonoBehaviour
{
    public HexCoordinates coordinates;

    public Color color;

    int elevation;
    public RectTransform uiRect;

    [SerializeField]
    HexCell[] neighbors;

    public HexCell GetNeighbor (HexDirection direction)
    {
        return neighbors[(int)direction];
    }
    public void SetNeighbor(HexDirection direction, HexCell cell)
    {
        neighbors[(int)direction] = cell;
        cell.neighbors[(int)direction.Opposite()] = this;
    }
    public int Elevation
    {
        get
        {
            return elevation;
        }
        set
        {
            elevation = value;
            Vector3 position = transform.localPosition;
            position.y = value * HexMetrics.elevationStep;
            position.y += (HexMetrics.SampleNoise(position).y * 2f - 1f) * HexMetrics.elevationPerturbStrength;
            transform.localPosition = position;

            Vector3 uiPosition = uiRect.localPosition;
            //uiPosition.z = elevation * -HexMetrics.elevationStep;
            uiPosition.z = -position.y; ;
            uiRect.localPosition = uiPosition;
        }
    }
    
    public HexEdgeType GetEdgeType(HexDirection direction)
    {
        return HexMetrics.GetEdgeType(elevation, neighbors[(int)direction].elevation);
    }
    public HexEdgeType GetEdgeType(HexCell otherCell)
    {
        return HexMetrics.GetEdgeType(elevation, otherCell.elevation);
    }
    public Vector3 Position
    {
        get { return transform.localPosition; }
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
