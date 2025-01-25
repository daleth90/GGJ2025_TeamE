using UnityEngine;

namespace Bubble
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class Monster : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float _moveSpeed;
        [SerializeField] private Vector2 _footOffset;

        [Header("Bullet")]
        [SerializeField] private GameObject _bullet;
        [SerializeField] private Vector2 _bulletStartOffset;
        [SerializeField] private BulletData _bulletData;

        [Header("Aiming method")]
        [SerializeField] private Collider2D _searchCollider;
        [SerializeField] private bool _useFixedPoint;
        [SerializeField] private Vector2 _fixedPoint;

        [Header("Reload")]
        [SerializeField] private float _shotCoolDownTime;

        private float _lastShotTime;

        private void Update()
        {
            if (_moveSpeed > 0)
                Move();

            if (_useFixedPoint && CanShoot())
            {
                Shoot((Vector2)transform.position + _fixedPoint);
                ResetLastShotTime();
            }
        }

        private void Move()
        {
            RefreshDirection();
            transform.position += _moveSpeed * Time.deltaTime * (Vector3)GetDirection();
        }

        private void RefreshDirection()
        {
            // Reverse if it will hit a wall or fall down.
            if (IsWallInFront() || IsNoGroundAhead())
            {
                // Turn around
                transform.Rotate(0, 180, 0);
            }
        }

        private bool IsWallInFront()
        {
            Vector2 checkPosition = GetFootPosition();
            checkPosition.y = transform.position.y;

            LayerMask mask = LayerMask.GetMask("Interactable");
            RaycastHit2D[] hits = Physics2D.LinecastAll(transform.position, checkPosition, mask);
            foreach (RaycastHit2D hit in hits)
            {
                if (hit.collider != _searchCollider)
                {
                    return true;
                }
            }
            return false;
        }

        private bool IsNoGroundAhead()
        {
            LayerMask mask = LayerMask.GetMask("Ground");
            RaycastHit2D[] hits = Physics2D.LinecastAll(transform.position, GetFootPosition(), mask);
            foreach (RaycastHit2D hit in hits)
            {
                if (hit.collider != _searchCollider)
                {
                    return false;
                }
            }
            return true;
        }

        private Vector2 GetDirection()
        {
            return (Mathf.Abs(transform.rotation.y) < 1) ? Vector2.right : Vector2.left;
        }

        private Vector2 GetFootPosition()
        {
            return transform.position + new Vector3(_footOffset.x * GetDirection().x, _footOffset.y);
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
            if (!_useFixedPoint && collider.CompareTag("Player"))
            {
                if (CanShoot())
                {
                    Shoot(collider.transform);
                    ResetLastShotTime();
                }
            }
        }

        private void Shoot(Vector2 targetPosition)
        {
            Bullet bullet = GenerateBullet();
            bullet.SetTarget(targetPosition, _bulletData);
        }

        private void Shoot(Transform player)
        {
            Bullet bullet = GenerateBullet();
            bullet.SetTarget(player, _bulletData);
        }

        private Bullet GenerateBullet()
        {
            return Instantiate(
                _bullet,
                transform.position + (Vector3)_bulletStartOffset,
                Quaternion.identity
            ).GetComponent<Bullet>();
        }

        // Debug
        private void OnDrawGizmosSelected()
        {
            // Draw foot location.
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, GetFootPosition());

            // Draw attack line.
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(transform.position + (Vector3)_bulletStartOffset, 0.1f);
            Gizmos.DrawLine(
                (Vector2)transform.position + _bulletStartOffset,
                (Vector2)transform.position + _fixedPoint
            );
        }
    }
}