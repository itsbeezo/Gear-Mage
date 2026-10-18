using UnityEngine;
public class TriggerProbe : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other) { Debug.Log(name + " got OnTriggerEnter2D from " + other.name); }
}