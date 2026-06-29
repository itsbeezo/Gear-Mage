using UnityEngine;
using UnityEngine.Rendering;
using System.Collections.Generic;
using System.Linq; 

public class ClickerRotate : MonoBehaviour
{
    public GameObject Clicker;

    public GameObject Gear;

    private float tickTimer = 0;

    [Header("Gear Settings")]
    [SerializeField]
    public float RotationSpeed = 50f;

    [Header("Production Settings")]
    [SerializeField]
    public float totalRotations;
    public float rotationPerTick = .25f;
    public float rotationInterval = 1f;
    public float tickProgress = 0f;


    // Update is called once per frame
    void Update()
    {

        tickTimer += Time.deltaTime;

        tickProgress = Mathf.Clamp01(tickTimer / rotationInterval);

        if (tickTimer >= rotationInterval)
        {
            tickTimer = 0;
            totalRotations += 1;
            ClickerRotation();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            ClickerRotation();
        }
        
    }

    private GearRotate currentTouchingGear = null;

    private void OnTriggerEnter2D(Collider2D collision)
    {

        GearRotate gear = collision.gameObject.GetComponent<GearRotate>();
        if (gear != null)
        {
            currentTouchingGear = gear;
        }

        //if (collision.gameObject.name == "Clicker")
        //{
        //    //Debug.Log("Rotated Gear");
        //    //GearRotation();
        //}
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Clicker")
        {
            //Debug.Log("Stay");
        }
     }
        
    private void OnTriggerExit2D(Collider2D collision)
    {
        GearRotate gear = collision.gameObject.GetComponent<GearRotate>();
    if (gear != null && gear == currentTouchingGear)
    {
        currentTouchingGear = null;
    }

        //if (collision.gameObject.name == "Clicker")
        //{
        //    //Debug.Log("Exit");
        //}
    }

    public void ClickerRotation()
    {

        Clicker.transform.Rotate(0, 0, -90);


        if (currentTouchingGear != null)
        {
            currentTouchingGear.pulse(new HashSet<GearRotate>());
        }
        //GearRotate firstGear = Gear.GetComponent<GearRotate>();
        //if (firstGear != null)n
        //{
        //    Debug.Log("Clicker is pulsing: " + firstGear.gameObject.name);
        //    firstGear.pulse(new HashSet<GearRotate>());
        //}
    }

    public void GearRotation()
    {
        Gear.transform.Rotate(0, 0, -90);
    }
}
