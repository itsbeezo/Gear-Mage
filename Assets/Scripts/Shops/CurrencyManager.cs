using System.Collections;
using TMPro;
using UnityEngine;

// Shows gold (run-only, RunData) and bones (permanent, PlayerProgress) and plays the coin animation.
// Scene-scoped: nothing here survives a reload except what RunManager saved to PlayerProgress.
public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager instance { get; private set; }

    [SerializeField] private TextMeshProUGUI goldCountDisplay;
    [SerializeField] private TextMeshProUGUI boneCountDisplay;
    [SerializeField] private GameObject goldPrefab;
    [SerializeField] private GameObject coinFallArea;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        UpdateDisplayText();
        UpdateDisplayTextBone();
    }

    public void AddGold(int goldAmount, Vector2 enemyDeathPosition)
    {
        if (goldPrefab == null)
        {
            Run.AddGold(goldAmount);
            UpdateDisplayText();
            return;
        }

        GameObject goldInstance = Instantiate(goldPrefab, enemyDeathPosition, Quaternion.identity);
        StartCoroutine(MoveGoldAnimation(goldAmount, goldInstance));
    }

    public void RefundGold(int goldAmount)
    {
        Run.AddGold(goldAmount);
        UpdateDisplayText();
    }

    // Spends gold only if the balance covers it. Returns false and changes nothing otherwise.
    public bool TrySpendGold(int goldAmount)
    {
        if (!Run.TrySpendGold(goldAmount)) return false;

        UpdateDisplayText();
        return true;
    }

    // Spends gold for one gear at its catalog cost. Called when a shop offer is dropped on the
    // gearbox. Returns false and spends nothing if the id is unknown or the player can't
    // afford it. Which shops are open is enforced by scene visibility, not state.
    // Placing the gear itself is handled by UIGearSlot.
    public bool TryBuyGear(int id)
    {
        GearDefinition def = GearCatalog.instance != null ? GearCatalog.instance.GetDefinition(id) : null;
        if (def == null)
        {
            Debug.Log("Id provided for gear is invalid " + id);
            return false;
        }

        if (!TrySpendGold(def.cost))
        {
            Debug.Log("Not enough gold for " + def.displayName + " (costs " + def.cost + ")");
            return false;
        }

        Debug.Log("Bought " + def.displayName + " for " + def.cost + " gold");
        return true;
    }

    public void AddBone(int boneAmount)
    {
        RunManager.instance.AddBones(boneAmount);
        UpdateDisplayTextBone();
    }

    private static RunData Run
    {
        get { return RunManager.instance.Current; }
    }

    IEnumerator MoveGoldAnimation(int goldAmount, GameObject goldInstance)
    {
        if (coinFallArea != null)
        {
            Vector2 targetPos = coinFallArea.transform.position;

            while (Vector2.Distance(goldInstance.transform.position, targetPos) > 0.1f)
            {
                goldInstance.transform.position = Vector2.MoveTowards(goldInstance.transform.position, targetPos, 5f * Time.deltaTime);

                if ((Vector2.Distance(goldInstance.transform.position, targetPos) < 1.25f))
                    goldInstance.transform.localScale = Vector2.MoveTowards(goldInstance.transform.localScale, new Vector2(0, 0), 1.000001f * Time.deltaTime);

                yield return null;
            }
        }
        else
        {
            while (goldInstance.transform.localScale != Vector3.zero)
            {
                goldInstance.transform.localScale = Vector2.MoveTowards(goldInstance.transform.localScale, new Vector2(0, 0), 1.1f * Time.deltaTime);
                yield return null;
            }
        }

        Destroy(goldInstance);
        Run.AddGold(goldAmount);
        UpdateDisplayText();
    }

    private void UpdateDisplayText()
    {
        if (goldCountDisplay != null)
            goldCountDisplay.text = ":" + Run.gold;
    }

    private void UpdateDisplayTextBone()
    {
        if (boneCountDisplay != null)
            boneCountDisplay.text = "Bones:" + RunManager.instance.Progress.bones;
    }
}
