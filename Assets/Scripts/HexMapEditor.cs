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
        if (Physics.Raycast(inputRay, out hit)) { EditCell(hexGrid.GetCell(hit.point)); }
    }
    public void SetElevation(float elevation)
    {
        activeElevation = (int)elevation;
    }
    void EditCell(HexCell cell)
    {
        if(applyColor)
        {
            cell.Color = activeColor;
        }
        if (applyElevation)
        {
            cell.Elevation = activeElevation;
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
