using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Print))
        {
            ScreenCapture.CaptureScreenshot(Application.persistentDataPath + "/Screenshot"+Time.time.ToString()+".png");
            Debug.Log("A screenshot was set in " + Application.persistentDataPath + "/Screenshot" + Time.time.ToString() + ".png");
        }
    }
}
