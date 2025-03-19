using System.Collections.Generic;
using Stride.Input;
using Stride.Engine;
using Stride.Navigation;
using Stride.Graphics;
using Stride.Core.Mathematics;
using Stride.Physics;

namespace GenericNewGame
{
    public class Navigation : SyncScript
    {
        public float movementSpeed = 1.5f;
        public Entity character;
        public Entity sphereToClone;

        private NavigationComponent navigation;
        private List<Entity> waypointSpheres = [];
        private List<Vector3> waypoints = [];
        private int waypointIndex = 0;

        public override void Start()
        {
            navigation = character.Get<NavigationComponent>();
        }

        public override void Update()
        {
            if (Input.IsMouseButtonPressed(MouseButton.Left))
            {
                CleanupExistingPath();
                SetNewTarget();
            }

            UpdateMovement();
        }

        private void UpdateMovement()
        {
            if (waypoints.Count == 0)
            {
                return;
            }
            Vector3 nextWaypoint = waypoints[waypointIndex];
            Vector3 currentPosition = character.Transform.WorldMatrix.TranslationVector;
            float deltaTime = (float)Game.UpdateTime.Elapsed.TotalSeconds;
            float distance = Vector3.Distance(currentPosition, nextWaypoint);

            if (distance > 0.1f)
            {
                var velocity = nextWaypoint - currentPosition;
                velocity.Normalize();
                velocity *= deltaTime * movementSpeed;

                character.Transform.Position += velocity;
            }
            else
            {
                if (waypointIndex + 1 < waypoints.Count)
                {
                    waypointIndex++;
                }
                else
                {
                    CleanupExistingPath();
                }
            }
        }

        private void CleanupExistingPath()
        {
            foreach (Entity sphere in waypointSpheres)
            {
                Entity.Scene.Entities.Remove(sphere);
            }

            waypointSpheres.Clear();
            waypoints.Clear();
        }

        private void SetNewTarget()
        {
            Texture backBuffer = GraphicsDevice.Presenter.BackBuffer;
            Viewport viewPort = new Viewport(0, 0, backBuffer.Width, backBuffer.Height);
            CameraComponent camera = Entity.Get<CameraComponent>();
            Vector3 nearPosition = viewPort.Unproject(new Vector3(Input.AbsoluteMousePosition, 0.0f), 
                camera.ProjectionMatrix, camera.ViewMatrix, Matrix.Identity);
            Vector3 farPosition = viewPort.Unproject(new Vector3(Input.AbsoluteMousePosition, 1.0f), 
                camera.ProjectionMatrix, camera.ViewMatrix, Matrix.Identity);
            HitResult hitResult = this.GetSimulation().Raycast(nearPosition, farPosition);

            if (hitResult.Succeeded)
            {
                if (navigation.TryFindPath(hitResult.Point, waypoints))
                {
                    waypointIndex = 0;
                    foreach (Vector3 waypoint in waypoints)
                    {
                        Entity sphereClone = sphereToClone.Clone();
                        sphereClone.Transform.Position = waypoint;

                        waypointSpheres.Add(sphereClone);
                        Entity.Scene.Entities.Add(sphereClone);
                    }
                }
            }
        }
    }
}
