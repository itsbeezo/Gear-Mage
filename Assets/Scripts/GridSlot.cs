using NUnit.Framework;
using UnityEngine;



public class GridSlot : MonoBehaviour
{
    public enum ZoneName
    {
        Machine,
        inventory,
        abilities
    }

    public ZoneName Zone => zoneName;

    [Header("Grid Settings")]
    [SerializeField] private int row;
    [SerializeField] private int col;



    public void AssignCordinates(int newRow, int newCol)
    {
        row = newRow;
        col = newCol;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for (int i = row; row < 5; i++)
        {
            Debug.Log(row + i);
            for (int j = col; col <11; j++)
            {
                Debug.Log(col + j);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
