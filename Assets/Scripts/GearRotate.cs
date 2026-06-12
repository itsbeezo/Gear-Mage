using UnityEngine;

public class GearRotate : MonoBehaviour
{

    public GameObject Clicker;

    public GameObject Gear;

    [Header("Gear Settings")]
    [SerializeField]
    public float RotationSpeed = 50;


    private void Awake()
    {
        //gear =GameObject.
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name == "Clicker")
        {
            GearRotation();
        }
        if (collision.gameObject.name == "Gear")
        {
            GearRotation();
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.name == "Clicker")
        {
            Debug.Log("Stay");
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.name == "Clicker")
        {
            Debug.Log("Exit");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Clicker")
        {
            GearRotation();
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Clicker")
        {
            Debug.Log("Stay");
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
        Debug.Log("Rotation 90 degrees");
        Clicker.transform.Rotate(0, 0, -90);
    }

    public void GearRotation()
    {
        Debug.Log("Gear Rotated");
        Gear.transform.Rotate(0, 0, -90);
    }
}
