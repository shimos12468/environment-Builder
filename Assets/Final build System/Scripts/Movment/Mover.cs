

using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.XR;

namespace RPG.Movment
{
    public class Mover : MonoBehaviour
    {
        private Animator _anim;
        private NavMeshAgent agent;
        [SerializeField] float maxSpeed = 6f;
        [SerializeField] float maxPathLength = 40f;
        void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            _anim = GetComponentInChildren<Animator>();
            _controller = GetComponent<CharacterController>();

        }


        void LateUpdate()
        {
            UpdateAnimator();
        }

        private void UpdateAnimator()
        {
            Vector3 velocity = agent.velocity;
            Vector3 localVelocity = transform.InverseTransformDirection(velocity);
            _anim.SetFloat("Speed", localVelocity.z);
            _anim.SetFloat("MotionSpeed", 1);
        }

        public void StartMoveAction(Vector3 destination, float speedFraction)
        {

            MoveTo(destination, speedFraction);
        }

        public void MoveTo(Vector3 destination, float speedFraction)
        {
            agent.SetDestination(destination);
            agent.speed = maxSpeed * Mathf.Clamp01(speedFraction);
            agent.isStopped = false;
           
        }


       
        public void Cancel()
        {
            agent.isStopped = true;
        }
        internal bool CanMoveTo(Vector3 distination)
        {
            NavMeshPath path = new NavMeshPath();
            bool hasPath = NavMesh.CalculatePath(transform.position, distination, NavMesh.AllAreas, path);
            if (!hasPath) return false;
            if (path.status != NavMeshPathStatus.PathComplete) return false;
            if (GetPathLength(path) > maxPathLength) return false;
            return true;
        }
        private float GetPathLength(NavMeshPath path)
        {
            float totalDistance = 0;
            if (path.corners.Length < 2) return totalDistance;
            for (int i = 0; i < path.corners.Length - 1; i++)
            {
                totalDistance += Vector3.Distance(path.corners[i], path.corners[i + 1]);
            }
            return totalDistance;
        }
        public AudioClip LandingAudioClip;
        public AudioClip[] FootstepAudioClips;
        [Range(0, 1)] public float FootstepAudioVolume = 0.5f;


        private CharacterController _controller;
        private void OnFootstep(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                if (FootstepAudioClips.Length > 0)
                {
                    var index = UnityEngine.Random.Range(0, FootstepAudioClips.Length);
                    AudioSource.PlayClipAtPoint(FootstepAudioClips[index], transform.TransformPoint(_controller.center), FootstepAudioVolume);
                }
            }
        }

        private void OnLand(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                AudioSource.PlayClipAtPoint(LandingAudioClip, transform.TransformPoint(_controller.center), FootstepAudioVolume);
            }
        }


    }
}