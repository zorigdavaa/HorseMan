/* 
    ------------------- Code Monkey -------------------

    Thank you for downloading this package
    I hope you find it useful in your projects
    If you have any questions let me know
    Cheers!

               unitycodemonkey.com
    --------------------------------------------------
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Grid<TGridObject>
{

    public event EventHandler<OnGridObjectChangedEventArgs> OnGridObjectChanged;
    public class OnGridObjectChangedEventArgs : EventArgs
    {
        public int x;
        public int y;
    }
    Transform parent;
    private int width;
    private int height;
    private float cellSize;
    public Vector3 originPosition;
    private TGridObject[,] gridArray;

    public Grid(int width, int height, float cellSize, Vector3 originPosition, Func<Grid<TGridObject>, int, int, TGridObject> createGridObject, Transform parent)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.originPosition = originPosition;
        this.parent = parent;

        gridArray = new TGridObject[width, height];

        for (int x = 0; x < gridArray.GetLength(0); x++)
        {
            for (int y = 0; y < gridArray.GetLength(1); y++)
            {
                gridArray[x, y] = createGridObject(this, x, y);
            }
        }

        // bool showDebug = true;
        // if (showDebug)
        // {
        //     TextMeshPro[,] debugTextArray = new TextMeshPro[width, height];

        //     for (int x = 0; x < gridArray.GetLength(0); x++)
        //     {
        //         for (int y = 0; y < gridArray.GetLength(1); y++)
        //         {
        //             debugTextArray[x, y] = UtilsClass.CreateWorldText(gridArray[x, y]?.ToString(), null, GetLocalPLacement(x, y), 4, Color.white);
        //             Debug.DrawLine(GetLocalPLacement(x, y), GetWorldPosition(x, y + 1), Color.white, 100f);
        //             Debug.DrawLine(GetLocalPLacement(x, y), GetWorldPosition(x + 1, y), Color.white, 100f);
        //         }
        //     }
        //     Debug.DrawLine(GetLocalPLacement(0, height), GetLocalPLacement(width, height), Color.white, 100f);
        //     Debug.DrawLine(GetLocalPLacement(width, 0), GetLocalPLacement(width, height), Color.white, 100f);

        //     OnGridObjectChanged += (object sender, OnGridObjectChangedEventArgs eventArgs) =>
        //     {
        //         debugTextArray[eventArgs.x, eventArgs.y].text = gridArray[eventArgs.x, eventArgs.y]?.ToString();
        //     };
        // }
    }

    public int GetWidth()
    {
        return width;
    }

    public int GetHeight()
    {
        return height;
    }

    public float GetCellSize()
    {
        return cellSize;
    }

    public Vector3 GetWorldPosition(int x, int y)
    {
        return new Vector3(x, y) * cellSize + originPosition;
    }
    // public Vector3 GetWorldPlacement(int x, int y)
    // {
    //     return GetWorldPosition(x, y) + new Vector3(cellSize, cellSize) * .5f;
    // }
    public Vector3 GetWorldPLacement(int x, int y)
    {
        // return parent.TransformPoint(new Vector3(x, y) * cellSize + new Vector3(cellSize, cellSize) * .5f);
        return parent.TransformPoint(GetLocalPLacement(x, y));
    }
    public Vector3 GetLocalPLacement(int x, int y)
    {
        return (new Vector3(x, y) * cellSize + new Vector3(cellSize, cellSize) * .5f);
    }

    private void GetXY(Vector3 worldPosition, out int x, out int y)
    {
        x = Mathf.FloorToInt((worldPosition - originPosition).x / cellSize);
        y = Mathf.FloorToInt((worldPosition - originPosition).y / cellSize);
    }

    public void SetGridObject(int x, int y, TGridObject value)
    {
        if (x >= 0 && y >= 0 && x < width && y < height)
        {
            gridArray[x, y] = value;
            if (OnGridObjectChanged != null) OnGridObjectChanged(this, new OnGridObjectChangedEventArgs { x = x, y = y });
        }
    }

    public void TriggerGridObjectChanged(object sender, int x, int y)
    {
        if (OnGridObjectChanged != null) OnGridObjectChanged(sender, new OnGridObjectChangedEventArgs { x = x, y = y });
    }

    public void SetGridObject(Vector3 worldPosition, TGridObject value)
    {
        int x, y;
        GetXY(worldPosition, out x, out y);
        SetGridObject(x, y, value);
    }

    public TGridObject GetGridObject(int x, int y)
    {
        if (x >= 0 && y >= 0 && x < width && y < height)
        {
            return gridArray[x, y];
        }
        else
        {
            return default(TGridObject);
        }
    }

    public TGridObject GetGridObject(Vector3 worldPosition)
    {
        int x, y;
        GetXY(worldPosition, out x, out y);
        return GetGridObject(x, y);
    }

}
