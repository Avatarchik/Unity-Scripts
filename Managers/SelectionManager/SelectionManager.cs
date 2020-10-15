using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    public CameraManager cameraManager;
    public SelectionController selected;
    public bool hasSelected = false;
    // public GameObject hoveredObject;

    //[Header("Hover Selector Texture")]
    //public Texture topLeftBorderHover;
    //public Texture topRightBorderHover;
    //public Texture bottomRightBorderHover;
    //public Texture bottomLeftBorderHover;

    [Header("Selected Texture")]
    public Texture topLeftBorderSelect;
    public Texture topRightBorderSelect;
    public Texture bottomRightBorderSelect;
    public Texture bottomLeftBorderSelect;

    private new Camera camera;
    private float floor = 1000f;
    private float ceiling = 100000f;
    private float verticalOffset = 0f;
    private Vector3 horizontalPosition;

    private void Start()
    {
        camera = cameraManager.mainCamera;
    }

    private void Update()
    {
        if (selected != null)
        {
            Vector3 targetPosition = new Vector3(horizontalPosition.x, horizontalPosition.y + verticalOffset, horizontalPosition.z);
            Debug.DrawLine(selected.transform.position, targetPosition, Color.green);
            Debug.DrawLine(horizontalPosition, targetPosition, Color.green);
            Debug.DrawLine(selected.transform.position, horizontalPosition, Color.green);
            hasSelected = true;
        }
        else
        {
            hasSelected = false;
        }
    }

    public void MouseSetDestination()
    {
        Vector3 targetPosition = new Vector3(horizontalPosition.x, horizontalPosition.y + verticalOffset, horizontalPosition.z);
        targetPosition.y = Mathf.Clamp(targetPosition.y, floor, ceiling);
        if (Input.GetMouseButtonDown(1))
        {
            selected.ship.SetCurrentMoveTarget(targetPosition);
            ClearSelectedObject();
        }
    }

    public void MouseVerticalOffset()
    {
        verticalOffset -= Mathf.Round(Input.mouseScrollDelta.y * 10f);
    }

    public void MouseHorizontalPosition()
    {
        if (selected != null)
        {
            Plane plane = new Plane(Vector3.up, selected.transform.position);
            Ray ray = cameraManager.mainCamera.ScreenPointToRay(Input.mousePosition);
            if (plane.Raycast(ray, out float dist))
            {
                horizontalPosition = ray.GetPoint(dist);
            }
        }
    }

    public void MouseSelect()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hitInfo = cameraManager.GetCameraRay();
            Collider hitCollider = hitInfo.collider;

            if (hitCollider != null)
            {
                GameObject parentObject = hitCollider.transform.parent.gameObject;
                SelectionController selected = parentObject.GetComponent<SelectionController>();
                if (selected != null)
                    SetSelectedObject(selected);
            }
            else
            {
                ClearSelectedObject();
            }
        }
    }

    //public void MouseHover()
    //{
    //    RaycastHit hitInfo = cameraManager.GetCameraRay();
    //    if (hitInfo.transform != null)
    //    {
    //        GameObject hitObject = hitInfo.transform.root.gameObject;
    //        SetHoveredObject(hitObject);
    //    }
    //    else
    //    {
    //        ClearHoveredObject();
    //    }
    //}

    public void SetSelectedObject(SelectionController hit)
    {
        if (hit == null) return;

        verticalOffset = 0f;
        horizontalPosition = Vector3.zero;
        selected = hit;
    }

    //public void SetHoveredObject(GameObject obj)
    //{
    //    if (hoveredObject != null)
    //    {
    //        if (obj == hoveredObject)
    //            return;
    //        else
    //            ClearHoveredObject();
    //    }

    //    hoveredObject = obj;
    //}


    //public void ClearHoveredObject()
    //{
    //    if (hoveredObject == null) return;
    //    hoveredObject = null;
    //}

    public void ClearSelectedObject()
    {
        verticalOffset = 0f;
        horizontalPosition = Vector3.zero;

        if (selected == null) return;
        selected = null;
    }

    private void DrawSelectedIndicator()
    {
        if (selected == null) return;

        Bounds bounds = selected.GetComponentInChildren<Renderer>().bounds;

        Vector3 boundsPoint0 = new Vector3(bounds.center.x + bounds.extents.x, bounds.center.y + bounds.extents.y, bounds.center.z + bounds.extents.z);
        Vector3 boundsPoint1 = new Vector3(bounds.center.x + bounds.extents.x, bounds.center.y + bounds.extents.y, bounds.center.z - bounds.extents.z);
        Vector3 boundsPoint2 = new Vector3(bounds.center.x + bounds.extents.x, bounds.center.y - bounds.extents.y, bounds.center.z + bounds.extents.z);
        Vector3 boundsPoint3 = new Vector3(bounds.center.x + bounds.extents.x, bounds.center.y - bounds.extents.y, bounds.center.z - bounds.extents.z);

        Vector3 boundsPoint4 = new Vector3(bounds.center.x - bounds.extents.x, bounds.center.y + bounds.extents.y, bounds.center.z + bounds.extents.z);
        Vector3 boundsPoint5 = new Vector3(bounds.center.x - bounds.extents.x, bounds.center.y + bounds.extents.y, bounds.center.z - bounds.extents.z);
        Vector3 boundsPoint6 = new Vector3(bounds.center.x - bounds.extents.x, bounds.center.y - bounds.extents.y, bounds.center.z + bounds.extents.z);
        Vector3 boundsPoint7 = new Vector3(bounds.center.x - bounds.extents.x, bounds.center.y - bounds.extents.y, bounds.center.z - bounds.extents.z);

        // Convert corners into screen space
        Vector3[] screenPoints = new Vector3[8];
        screenPoints[0] = camera.WorldToScreenPoint(boundsPoint0);
        screenPoints[1] = camera.WorldToScreenPoint(boundsPoint1);
        screenPoints[2] = camera.WorldToScreenPoint(boundsPoint2);
        screenPoints[3] = camera.WorldToScreenPoint(boundsPoint3);

        screenPoints[4] = camera.WorldToScreenPoint(boundsPoint4);
        screenPoints[5] = camera.WorldToScreenPoint(boundsPoint5);
        screenPoints[6] = camera.WorldToScreenPoint(boundsPoint6);
        screenPoints[7] = camera.WorldToScreenPoint(boundsPoint7);

        Vector2 topLeftPosition = Vector2.zero;
        Vector2 topRightPosition = Vector2.zero;
        Vector2 bottomLeftPosition = Vector2.zero;
        Vector2 bottomRightPosition = Vector2.zero;

        for (int a = 0; a < screenPoints.Length; a++)
        {
            //Top Left
            if (topLeftPosition.x == 0 || topLeftPosition.x > screenPoints[a].x)
                topLeftPosition.x = screenPoints[a].x;
            if (topLeftPosition.y == 0 || topLeftPosition.y > Screen.height - screenPoints[a].y)
                topLeftPosition.y = Screen.height - screenPoints[a].y;

            //Top Right
            if (topRightPosition.x == 0 || topRightPosition.x < screenPoints[a].x)
                topRightPosition.x = screenPoints[a].x;
            if (topRightPosition.y == 0 || topRightPosition.y > Screen.height - screenPoints[a].y)
                topRightPosition.y = Screen.height - screenPoints[a].y;

            //Bottom Left
            if (bottomLeftPosition.x == 0 || bottomLeftPosition.x > screenPoints[a].x)
                bottomLeftPosition.x = screenPoints[a].x;
            if (bottomLeftPosition.y == 0 || bottomLeftPosition.y < Screen.height - screenPoints[a].y)
                bottomLeftPosition.y = Screen.height - screenPoints[a].y;

            //Bottom Right
            if (bottomRightPosition.x == 0 || bottomRightPosition.x < screenPoints[a].x)
                bottomRightPosition.x = screenPoints[a].x;
            if (bottomRightPosition.y == 0 || bottomRightPosition.y < Screen.height - screenPoints[a].y)
                bottomRightPosition.y = Screen.height - screenPoints[a].y;
        }

        GUI.DrawTexture(new Rect(topLeftPosition.x - 16, topLeftPosition.y - 16, 16, 16), topLeftBorderSelect);
        GUI.DrawTexture(new Rect(topRightPosition.x, topRightPosition.y - 16, 16, 16), topRightBorderSelect);
        GUI.DrawTexture(new Rect(bottomLeftPosition.x - 16, bottomLeftPosition.y, 16, 16), bottomLeftBorderSelect);
        GUI.DrawTexture(new Rect(bottomRightPosition.x, bottomRightPosition.y, 16, 16), bottomRightBorderSelect);
    }

    private void OnGUI()
    {
        DrawSelectedIndicator();
    }
}