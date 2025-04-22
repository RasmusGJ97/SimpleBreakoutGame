using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    float _speed = 25f;
    //float _maxSpeed = 35f;
    int _collisionCounter = 0;
    int _collisionsToAddBall = 100;

    Rigidbody _rigidbody;
    Vector3 _velocity;
    Renderer _renderer;
    public static Ball Instance { get; private set; }

    GameManager gameManager = GameManager.Instance;

    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _renderer = GetComponent<Renderer>();
        Invoke("Launch", 0.5f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Launch()
    {
        _rigidbody.velocity = Vector3.up * _speed;
    }

    private void FixedUpdate()
    {
        _rigidbody.velocity = _rigidbody.velocity.normalized * _speed;
        _velocity = _rigidbody.velocity;

        if (!_renderer.isVisible)
        {
            gameManager._ballsInPlay--;
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        _rigidbody.velocity = Vector3.Reflect(_velocity, collision.contacts[0].normal);
        _speed += 0.07f;
        if (_collisionCounter < _collisionsToAddBall)
        {
            _collisionCounter++;
        }
        else
        {
            gameManager.AddBall();
            _collisionCounter = 0;
        }
    }

    //public void NewLevel()
    //{
    //    if (_renderer.isVisible)
    //    {
    //        Destroy(gameObject);
    //        _speed = 25f;
    //        _collisionCounter = 0;
    //    }
    //}
}
