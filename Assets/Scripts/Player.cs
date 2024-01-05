using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using Unity.Burst.Intrinsics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Experimental.RestService;
using UnityEngine;
using UnityEngine.Tilemaps;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class Player : MonoBehaviour
{
    [SerializeField]
    float speed = 10f;

    Rigidbody2D _rb;
    Vector2 _movement;
    //Camera _camera;
    //private Vector2 screenBounds;
    float _playerWidth;


    private float xMin, xMax;
    private float yMin, yMax;

    public GameObject bulletPrefab;
    public float bulletSpeed = 10f; 
    public float bulletLifetime = 2f; // Lifetime of the bullet in seconds

    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();

        //_camera = GetComponent<Camera>();
        //screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
        var cameraBounds = GameObject.Find("CameraBounds");
        var cameraBoxCollider = cameraBounds.GetComponent<BoxCollider2D>();
        var screenBounds = cameraBoxCollider.size;
        //var confiner = cameraBounds.GetComponent<CinemachineConfiner2D>();
        //var boundingShape = GetComponent<BoundingShape2D>();
        //var screenBounds = confiner.m_BoundingShape2D.bounds.size;
        //var cam = Camera.main;// Camera component to get their size, if this change in runtime make sure to update values
        //var h = cam.orthographicSize;
        //var w = cam.orthographicSize * cam.aspect;

        float xWidth = screenBounds.x * .5f;
        float yWidth = screenBounds.y * .5f;

        //_playerWidth = GetComponent<SpriteRenderer>().bounds.size.x / 2;
        var _playerWidth = GetComponent<SpriteRenderer>().bounds.size.x * .5f; // Working with a simple box here, adapt to you necessity

        yMin = -yWidth + _playerWidth; // lower bound
        yMax = yWidth - _playerWidth; // upper bound

        xMin = -xWidth + _playerWidth; // left bound
        xMax = xWidth - _playerWidth; // right bound 
    }

    // Update is called once per frame
    void Update()
    {
        _movement = new Vector2(Mathf.Clamp(Input.GetAxis("Horizontal"), xMin, xMax), Mathf.Clamp(Input.GetAxis("Vertical"), yMin, yMax));

        if (_movement.x != 0 || _movement.y != 0)
        {
            //moveCharacter(_movement);
        }

        //var direction = new Vector2(_movement.x, _movement.y).normalized;
        //direction *= speed * Time.deltaTime; // apply speed

        var xValidPosition = Mathf.Clamp(transform.position.x, xMin, xMax);
        var yValidPosition = Mathf.Clamp(transform.position.y, yMin, yMax);

        transform.position = new Vector3(xValidPosition, yValidPosition, 0f);

        if (Input.GetKey(KeyCode.Mouse0))
        {
            //Vector3 mousePos = Input.mousePosition;
            //Vector3 aim = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, Camera.main.nearClipPlane));

            //GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
            //bullet.transform.LookAt(aim);
            //Rigidbody rb = bullet.GetComponent<Rigidbody>();
            //rb.AddRelativeForce(Vector3.forward * .1f);//force

            ShootBullet();
        }
    }

    private void FixedUpdate()
    {
        moveCharacter(_movement);
    }

    void moveCharacter(Vector2 direction)
    {
        _rb.MovePosition((Vector2)transform.position + (direction * speed * Time.deltaTime));

        Debug.Log($"xMin:{xMin},xMax:{xMax},yMin:{yMin},yMax:{yMax}");
        Debug.Log($"({_rb.position.x},{_rb.position.y})");
    }

    void ShootBullet()
    {
        // Instantiate a bullet at the player's position
        GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);

        // Get the direction from the player to the mouse pointer
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mousePosition - transform.position).normalized;

        // Set the bullet's velocity to move in the direction of the mouse pointer
        bullet.GetComponent<Rigidbody2D>().velocity = new Vector2(direction.x, direction.y) * bulletSpeed;

        // Destroy the bullet after a specified lifetime
        Destroy(bullet, bulletLifetime);
    }
}
