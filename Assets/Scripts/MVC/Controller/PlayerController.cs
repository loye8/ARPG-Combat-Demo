using System.Collections;
using UnityEngine;
using ARPGCombat.Core;
using ARPGCombat.Data;
using ARPGCombat.MVC.Model;
using ARPGCombat.Utils;
using ARPGCombat.Gameplay;

namespace ARPGCombat.MVC.Controller
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(PlayerInput))]
    [RequireComponent(typeof(Animation))]
    public class PlayerController : MonoBehaviour
    {
        [Header("配置")]
        [Tooltip("SO 角色配置")]
        public CharacterConfig config;

        [Tooltip("主摄像机(如 Cinemachine 则填 Brain 挂的相机)")]
        public Camera cam;

        [Tooltip("敌方层掩码")]
        public LayerMask enemyLayerMask;

        private CharacterController _cc;
        private PlayerInput _input;
        private Animator _anim;
        private CharacterModel _model;
        private int _animIdMoveAmount;
        private int _animIdAttack;

        private Coroutine _activeAttackCoroutine;
        private Vector3 _velocity;

        private void Awake()
        {
            _cc = GetComponent<CharacterController>();
            _input = GetComponent<PlayerInput>();
            _anim = GetComponent<Animator>();

            _animIdMoveAmount = Animator.StringToHash("MoveAmount");
            _animIdAttack = Animator.StringToHash("Attack");

            _model = new CharacterModel(GetInstanceID(), config);

            var playerCreateData = new { instanceId = _model.InstanceId, transform = transform, model = _model };
            EventCenter.Instance?.Emit("PlayerCreated", playerCreateData);
        }

        private void Start()
        {
            if (cam == null) cam = Camera.main;
        }

        private void Update()
        {
            if (_model.IsDead) return;

            HandleMove();
            HandleRotation();
            HandleAttack();

        }

        private void OnDestroy()
        {
            if (_activeAttackCoroutine != null) StopCoroutine(_activeAttackCoroutine);
        }

        private void HandleMove()
        {
            Vector2 axis = _input.MoveAxis;
            float speed = _model.MoveSpeed;

            Vector3 moveDir = Vector3.zero;

            if (axis.sqrMagnitude > 0.01f)
            {
                Vector3 camFwd = cam.transform.forward.Flatten().normalized;
                Vector3 camRight = cam.transform.right.Flatten().normalized;
                moveDir = (camFwd * axis.y + camRight * axis.x).normalized;

                _cc.Move(moveDir * speed * Time.deltaTime);
            }

            if (_cc.isGrounded && _velocity.y < 0)
                _velocity.y = -2f;
            else
                _velocity.y += Physics.gravity.y * Time.deltaTime;

            _cc.Move(_velocity * Time.deltaTime);

            float animAmount = axis.sqrMagnitude > 0.01f ? 1f : 0f;
            _anim.SetFloat(_animIdMoveAmount, animAmount);
        }

        private void HandleRotation()
        {
            Vector3 targePoint = cam.ScreenPointToGroundPlane(Input.mousePosition);
            Vector3 toTarget = (targePoint - transform.position).Flatten();

            if (toTarget.sqrMagnitude > 0.01f)
            {
                Quaternion target = Quaternion.LookRotation(toTarget);
                float rotSpeed = _model.RotationSpeed;
                transform.rotation = Quaternion.Slerp(transform.rotation, target, rotSpeed * Time.deltaTime);
            }
        }

        private void HandleAttack()
        {
            if (!_input.AttackPressed) return;
            if (!_model.CanAttack(Time.time)) return;
            if (_activeAttackCoroutine != null) return;

            _model.MarkAttacked(Time.time);
            _anim.SetTrigger(_animIdAttack);
            EventCenter.Instance?.Emit("PlayerAttackStarted", _model.InstanceId);
            _activeAttackCoroutine = StartCoroutine(AttackRoutine());
        }

        private IEnumerator AttackRoutine()
        {
            yield return new WaitForSeconds(_model.AttackWindup);
            Vector3 hitCenter = transform.position + transform.forward * 1f;

            Collider[] hits = Physics.OverlapSphere(
                position: hitCenter,
                radius: _model.AttackRange,
                layerMask: enemyLayerMask
                );

            foreach (var hit in hits)
            {
                Vector3 hitPoint = hit.transform.position;
                var hitData = new
                {
                    instigatorId = _model.InstanceId,
                    targetCollider = hit,
                    rawDamage = _model.AttackPower,
                    isCritical = false
                };
                EventCenter.Instance?.Emit("PlayerAttackHit", hitData);
            }

            EventCenter.Instance?.Emit("PlayerAttackEnded", _model.InstanceId);
            _activeAttackCoroutine = null;
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (config == null) return;
            Gizmos.color = Color.red;
            Vector3 hitCenter = transform.position + transform.forward * 1f;
            Gizmos.DrawWireSphere(hitCenter, config.attackRange);

            Gizmos.color = Color.blue;
            Gizmos.DrawRay(transform.position, transform.forward * 2f);
        }
#endif
    }
}