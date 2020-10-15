using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [Header("Camera Positioning")]
    public Vector3 cameraOffset = new Vector3(0f, 25f, 20f);
    public float lookAtOffset = 3f;

    [Header("Move Controls")]
    public float inAndOutSpeed = 10f;
    public float lateralSpeed = 10f;
    public float rotateSpeed = 45f;
    public float scrollSensitivity = 100f;

    [Header("Movement Bounds")]
    public Vector2 minBounds, maxBounds;

    [Header("Zoom Controls")]
    public float zoomSpeed = 1f;
    public float minZoomIn = 1f;
    public float maxZoomOut = 40f;
    public float startingZoom = 5f;

    public Camera mainCamera;

    private float currentZoomLevel;
    private Vector2Int screen;
    private Vector3 normalizedCameraPosition;

    private void Awake()
    {
        screen = new Vector2Int(Screen.width, Screen.height);

        mainCamera = GetComponentInChildren<Camera>();
        mainCamera.transform.localPosition = new Vector3(Mathf.Abs(cameraOffset.x), Mathf.Abs(cameraOffset.y), -Mathf.Abs(cameraOffset.z));

        normalizedCameraPosition = new Vector3(Mathf.Abs(cameraOffset.x), Mathf.Abs(cameraOffset.y), -Mathf.Abs(cameraOffset.z));
        normalizedCameraPosition.Normalize();
        currentZoomLevel = startingZoom;

        mainCamera.transform.LookAt(transform.position + Vector3.up * lookAtOffset);
    }

    public void KeyboardPan()
    {
        if (Input.GetKey(KeyCode.W))
            Pan(Vector3.forward);
        if (Input.GetKey(KeyCode.S))
            Pan(-Vector3.forward);
        if (Input.GetKey(KeyCode.A))
            Pan(-Vector3.right);
        if (Input.GetKey(KeyCode.D))
            Pan(Vector3.right);
    }

    public void KeyboardRotate()
    {
        if (Input.GetKey(KeyCode.Q))
            Rotate(1f);
        if (Input.GetKey(KeyCode.E))
            Rotate(-1f);
    }

    public void KeyboardZoom()
    {
        if (Input.GetKey(KeyCode.X))
            ZoomIn(scrollSensitivity);
        if (Input.GetKey(KeyCode.Z))
            ZoomOut(scrollSensitivity);
    }

    public void MouseZoom()
    {
        if (Input.mouseScrollDelta.y < 0)
            ZoomIn(scrollSensitivity);
        else if (Input.mouseScrollDelta.y > 0)
            ZoomOut(scrollSensitivity);
    }

    public void MousePan()
    {
        Vector3 mp = Input.mousePosition;
        bool isMousePosValid =
            mp.y <= screen.y * 1.05f &&
            mp.y >= screen.y * -0.05f &&
            mp.x <= screen.x * 1.05f &&
            mp.x >= screen.x * -0.05f;

        if (!isMousePosValid) return;

        if (mp.y > screen.y * 0.95f)
            Pan(Vector3.forward);
        else if (mp.y < screen.y * 0.05f)
            Pan(-Vector3.forward);

        if (mp.x > screen.x * 0.95f)
            Pan(Vector3.right);
        else if (mp.x < screen.x * 0.05f)
            Pan(-Vector3.right);
    }

    public void MouseRotate()
    {
        // TODO implement mouse rotate
    }

    public RaycastHit GetCameraRay()
    {
        RaycastHit hitInfo;
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hitInfo))
            return hitInfo;

        return hitInfo;
    }

    private void Pan(Vector3 panVector)
    {
        if (panVector == Vector3.zero) return;

        Vector3 speedModFramePan = new Vector3(panVector.x * lateralSpeed, panVector.y, panVector.z * inAndOutSpeed);
        transform.position += transform.TransformDirection(speedModFramePan) * Time.deltaTime;
        LockPositionInBounds();
    }

    private void Rotate(float rotateAmount)
    {
        transform.Rotate(Vector3.up, rotateAmount * rotateSpeed * Time.deltaTime);
    }

    private void ZoomIn(float zoomAmount)
    {
        if (currentZoomLevel <= minZoomIn) return;

        currentZoomLevel = Mathf.Max(currentZoomLevel - zoomAmount, minZoomIn);
        mainCamera.transform.localPosition = normalizedCameraPosition * currentZoomLevel;
    }

    private void ZoomOut(float zoomAmount)
    {
        if (currentZoomLevel >= maxZoomOut) return;

        currentZoomLevel = Mathf.Min(currentZoomLevel + zoomAmount, maxZoomOut);
        mainCamera.transform.localPosition = normalizedCameraPosition * currentZoomLevel;
    }

    private void LockPositionInBounds()
    {
        transform.position = new Vector3(
            Mathf.Clamp(transform.position.x, minBounds.x, maxBounds.x),
            transform.position.y,
            Mathf.Clamp(transform.position.z, minBounds.y, maxBounds.y));
    }
}
