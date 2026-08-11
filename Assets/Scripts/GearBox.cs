using UnityEngine;

public class GearBox : MonoBehaviour
{
    [SerializeField] private int xIndex;
    [SerializeField] private int yIndex;

    public int GetXIndex()
    {
        return xIndex;
    }
    public int GetYIndex()
    {
        return yIndex;
    }
}
