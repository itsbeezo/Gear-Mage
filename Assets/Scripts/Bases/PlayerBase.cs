using UnityEngine;

public class PlayerBase : MonoBehaviour
{
    public static PlayerBase instance { get; private set; }

    private void Awake()
    {
        instance = this;
    }
}
