using UnityEngine;
using UnityEngine.UI;

public class BaseBase : MonoBehaviour
{
    [SerializeField] private float maxHP;
    private float currentHP;
    private float effectiveMaxHP;
    private bool isPlayerBase;
    private Rigidbody2D body;
    [SerializeField] private Image healthBar;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        isPlayerBase = TryGetComponent(out PlayerBase _);
    }

    private void Start()
    {
        SetHPToMax();
    }

    private void FixedUpdate()
    {
        body.linearVelocity = Vector2.zero;
        body.freezeRotation = true;
        if (currentHP <= 0)
        {
            DestroySelf();
        }
        healthBar.fillAmount = currentHP / effectiveMaxHP;
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
        RefreshMaxHP();
        currentHP = effectiveMaxHP;
    }

    // The player base grows only through upgrades: its own max HP + the stat module [W7, B1].
    public void RefreshMaxHP()
    {
        effectiveMaxHP = isPlayerBase && ModifierManager.instance != null
            ? ModifierManager.instance.PlayerBaseMaxHp(maxHP)
            : maxHP;
    }

    // Hook for level-shop repair items; the items come later [B1].
    public void Repair(float amount)
    {
        currentHP = RepairedHP(currentHP, amount, effectiveMaxHP);
    }

    // HP after repairing by amount, capped at max. A negative amount changes nothing.
    public static float RepairedHP(float current, float amount, float max)
    {
        return Mathf.Min(max, current + Mathf.Max(0f, amount));
    }

    // A base only reports that it fell; GameManager decides what that means.
    public void DestroySelf()
    {
        UnitManager.instance.SetUnitsCanMove(false);
        gameObject.SetActive(false);
        GameManager.instance.BaseDestroyed(TryGetComponent(out EnemyBase _));
    }
}
