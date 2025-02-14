using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System;

public class HexGrid : MonoBehaviour
{
    public HexGridChunk chunkPrefab;
    public Texture2D noiseSourse;
    //public int cellCountX = 4;
    //public int cellCountZ = 3;

    int cellCountX, cellCountZ;
    public int chunkCountX = 4, chunkCountZ = 3;

    public HexCell cellPrefab;
    public Text cellLabelPrefab;

    public Color defaultColor = Color.white;
    //public Color touchedColor = Color.magenta;

    HexCell[] cells;
    HexGridChunk[] chunks;
    //Canvas gridCanvas;
    //HexMesh hexMesh;

    private void Awake()
    {
        HexMetrics.noiseSource = noiseSourse;

        //hexMesh= GetComponentInChildren<HexMesh>();
        //gridCanvas = GetComponentInChildren<Canvas>();

        cellCountX = chunkCountX * HexMetrics.chunkSizeX;
        cellCountZ = chunkCountZ * HexMetrics.chunkSizeZ;

        CreateChunks();
        CreateCells();
        

    }
    void CreateCells()
    {
        cells = new HexCell[cellCountX * cellCountZ];
        for (int z = 0, i = 0; z < cellCountZ; z++)
        {
            for (int x = 0; x < cellCountX; x++)
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
        //cell.transform.SetParent(transform, false); //Делает родителем ячейки HexGrid
        cell.transform.localPosition = position;
        cell.coordinates = HexCoordinates.FromOffsetCoordinates(x, z);
        cell.Color = defaultColor;

        if(x > 0) { cell.SetNeighbor(HexDirection.W, cells[i - 1]); }
        if (z > 0)
        {
            if ((z & 1) == 0) { 
                cell.SetNeighbor(HexDirection.SE, cells[i - cellCountX]); 
                if (x > 0)
                {
                    cell.SetNeighbor(HexDirection.SW, cells[i - cellCountX - 1]);
                }
            }
            else
            {
                cell.SetNeighbor(HexDirection.SW, cells[i - cellCountX]);
                if(x < cellCountX - 1) { 
                    cell.SetNeighbor(HexDirection.SE, cells[i - cellCountX + 1]);
                }
            }
        }

        Text label = Instantiate<Text>(cellLabelPrefab);
        //label.rectTransform.SetParent(gridCanvas.transform, false);
        label.rectTransform.anchoredPosition = new Vector2(position.x, position.z);
        label.text = cell.coordinates.ToStringOnSeparateLines();
        cell.uiRect = label.rectTransform;

        cell.Elevation = 0;

        AddCellToChunk(x, z, cell);
    }
    void CreateChunks()
    {
        chunks = new HexGridChunk[chunkCountX * chunkCountZ];

        for (int z = 0, i = 0; z < chunkCountZ; z++)
        {
            for (int x = 0; x < chunkCountX; x++)
            {
                HexGridChunk chunk = chunks[i++] = Instantiate(chunkPrefab);
                chunk.transform.SetParent(transform);
            }
        }
    }
    void AddCellToChunk(int x, int z, HexCell cell)
    {
        int chunkX = x / HexMetrics.chunkSizeX;
        int chunkZ = z / HexMetrics.chunkSizeZ;
        HexGridChunk chunk = chunks[chunkX + chunkZ * chunkCountX];

        int localX = x - chunkX * HexMetrics.chunkSizeX;
        int localZ = z - chunkZ * HexMetrics.chunkSizeZ;
        chunk.AddCell(localX + localZ * HexMetrics.chunkSizeZ, cell);
    }

    
    #region Косание ячеек
    public HexCell GetCell (Vector3 position)
    {
        position = transform.InverseTransformPoint(position);
        HexCoordinates coordinates = HexCoordinates.FromPosition(position);
        int index = coordinates.X + coordinates.Z * cellCountX + coordinates.Z / 2;
        return cells[index];
        //HexCell cell = cells[index];
        //cell.color = color;
        //hexMesh.Triangulate(cells);
        //Debug.Log("touch at" + position + "/" + coordinates.ToString() + "/" + index + "/" + color.ToString()) ;
    }
    public void Refresh()
    {
        //hexMesh.Triangulate(cells);
    }
    #endregion
    // Start is called before the first frame update
    void Start()
    {
        //hexMesh.Triangulate(cells);
    }

    private void OnEnable()
    {
        HexMetrics.noiseSource = noiseSourse;
    }
}
