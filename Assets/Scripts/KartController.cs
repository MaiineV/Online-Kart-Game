using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KartController : MonoBehaviour
{
    public float aceleration;
    public float deaceleration;
    public float maxSpeed;
    public float speed;
    public float maxRotationAngle;
    public float rotationAngle;

    float horizontal;
    float vertical;

    public LayerMask floor;
    public LayerMask collisions;
    public bool isOnAir = true;

    public float verticalForce;
    public float gravity;
    public float forceJump;

    public Vector3 boxSize;
    public Transform collisionPoint;

    public Vector3 dirCrashForce;
    public float crashForce;
    public float maxCrashForce;
    public float deacelerationCrashForce;

    public Vector3 actualDirMovement;
    public bool isMoveByForce = false;

    public bool canMove;

    public Transform rotationPoint;


    void Start()
    {

    }

    void Update()
    {
        CrashForce();

        if (!canMove) return;

        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");


        if (vertical > 0)
        {
            if (speed >= 0)
            {
                speed += aceleration * Time.deltaTime;
            }
            else
            {
                speed += deaceleration * Time.deltaTime;
            }
        }
        else if (vertical < 0)
        {
            if (speed <= 0)
            {
                speed -= aceleration * Time.deltaTime;
            }
            else
            {
                speed -= deaceleration * Time.deltaTime;
            }
        }
        else
        {
            speed += (0 - speed) * deaceleration * Time.deltaTime;

            if (0 - speed < 0.1f)
                speed = 0;
        }

        speed = Mathf.Clamp(speed, -maxSpeed, maxSpeed);



        if (horizontal > 0.2 || horizontal < -0.2)
        {
            if (speed >= 0)
                rotationAngle = maxRotationAngle * horizontal * Mathf.Abs(speed) / maxSpeed;
            else if (speed < 0)
                rotationAngle = maxRotationAngle * horizontal * -1 * Mathf.Abs(speed) / maxSpeed;
        }
        else
        {
            rotationAngle = 0;
        }

        rotationPoint.Rotate(new Vector3(0, rotationAngle, 0));
        transform.position += rotationPoint.forward * speed * Time.deltaTime;

        GravityForce();

        Collider[] forceColliders = Physics.OverlapSphere(transform.position, 4);
        Collider[] crashCollider = Physics.OverlapBox(collisionPoint.position, boxSize);

        if (isMoveByForce)
        {
            foreach (Collider collider in forceColliders)
            {
                if (collider.gameObject.tag == "Player" && collider.gameObject != this.gameObject)
                {
                    Vector3 forceDir = (collider.transform.position - transform.position).normalized;
                    collider.gameObject.GetComponent<KartController>().AddCrashForce(forceDir, maxCrashForce);

                }
            }
        }
        else
        {
            foreach (Collider collider in crashCollider)
            {
                if (collider.gameObject.tag == "Player" && collider.gameObject != this.gameObject)
                {
                    Vector3 forceDir = (collider.transform.position - transform.position).normalized;
                    collider.gameObject.GetComponent<KartController>().AddCrashForce(forceDir, maxCrashForce * (speed / maxSpeed));
                }
            }
        }
    }

    void GravityForce()
    {
        Debug.DrawRay(transform.position, Vector3.down, Color.red, 1f);
        RaycastHit hit;
        if (isOnAir && Physics.Raycast(transform.position, Vector3.down, 1.5f, floor))
        {
            isOnAir = false;
            verticalForce = 0;

        }
        else if (!isOnAir && !Physics.Raycast(transform.position, Vector3.down, 1.5f, floor))
        {
            isOnAir = true;
        }

        if (isOnAir)
        {
            verticalForce += gravity * Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.Space) && !isOnAir)
        {
            verticalForce += forceJump;
        }

        if (Physics.Raycast(transform.position, Vector3.down, out hit, 1.5f, floor))
        {
            transform.rotation = Quaternion.Euler(new Vector3(hit.collider.transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, hit.collider.transform.rotation.eulerAngles.z));
        }

        transform.position += Vector3.up * verticalForce * Time.deltaTime;

    }

    void CrashForce()
    {
        if (crashForce == 0) { isMoveByForce = false; return; }

        Vector3 dirX = new Vector3(dirCrashForce.x, 0, 0);
        CheckCrashCollions(dirX, 0);

        Vector3 dirY = new Vector3(0, dirCrashForce.y, 0);
        CheckCrashCollions(dirY, 1);

        Vector3 dirZ = new Vector3(0, 0, dirCrashForce.z);
        CheckCrashCollions(dirZ, 2);

        transform.position += dirCrashForce * crashForce * Time.deltaTime;

        actualDirMovement = dirCrashForce;

        crashForce += deacelerationCrashForce * Time.deltaTime;
        crashForce = Mathf.Clamp(crashForce, 0, maxCrashForce);
    }

    void CheckCrashCollions(Vector3 dirToCheck, int eje)
    {
        if (Physics.Raycast(transform.position, dirToCheck, 2, collisions))
        {
            if (eje == 0)
            {
                dirCrashForce.x = dirCrashForce.x * -1;
            }
            else if (eje == 1)
            {
                dirCrashForce.y = dirCrashForce.y * -1;
            }
            else if (eje == 2)
            {
                dirCrashForce.z = dirCrashForce.z * -1;
            }
        }
    }

    public void AddCrashForce(Vector3 dir, float addForce)
    {
        crashForce = addForce;
        dirCrashForce = dir;
        isMoveByForce = true;
    }

    private void OnDrawGizmos()
    {
        //Gizmos.DrawWireSphere(transform.position, 4);
        //Gizmos.DrawWireCube(collisionPoint.position, boxSize);
    }
}
