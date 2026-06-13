using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

public class GearRotate : MonoBehaviour
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
        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("E was pressed");
            GearRotation();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Clicker")
        {
            GearRotation();
        }
        if (collision.gameObject.tag == "Gears")
        {
            Debug.Log("Gears entered");
            GearRotation();
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Clicker")
        {
            //Debug.Log("Stay");
        }
        if (collision.gameObject.tag == "Gears")
        {
            //Debug.Log("Gears are overlapping");
            
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Clicker")
        {
            Debug.Log("Exit");
        }
    }

    public void ClickerRotation()
    {
       
        Clicker.transform.Rotate(0, 0, -90);
    }

    public void GearRotation()
    {
        Gear.transform.Rotate(0, 0, -90);
    }
}
