using UnityEngine;
using UnityEngine.UI;

public class BaseBase : MonoBehaviour
{
    [SerializeField] private float maxHP;
    private float currentHP;
    [SerializeField] private Image healthBar;

    private void Start()
    {
        currentHP = maxHP;
    }
    private void FixedUpdate()
    {
        this.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
        this.GetComponent<Rigidbody2D>().freezeRotation = true;
        if (currentHP <= 0)
        {
            DestroySelf();
        }
        healthBar.fillAmount = currentHP / maxHP;
    }
    public float GetCurrentHP()
    {
        return currentHP;
    }
    public void SetHP(float newHP)
    {
        currentHP = newHP;
    }
    public void SetHPToMax()
    {
        currentHP = maxHP;
    }
    public void DestroySelf()
    {
        //Temp if statements
        if (GameManager.instance.GetCurrentWave() < 3 && TryGetComponent(out EnemyBase enemyBase))
        {
            GameManager.instance.SetStateWaveVictory();
            GameManager.instance.NextWave();
        }
        else
        {
            GameManager.instance.SetStateEndGame();
        }
        UnitManager.instance.SetUnitsCanMove(false);
        gameObject.SetActive(false);
    }
}
