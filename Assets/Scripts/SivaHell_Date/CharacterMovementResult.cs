using UnityEngine;

namespace Bubble
{
    public struct CharacterMovementResult
    {
        public Vector2 newPosition;
        public bool isGrounded;
        public bool isCeiling;
    }

    public struct RaycastResult
    {
        public bool isHit;
        public Vector2 point;
        public Vector2 normal;
        public float distance;
    }

    public struct RaycastResult<T>
    {
        public bool isHit;
        public Vector2 point;
        public Vector2 normal;
        public float distance;
        public T hitObject;
    }

    public interface ICharacterMovementBody
    {
        Vector2 Position { get; }
        Vector2 BoxSize { get; }
        float SkinWidth { get; }
        float SlopeLimit { get; }
        float DistanceBetweenRays { get; }
        float Pinching { get; }

        RaycastResult RaycastObstacle(Vector2 origin, Vector2 direction, float distance);
    }

    public struct PlatformPushmentResult
    {
        public ICharacterMovementBody character;
        public Vector2 pushment;
        public bool beforePlatform;
    }
}
