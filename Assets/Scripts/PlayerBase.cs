using UnityEngine;
using UnityEngine.UI;

public class PlayerBase : MonoBehaviour
{
    public static PlayerBase instance;
    private void Start()
    {
        instance = this;
    }

}
