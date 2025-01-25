using Physalia;
using UnityEngine;

namespace Bubble
{
    public class BodyInfo
    {
        public float skinWidth;
        public float slopeLimit;

        public Vector2 position;
        public Vector2 bottomLeft;
        public Vector2 bottomRight;
        public Vector2 topLeft;
        public Vector2 topRight;
    }

    public class CollisionInfo
    {
        public bool bottom;
        public bool top;
        public bool left;
        public bool right;
        public bool slopeAscending;
        public bool slopeDescending;
        public bool slopeAscendingAir;
        public bool pinchBottomBlocked;
        public bool pinchTopBlocked;
        public bool pinchCenterBlocked;

        public void Reset()
        {
            bottom = false;
            top = false;
            left = false;
            right = false;
            slopeAscending = false;
            slopeDescending = false;
            slopeAscendingAir = false;
            pinchBottomBlocked = false;
            pinchTopBlocked = false;
            pinchCenterBlocked = false;
        }
    }

    public class CharacterMovement
    {
        private readonly ICharacterMovementBody _body;
        private readonly BodyInfo _bodyInfo = new();
        private readonly CollisionInfo _collisionInfo = new();

        private int _horizontalRayCount;
        private int _verticalRayCount;
        private float _horizontalRaySpace;
        private float _verticalRaySpace;

        private Vector2 _originalMovement;
        private float _currentSlope = 0f;
        private float _previousSlope = 0f;

        public Vector2 BottomLeft => _bodyInfo.bottomLeft;
        public Vector2 BottomRight => _bodyInfo.bottomRight;
        public Vector2 TopLeft => _bodyInfo.topLeft;
        public Vector2 TopRight => _bodyInfo.topRight;

        public CollisionInfo CollisionInfo => _collisionInfo;

        public CharacterMovement(ICharacterMovementBody body)
        {
            _body = body;
        }

        public Vector2 Calculate(float x, float y)
        {
            _collisionInfo.Reset();
            _previousSlope = _currentSlope;
            _currentSlope = 0f;
            UpdateBodyInfo();
            UpdateRayCounts();

            _originalMovement = new Vector2(x, y);
            var finalMovement = _originalMovement;

            if (finalMovement.x != 0f && finalMovement.y < 0f)
            {
                PreDescendSlope(ref finalMovement);
            }
            if (finalMovement.x != 0f)
            {
                HorizontalMove(ref finalMovement);
                HorizontalPinch(_originalMovement, ref finalMovement);
            }
            if (finalMovement.y != 0f)
            {
                VerticalMove(ref finalMovement);
            }
            if (finalMovement.x != 0f)
            {
                PostDescendSlope(ref finalMovement);
            }
            return finalMovement;
        }

        private void UpdateRayCounts()
        {
            int horizontalRaySpaceCount = Mathf.RoundToInt((_body.BoxSize.y - _body.SkinWidth * 2f) / _body.DistanceBetweenRays);
            int verticalRaySpaceCount = Mathf.RoundToInt((_body.BoxSize.x - _body.SkinWidth * 2f) / _body.DistanceBetweenRays);

            _horizontalRayCount = horizontalRaySpaceCount + 1;
            _verticalRayCount = verticalRaySpaceCount + 1;
            _horizontalRaySpace = (_body.BoxSize.y - _body.SkinWidth * 2f) / horizontalRaySpaceCount;
            _verticalRaySpace = (_body.BoxSize.x - _body.SkinWidth * 2f) / verticalRaySpaceCount;
        }

        private void UpdateBodyInfo()
        {
            _bodyInfo.skinWidth = _body.SkinWidth;
            _bodyInfo.slopeLimit = _body.SlopeLimit;

            // Note: The pivot is at bottom center
            _bodyInfo.position = _body.Position;
            Vector2 boxSize = _body.BoxSize;
            float extentX = boxSize.x * 0.5f - _bodyInfo.skinWidth;
            _bodyInfo.bottomLeft = _bodyInfo.position + new Vector2(-extentX, _bodyInfo.skinWidth);
            _bodyInfo.bottomRight = _bodyInfo.position + new Vector2(extentX, _bodyInfo.skinWidth);
            _bodyInfo.topLeft = _bodyInfo.position + new Vector2(-extentX, boxSize.y - _bodyInfo.skinWidth);
            _bodyInfo.topRight = _bodyInfo.position + new Vector2(extentX, boxSize.y - _bodyInfo.skinWidth);
        }

