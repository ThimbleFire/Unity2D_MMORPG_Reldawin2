using UnityEngine;
namespace AlwaysEast
{
    public class CameraFollow : MonoBehaviour
    {
        public enum CameraMode
        {
            NoOffset,
            InventoryOffset
        };

        private Vector3 startingPosition;
        public Transform followTarget;
        private Vector3 targetPos;
        public float moveSpeed;
        public CameraMode cameraMode = CameraMode.NoOffset;

        private void Start() {
            startingPosition = transform.position;
        }

        private void Update() {
            if( followTarget != null ) {
                targetPos = new Vector3( followTarget.position.x, followTarget.position.y, transform.position.z );
                if( cameraMode == CameraMode.InventoryOffset )
                    targetPos += Vector3.right * 1.8f;
                Vector3 velocity = (targetPos - transform.position) * moveSpeed;
                transform.position = Vector3.SmoothDamp( transform.position, targetPos, ref velocity, 0.25f, Time.deltaTime );
            }
        }

        public void SetCameraMode( CameraMode mode ) {
            cameraMode = mode;
        }
    }
}