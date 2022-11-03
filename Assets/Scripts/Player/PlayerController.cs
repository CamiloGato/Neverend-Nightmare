using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Pickable;
using TimeLoop;
using Unity.Collections;
using UnityEngine;
using Volume;

namespace Player
{

    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CapsuleCollider))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Body")]
        [SerializeField] private Animator animator;
        private Rigidbody _rigidbody;
        private CapsuleCollider _capsuleCollider;
        
        [Header("Player Movement")]   
        [SerializeField] private float speed = 5f;
        [SerializeField] private float jumpForce = 5f;
        [SerializeField] private float jumpDurationShake = 0.3f;
        [SerializeField] private float jumpIntensityShake = 0.5f;

        [Header("Physics")]
        [SerializeField] private Vector3 gravity = Physics.gravity;
        [SerializeField] private float gravityMultiplier = 2f;
        [SerializeField] private float fallFreeDistance = 10f;
        [SerializeField] private float groundDistance = 1.21f;
        [SerializeField] private float velMaxFall = 20f;
        [SerializeField] private Rigidbody pelvis;
        [SerializeField] private float amplifyEnemyPush = 1f;

        [Header("Camera Settings")]
        [SerializeField] private Vector3 cameraOffset = new Vector3(0, 1.5f, 0);
        [SerializeField] private Transform targetTransform;
        [SerializeField] private Transform bodyTransform;
        [SerializeField] private float cameraSensitivity = 100f;
        [SerializeField] private float cameraRotationSmoothing = 0.1f;
        [SerializeField] private float slowMotionDuration = 1f;
        [SerializeField] private float slowMotionScale = 0.2f;

        [Header("Ray/Sphere Settings")]
        [SerializeField] private float raycastDistance = 1f;
        [SerializeField] private float sphereCastRadius = 0.5f;
        [SerializeField] private LayerMask raycastLayerClimb;
        [SerializeField] private LayerMask raycastLayerPickup;
        [SerializeField] private LayerMask raycastLayerEnemy;

        [Header("Mechanics Settings")]
        [SerializeField] private float climbTimeSmooth = 0.1f;
        [SerializeField] private Vector3 climbVectorPosition = new Vector3(0, 1, 0);
        [SerializeField] private float multiplierPositionClimb = 2f;
        
        [Header("Debug")]
        [SerializeField] private bool isGrounded;
        [SerializeField] private bool doingAction;
        [SerializeField] private bool isJumping;
        
        private List<Rigidbody> _rigidbodies;
        private List<Collider> _rigidbodyCollider;
        private Vector3 _startPosition;
        private PickableController _pickableController;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _capsuleCollider = GetComponent<CapsuleCollider>();
            _rigidbodies = GetComponentsInChildren<Rigidbody>().ToList();
            _rigidbodyCollider = GetComponentsInChildren<Collider>().ToList();
            Physics.gravity = gravity * gravityMultiplier;
            _startPosition = transform.position;
        }

        private void Start()
        {
            if (_rigidbodyCollider.Contains(_capsuleCollider)) _rigidbodyCollider.Remove(_capsuleCollider);
            if (_rigidbodies.Contains(_rigidbody)) _rigidbodies.Remove(_rigidbody);
            DeactivateRagdoll();
        }

        private void Update()
        {
            PlayerMovement();
            targetTransform.position = transform.position + cameraOffset;
            if (TimeLoopController.Instance.die){
                StartCoroutine(PlayerChangePosition(_startPosition));
            }

            if (TimeLoopController.Instance.leftTime < 1f)
            {
                PlayerDieEffects();
            }
        }

        private void FixedUpdate()
        {
            
            if (_rigidbody.velocity.magnitude > velMaxFall) _rigidbody.velocity = Vector3.ClampMagnitude(_rigidbody.velocity, velMaxFall);
            pelvis.velocity = Vector3.ClampMagnitude(_rigidbody.velocity, velMaxFall);

        }

        private void LateUpdate()
        {
            RayCastCol();
            EnemyDetection();
            PickupObjectDetection();
            AnimatorStates();
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Ground"))
            {
                isGrounded = true;
                CinemachineEffects.Instance.ShakeCamera(jumpIntensityShake, jumpDurationShake);
            }

            if (collision.gameObject.CompareTag("Object"))
            {
                StartCoroutine(PickupObject(collision.gameObject.GetComponent<PickableController>()));
            }

            if (collision.gameObject.CompareTag("Enemy"))
            {
                _rigidbody.AddForce(-transform.forward.normalized * (jumpForce * amplifyEnemyPush), ForceMode.Impulse);
                PlayerDieEffects();
            }

            if (collision.gameObject.CompareTag("Block/Wall"))
            {
                if (!isGrounded) _rigidbody.AddForce(-transform.forward.normalized * jumpForce/2, ForceMode.Impulse);
            }

        }

        private void OnCollisionExit(Collision collision)
        {
            if (collision.gameObject.CompareTag("Ground"))
            {
                isGrounded = false;
            }
        }
        
        
        /*
        <Summary>
            <Description>
                This method is used to control the player's movement.
                Note: This method is called in the Update method.
                Required: Target, Body, Speed, JumpForce, Rigidbody, Animator.
            </Description>
        </Summary>
        */
        private void PlayerMovement()
        {
            // Get Imput from the player
            float horizontalInput = Input.GetAxisRaw("Horizontal");
            float verticalInput = Input.GetAxisRaw("Vertical");
            float mouseX = Input.GetAxis("Mouse X");
            
            // Get the direction of the player and transform it to the camera's direction.
            Vector3 movement = new Vector3(horizontalInput, 0, verticalInput);
            Vector3 movementDirection = transform.TransformDirection(movement).normalized;
            
            float horizontalRotation = mouseX * cameraSensitivity * Time.deltaTime;
            targetTransform.Rotate(0, horizontalRotation, 0);

            // Move the forward of the body and the camera to the direction of the player and rotate the body.
            if ( (verticalInput != 0 || horizontalInput != 0) && isGrounded && !doingAction)
            {
                _rigidbody.velocity = movementDirection * speed;
                Vector3 forward = targetTransform.forward;
                Vector3 bodyTargetDirection = ( verticalInput*forward + movementDirection ).normalized; 
                transform.forward = Vector3.Lerp(transform.forward, forward, cameraRotationSmoothing);
                bodyTransform.forward = Vector3.Lerp(bodyTransform.forward, bodyTargetDirection, cameraRotationSmoothing);
            }

            // Verify if the player get the input (space) to jump
            if (Input.GetKeyDown(KeyCode.Space) && isGrounded && !doingAction)
            {
                StartCoroutine(Jump());
            }

            // Make Wave
            if (Input.GetKeyDown(KeyCode.H))
            {
                StartCoroutine(Wave());
            }
            
        }

        private float DistanceToObjetive(LayerMask layerMaskObjetive)
        {
            Collider[] hitColliders = new Collider[3];
            int numColliders = Physics.OverlapSphereNonAlloc(transform.position, sphereCastRadius, hitColliders, layerMaskObjetive);
            float distance = 0;
            if (numColliders == 0) return -1;
            foreach (Collider col in hitColliders)
            {
                if (col == null) continue;
                float colDistance = sphereCastRadius - Vector3.Distance(transform.position, col.transform.position);
                if (colDistance > distance)
                {
                    distance = colDistance;
                }
            }

            return distance;
        }
        
        private void PickupObjectDetection()
        {
            float distance = DistanceToObjetive(raycastLayerPickup);

            if (distance >= 0)
            {
                VolumeController.Instance.volumeEffects.ChangeBloom(distance);
            }
        }
        
        private void EnemyDetection()
        {
            float distance = DistanceToObjetive(raycastLayerEnemy);
            // Apply Effects
            if (distance >= 0)
            {
                CinemachineEffects.Instance.ShakeCamera(distance, 3f);
                VolumeController.Instance.volumeEffects.EnemyTheme(distance);
            }
        }
        
        private void RayCastCol()
        {
            
            var startPoint = bodyTransform.position;
            var direction = bodyTransform.forward.normalized;
            if (Physics.Raycast(startPoint, direction, out var hitClimb, raycastDistance, raycastLayerClimb) && !doingAction)
            {
                if (hitClimb.distance < 0.6f)
                {
                    StartCoroutine(Climb(hitClimb));
                }
            }
            
            if (Physics.Raycast(startPoint, Vector3.down, out var hitFall, float.MaxValue, raycastLayerClimb) && !doingAction)
            {
                if (hitFall.distance > fallFreeDistance && !doingAction)
                {
                    StartCoroutine(FallFree());
                }

                if (hitFall.distance < groundDistance && !isJumping )
                {
                    isGrounded = true;
                }
                if (hitFall.distance > groundDistance && !isJumping)
                {
                    isGrounded = false;
                }
                
            }
            
        }

        private IEnumerator Jump()
        {
            isGrounded = false;
            isJumping = true;
            _rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            yield return new WaitUntil(() => isGrounded);
            isJumping = false;
        }
        
        private IEnumerator FallFree()
        {
            doingAction = true;
            ActivateRagdoll();
            yield return new WaitUntil(() => isGrounded);
            yield return new WaitForSeconds(2f);
            DeactivateRagdoll();
            doingAction = false;
        }
        
        private IEnumerator Wave()
        {
            doingAction = true;
            animator.SetTrigger("Wave");
            yield return new WaitForSeconds(2f);
            doingAction = false;
        }

        private IEnumerator Climb(RaycastHit hit)
        {
            doingAction = true;
            _rigidbody.useGravity = false;
            _capsuleCollider.enabled = false;
            
            animator.SetTrigger("Climbing");
            
            float multiplier = hit.distance * multiplierPositionClimb;
            // Y axis first
            Vector3 newPosition = new Vector3(transform.position.x, hit.point.y + climbVectorPosition.y,
                transform.position.z);
            yield return new WaitUntil(() =>
            {
                transform.position = Vector3.Lerp(transform.position, newPosition, climbTimeSmooth);
                return Vector3.Distance(transform.position, newPosition) < 0.5f;
            });
            
            // X and Z axis second
            newPosition = hit.point + transform.forward * multiplier + climbVectorPosition;
            yield return new WaitUntil(() =>
            {
                transform.position = Vector3.Lerp(transform.position, newPosition, climbTimeSmooth);
                return Vector3.Distance(transform.position, newPosition) < 0.5f;
            });

            _capsuleCollider.enabled = true;
            _rigidbody.useGravity = true;
            doingAction = false;
        }

        private IEnumerator PickupObject(PickableController obj)
        {
            doingAction = true;
            obj.PickUp();
            animator.SetTrigger("Pickup");
            yield return new WaitForSeconds(1f);
            doingAction = false;
        }

        // Control the animator states
        private void AnimatorStates()
        {
            animator.SetBool("Grounded", isGrounded);
            animator.SetFloat("MoveSpeed", _rigidbody.velocity.magnitude);
        }
        
        private void DeactivateRagdoll()
        {
            foreach (Rigidbody rigidbody in _rigidbodies)
            {
                rigidbody.isKinematic = true;
            }

            foreach (Collider collider1 in _rigidbodyCollider)
            {
                collider1.enabled = false;
            }
            animator.enabled = true;
            doingAction = false;
        }
        
        private void ActivateRagdoll()
        {
            foreach (Rigidbody rigidbody in _rigidbodies)
            {
                rigidbody.isKinematic = false;
            }
            foreach (Collider collider1 in _rigidbodyCollider)
            {
                collider1.enabled = true;
            }
            animator.enabled = false;
            doingAction = true;
        }

        private void PlayerDieEffects()
        {
            ActivateRagdoll();
            TimeLoopController.Instance.die = true;
            PickableCanvas.Instance.isDie = true;
            CinemachineEffects.Instance.SlowMotion(slowMotionScale, slowMotionDuration);
            StartCoroutine(PlayerChangePosition(_startPosition));
        }
        
        
        private IEnumerator PlayerChangePosition(Vector3 newPosition)
        {
            doingAction = true;
            Vector2 vignettePosition = new Vector2(3, 0.5f);
            yield return new WaitForSeconds(1f);
            // VolumeController.Instance.volumeEffects.ChangeVignette(1, 1, vignettePosition);
            transform.position = newPosition;
            doingAction = false;
            TimeLoopController.Instance.die = false;
            PickableCanvas.Instance.isDie = false;
            TimeLoopController.Instance.SetTime();
            VolumeController.Instance.volumeEffects.ChangeVignette(0);
            VolumeController.Instance.volumeEffects.ChangeColorAdjustments(0);
            DeactivateRagdoll();
        }
        
    }
}
