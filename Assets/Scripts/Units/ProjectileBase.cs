using UnityEngine;

public class ProjectileBase : MonoBehaviour
{
    private GameObject currentEnemyGO;
    private UnitBase currentEnemy;
    private BaseBase currentBase;
    [SerializeField] private float moveSpeed;
    private float attack;
    public void SetTarget(GameObject target)
    {
        this.GetComponent<Rigidbody2D>().linearVelocity = (target.transform.position - this.transform.position) / Mathf.Max(((target.transform.position - this.transform.position).magnitude / moveSpeed), Time.fixedDeltaTime);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        currentEnemyGO = collision.gameObject;

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
        DestroySelf();
    }
    public float GetAttack()
    {
        return attack;
    }
    public void SetAttack(float nAttack)
    {
        attack = nAttack;

    }
    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}
