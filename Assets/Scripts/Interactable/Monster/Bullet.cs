using Bubble;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _lifeTime;
    [SerializeField] private int _damage;

    // Target
    private Transform _player;
    private Vector3 _targetPosition;

    private float _startTime = float.MaxValue;

    public void SetTarget(Vector2 target)
    {
        _startTime = Time.time;
        _targetPosition = target;
    }

    public void SetTarget(Transform player)
    {
        _startTime = Time.time;
        _player = player;
    }

    private void Update()
    {
        Move();

        AttackIfArrivedOrTimeout();
    }

    private void Move()
    {
        // Get position by player or fixed point.
        Vector3 target = (_player != null) ? _player.position : _targetPosition;

        // Calculate next position.
        target = Vector3.MoveTowards(transform.position, target, _moveSpeed * Time.deltaTime);

        // Move
        transform.position = target;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // If find player then attack.
        if (collision.collider.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerStatus>().oxygen -= _damage;
            Destroy(gameObject);
        }
    }

    // TODO: If want to add an explosion effect...
    private void Attack()
    {
        Destroy(gameObject);
    }

    private void AttackIfArrivedOrTimeout()
    {
        Debug.Log($"{_startTime + _lifeTime},  {Time.time}");
        // Attack/Destroy if arrived or timeout
        if ((_player == null && transform.position == _targetPosition)
            || _startTime + _lifeTime < Time.time)
            Attack();
    }
}