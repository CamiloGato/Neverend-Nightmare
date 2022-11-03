using System;
using UnityEngine;

namespace Pickable
{
    public class PickableController : MonoBehaviour
    {
        [SerializeField] private int _id; 
        private GameObject _player;
        private Rigidbody _rigidbody;
        private Collider[] _colliders;
        private ParticleSystem[] _particleExplosion;
        private void Awake()
        {
            _player = GameObject.FindWithTag("Player");
            _rigidbody = GetComponent<Rigidbody>();
            _colliders = GetComponentsInChildren<Collider>();
            _particleExplosion = GetComponentsInChildren<ParticleSystem>();
        }

        private void Start()
        {
            _rigidbody.useGravity = false;
        }

        private void Update()
        {
            transform.LookAt(_player.transform);
            if (transform.position.y < -10)
            {
                Destroy(gameObject);
            }
        }
        
        public void PickUp()
        {
            PickableCanvas.Instance.OnPick(_id);
            foreach (var particle in _particleExplosion)
            {
                particle.Play();
            }
            _rigidbody.useGravity = true;
            _rigidbody.AddForce(Vector3.up * (100f), ForceMode.Impulse);
            foreach (Collider colliderObject in _colliders)
            {
                colliderObject.isTrigger = true;
            }
        }
        
    }
}

