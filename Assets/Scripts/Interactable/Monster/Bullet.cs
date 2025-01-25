using UnityEngine;

namespace Bubble
{
    public class Bullet : MonoBehaviour
    {
        // Target
        private Transform _player;
        private Vector3 _targetPosition;

        private BulletData _bulletData;

        private float _startTime = float.MaxValue;

        public void SetTarget(Vector2 target, BulletData data)
        {
            _startTime = Time.time;
            _targetPosition = target;
            _bulletData = data;
        }

        public void SetTarget(Transform player, BulletData data)
        {
            _startTime = Time.time;
            _player = player;
            _bulletData = data;
        }

        private void Update()
        {
            Move();

            ExplodeIfArrivedOrTimeout();
        }

        private void Move()
        {
            // Get position by player or fixed point.
            Vector3 target = (_player != null) ? _player.position : _targetPosition;

            // Calculate next position.
            target = Vector3.MoveTowards(transform.position, target, _bulletData.moveSpeed * Time.deltaTime);

            // Move
            transform.position = target;
        }

        private void OnTriggerEnter2D(Collider2D collider)
        {
            // If find player then attack.
            if (collider.CompareTag("Player"))
            {
                collider.GetComponent<PlayerStatus>().oxygen -= _bulletData.damage;
                Destroy(gameObject);
            }
        }

        // TODO: If want to add an explosion effect...
        private void Explode()
        {
            Destroy(gameObject);
        }

        private void ExplodeIfArrivedOrTimeout()
        {
            // Attack/Destroy if arrived or timeout
            if ((_player == null && transform.position == _targetPosition)
                || _startTime + _bulletData.lifeTime < Time.time)
                Explode();
        }
    }
}