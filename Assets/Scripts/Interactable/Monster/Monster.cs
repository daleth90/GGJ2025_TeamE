using UnityEngine;

namespace Bubble
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class Monster : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float _moveSpeed;
        [SerializeField] private float _footOffset;

        private SpriteRenderer _spriteRenderer;

        [Header("Bullet")]
        [SerializeField] private GameObject _bullet;
        [SerializeField] private Vector2 _bulletStartOffset;

        [Header("Search Range or Target Position")]
        [SerializeField] private bool _useSearchCollider;
        [SerializeField] private Collider2D _searchCollider;
        [SerializeField] private Vector2 _bulletTargetPosition;

        [SerializeField] private float _shotCoolDownTime;
        private float _lastShotTime;

        private void Start()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Update()
        {
            if (_moveSpeed > 0)
                Move();

            if (!_useSearchCollider && CanShoot())
            {
                Shoot((Vector2)transform.position + _bulletTargetPosition);
                ResetLastShotTime();
            }
        }

        private void Move()
        {
            // Move to next position
            transform.position += _moveSpeed * Time.deltaTime * GetDirection();
        }

        private Vector3 GetDirection()
        {
            // Get and Check direction
            Vector3 direction = _spriteRenderer.flipX ? Vector3.left : Vector3.right;

            // Get move position
            Vector3 movePosition = transform.position + _moveSpeed * Time.deltaTime * direction;

            // Reverse if it will hit a wall or fall down.
            if (Physics2D.Linecast(transform.position, movePosition)
                || !Physics2D.Linecast(transform.position, movePosition + _footOffset * Vector3.down))
            {
                _spriteRenderer.flipX = !_spriteRenderer.flipX;
                direction = (direction == Vector3.left) ? Vector3.right : Vector3.left;
            }

            return direction;
        }

        private bool CanShoot()
        {
            return _lastShotTime + _shotCoolDownTime <= Time.time;
        }

        private void ResetLastShotTime()
        {
            _lastShotTime = Time.time;
        }

        private void OnTriggerStay2D(Collider2D collider)
        {
            if (_useSearchCollider && collider.CompareTag("Player"))
            {
                if (CanShoot())
                {
                    Debug.Log(collider.transform == null);
                    Shoot(collider.transform);
                    ResetLastShotTime();
                }
            }
        }

        private void Shoot(Vector2 targetPosition)
        {
            Bullet bullet = GenerateBullet();
            bullet.SetTarget(targetPosition);
        }

        private void Shoot(Transform player)
        {
            Bullet bullet = GenerateBullet();
            bullet.SetTarget(player);
        }

        private Bullet GenerateBullet()
        {
            return Instantiate(
                _bullet,
                transform.position + (Vector3)_bulletStartOffset,
                Quaternion.identity,
                transform
            ).GetComponent<Bullet>();
        }

        private void OnDrawGizmosSelected()
        {
            // Draw foot location.
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, transform.position + _footOffset * Vector3.down);

            // Draw attack line.
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position + (Vector3)_bulletTargetPosition);

            // Draw start shoot position.
            Gizmos.color = CanShoot() ? Color.green : Color.blue;
            Gizmos.DrawSphere(transform.position + (Vector3)_bulletStartOffset, 0.3f);
        }
    }
}