        private void HorizontalMove(ref Vector2 movement)
        {
            float directionX = Mathf.Sign(movement.x);
            float rayDistance = (directionX > 0f ? movement.x : -movement.x) + _body.SkinWidth;

            for (var i = 0; i < _horizontalRayCount; i++)
            {
                Vector2 rayOrigin = directionX > 0f ? _bodyInfo.bottomRight : _bodyInfo.bottomLeft;
                rayOrigin.y += _horizontalRaySpace * i;

                RaycastResult result = _body.RaycastObstacle(rayOrigin, Vector2.right * directionX, rayDistance);
                if (result.isHit)
                {
                    float slopeAngle = Vector2.Angle(result.normal, Vector2.up);
                    if (i == 0 && slopeAngle <= _bodyInfo.slopeLimit)
                    {
                        // Edge Case: If encounter another slope ascending while descending, reset the movement
                        if (_collisionInfo.slopeDescending)
                        {
                            _collisionInfo.slopeDescending = false;
                            movement = _originalMovement;
                        }

                        float distanceToSlopeStart = 0f;
                        if (slopeAngle != _previousSlope)
                        {
                            distanceToSlopeStart = result.distance - _body.SkinWidth;
                            movement.x -= distanceToSlopeStart * directionX;
                        }

                        AscendSlope(ref movement, slopeAngle);
                        movement.x += distanceToSlopeStart * directionX;
                    }

                    if (!(_collisionInfo.slopeAscending || _collisionInfo.slopeAscendingAir) || slopeAngle > _bodyInfo.slopeLimit)
                    {
                        // Edge Case: When being distance 0 with the obstacle, the value might be very small (less than 1e-5, due to floating error).
                        // If we don't discard it, it will make the character move forward "very slowly".
                        // When accumulate to enough value, it result "very small and unseen" pushing back, making unexpected collision info.
                        float movementX = result.distance - _body.SkinWidth;
                        if (MathUtility.ApproximatelyZero(movementX))
                        {
                            movementX = 0f;
                        }

                        movement.x = movementX * directionX;
                        rayDistance = result.distance;

                        // Edge Case: Hit the wall when ascending on the slope
                        if (_collisionInfo.slopeAscending)
                        {
                            movement.y = Mathf.Tan(_currentSlope * Mathf.Deg2Rad) * Mathf.Abs(movement.x);
                        }

                        _collisionInfo.left = directionX < 0f;
                        _collisionInfo.right = directionX > 0f;

                        if (movementX == 0f)
                        {
                            break;
                        }
                    }
                }
            }
        }

