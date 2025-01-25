using UnityEngine;

namespace Bubble
{
    public class CharacterMovementBody : MonoBehaviour, ICharacterMovementBody
    {
        private const float DebugRenderTime = 1f / 30f;

        [SerializeField]
        private Vector2 _boxSize = new(1f, 1f);
        [SerializeField]
        [Range(0.01f, 0.1f)]
        private float _skinWidth = 0.01f;
        [SerializeField]
        private float _slopeLimit = 60f;
        [SerializeField]
        private float _distanceBetweenRays = 0.5f;
        [SerializeField]
        [Range(0f, 1f)]
        private float _pinching = 0.5f;

        [Space]
        [SerializeField]
        private bool _debug;

        private CharacterMovement _movement;
        private Vector2 _position;
        private int _groundLayerMask;

        public Vector2 Position => _position;
        public Vector2 BoxSize => _boxSize;
        public float SkinWidth => _skinWidth;
        public float SlopeLimit => _slopeLimit;
        public float DistanceBetweenRays => _distanceBetweenRays;
        public float Pinching => _pinching;

        private void Awake()
        {
            _movement = new CharacterMovement(this);
        }

        private void Start()
        {
            SetLayerMask("Ground", "Vine");
        }

        public void SetLayerMask(params string[] layerNames)
        {
            _groundLayerMask = LayerMask.GetMask(layerNames);
        }

        public void ApplyPosition(Vector2 position)
        {
            _position = position;
            transform.position = position;
        }

        public CharacterMovementResult Move(Vector2 position, float deltaX, float deltaY, bool applyPosition = true)
        {
            // Important: Synchronize properties for internal usage
            _position = position;

            // Calculate
            Vector2 translate = _movement.Calculate(deltaX, deltaY);

#if UNITY_EDITOR
            if (_debug)
            {
                Debug.DrawLine(_movement.BottomLeft, _movement.BottomRight, Color.green, DebugRenderTime, false);
                Debug.DrawLine(_movement.BottomRight, _movement.TopRight, Color.green, DebugRenderTime, false);
                Debug.DrawLine(_movement.TopRight, _movement.TopLeft, Color.green, DebugRenderTime, false);
                Debug.DrawLine(_movement.TopLeft, _movement.BottomLeft, Color.green, DebugRenderTime, false);

                if (_pinching > _skinWidth)
                {
                    float leftX = _movement.BottomLeft.x - 0.5f;
                    float rightX = _movement.BottomRight.x + 0.5f;
                    float pinchingTop = _movement.TopLeft.y - (_pinching - _skinWidth);
                    float pinchingBottom = _movement.BottomLeft.y + (_pinching - _skinWidth);
                    Debug.DrawLine(new Vector2(leftX, pinchingTop), new Vector2(rightX, pinchingTop), Color.yellow, DebugRenderTime, false);
                    Debug.DrawLine(new Vector2(leftX, pinchingBottom), new Vector2(rightX, pinchingBottom), Color.yellow, DebugRenderTime, false);
                }
            }
#endif

            Vector2 newPosition = _position + translate;
            if (applyPosition)
            {
                ApplyPosition(newPosition);
            }

            var result = new CharacterMovementResult
            {
                newPosition = newPosition,
                isGrounded = _movement.CollisionInfo.bottom,
                isCeiling = _movement.CollisionInfo.top,
                isWall = _movement.CollisionInfo.left || _movement.CollisionInfo.right,
            };

            return result;
        }

        public RaycastResult RaycastObstacle(Vector2 origin, Vector2 direction, float distance)
        {
            var result = new RaycastResult();
            RaycastHit2D hit = Physics2D.Raycast(origin, direction, distance, _groundLayerMask);
            if (hit.collider != null)
            {
                result.isHit = true;
                result.point = hit.point;
                result.normal = hit.normal;
                result.distance = hit.distance;
            }

#if UNITY_EDITOR
            if (_debug)
            {
                Debug.DrawRay(origin, direction * distance, Color.red, 1f / 30f, false);
            }
#endif

            return result;
        }
    }
}
