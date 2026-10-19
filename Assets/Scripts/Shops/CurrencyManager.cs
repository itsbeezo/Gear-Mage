using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager instance { get; private set; }
    [SerializeField] private TextMeshProUGUI goldCountDisplay;
    [SerializeField] private TextMeshProUGUI boneCountDisplay;
    [SerializeField] private GameObject goldPrefab;
    [SerializeField] private GameObject coinFallArea;
    private int goldCount;
    private int boneCount;
    public int cost;

    private static bool isFirstGoldLoadInSequence = true;

    private const string BONE_SAVE_KEY = "SavedBoneCount";

    private const string GOLD_SAVE_KEY = "GoldIsSaved";


    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindSceneReferences();
    }
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        LoadBone();

        if (isFirstGoldLoadInSequence)
        {
            isFirstGoldLoadInSequence = false;
            ClearGoldSave();
        }
        else
        {
            LoadGold();
        }
    }
    public void FindSceneReferences()
    {
        GameObject displayObject = GameObject.FindGameObjectWithTag("Gold Display");
        GameObject fallObject = GameObject.FindGameObjectWithTag("CoinFall Area");
        GameObject boneDisplayObject = GameObject.FindGameObjectWithTag("Bone Display");


        if (displayObject != null)
        {
            goldCountDisplay = displayObject.GetComponent<TextMeshProUGUI>();
            UpdateDisplayText();
        }

        if (fallObject != null)
        {
            coinFallArea = fallObject;
        }

        if (boneDisplayObject != null)
        {
            boneCountDisplay = boneDisplayObject.GetComponent<TextMeshProUGUI>();
            UpdateDisplayTextBone();
        }

    }

    public void AddGold(int goldAmount, Vector2 enemyDeathPosition)
    {
        if (goldCountDisplay == null || coinFallArea == null)
        {
            FindSceneReferences();
        }

        if (goldPrefab == null)
        {
            goldCount += goldAmount;
            //SaveBone();
            UpdateDisplayText();

            return;
        }

        GameObject goldInstance = Instantiate(goldPrefab, enemyDeathPosition, Quaternion.identity);
        StartCoroutine(MoveGoldAnimation(goldAmount, goldInstance)); 
        SaveGold();
    }

    public void RefundGold(int goldAmount)
    {
        goldCount += goldAmount;
        UpdateDisplayText();
        SaveGold();
    }

    // Spends gold only if the balance covers it. Returns false and changes nothing otherwise.
    public bool TrySpendGold(int goldAmount)
    {
        if (goldAmount > goldCount) return false;

        goldCount -= goldAmount;
        UpdateDisplayText();
        SaveGold();
        return true;
    }

    public void AddBone(int boneAmount)
    {
            boneCount += boneAmount;
            SaveBone();
            UpdateDisplayTextBone();
            return;
        
    }

    IEnumerator MoveGoldAnimation(int goldAmount, GameObject goldInstance)
    {
        if(coinFallArea != null)
        {
            Vector2 targetPos = coinFallArea.transform.position;

            while (Vector2.Distance(goldInstance.transform.position, targetPos) > 0.1f)
            {
                goldInstance.transform.position = Vector2.MoveTowards(goldInstance.transform.position, targetPos, 5f * Time.deltaTime);

                if ((Vector2.Distance(goldInstance.transform.position, targetPos) < 1.25f))
                    goldInstance.transform.localScale = Vector2.MoveTowards(goldInstance.transform.localScale, new Vector2(0, 0), 1.000001f * Time.deltaTime);

                yield return null;;
            }
        }
        else if(coinFallArea == null)
        {
            while (goldInstance.transform.localScale != Vector3.zero)
            {
                goldInstance.transform.localScale = Vector2.MoveTowards(goldInstance.transform.localScale, new Vector2(0, 0), 1.1f * Time.deltaTime);
                yield return null;
            }
        }

        Destroy(goldInstance);
        goldCount += goldAmount;

        //SaveBone();
        UpdateDisplayText();
        SaveGold();
    }

    private void UpdateDisplayText()
    {
        if (goldCountDisplay != null)
            goldCountDisplay.text = ":" + goldCount;
    }

    private void UpdateDisplayTextBone()
    {
        if (boneCountDisplay != null)
            boneCountDisplay.text = "Bones:" + boneCount;
    }

    public void SaveGold()
    {
        PlayerPrefs.SetInt(GOLD_SAVE_KEY, goldCount);
        PlayerPrefs.Save();
    }

    public void LoadGold()
    {
        goldCount = PlayerPrefs.GetInt(GOLD_SAVE_KEY, 0);
        UpdateDisplayText();
    }

    public void ClearGoldSave()
    {
        PlayerPrefs.DeleteKey(GOLD_SAVE_KEY);
        goldCount = 0;
    }

    public void SaveBone()
    {
        PlayerPrefs.SetInt(BONE_SAVE_KEY, boneCount);
        PlayerPrefs.Save(); 
    }

    public void LoadBone()
    {
        boneCount = PlayerPrefs.GetInt(BONE_SAVE_KEY, 0);
        UpdateDisplayTextBone();
    }

    private void OnApplicationQuit()
    {
        SaveBone();
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            SaveBone();
        }
    }

}
