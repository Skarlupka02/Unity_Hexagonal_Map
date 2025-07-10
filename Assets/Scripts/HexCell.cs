using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexCell : MonoBehaviour
{
    public HexCoordinates coordinates;
    public bool hasIncomingRiver, hasOutgoingRiver;
    public HexDirection incomingRiver, outgoingRiver;
    public RectTransform uiRect;
    public HexGridChunk chunk;

    Color color;

    int elevation = int.MinValue;
    int waterLevel;


    [SerializeField]
    bool[] roads;
    [SerializeField]
    HexCell[] neighbors;

    public Color Color
    {
        get { return color; } 
        set { 
            if(color == value) { return; }
            color = value;
            Refresh();
        }
    }

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
            if (elevation == value) { return; }

            elevation = value;
            Vector3 position = transform.localPosition;
            position.y = value * HexMetrics.elevationStep;
            position.y += (HexMetrics.SampleNoise(position).y * 2f - 1f) * HexMetrics.elevationPerturbStrength;
            transform.localPosition = position;

            Vector3 uiPosition = uiRect.localPosition;
            //uiPosition.z = elevation * -HexMetrics.elevationStep;
            uiPosition.z = -position.y; ;
            uiRect.localPosition = uiPosition;

            if (hasOutgoingRiver && elevation < GetNeighbor(outgoingRiver).elevation) { RemoveOutgoingRiver(); }
            if (hasIncomingRiver && elevation > GetNeighbor(incomingRiver).elevation) { RemoveIncomingRiver(); }

            //Удаляем дорогу если высота стала больше 1
            for (int i = 0; i < roads.Length; i++)
            {
                if (roads[i] && GetElevationDifference((HexDirection)i) > 1)
                {
                    SetRoad(i, false);
                }
            }

            Refresh();
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
    
    void Refresh()
    {
        if (chunk)
        {
            chunk.Refresh();
            for (int i = 0; i < neighbors.Length; i++)
            {
                HexCell neighbor = neighbors[i];
                if(neighbor != null && neighbor.chunk != chunk)
                {
                    neighbor.chunk.Refresh();
                }
            }
        }
    }
    void RefreshSelfOnly()
    {
        chunk.Refresh();
    }

    /// <summary>
    /// Возвращает разницу высот в определённом направлении
    /// </summary>
    /// <param name="direction">Направление</param>
    /// <returns></returns>
    public int GetElevationDifference(HexDirection direction)
    {
        int difference = elevation - GetNeighbor(direction).elevation;
        return difference >= 0 ? difference : -difference;
    }

    #region River methods
    public bool HasIncomingRiver
    {
        get
        {
            return hasIncomingRiver;
        }
    }
    public bool HasOutgoingRiver
    {
        get
        {
            return hasOutgoingRiver;
        }
    }
    public HexDirection IncomingRiver
    {
        get
        {
            return incomingRiver;
        }
    }
    public HexDirection OutgoingRiver
    {
        get
        {
            return outgoingRiver;
        }
    }
    public bool HasRiver
    {
        get
        {
            return hasIncomingRiver || hasOutgoingRiver;
        }
    }
    public bool HasRiverBeginOrEnd
    {
        get
        {
            return hasIncomingRiver != hasOutgoingRiver;
        }
    }
    public bool HasRiverThroughEdge (HexDirection direction)
    {
        return
            hasIncomingRiver && incomingRiver == direction ||
            hasOutgoingRiver && outgoingRiver == direction;
    }
    public void RemoveOutgoingRiver()
    {
        if (!hasOutgoingRiver)
        {
            return;
        }
        hasOutgoingRiver = false;
        RefreshSelfOnly();

        HexCell neighbor = GetNeighbor(outgoingRiver);
        neighbor.hasIncomingRiver = false;
        neighbor.RefreshSelfOnly();

    }
    public void RemoveIncomingRiver()
    {
        if (!hasIncomingRiver)
        {
            return;
        }
        hasIncomingRiver = false;
        RefreshSelfOnly();

        HexCell neighbor = GetNeighbor (incomingRiver);
        neighbor.hasOutgoingRiver = false;
        neighbor.RefreshSelfOnly();
    }
    public void RemoveRiver()
    {
        RemoveIncomingRiver();
        RemoveOutgoingRiver();
    }
    public void SetOutgoingRiver(HexDirection direction)
    {
        if(hasOutgoingRiver && outgoingRiver == direction)
        {
            return;
        }

        HexCell neighbor = GetNeighbor(direction);
        if (!neighbor || elevation < neighbor.elevation)
        {
            return;
        }

        RemoveOutgoingRiver();
        if (hasIncomingRiver && incomingRiver == direction)
        {
            RemoveIncomingRiver();
        }

        hasOutgoingRiver = true;
        outgoingRiver = direction;
        //RefreshSelfOnly ();

        neighbor.RemoveIncomingRiver();
        neighbor.hasIncomingRiver = true;
        neighbor.incomingRiver = direction.Opposite();
        //neighbor.RefreshSelfOnly();

        //Удаление дороги на пути реки
        SetRoad((int)direction, false);
    }

    public float StreamBedY
    {
        get
        {
            return (elevation + HexMetrics.streamBedElevantionOffset) * HexMetrics.elevationStep;
        }
    }

    public float RiverSurfaceY
    {
        get
        {
            return (elevation + HexMetrics.waterElevationOffset) * HexMetrics.elevationStep;
        }
    }

    public HexDirection RiverBeginOrEndDirection
    {
        get
        {
            return hasIncomingRiver ? incomingRiver : outgoingRiver;
        }
    }

    #endregion

    #region Roads methods

    /// <summary>
    /// Проверяем имеет ли ячейка дорогу в определённом направлении
    /// </summary>
    /// <param name="direction">Направление</param>
    /// <returns></returns>
    public bool HasRoadThroughEdge(HexDirection direction)
    {
        return roads[(int)direction];
    }

    /// <summary>
    /// Проверяем есть ли хоть одна дорога в ячейке
    /// </summary>
    public bool HasRoads
    {
        get
        {
            for (int i = 0; i < roads.Length; i++)
            {
                if (roads[i])
                {
                    return true;
                }
            }
            return false;
        }
    }

    /// <summary>
    /// Удаление всех дорог из ячейки
    /// </summary>
    public void RemoveRoads()
    {
        for(int i = 0;i < neighbors.Length;i++)
        {
            if (roads[i])
            {
                SetRoad(i, false);
                
            }
        }
    }

    /// <summary>
    /// Частный метод удаления и добавления дороги
    /// </summary>
    /// <param name="index">Ячейка по направлению</param>
    /// <param name="state">Удаление или добавление</param>
    void SetRoad(int index, bool state)
    {
        roads[index] = state;
        neighbors[index].roads[(int)((HexDirection)index).Opposite()] = state; //Отключаем дороги в соседних ячейках
        //Обновляем каждую из ячеек бех их соседей
        neighbors[index].RefreshSelfOnly();
        RefreshSelfOnly();
    }

    /// <summary>
    /// Добавление дороги
    /// </summary>
    /// <param name="direction">Направление</param>
    public void AddRoad(HexDirection direction)
    {
        //Проверяем есть ли место для дороги и дорога не идёт на большую высоту
        if (!roads[(int)direction] && !HasRiverThroughEdge(direction) && GetElevationDifference(direction) <= 1)
        {
            SetRoad((int)direction, true);
        }
    }

    #endregion

    #region Water methods

    public int WaterLevel
    {
        get
        {
            return waterLevel;
        }
        set
        {
            if(waterLevel == value)
            {
                return;
            }
            waterLevel = value;
            Refresh();
        }
    }

    public bool IsUnderwater
    {
        get
        {
            return waterLevel > elevation;
        }
    }

    public float WaterSurfaceY
    {
        get
        {
            return (waterLevel + HexMetrics.waterElevationOffset) * HexMetrics.elevationStep;
        }
    }

    #endregion

}
