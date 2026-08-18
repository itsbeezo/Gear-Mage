using System.Runtime.CompilerServices;
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
    private UnitPlayer[] PlayerList;
    private UnitEnemy[] EnemyList;
    private void Start()
    {
        if (this.TryGetComponent(out UnitPlayer unitPlayer))
            maxHP += GearManager.instance.GetHPMod();
        currentHP = maxHP;
    }
    private void FixedUpdate()
    {
        PlayerList = FindObjectsByType<UnitPlayer>();
        EnemyList = FindObjectsByType<UnitEnemy>();

        if(UnitManager.instance.GetUnitsCanMove())
            moveRateStep += 1;
        if (isColliding)
            attackRateStep += 1;

        if (this.TryGetComponent(out UnitPlayer unitPlayer))
        {
            if (moveRateStep >= moveRate && !isColliding && UnitManager.instance.GetUnitsCanMove())
            {
                if (EnemyList.Length <= 0)
                    this.GetComponent<Rigidbody2D>().linearVelocity = (EnemyBase.instance.transform.position - this.transform.position) / Mathf.Max(((EnemyBase.instance.transform.position - this.transform.position).magnitude / moveSpeed), Time.fixedDeltaTime);
                else
                    this.GetComponent<Rigidbody2D>().linearVelocity = (GetClosestEnemy(this).transform.position - this.transform.position) / Mathf.Max(((GetClosestEnemy(this).transform.position - this.transform.position).magnitude / moveSpeed), Time.fixedDeltaTime);
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
                    CurrencyManager.instance.AddGold(10, transform.position);
                }
                attackRateStep = 0;
            }
        }
        else if (this.TryGetComponent(out UnitEnemy unitEnemy))
        {
            if (moveRateStep >= moveRate && !isColliding && UnitManager.instance.GetUnitsCanMove())
            {
                if (PlayerList.Length <= 0)
                    this.GetComponent<Rigidbody2D>().linearVelocity = (EnemyBase.instance.transform.position - this.transform.position) / Mathf.Max(((PlayerBase.instance.transform.position - this.transform.position).magnitude / moveSpeed), Time.fixedDeltaTime);
                else
                    this.GetComponent<Rigidbody2D>().linearVelocity = (GetClosestPlayer(this).transform.position - this.transform.position) / Mathf.Max(((GetClosestPlayer(this).transform.position - this.transform.position).magnitude / moveSpeed), Time.fixedDeltaTime);

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
            this.GetComponent<Rigidbody2D>().freezeRotation = true;
        }

        if (currentHP <= 0)
        {
            DestroySelf();
        }
        healthBar.fillAmount = currentHP / maxHP;
        //Debug.Log(currentEnemyGO);
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
    private UnitPlayer GetClosestPlayer(UnitBase enemy)
    {
        Vector3 heading;
        float distanceBetween = 100;
        float distanceBetweenTemp;
        int index = 0;
        for (int i = 0; i < PlayerList.Length; i++)
        {
            heading = PlayerList[i].transform.position - enemy.transform.position;
            distanceBetweenTemp = heading.sqrMagnitude;
            if(distanceBetweenTemp < distanceBetween)
            {
                index = i;
            }
        }
        return PlayerList[index];
    }
    private UnitEnemy GetClosestEnemy(UnitBase player)
    {
        Vector3 heading;
        float distanceBetween = 100;
        float distanceBetweenTemp;
        int index = 0;
        for (int i = 0; i < EnemyList.Length; i++)
        {
            heading = EnemyList[i].transform.position - player.transform.position;
            distanceBetweenTemp = heading.sqrMagnitude;
            if (distanceBetweenTemp < distanceBetween)
            {
                index = i;
            }
        }
        return EnemyList[index];
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
            if(TryGetComponent(out UnitSoldier soldier))
                CurrencyManager.instance.AddGold(10, transform.position);
            if (TryGetComponent(out UnitTank tank))
                CurrencyManager.instance.AddGold(30, transform.position);
        }
        Destroy(gameObject);
    }
}
