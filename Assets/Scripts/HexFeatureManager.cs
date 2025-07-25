using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HexFeatureManager : MonoBehaviour
{

    public Transform[] urbanPrefabs;
    Transform container;

    public void Clear() 
    {
        if (container)
        {
            Destroy(container.gameObject);
        }
        container = new GameObject("Features Container").transform;
        container.SetParent(transform, false);
    }

    public void Apply() { }

    public void AddFeature(HexCell cell, Vector3 position) 
    {
        HexHash hash = HexMetrics.SampleHashGrid(position);
        //if(hash.a >= cell.UrbanLevel * 0.25f) return;
        // Transform instance = Instantiate(urbanPrefabs[cell.UrbanLevel - 1]);
        Transform prefab = PickPrefab(cell.UrbanLevel, hash.a);
        if (!prefab) return;
        Transform instance = Instantiate(prefab);
        position.y += instance.localScale.y * 0.5f;
        instance.localPosition = HexMetrics.Perturb(position);
        instance.localRotation = Quaternion.Euler(0f, 360f * hash.b, 0f);
        instance.SetParent(container, false);
    }

    Transform PickPrefab (int level, float hash)
    {
        if(level > 0)
        {
            float[] thresholds = HexMetrics.GetFeatureThresholds(level - 1);
            for (int i = 0; i < thresholds.Length; i++) { 
                if(hash < thresholds[i]) return urbanPrefabs[i];
            }
        }
        return null;
    }

}
