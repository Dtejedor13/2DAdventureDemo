using UnityEngine;


namespace Assets.Scripts.Core
{
    public class GroundCheck : MonoBehaviour
    {
        [SerializeField] private LayerMask enviromentLayerMask;
        public bool IsGrounded;

        private void OnTriggerStay2D(Collider2D collider)
        {
            IsGrounded = collider!= null && ((1 << collider.gameObject.layer) & enviromentLayerMask) != 0;
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            IsGrounded = false;
        }
    }
}
