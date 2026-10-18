using UnityEngine;
using UnityEngine.UI;

public class EnemyBase : MonoBehaviour
{
    public static EnemyBase instance;
    private void Start()
    {
        instance = this;
    }
}
