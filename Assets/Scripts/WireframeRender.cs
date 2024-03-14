using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WireframeRender : MonoBehaviour
{
    [SerializeField]
    public bool onWireframe;
    private void OnPreRender()
    {
        if (onWireframe) GL.wireframe = true;
    }
    private void OnPostRender()
    {
        if(onWireframe) GL.wireframe = false;
    }
}
