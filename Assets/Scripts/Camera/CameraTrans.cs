using UnityEngine;

public class CameraTrans : MonoBehaviour
{

    public static CameraTrans instance { get; private set; }

    public Camera LevelCamera;
    public Camera GearBoxCamera;

    public float smoothspeed;

    private Vector3 targetPos, newpos;

    public Vector3 minPos, maxPos;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        LevelCamera.enabled = false;
        GearBoxCamera.enabled = true;
    }

    public void SwtichCamera()
    {
        if (LevelCamera.enabled == true)
        {
            LevelCamera.enabled = false;
            GearBoxCamera.enabled = true;
        }
        else if (GearBoxCamera.enabled == true)
        {
            GearBoxCamera.enabled = false;
            LevelCamera.enabled = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SwtichCamera();
        }
    }
}
