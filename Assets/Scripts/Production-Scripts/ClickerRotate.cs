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

        if (UnitManager.instance == null) return;
        if (GameManager.instance == null || GameManager.instance.GetState() != GameManager.State.Normal) return;



        tickTimer += Time.deltaTime;

        tickProgress = Mathf.Clamp01(tickTimer / rotationInterval);

        if (tickTimer >= rotationInterval)
        {
            tickTimer = 0;
            totalRotations += rotationPerTick;
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
            gear.Pulse();
        }
    }
        
    private void OnTriggerExit2D(Collider2D collision)
    {
        GearRotate gear = collision.gameObject.GetComponent<GearRotate>();
    if (gear != null && gear == currentTouchingGear)
    {
        currentTouchingGear = null;
    }
    }

    public void ClickerRotation()
    {
        Clicker.transform.Rotate(0, 0, -90);

        // Pulsing currentTouchingGear here was redundant with OnTriggerEnter2D:
        // this tick fires on its own independent clock, so any tick landing
        // while contact was still ongoing from a prior Enter re-pulsed the same
        // gear a second time for what was really one pass - the "double spin" bug.
        // Enter already pulses exactly once per genuine contact start; this method's
        // only remaining job is spinning the Clicker itself, which is what sweeps
        // it into new gears (producing the next Enter) in the first place.

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
