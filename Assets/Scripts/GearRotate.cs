using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class GearRotate : MonoBehaviour
{

    public GameObject Clicker;

    public GameObject Gear;

    [Header("Gear Settings")]
    [SerializeField]
    public float RotationSpeed = 50f;

    [Header("Production Settings")]
    [SerializeField]
    public float totalRotations;
    public float rotationPerTick = .25f;
    public float rotationInterval = 1f;
    public float tickProgress = 0f;

    public float rotationStep = 90f;

    private List<GearRotate> neighbors = new List<GearRotate>();

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.E))
        //{
        //    Debug.Log("E was pressed");
        //    GearRotation();
        //}
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Clicker"))
        {
            //print("this is working");
            //GearRotation();
        }
        
        GearRotate otherGear = collision.gameObject.GetComponent<GearRotate>();
        if (otherGear != null && !neighbors.Contains(otherGear))
        {
            neighbors.Add(otherGear);
            Debug.Log(gameObject.name + " added neighbor " + otherGear.gameObject.name + " | neighbor count now: " + neighbors.Count);
        }
    }


    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Gear")
        {
            //print("Gears are overlapping");
            
            
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        GearRotate otherGear = collision.gameObject.GetComponent<GearRotate>();
        if (otherGear != null)
        {
            neighbors.Remove(otherGear);
        }
    }

    public void pulse(HashSet<GearRotate> hasPulsed)
    {
        //if // Something here to only call gears that are touching the clicker, not all gears in the scene.
        //{
            Debug.Log(gameObject.name + " is pulsing. Neighbor count: " + neighbors.Count); 
            if (hasPulsed.Contains(this)) return;
            hasPulsed.Add(this);

            transform.Rotate(0,0, -rotationStep);

                foreach (var neighbor in neighbors)
                {
                    neighbor.pulse(hasPulsed);
                }
        //}else
        //{
            // Put if (hasPulsed) logic here instead of first pulse method, so that it only pulses the gears that are touching the gear that touched the clicker, not all gears in the scene.
        //}
    }

    public void GearRotation()
    { 
        Gear.transform.Rotate(0, 0, -90);
        //Gear.transform.Rotate(0, 0, -45);
    }
}
