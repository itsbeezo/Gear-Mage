using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    public static EnemyBase instance { get; private set; }

    private void Awake()
    {
        instance = this;
    }
}
