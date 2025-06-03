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
    bool isDrag;

    int activeElevation;
    int brushSize;

    HexDirection dragDirection;

    HexCell previousCell;

    void Awake()
    {
        SelectColor(0);
    }
    void Update()
    {
        if (Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            HandleInput();
        }
        else { previousCell = null; }
    }
    void HandleInput()
    {
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(inputRay, out hit)) { 
            HexCell currentCell = hexGrid.GetCell(hit.point);
            if (previousCell && previousCell != currentCell) { ValidateDrag(currentCell); }
            else isDrag = false;
            EditCells(currentCell); 
            previousCell = currentCell;
            isDrag = true;
        }
        else previousCell = null;
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
            if (riverMode == OptionalToggle.No)
            {
                cell.RemoveRiver();
            }
            else if (isDrag && riverMode == OptionalToggle.Yes)
            {
                HexCell otherCell = cell.GetNeighbor(dragDirection.Opposite());
                if (otherCell)
                {
                    otherCell.SetOutgoingRiver(dragDirection);
                }
            }
        }
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
    public void ShowUI(bool visible)
    {
        hexGrid.ShowUI(visible);
    }

    enum OptionalToggle
    {
        Ignore, Yes, No
    }

    OptionalToggle riverMode;

    public void SetRiverMode(int mode)
    {
        riverMode = (OptionalToggle)mode;
        Debug.Log(mode);
    }

    void ValidateDrag (HexCell currentCell)
    {
        for( dragDirection = HexDirection.NE; dragDirection <= HexDirection.NW; dragDirection++)
        {
            if(previousCell.GetNeighbor(dragDirection) == currentCell)
            {
                isDrag = true; return;
            }
        }
        isDrag = false;

    }
}
