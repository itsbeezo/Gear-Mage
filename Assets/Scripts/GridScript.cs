using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GridScript : MonoBehaviour
{
    //public enum GameObjectType
    //{
    //    Gear,
    //    Clicker
    //}

    //public GameObjectType Type;

    public GameObject ObjectToPlace;
    //private GameObject ClickerToPlace;
    private GameObject OldObject;
    private HashSet<Vector3> OccupiedPosition = new HashSet<Vector3>();
    public float gridSize = 0.3f;


    private void Start()
    {
        createGhostObject();
    }

    private void Update()
    {
        UpdateGhostObject();

        if (Input.GetMouseButtonDown(0))
        {
            placeObject();
        }

    }

    void createGhostObject()
    {
        OldObject = Instantiate(ObjectToPlace);
        OldObject.GetComponent<Collider>().enabled = false;

        Renderer[] renderers = OldObject.GetComponentsInChildren<Renderer>();

        foreach (Renderer renderer in renderers)
        {
            Material mat = renderer.material;
            Color color = mat.color;
            color.a = 0.5f; // Set the alpha value to 0.5 for transparency
            mat.color = color;

            mat.SetFloat("_Mode", 2); // Set the rendering mode to Transparent
            mat.SetInt("_SRCBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            mat.SetInt("_ZWrite", 0);
            mat.DisableKeyword("_ALPHATEST_ON");
            mat.EnableKeyword("_ALPHABLEND_ON");
            mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            mat.renderQueue = 3000; // Set the render queue to Transparent

        }

    }


    private void UpdateGhostObject()
    {
      Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Vector2 position = hit.point;

            Vector2 snappedPosition = new Vector2(
                Mathf.Round(position.x / gridSize) * gridSize,
                Mathf.Round(position.y / gridSize) * gridSize
                
            );
            
            OldObject.transform.position = snappedPosition;

            if (OccupiedPosition.Contains(snappedPosition))
            {
                setGhostColor(Color.red);
            }
            else
            {
                setGhostColor(Color.green);
            }
        }
    }

    void setGhostColor(Color color)
    {
        Renderer[] renderers = OldObject.GetComponentsInChildren<Renderer>();

        foreach (Renderer renderer in renderers)
        {
            Material mat = renderer.material;
            mat.color = color;

        }

    }

    void placeObject()
    {
        Vector3 placement= OldObject.transform.position;

        if (!OccupiedPosition.Contains(placement))
        {
            Instantiate(ObjectToPlace, placement, Quaternion.identity);

            OccupiedPosition.Add(placement);
        }
    }
}



