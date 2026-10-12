using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class LidOpen : MonoBehaviour
{
   
   public SpriteRenderer spriteRenderer;
   public Sprite newSprite;
   private Sprite originalSprite;


    void Start()
    {

        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();

            originalSprite = spriteRenderer.sprite; 
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        spriteRenderer.sprite = newSprite;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        spriteRenderer.sprite = originalSprite;
    }   
}
