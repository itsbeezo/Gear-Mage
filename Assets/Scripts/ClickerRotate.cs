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
            gear.pulse(new HashSet<GearRotate>());
            Debug.Log("Clicker has entered gear: " + gear.gameObject.name);
        }
    }
        
    private void OnTriggerExit2D(Collider2D collision)
    {
        GearRotate gear = collision.gameObject.GetComponent<GearRotate>();
    if (gear != null && gear == currentTouchingGear)
    {
        currentTouchingGear = null;
            Debug.Log("Clicker has exited gear");
        }
    }

    public void ClickerRotation()
    {
        Clicker.transform.Rotate(0, 0, -90);

        //GearRotate firstGear = Gear.GetComponent<GearRotate>();
        //if (firstGear != null)
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
