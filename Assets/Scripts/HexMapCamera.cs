using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexMapCamera : MonoBehaviour
{

    Transform swivel, stick;

    float zoom = 1f;
    float mouseSwivel;
    float rotationAngle;

    public float stickMinZoom, stickMaxZoom;
    public float swivelMinZoom, swivelMaxZoom;
    public float moveSpeedMinZoom, moveSpeedMaxZoom;
    public float rotationSpeed;

    public HexGrid grid;

    private void Awake()
    {
        swivel = transform.GetChild(0);
        stick = swivel.GetChild(0);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float zoomDelta = Input.GetAxis("Mouse ScrollWheel");
        if(zoomDelta != 0f)
        {
            AdjustZoom(zoomDelta);
        }

        float rotationDelta = Input.GetAxis("Rotation");
        if(rotationDelta != 0f)
        {
            AdjustRotation(rotationDelta);
        }

        if (Input.GetMouseButton(2))
        {
            float mouseRotationDelta = Input.GetAxis("Mouse X");
            if(mouseRotationDelta != 0f)
            {
                AdjustMouseRotation(mouseRotationDelta);
            }

            float mouseSwivelDelta = Input.GetAxis("Mouse Y");
            if(mouseSwivelDelta != 0f)
            {
                AdjustSwivel(mouseSwivelDelta);
            }
        }


        float xDelta = Input.GetAxis("Horizontal");
        float zDelta = Input.GetAxis("Vertical");
        if(xDelta != 0f || zDelta != 0f) 
        {
            AdjustPosition(xDelta, zDelta);
        }
    }

    void AdjustZoom(float delta)
    {
        zoom = Mathf.Clamp01(zoom + delta);

        float distance = Mathf.Lerp(stickMinZoom, stickMaxZoom, zoom);
        stick.localPosition = new Vector3(0f, 0f, distance);
    }

    void AdjustSwivel(float delta)
    {
        mouseSwivel += delta * rotationSpeed * 3 * Time.deltaTime;
        if (mouseSwivel < swivelMaxZoom) mouseSwivel = swivelMaxZoom;
        else if (mouseSwivel > swivelMinZoom) mouseSwivel = swivelMinZoom;
        swivel.localRotation = Quaternion.Euler(mouseSwivel, 0f, 0f);
    }

    void AdjustPosition(float xDelta, float zDelta)
    {
        Vector3 direction = transform.localRotation * new Vector3(xDelta, 0f, zDelta).normalized;
        float damping = Mathf.Max(Mathf.Abs(xDelta), Mathf.Abs(zDelta));
        float distance = Mathf.Lerp(moveSpeedMinZoom, moveSpeedMaxZoom, zoom) * damping * Time.deltaTime;

        Vector3 position = transform.localPosition;
        position += direction * distance;
        transform.localPosition = position;

        transform.localPosition = ClampPosition(position);
    }

    Vector3 ClampPosition(Vector3 position)
    {
        float xMax = (grid.chunkCountX * HexMetrics.chunkSizeX - 0.5f) * (2f * HexMetrics.innerRadius);
        position.x = Mathf.Clamp(position.x, 0f, xMax);

        float zMax = (grid.chunkCountZ * HexMetrics.chunkSizeZ - 1f) * (1.5f * HexMetrics.outerRadius);
        position.z = Mathf.Clamp(position.z, 0f, zMax);

        return position;
    }

    void AdjustRotation(float delta)
    {
        rotationAngle += delta * rotationSpeed * Time.deltaTime;
        if (rotationAngle < 0f) rotationAngle += 360f;
        else if (rotationAngle > 360f) rotationAngle -= 360f;
        transform.localRotation = Quaternion.Euler(0f, rotationAngle, 0f);
    }
    void AdjustMouseRotation(float delta)
    {
        rotationAngle += delta * rotationSpeed * 3 * Time.deltaTime;
        if (rotationAngle < 0f) rotationAngle += 360f;
        else if (rotationAngle > 360f) rotationAngle -= 360f;
        transform.localRotation = Quaternion.Euler(0f, rotationAngle, 0f);
    }
}
