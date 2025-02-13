using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System;

public class HexGrid : MonoBehaviour
{
    public Texture2D noiseSourse;
    public int width = 6;
    public int height = 6;

    public HexCell cellPrefab;
    public Text cellLabelPrefab;

    public Color defaultColor = Color.white;
    //public Color touchedColor = Color.magenta;

    HexCell[] cells;
    Canvas gridCanvas;
    HexMesh hexMesh;

    private void Awake()
    {
        HexMetrics.noiseSource = noiseSourse;
        hexMesh= GetComponentInChildren<HexMesh>();
        gridCanvas = GetComponentInChildren<Canvas>();

        cells = new HexCell[width * height];
        for (int z = 0, i = 0; z < height; z++){
            for (int x = 0; x < width; x++)
            {
                CreateCell(x, z, i++);
            }
        }
    }
    void CreateCell (int x, int z, int i)
    {
        Vector3 position;
        position.x = (x + z * 0.5f - z / 2) * (HexMetrics.innerRadius * 2f);
        position.y = 0f;
        position.z = z * (HexMetrics.outerRadius * 1.5f); 

        HexCell cell = cells[i] = Instantiate<HexCell>(cellPrefab);
        cell.transform.SetParent(transform, false); //������ ��������� ������ HexGrid
        cell.transform.localPosition = position;
        cell.coordinates = HexCoordinates.FromOffsetCoordinates(x, z);
        cell.color = defaultColor;

        if(x > 0) { cell.SetNeighbor(HexDirection.W, cells[i - 1]); }
        if (z > 0)
        {
            if ((z & 1) == 0) { 
                cell.SetNeighbor(HexDirection.SE, cells[i - width]); 
                if (x > 0)
                {
                    cell.SetNeighbor(HexDirection.SW, cells[i - width - 1]);
                }
            }
            else
            {
                cell.SetNeighbor(HexDirection.SW, cells[i - width]);
                if(x < width - 1) { 
                    cell.SetNeighbor(HexDirection.SE, cells[i - width + 1]);
                }
            }
        }

        Text label = Instantiate<Text>(cellLabelPrefab);
        label.rectTransform.SetParent(gridCanvas.transform, false);
        label.rectTransform.anchoredPosition = new Vector2(position.x, position.z);
        label.text = cell.coordinates.ToStringOnSeparateLines();
        cell.uiRect = label.rectTransform;

        cell.Elevation = 0;
    }

    #region ������� �����
    public HexCell GetCell (Vector3 position)
    {
        position = transform.InverseTransformPoint(position);
        HexCoordinates coordinates = HexCoordinates.FromPosition(position);
        int index = coordinates.X + coordinates.Z * width + coordinates.Z / 2;
        return cells[index];
        //HexCell cell = cells[index];
        //cell.color = color;
        //hexMesh.Triangulate(cells);
        //Debug.Log("touch at" + position + "/" + coordinates.ToString() + "/" + index + "/" + color.ToString()) ;
    }
    public void Refresh()
    {
        hexMesh.Triangulate(cells);
    }
    #endregion
    // Start is called before the first frame update
    void Start()
    {
        hexMesh.Triangulate(cells);
    }

    private void OnEnable()
    {
        HexMetrics.noiseSource = noiseSourse;
    }
}
