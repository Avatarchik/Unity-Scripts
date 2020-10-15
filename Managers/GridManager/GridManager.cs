using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] private int numCellsOnX = 1;
    [SerializeField] private int numCellsOnZ = 1;
    [SerializeField] private float defaultLineWidth = 0.01f;
    [SerializeField] private Color lineColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);

    private void Start()
    {
        float origin = transform.position.y;

        for (int i = -numCellsOnZ; i <= numCellsOnZ; i += 1000)
        {
            if (i + 2 > numCellsOnZ || i == -numCellsOnZ)
                CreateHorizontalLine(new Vector3(-numCellsOnX, origin, i), new Vector3(numCellsOnX, origin, i), 5);
            else
                CreateHorizontalLine(new Vector3(-numCellsOnX, origin, i), new Vector3(numCellsOnX, origin, i));
        }
        for (int i = -numCellsOnX; i <= numCellsOnX; i += 1000)
        {
            if (i + 2 > numCellsOnX || i == -numCellsOnX)
                CreateHorizontalLine(new Vector3(i, origin, -numCellsOnZ), new Vector3(i, origin, numCellsOnZ), 5);
            else
                CreateHorizontalLine(new Vector3(i, origin, -numCellsOnZ), new Vector3(i, origin, numCellsOnZ));
        }
    }

    private void CreateHorizontalLine(Vector3 start, Vector3 end, float lineWidthModifier = 1f)
    {
        LineRenderer lr = new GameObject("Grid Line").AddComponent<LineRenderer>();
        lr.transform.parent = transform;
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.startWidth = defaultLineWidth * lineWidthModifier;
        lr.endWidth = defaultLineWidth * lineWidthModifier;
        lr.positionCount = 2;
        lr.startColor = lineColor;
        lr.endColor = lineColor;
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
    }
}