        #region Pinch
        private void HorizontalPinch(Vector2 originalMovement, ref Vector2 movement)
        {
            // If movement not changed, no pinch.
            if (MathUtility.Approximately(movement.x, originalMovement.x))
            {
                return;
            }

            // If is on slope, no pinch.
            if ((_collisionInfo.slopeAscending || _collisionInfo.slopeAscendingAir) && _currentSlope <= _bodyInfo.slopeLimit)
            {
                return;
            }

            ProbePinchBlock(originalMovement);

            // If center is blocked, treat as all blocked
            if (_collisionInfo.pinchCenterBlocked)
            {
                return;
            }

            // If bottom and top are blocked, treat as all blocked
            if (_collisionInfo.pinchBottomBlocked && _collisionInfo.pinchTopBlocked)
            {
                return;
            }

            // If the main ray has no blocked, since x-movement is already changed,
            // there must be a small wall to block the movement.
            // Use double pinch
            if (!_collisionInfo.pinchBottomBlocked && !_collisionInfo.pinchTopBlocked)
            {
                bool isGroundPinchSuccess = PinchGroundFromCenter(originalMovement, out float offsetYToTopGround);
                bool isCeilingPinchSuccess = PinchCeilingFromCenter(originalMovement, out float offsetYToBottomCeiling);
                if (isGroundPinchSuccess && isCeilingPinchSuccess)
                {
                    float offsetY = Mathf.Min(offsetYToTopGround, offsetYToBottomCeiling);
                    movement.x = originalMovement.x;
                    movement.y = offsetY;
                }
                else if (isGroundPinchSuccess)
                {
                    movement.x = originalMovement.x;
                    movement.y = offsetYToTopGround;
                }
                else if (isCeilingPinchSuccess)
                {
                    movement.x = originalMovement.x;
                    movement.y = offsetYToBottomCeiling;
                }
            }
            else
            {
                // We have only 1 blocked to treat these cases
                if (_collisionInfo.pinchBottomBlocked)
                {
                    bool success = PinchGroundFromCenter(originalMovement, out float offsetY);
                    if (success)
                    {
                        movement.x = originalMovement.x;
                        if (movement.y < offsetY)
                        {
                            movement.y = offsetY;
                        }
                    }
                }
                else if (_collisionInfo.pinchTopBlocked)
                {
                    bool success = PinchCeilingFromCenter(originalMovement, out float offsetY);
                    if (success)
                    {
                        movement.x = originalMovement.x;
                        if (movement.y > offsetY)
                        {
                            movement.y = offsetY;
                        }
                    }
                }
            }
        }

        private void ProbePinchBlock(Vector2 movement)
        {
            float directionX = Mathf.Sign(movement.x);
            float rayDistance = (directionX > 0f ? movement.x : -movement.x) + _body.SkinWidth;

            Vector2 centerRayOrigin = directionX > 0f ?
                (_bodyInfo.bottomRight + _bodyInfo.topRight) * 0.5f :
                (_bodyInfo.bottomLeft + _bodyInfo.topLeft) * 0.5f;
            _collisionInfo.pinchCenterBlocked = ProbeRay(centerRayOrigin, Vector2.right * directionX, rayDistance);

            Vector2 bottomRayOrigin = directionX > 0f ? _bodyInfo.bottomRight : _bodyInfo.bottomLeft;
            _collisionInfo.pinchBottomBlocked = ProbeRay(bottomRayOrigin, Vector2.right * directionX, rayDistance);

            Vector2 topRayOrigin = directionX > 0f ? _bodyInfo.topRight : _bodyInfo.topLeft;
            _collisionInfo.pinchTopBlocked = ProbeRay(topRayOrigin, Vector2.right * directionX, rayDistance);

            bool ProbeRay(Vector2 rayOrigin, Vector2 direction, float distance)
            {
                RaycastResult result = _body.RaycastObstacle(rayOrigin, direction, distance);
                return result.isHit;
            }
        }

        private bool PinchGroundFromCenter(Vector2 movement, out float offsetY)
        {
            float directionX = Mathf.Sign(movement.x);
            float rayDistance = (directionX > 0f ? movement.x : -movement.x) + _body.SkinWidth;
            Vector2 rayOrigin = directionX > 0f ?
                (_bodyInfo.bottomRight + _bodyInfo.topRight) * 0.5f :
                (_bodyInfo.bottomLeft + _bodyInfo.topLeft) * 0.5f;

            RaycastResult pinchingResult = _body.RaycastObstacle(rayOrigin, Vector2.right * directionX, rayDistance);
            if (!pinchingResult.isHit)
            {
                // Raycast to check the ground Y position
                Vector2 groundRayOrigin = rayOrigin;
                groundRayOrigin.x += movement.x;

                RaycastResult groundResult = _body.RaycastObstacle(groundRayOrigin, Vector2.down, _body.BoxSize.y * 0.5f);
                if (groundResult.isHit)
                {
                    RaycastResult ceilingResult = _body.RaycastObstacle(groundRayOrigin, Vector2.up, _body.BoxSize.y);
                    if (!ceilingResult.isHit || (ceilingResult.point.y - groundResult.point.y) >= _body.BoxSize.y)
                    {
                        offsetY = _body.BoxSize.y * 0.5f - groundResult.distance;
                        return true;
                    }
                }
            }

            offsetY = 0f;
            return false;
        }

