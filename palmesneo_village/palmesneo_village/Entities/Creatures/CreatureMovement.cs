using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace palmesneo_village
{
    public enum MovementState
    {
        Success,
        Running,
        Completion
    }

    public class CreatureMovement : Entity
    {
        public MovementState State { get; private set; } = MovementState.Success;

        public Direction Direction { get; private set; }

        private float speed;

        private List<PathNode> path;
        private PathNode currentNode;
        private PathNode nextNode;

        private float movementPercent = 0;

        private float currentRotation = 0.0f;
        private float targetRotation = 0.0f;
        private int[] rotationSetInDegrees = [-15, 15];

        public CreatureMovement(float speed)
        {
            this.speed = speed;
        }

        public override void Update()
        {
            base.Update();

            switch(State)
            {
                case MovementState.Running:
                    {
                        if(nextNode == currentNode)
                        {
                            if(path.Count == 0)
                            {
                                path = null;

                                State = MovementState.Success;
                            }
                            else
                            {
                                nextNode = path[path.Count - 1];
                                path.RemoveAt(path.Count - 1);

                                if (currentNode.X < nextNode.X)
                                {
                                    Direction = Direction.Right;
                                }
                                else if (currentNode.X > nextNode.X)
                                {
                                    Direction = Direction.Left;
                                }

                                targetRotation = MathHelper.ToRadians(Calc.Random.Choose(rotationSetInDegrees));
                            }
                        }

                        UpdateMovement();
                    }
                    break;
                case MovementState.Completion:
                    {
                        if (nextNode != currentNode)
                        {
                            UpdateMovement();
                        }
                        else
                        {
                            State = MovementState.Success;
                        }
                    }
                    break;
            }
        }

        private void UpdateMovement()
        {
            float distToTravel = Vector2.Distance(currentNode.ToVector2(), nextNode.ToVector2());

            float finalSpeed = speed * (100f - nextNode.MovementPenalty) / 100f;

            float distThisFrame = finalSpeed * Engine.GameDeltaTime;

            float percThisFrame = distThisFrame / distToTravel;

            if (float.IsNaN(percThisFrame))
                percThisFrame = 0;

            movementPercent += percThisFrame;

            if (movementPercent >= 1)
            {
                currentNode = nextNode;
                movementPercent = 0;

                currentRotation = 0;
            }

            Parent.LocalPosition = Vector2.Lerp(currentNode.ToVector2(), nextNode.ToVector2(), movementPercent) * Engine.TILE_SIZE;

            if (movementPercent <= 0.5f)
            {
                currentRotation = MathHelper.Lerp(0, targetRotation, movementPercent / 0.5f);
            }
            else if (movementPercent > 0.5f)
            {
                currentRotation = MathHelper.Lerp(targetRotation, 0, (movementPercent - 0.5f) / 0.5f);
            }
        }

        public void SetPath(List<PathNode> path)
        {
            this.path = path;

            State = MovementState.Running;
        }

        public void TeleportTo(PathNode node)
        {
            currentNode = nextNode = node;

            Parent.LocalPosition = currentNode.ToVector2() * Engine.TILE_SIZE;
        }

        public void Complete()
        {
            path = null;
            State = MovementState.Completion;
        }

        public float GetRotation()
        {
            return currentRotation;
        }
    }
}
