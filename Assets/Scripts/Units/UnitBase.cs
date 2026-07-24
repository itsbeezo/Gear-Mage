using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UnitBase : MonoBehaviour
{
    [SerializeField] private int unitIndex;
    [SerializeField] private float maxHP;
    private float currentHP;
    [SerializeField] private float attack;
    [SerializeField] private float moveSpeed;
    private int moveRate = 5;
    private int moveRateStep;
    [SerializeField] private float attackRate;
    private float attackRateStep;
    private bool isColliding = false;
    private GameObject currentEnemyGO;
    private UnitBase currentEnemy;
    [SerializeField] private Image healthBar;

    private void Start()
    {
        currentHP = maxHP;
    }
    private void FixedUpdate()
    {
        moveRateStep += 1;
        if (isColliding)
            attackRateStep += 1;

        if (this.TryGetComponent(out UnitPlayer unitPlayer))
        {
            if (moveRateStep >= moveRate && !isColliding)
            {
                this.transform.Translate(moveSpeed, 0, 0);
                moveRateStep = 0;
            }
            if (attackRateStep >= attackRate)
            {
                currentEnemy.SetHP(currentEnemy.GetCurrentHP() - GetAttack());
                attackRateStep = 0;
            }
        }
        else if (this.TryGetComponent(out UnitEnemy unitEnemy))
        {
            if (moveRateStep >= moveRate && !isColliding)
            {
                this.transform.Translate(moveSpeed, 0, 0);
                moveRateStep = 0;
            }
            if (attackRateStep >= attackRate)
            {
                currentEnemy.SetHP(currentEnemy.GetCurrentHP() - GetAttack());
                attackRateStep = 0;
            }
        }


        if (currentHP <= 0)
        {
            DestroySelf();
        }
        healthBar.fillAmount = currentHP / maxHP;

    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        currentEnemyGO = collision.gameObject;
        currentEnemy = currentEnemyGO.GetComponent<UnitBase>();
        isColliding = true;
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        isColliding = false;
        attackRateStep = 0;
    }
    public float GetCurrentHP()
    {
        return currentHP;
    }
    public float GetMaxHP()
    {
        return maxHP;
    }
    public void SetHP(float newHP)
    {
        currentHP = newHP;
    }
    public float GetAttack()
    {
        return attack;
    }
    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}
