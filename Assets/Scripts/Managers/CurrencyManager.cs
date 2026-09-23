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

    private const string Bone_SAVE_KEY = "SavedBoneCount";

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
    void Start()
    {
        LoadBone();
    }

    public void FindSceneReferences()
    {
        GameObject displayObject = GameObject.FindGameObjectWithTag("Gold Display");
        GameObject fallObject = GameObject.FindGameObjectWithTag("CoinFall Area");

        if (displayObject != null)
        {
            goldCountDisplay = displayObject.GetComponent<TextMeshProUGUI>();
            UpdateDisplayText();
        }

        if (fallObject != null)
        {
            coinFallArea = fallObject;
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
    }

    public void AddBone(int boneAmount)
    {
        if (boneCountDisplay != null)
        {
            boneCount += boneAmount;
            SaveBone();
            UpdateDisplayTextBone();

            return;
        }
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
    }

    private void UpdateDisplayText()
    {
        if (goldCountDisplay != null)
            goldCountDisplay.text = "Gold:" + goldCount;
    }

    private void UpdateDisplayTextBone()
    {
        if (boneCountDisplay != null)
            boneCountDisplay.text = "Bones:" + boneCount;
    }

    public void SaveBone()
    {
        PlayerPrefs.SetInt(Bone_SAVE_KEY, boneCount);
        PlayerPrefs.Save(); 
    }

    public void LoadBone()
    {
        boneCount = PlayerPrefs.GetInt(Bone_SAVE_KEY, 0);
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
