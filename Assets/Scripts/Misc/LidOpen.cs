using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

// REVERT: public class LidOpen : MonoBehaviour
// Needs IPointerEnterHandler/IPointerExitHandler, otherwise Unity's EventSystem
// never calls OnPointerEnter/OnPointerExit below - they were dead code.
// Also needs a Collider2D on this object for the Physics2DRaycaster to hit it;
// RequireComponent nags/auto-adds one if it's missing.
[RequireComponent(typeof(BoxCollider2D))]
public class LidOpen : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

   public SpriteRenderer spriteRenderer;
   public Sprite newSprite;
   private Sprite originalSprite;


    void Start()
    {

        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();

            // REVERT: originalSprite = spriteRenderer.sprite;
            // Moved outside the if so it's captured even if spriteRenderer was
            // already assigned in the Inspector (otherwise OnPointerExit would
            // reset the sprite to null in that case).
        }

        originalSprite = spriteRenderer.sprite;
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
