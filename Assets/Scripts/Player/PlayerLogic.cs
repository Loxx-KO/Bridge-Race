using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLogic : MonoBehaviour, IBlockCollector
{
    [Header("Horizontal movement")]
    private float movementSpeed = 2f;
    private float maxVelocity = 4f;
    Vector3 direction = Vector3.zero;
    public float rotationSpeed = 500f;

    [Header("Slope Handling")]
    public float maxSlopeAngle = 50;
    private RaycastHit slopeHit;

    [Header("Touch control")]
    private Vector2 touchDirection = Vector2.zero;
    private bool canMove = false;

    [Header("Physics")]
    public Rigidbody rb;
    public CapsuleCollider capsuleCollider;
    private float playerHeight;
    private bool canPlay = true;

    [Header("Other refs")]
    private Camera mainCam;
    public Material material;
    public List<BlockController> blocks;

    [Header("Anim")]
    public Animator animator;

    [Header("Attack")]
    public float attackPointRadius;
    public Transform attackPoint;
    public LayerMask enemyMask;
    public float recoilForce = 10f;

    void Start()
    {
        mainCam = Camera.main;
        rb = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();

        playerHeight = capsuleCollider.height;
        blocks = new List<BlockController>();
        material = GetComponent<MeshRenderer>().material;
    }
    public void UpdateMovementFromControls()
    {
        touchDirection = InputManagerScript.GetInstance().GetTouchDirection();
        canMove = InputManagerScript.GetInstance().GetTouchState();
    }

    public void UpdateDirection()
    {
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
        else
        {
            if (rb.velocity.magnitude > maxVelocity) rb.velocity = rb.velocity.normalized * movementSpeed;
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

                if (rb.velocity.y > 0)
                {
                    rb.AddForce(Vector3.down * (movementSpeed / 4));
                }
            }
            else
            {
                rb.AddForce(direction * movementSpeed);
                //rb.AddForce(direction * 10f);
            }

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
            
            foreach(BlockController block in blocks)
            {
                block.blockRotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
            }
        }
    }

    public void CollectBlock(BlockController block)
    {
        if (!block.isHeld)
        {
            if (block.material.name == material.name.Split()[0])
            {
                if (blocks.Count == 0)
                {
                    block.blockLocalPos = new Vector3(0, playerHeight / 2,
                        transform.position.z - capsuleCollider.radius * 2);
                    block.transform.SetParent(transform);

                    block.isHeld = true;
                    blocks.Add(block);

                }
                else if (blocks.Count > 0)
                {
                    block.blockLocalPos = new Vector3(0, (playerHeight / 2) + (block.sizeY / 4) * blocks.Count,
                        transform.position.z - capsuleCollider.radius * 2);
                    block.transform.SetParent(transform);

                    block.isHeld = true;
                    blocks.Add(block);
                }
            }
        }
    }

    public int CheckIfPlayerHasBlocks()
    {
        return blocks.Count;
    }

    public void PlaceBlock(SlopeStepController step)
    {
        DropBlock(blocks.Count - 1);
        step.material = material;
        step.isTaken = true;
    }

    public void DropBlock(int i)
    {
        BlockController block = blocks[i];
        block.blockLocalPos = block.defaultBlockPos;
        block.isHeld = false;
        blocks.RemoveAt(i);
    }

    public void DropBlocks(Vector3 hitDir)
    {
        if(blocks.Count > 0)
        {
            for(int i = blocks.Count - 1; i >= 0; i--)
            {
                DropBlock(i);
            }

            Vector3 force = recoilForce * -hitDir;
            rb.AddForce(force);
            rb.velocity = Vector3.zero;
        }
    }

    public void PushEnemy()
    {
        Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackPointRadius, enemyMask);

        if (hitEnemies.Length > 0)
        {
            foreach (Collider enemy in hitEnemies)
            {
                enemy.GetComponent<AIController>().DropBlocks(direction);
            }
        }
    }

    public void PushBack()
    {
        Vector3 force = recoilForce * 3 * -direction.z * Vector3.forward;
        rb.AddForce(force);
        rb.velocity = Vector3.zero;
    }

    public void FinishGame(Vector3 pos)
    {
        canPlay = false;

        for (int i = blocks.Count - 1; i >= 0; i--)
        {
            DropBlock(i);
        }

        transform.position = pos;
        transform.rotation = new Quaternion(0, 0, -1, transform.rotation.w);
    }

    void Update()
    {
        if (canPlay)
        {
            UpdateMovementFromControls();
            UpdateDirection();
            Move();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackPointRadius);
    }
}
