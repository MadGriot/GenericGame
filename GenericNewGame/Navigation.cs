using System.Collections.Generic;
using Stride.Input;
using Stride.Engine;
using Stride.Navigation;

namespace GenericNewGame
{
    public class Navigation : SyncScript
    {
        public Entity character;
        public Entity sphereToClone;

        private NavigationComponent navigation;
        private List<Entity> waypointSpheres;

        public override void Start()
        {
            navigation = character.Get<NavigationComponent>();
        }

        public override void Update()
        {
            if (Input.IsMouseButtonPressed(MouseButton.Left))
            {
                SetNewTarget();
            }
        }

        private void SetNewTarget()
        {

        }
    }
}
