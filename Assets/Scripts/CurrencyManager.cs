using System.Collections;
using TMPro;
using UnityEngine;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager instance { get; private set; }
    [SerializeField] private TextMeshProUGUI goldCountDisplay;
    [SerializeField] private GameObject goldPrefab;
    [SerializeField] private GameObject coinFallArea;
    private int goldCount;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        instance = this;
        goldCount = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddGold(int goldAmount, Vector2 enemyDeathPosition)
    {
        GameObject goldInstance = Instantiate(goldPrefab, enemyDeathPosition, Quaternion.identity);
        StartCoroutine(MoveGoldAnimation(goldAmount, goldInstance)); 
    }

    IEnumerator MoveGoldAnimation(int goldAmount, GameObject goldInstance)
    {
        Vector2 targetPos = coinFallArea.transform.position;

        while (Vector2.Distance(goldInstance.transform.position, targetPos) > 0.1f)
        {
            goldInstance.transform.position = Vector2.MoveTowards(goldInstance.transform.position, targetPos, 5f * Time.deltaTime);
            yield return null;
        }

        Destroy(goldInstance);
        goldCount += goldAmount;
        goldCountDisplay.text = "Gold:" + goldCount;
    }

}