        private bool PinchCeilingFromCenter(Vector2 movement, out float offsetY)
        {
            float directionX = Mathf.Sign(movement.x);
            float rayDistance = (directionX > 0f ? movement.x : -movement.x) + _body.SkinWidth;
            Vector2 rayOrigin = directionX > 0f ?
                (_bodyInfo.bottomRight + _bodyInfo.topRight) * 0.5f :
                (_bodyInfo.bottomLeft + _bodyInfo.topLeft) * 0.5f;

            RaycastResult pinchingResult = _body.RaycastObstacle(rayOrigin, Vector2.right * directionX, rayDistance);
            if (!pinchingResult.isHit)
            {
                // Raycast to check the ceiling Y position
                Vector2 ceilingRayOrigin = rayOrigin;
                ceilingRayOrigin.x += movement.x;

                RaycastResult ceilingResult = _body.RaycastObstacle(ceilingRayOrigin, Vector2.up, _body.BoxSize.y * 0.5f);
                if (ceilingResult.isHit)
                {
                    RaycastResult groundResult = _body.RaycastObstacle(ceilingRayOrigin, Vector2.down, _body.BoxSize.y);
                    if (!groundResult.isHit || (ceilingResult.point.y - groundResult.point.y) >= _body.BoxSize.y)
                    {
                        offsetY = -(_body.BoxSize.y * 0.5f - ceilingResult.distance);
                        return true;
                    }
                }
            }

            offsetY = 0f;
            return false;
        }
        #endregion

        private void VerticalMove(ref Vector2 movement)
        {
            float directionY = Mathf.Sign(movement.y);
            float rayDistance = (directionY > 0f ? movement.y : -movement.y) + _body.SkinWidth;

            for (var i = 0; i < _verticalRayCount; i++)
            {
                Vector2 rayOrigin = directionY > 0f ? _bodyInfo.topLeft : _bodyInfo.bottomLeft;
                rayOrigin.x += _verticalRaySpace * i + movement.x;

                RaycastResult result = _body.RaycastObstacle(rayOrigin, Vector2.up * directionY, rayDistance);
                if (result.isHit)
                {
                    // Edge Case: See the corresponding comment in horizontal method
                    float movementY = result.distance - _body.SkinWidth;
                    if (MathUtility.ApproximatelyZero(movementY))
                    {
                        movementY = 0f;
                    }

                    movement.y = movementY * directionY;
                    rayDistance = result.distance;

                    // Edge Case: Hit the ceiling when ascending on the slope
                    if (_collisionInfo.slopeAscending)
                    {
                        movement.x = movement.y / Mathf.Tan(_currentSlope * Mathf.Deg2Rad) * Mathf.Sign(movement.x);
                    }

                    _collisionInfo.bottom = directionY < 0f;
                    _collisionInfo.top = directionY > 0f;

                    if (movementY == 0f)
                    {
                        break;
                    }
                }
            }

            // Edge Case: When moving to the different slope, we need to fix x-movement.
            if (_collisionInfo.slopeAscending)
            {
                float directionX = Mathf.Sign(movement.x);
                Vector2 rayOrigin = directionX > 0f ? _bodyInfo.bottomRight : _bodyInfo.bottomLeft;

                // First raycast: Check if we encounter steeper slope
                rayOrigin.y += movement.y;
                rayDistance = (directionX > 0f ? movement.x : -movement.x) + _body.SkinWidth;
                RaycastResult firstResult = _body.RaycastObstacle(rayOrigin, Vector2.right * directionX, rayDistance);
                if (firstResult.isHit)
                {
                    float slopeAngle = Vector2.Angle(firstResult.normal, Vector2.up);
                    if (slopeAngle != _currentSlope)
                    {
                        movement.x = (firstResult.distance - _bodyInfo.skinWidth) * directionX;
                        _currentSlope = slopeAngle;
                        return;
                    }
                }

                // Second raycast: We might encounter smoother slope or flat ground
                rayOrigin.x += movement.x;
                rayDistance = movement.y + _body.SkinWidth;
                RaycastResult secondResult = _body.RaycastObstacle(rayOrigin, Vector2.down, rayDistance);
                if (secondResult.isHit)
                {
                    float slopeAngle = Vector2.Angle(secondResult.normal, Vector2.up);
                    if (slopeAngle != _currentSlope)
                    {
                        movement.y -= secondResult.distance - _bodyInfo.skinWidth;
                        _currentSlope = slopeAngle;
                    }
                }
            }
        }

