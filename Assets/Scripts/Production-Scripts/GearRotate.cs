using System.Collections.Generic;
using UnityEngine;

public class GearRotate : MonoBehaviour
{

    public GameObject Clicker;

    public GameObject Gear;

    private float tickTimer = 0;


    [Header("Gear Settings")]
    [SerializeField]
    public float RotationSpeed = 50f;
    public float rotationStep = 90f;

    [Header("Production Settings")]
    [SerializeField]
    public float totalRotations;
    public float rotationPerTick = .25f;
    public float gearspeed;
    public float tickProgress = 0f;

    private List<GearRotate> neighbors = new List<GearRotate>();

    // Update is called once per frame
    void Update()
    {

        // if (GearRotation())
        // {
            gearspeed = 2f;
            tickTimer += Time.deltaTime;

            tickProgress = Mathf.Clamp01(tickTimer / gearspeed);
        
            if (tickTimer >= gearspeed)
            {
            tickTimer = 0;
            totalRotations += rotationPerTick;
            }
        // }
        //if (Input.GetKeyDown(KeyCode.E))
        //{
        //    Debug.Log("E was pressed");
        //    GearRotation();
        //}
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GearRotate neighbor = collision.gameObject.GetComponent<GearRotate>();
        if (neighbor != null && !neighbors.Contains(neighbor))
        {
            neighbors.Add(neighbor);
            //Debug.Log(gameObject.name + " added neighbor " + neighbor.gameObject.name + " | neighbor count now: " + neighbors.Count);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        GearRotate neighbor = collision.gameObject.GetComponent<GearRotate>();
        if (neighbor != null)
        {
            neighbors.Remove(neighbor);
        }
    }

    public void pulse(HashSet<GearRotate> hasPulsed)
    {
        Debug.Log(gameObject.name + " is pulsing. Neighbor count: " + neighbors.Count); 
        if (hasPulsed.Contains(this)) return;
        hasPulsed.Add(this);

        transform.Rotate(0,0, -rotationStep);

            foreach (var neighbor in neighbors)
            {
                neighbor.pulse(hasPulsed);
            }
    }

    public void GearRotation()
    { 
        Gear.transform.Rotate(0, 0, -90);
        //Gear.transform.Rotate(0, 0, -45);
    }
}
