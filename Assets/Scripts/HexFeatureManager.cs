using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexFeatureManager : MonoBehaviour
{

    public Transform featurePrefab;

    public void Clear() { }

    public void Apply() { }

    public void AddFeature(Vector3 position) 
    {
        Transform instance = Instantiate(featurePrefab);
        position.y += instance.localScale.y * 0.5f;
        instance.localPosition = HexMetrics.Perturb(position);
    }

    public void RemoveFeature() { }

}
