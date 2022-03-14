using UnityEngine;

namespace LowCloud.Reldawin
{
    public class OtherPlayerCharacter : Entity
    {
        private void Awake()
        {
            MovementSpeed = WalkSpeed;
        }

        public override void Interrupt( params object[] args )
        {
            base.Interrupt();

            Interacting = false;
        }

        public void Setup( ClientParams opc )
        {
            transform.position = MyMath.CellToIsometric( opc.GetCellPos );
            gameObject.name = string.Format( "OPC: {0} [{1}]", opc.username, opc.ID );

            CellPositionInWorld = opc.GetCellPos;
            MovingToward = transform.position;
            InLastChunk = InCurrentChunk;
            DatabaseID = (ushort)opc.ID;

            //set movement mode
            Running = opc.Running;
            Swimming = opc.Swimming;

            //set movement speed
            MovementSpeed = opc.Running ? RunSpeed : WalkSpeed;
            MovementSpeed = opc.Swimming ? WalkSpeed : MovementSpeed;

            animationConroller.ToggleRun(Running);
        }

        public void SetPath(Vector2 pointClicked, bool hasInventorySpace)
        {
            HasInventorySpace = hasInventorySpace;
        
            Vector2Int cellClicked = MyMath.IsometricToCell( pointClicked );

            base.pointClicked = pointClicked;

            path = Pathfinder.GetPath( CellPositionInWorld, cellClicked, out lastNodeOccupied );

            //stop whatever you were in the middle of
            Interrupt();

            if ( path == null )
            {
                SetTargetPosition( pointClicked );

                return;
            }

            if ( path.Count == 0 )
            {
                return;
            }

            MoveToNextNode();
        }

        public void Destroy()
        {
            Destroy( gameObject );
        }
    }
}
