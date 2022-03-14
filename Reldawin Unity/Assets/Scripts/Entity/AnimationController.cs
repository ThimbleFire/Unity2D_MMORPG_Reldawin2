using UnityEngine;

namespace LowCloud.Reldawin
{
    /// <summary>
    /// Animated entities inherit this class
    /// </summary>
    public class AnimationController : MonoBehaviour
    {
        [SerializeField] protected Animator Animator;

        private int activeAnimationHash = int.MinValue;

        private readonly int moveXHash = Animator.StringToHash( "MoveX" );
        private readonly int moveYHash = Animator.StringToHash( "MoveY" );
        private readonly int movingHash = Animator.StringToHash( "Moving" );
        private readonly int runningHash = Animator.StringToHash( "Running" );
        private readonly int swimmingHash = Animator.StringToHash( "Swimming" );

        private readonly int woodcuttingHash = Animator.StringToHash( "Woodcutting" );
        private readonly int miningHash = Animator.StringToHash( "Mining" );
        private readonly int gatheringHash = Animator.StringToHash( "GatheringGrass" );

        private Vector2 LastMoveDirection { get; set; }
        private Vector2 MoveDirection { get; set; }

        public bool Running { get; set; }
        public bool Swimming { get; set; }
                
        public void ToggleRun(bool running)
        {
            Running = running;

            Animator.SetBool( runningHash, running );
        }

        public void ToggleSwimming(bool swimming)
        {
            Swimming = swimming;

            Animator.SetBool( swimmingHash, swimming );
        }

        public void Interrupt()
        {
            if(activeAnimationHash != 999)
                Animator.SetBool( activeAnimationHash, false );

            activeAnimationHash = 999;
        }

        public void Interact(Vector2Int doodadWorldCell, int doodadType)
        {
            switch ( doodadType )
            {
                case 0:
                case 1:
                case 2:
                case 3:
                case 4:
                    activeAnimationHash = gatheringHash;
                    break;
                case 5:
                    activeAnimationHash = woodcuttingHash;
                    break;
                case 6:
                    activeAnimationHash = miningHash;
                    break;
            }

            Animator.SetBool( activeAnimationHash, true );
        }

        public void FaceDirection( Vector2 worldDirection )
        {
            LastMoveDirection = transform.position - (Vector3)worldDirection;

            Animator.SetBool( movingHash, false );
            Animator.SetFloat( moveXHash, -LastMoveDirection.x );
            Animator.SetFloat( moveYHash, -LastMoveDirection.y );
        }

        public void OnAnimationDestinationMet()
        {
            LastMoveDirection = MoveDirection;

            Animator.SetBool( movingHash, false );
            Animator.SetFloat( moveXHash, -LastMoveDirection.x );
            Animator.SetFloat( moveYHash, -LastMoveDirection.y );
        }

        public void SetAnimationMoveDirection( Vector2 direction )
        {
            MoveDirection = direction;

            Animator.SetBool( movingHash, true );
            Animator.SetFloat( moveXHash, -direction.x );
            Animator.SetFloat( moveYHash, -direction.y );
        }
    }
}