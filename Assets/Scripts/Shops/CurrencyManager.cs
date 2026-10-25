using System.Collections;
using TMPro;
using UnityEngine;

// Shows gold (run-only, RunData) and bones (permanent, ProgressManager) and plays the coin animation.
// Scene-scoped: nothing here survives a reload except what ProgressManager saved.
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

    public void AddBone(int boneAmount)
    {
        ProgressManager.instance.AddBones(boneAmount);
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
            boneCountDisplay.text = "Bones:" + ProgressManager.instance.Progress.bones;
    }
}
