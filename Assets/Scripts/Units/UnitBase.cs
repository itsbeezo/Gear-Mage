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
    private BaseBase currentBase;
    [SerializeField] private Image healthBar;
    private void Start()
    {
        if (this.TryGetComponent(out UnitPlayer unitPlayer))
            maxHP += GearManager.instance.GetHPMod();
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
                this.GetComponent<Rigidbody2D>().linearVelocityX = moveSpeed + GearManager.instance.GetMoveSpeedMod();
                moveRateStep = 0;
            }
            if (attackRateStep >= (attackRate - GearManager.instance.GetAttackSpeedMod()))
            {
                if (currentEnemyGO.TryGetComponent(out UnitBase unitBase))
                {
                    currentEnemy = currentEnemyGO.GetComponent<UnitBase>();
                    currentEnemy.SetHP(currentEnemy.GetCurrentHP() - (GetAttack() + GearManager.instance.GetAttackMod()));
                }
                else if (currentEnemyGO.TryGetComponent(out BaseBase baseBase))
                {
                    currentBase = currentEnemyGO.GetComponent<BaseBase>();
                    currentBase.SetHP(currentBase.GetCurrentHP() - (GetAttack() + GearManager.instance.GetAttackMod()));
                }
                attackRateStep = 0;
            }
        }
        else if (this.TryGetComponent(out UnitEnemy unitEnemy))
        {
            if (moveRateStep >= moveRate && !isColliding)
            {
                this.GetComponent<Rigidbody2D>().linearVelocityX = moveSpeed;
                moveRateStep = 0;
            }
            if (attackRateStep >= attackRate)
            {
                if (currentEnemyGO.TryGetComponent(out UnitBase unitBase))
                {
                    currentEnemy = currentEnemyGO.GetComponent<UnitBase>();
                    currentEnemy.SetHP(currentEnemy.GetCurrentHP() - GetAttack());
                }
                else if (currentEnemyGO.TryGetComponent(out BaseBase baseBase))
                {
                    currentBase = currentEnemyGO.GetComponent<BaseBase>();
                    currentBase.SetHP(currentBase.GetCurrentHP() - GetAttack());
                }
                attackRateStep = 0;
            }
        }

        if(isColliding)
        {
            this.GetComponent<Rigidbody2D>().linearVelocityX = 0;
            this.GetComponent<Rigidbody2D>().linearVelocityY = 0;
        }

        if (currentHP <= 0)
        {
            DestroySelf();
        }
        healthBar.fillAmount = currentHP / maxHP;
        Debug.Log(currentEnemyGO);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        currentEnemyGO = collision.gameObject;
        isColliding = true;
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        currentEnemyGO = collision.gameObject;
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
        //Checks if defeated unit is from enemy or player 
        if (TryGetComponent(out UnitEnemy enemy))
        {
            CurrencyManager.instance.AddGold(10, transform.position);
        }
        Destroy(gameObject);
    }
}
