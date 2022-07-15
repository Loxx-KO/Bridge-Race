using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLogic : MonoBehaviour
{
    [Header("Horizontal movement")]
    private float movementSpeed = 1.5f;
    Vector3 direction = Vector3.zero;
    public float rotationSpeed = 2f;

    [Header("Slope Handling")]
    public float maxSlopeAngle = 50;
    private RaycastHit slopeHit;

    [Header("Touch control")]
    private Vector2 touchDirection = Vector2.zero;
    private bool canMove = false;

    [Header("Physics")]
    public Rigidbody rb;
    public CapsuleCollider capsuleCollider;
    public float playerHeight;

    [Header("Other refs")]
    private Camera mainCam;
    public Material material;
    public List<BlockController> blocksGameObjects;

    [Header("Anim")]
    public Animator animator;

    [Header("Attack")]
    public float attackPointRadius;
    public Transform attackPoint;
    public LayerMask enemyMask;

    void Start()
    {
        mainCam = Camera.main;
        rb = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();

        playerHeight = capsuleCollider.height;
        blocksGameObjects = new List<BlockController>();
        material = GetComponent<MeshRenderer>().material;
    }
    public void UpdateMovementFromControls()
    {
        touchDirection = InputManagerScript.GetInstance().GetTouchDirection();
        canMove = InputManagerScript.GetInstance().GetTouchState();
    }

    public void UpdateDirection()
    {
        //still funky
        if (touchDirection == Vector2.zero && canMove) return;

        direction.x = touchDirection.x;
        direction.z = touchDirection.y;
    }

    private bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            if (angle < maxSlopeAngle && angle != 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    private Vector3 GetSlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(direction, slopeHit.normal);
    }

    public void UpdatePlayerSpeed()
    {
        if (OnSlope())
        {
            if (rb.velocity.magnitude > movementSpeed/2)
            {
                rb.velocity = rb.velocity.normalized * movementSpeed;
            }
        }
    }

    public void Move()
    {
        UpdatePlayerSpeed();

        if (canMove)
        {
            if (OnSlope())
            {
                rb.AddForce(GetSlopeMoveDirection() * movementSpeed);

                if(rb.velocity.y > 0)
                {
                    rb.AddForce(Vector3.down * (movementSpeed/4));
                }
            }
            else
                rb.AddForce(direction * movementSpeed);

            RotateWhileMoving();

            //check if player can push enemies while moving
            PushEnemy();
        }

        //turn off gravity on slopes
        rb.useGravity = !OnSlope();
    }

    public void RotateWhileMoving()
    {
        if (direction != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(direction.normalized, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
            
            foreach(BlockController block in blocksGameObjects)
            {
                block.blockRotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
            }
        }
    }

    public void CollectBlock(BlockController block)
    {
        if (!block.isHeld)
        {
            if (blocksGameObjects.Count == 0)
            {
                block.blockLocalPos = new Vector3(0, playerHeight / 2,
                    transform.position.z - capsuleCollider.radius * 2);
                block.transform.SetParent(transform);

                block.isHeld = true;
                blocksGameObjects.Add(block);

            }
            else if (blocksGameObjects.Count > 0)
            {
                block.blockLocalPos = new Vector3(0, (playerHeight/2) + (block.sizeY/4) * blocksGameObjects.Count,
                    transform.position.z - capsuleCollider.radius * 2);
                block.transform.SetParent(transform);

                block.isHeld = true;
                blocksGameObjects.Add(block);
            }
        }
    }

    public int CheckIfPlayerHasBlocks()
    {
        return blocksGameObjects.Count;
    }

    public void PlaceBlock(SlopeStepController step)
    {
        DropBlock(blocksGameObjects.Count - 1);
        step.material = material;
        step.isTaken = true;
    }

    public void DropBlock(int i)
    {
        BlockController block = blocksGameObjects[i];
        block.blockLocalPos = block.defaultBlockPos;
        block.isHeld = false;
        blocksGameObjects.RemoveAt(i);
    }

    public void DropAllBlocks()
    {
        if(blocksGameObjects.Count > 0)
        {
            for(int i = blocksGameObjects.Count - 1; i > 0; i--)
            {
                DropBlock(i);
            }
        }
    }

    public void PushEnemy()
    {
        Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackPointRadius, enemyMask);

        if (hitEnemies.Length > 0)
        {
            foreach (Collider enemy in hitEnemies)
            {
                enemy.GetComponent<IEnemyController>().DropBlocks();
            }
            //shake camera
            //Camera_Shake.Instance.ShakeCamera(shakeStrength, shakeDuration);
        }
    }

    void Update()
    {
        UpdateMovementFromControls();
        UpdateDirection();
        Move();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackPointRadius);
    }
}