        private void AscendSlope(ref Vector2 movement, float slopeAngle)
        {
            // Note: Keep the x-movement
            float slopeMovementY = Mathf.Abs(movement.x) * Mathf.Tan(slopeAngle * Mathf.Deg2Rad);

            // Jumping above the slope
            if (movement.y > 0f)
            {
                movement.y += slopeMovementY;
                _collisionInfo.slopeAscendingAir = true;
                return;
            }
            else
            {
                movement.y = slopeMovementY;
                _collisionInfo.bottom = true;
                _collisionInfo.slopeAscending = true;
                _currentSlope = slopeAngle;
            }
        }

        private void PreDescendSlope(ref Vector2 movement)
        {
            float directionX = Mathf.Sign(movement.x);
            Vector2 rayOrigin = directionX > 0f ? _bodyInfo.bottomLeft : _bodyInfo.bottomRight;  // Note: Do raycast from back

            RaycastResult result = _body.RaycastObstacle(rayOrigin, Vector2.down, Mathf.Infinity);
            if (result.isHit)
            {
                float slopeAngle = Vector2.Angle(result.normal, Vector2.up);
                if (slopeAngle != 0f && slopeAngle <= _bodyInfo.slopeLimit)
                {
                    if (Mathf.Sign(result.normal.x) == directionX)
                    {
                        // Note: Keep the x-movement
                        float slopeMovementY = Mathf.Abs(movement.x) * Mathf.Tan(slopeAngle * Mathf.Deg2Rad);
                        if (result.distance - _bodyInfo.skinWidth <= slopeMovementY)
                        {
                            movement.x = Mathf.Abs(movement.x) * directionX;
                            movement.y -= slopeMovementY;

                            _collisionInfo.bottom = true;
                            _collisionInfo.slopeDescending = true;
                            _currentSlope = slopeAngle;
                        }
                    }
                }
            }
        }

        // Edge Case: Check if we are going to descend
        private void PostDescendSlope(ref Vector2 movement)
        {
            if (_collisionInfo.slopeDescending)
            {
                return;
            }

            if (_collisionInfo.bottom)
            {
                return;
            }

            if (movement.y > 0f)
            {
                return;
            }

            float directionX = Mathf.Sign(movement.x);
            Vector2 rayOrigin = directionX > 0f ? _bodyInfo.bottomLeft : _bodyInfo.bottomRight;  // Note: Do raycast from back
            rayOrigin += movement;
            float rayDistance = Mathf.Abs(movement.x) * Mathf.Tan(_bodyInfo.slopeLimit * Mathf.Deg2Rad) + _bodyInfo.skinWidth;

            RaycastResult result = _body.RaycastObstacle(rayOrigin, Vector2.down, rayDistance);
            if (result.isHit)
            {
                float slopeAngle = Vector2.Angle(result.normal, Vector2.up);
                if (slopeAngle != 0f && slopeAngle <= _bodyInfo.slopeLimit)
                {
                    if (Mathf.Sign(result.normal.x) == directionX)
                    {
                        movement.y -= result.distance - _bodyInfo.skinWidth;
                        _collisionInfo.bottom = true;
                        _collisionInfo.slopeDescending = true;
                        _currentSlope = slopeAngle;
                    }
                }
            }
        }
    }
}
