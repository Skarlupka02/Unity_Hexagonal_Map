using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.PlayerLoop;

public class HexMapEditor : MonoBehaviour
{
    public Color[] colors;
    public HexGrid hexGrid;
    private Color activeColor;
    bool applyColor;
    bool applyElevation = true;
    int activeElevation;
    int brushSize;

    void Awake()
    {
        SelectColor(0);
    }
    void Update()
    {
        if(Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            HandleInput();
        }
    }
    void HandleInput()
    {
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(inputRay, out hit)) { EditCells(hexGrid.GetCell(hit.point)); }
    }
    void EditCells(HexCell center)
    {
        int centerX = center.coordinates.X;
        int centerZ = center.coordinates.Z;
        for (int r = 0, z = centerZ - brushSize; z <= centerZ; z++, r++)
        {
            for(int x = centerX - r; x <= centerX + brushSize; x++)
            {
                EditCell(hexGrid.GetCell(new HexCoordinates(x, z)));
            }
        }
        for (int r = 0, z = centerZ + brushSize; z > centerZ; z--, r++)
        {
            for (int x = centerX - brushSize; x <= centerX + r; x++)
            {
                EditCell(hexGrid.GetCell(new HexCoordinates(x, z)));
            }
        }
    }
    public void SetElevation(float elevation)
    {
        activeElevation = (int)elevation;
    }
    void EditCell(HexCell cell)
    {
        Debug.Log(brushSize);
        if (cell)
        {
            if (applyColor)
            {
                cell.Color = activeColor;
            }
            if (applyElevation)
            {
                cell.Elevation = activeElevation;
            }
        }
        
        //hexGrid.Refresh();
    }
    public void SelectColor(int index)
    {
        applyColor = index >= 0;
        if (applyColor) { 
            activeColor = colors[index];
        }
    }

    public void SetApplyElevation (bool elevation)
    {
        applyElevation = elevation;
    }

    public void SetBrushSize (float size)
    {
        brushSize = (int)size;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }
}
