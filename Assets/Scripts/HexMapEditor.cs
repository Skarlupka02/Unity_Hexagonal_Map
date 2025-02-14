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
    int activeElevation;

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

        cell.Color = activeColor;
        cell.Elevation = activeElevation;
        //hexGrid.Refresh();
    }
    public void SelectColor(int index)
    {
        activeColor = colors[index];
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }
}
