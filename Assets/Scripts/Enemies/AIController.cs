using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour, IBlockCollector
{
    [Header("Horizontal movement")]
    private float movementSpeed = 1.3f;
    private float maxVelocity = 2f;
    Vector3 direction = Vector3.zero;
    public float rotationSpeed = 600f;

    [Header("Slope Handling")]
    public float maxSlopeAngle = 50;
    private RaycastHit slopeHit;

    [Header("Physics")]
    public Rigidbody rb;
    public CapsuleCollider capsuleCollider;
    public float aiHeight;

    [Header("Other refs")]
    public Material material;
    public List<BlockController> blocks;

    [Header("Anim")]
    public Animator animator;

    [Header("Attack")]
    public float attackPointRadius;
    public Transform attackPoint;
    public LayerMask enemyMask;
    public float recoilForce = 10f;

    [Header("BlockCollection")]
    public int blockslvl = 0;
    private bool lvlChanged = false;
    public Vector3 destinationPoint;
    public int blocksToCollect = 2;
    public int blocksToCollectMax = 5;
    public int blocksToCollectMin = 1;

    public Transform blocksParentOnLvl;
    public List<Transform> levels = new List<Transform>();
    public Transform slopesParentOnLvl;
    public List<Transform> slopeParentsOnLvls = new List<Transform>();
    public List<Transform> slopesOnLvl = new List<Transform>();

    private bool isGoingToBuild = false;
    private bool isBulding = false;
    int slopeNum;
    private bool blockChoosen = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();

        aiHeight = capsuleCollider.height;
        blocks = new List<BlockController>();
        material = GetComponent<MeshRenderer>().material;

        destinationPoint = transform.position;
        blocksParentOnLvl = levels[blockslvl];

        for (int i = 0; i < slopesParentOnLvl.childCount; i++)
        {
            slopesOnLvl.Add(slopesParentOnLvl.GetChild(i));
        }

        blocksToCollect = Random.Range(blocksToCollectMin, blocksToCollectMax);
        ChooseBlockToCollect();
    }

    private bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, aiHeight * 0.5f + 0.3f))
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
            if (rb.velocity.magnitude > movementSpeed / 2)
            {
                rb.velocity = rb.velocity.normalized * movementSpeed;
            }
        }
        else
        {
            if(rb.velocity.magnitude > maxVelocity) rb.velocity = rb.velocity.normalized * movementSpeed;
        }
    }

    public void DecideAction()
    {
        if (blocksToCollect <= blocks.Count && !isGoingToBuild)
        {
            GoTryPlaceBlocks();
        }
        else if (isGoingToBuild)
        {
            if (lvlChanged)
            {
                blocksToCollect = Random.Range(blocksToCollectMin, blocksToCollectMax);
                ChooseBlockToCollect();

                isGoingToBuild = false;
                isBulding = false;
                lvlChanged = false;
            }
        }
        else if(!blockChoosen) ChooseBlockToCollect();
    }

    public void StartBuilding()
    {
        if(isGoingToBuild)
        {
            isBulding = true;
        }
    }

    public void StopBuilding()
    {
        isBulding = false;
        isGoingToBuild = false;
        blocksToCollect = Random.Range(blocksToCollectMin, blocksToCollectMax);
        ChooseBlockToCollect();
    }

    public void Move()
    {
        UpdatePlayerSpeed();

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
        }

        RotateWhileMoving();

        //turn off gravity on slopes
        rb.useGravity = !OnSlope();
    }

    public void UpdateDirection()
    {
        if(isBulding && blocks.Count > 0) direction = Vector3.forward * 2;
        else if(isBulding && blocks.Count == 0) direction = -Vector3.forward * 2;
        else direction = destinationPoint - new Vector3(transform.position.x, 0, transform.position.z);
    }

    public void GoTryPlaceBlocks()
    {
        slopeNum = Random.Range(0, slopesOnLvl.Count);
        destinationPoint = new Vector3(slopesOnLvl[slopeNum].position.x, 0, slopesOnLvl[slopeNum].position.z);

        isGoingToBuild = true;
        blockChoosen = false;
    }

    public void ChooseBlockToCollect()
    {
        int childCount = blocksParentOnLvl.childCount;
        float minDistance = float.MaxValue;

        for(int i = 0; i < childCount; i++)
        {
            if (blocksParentOnLvl.GetChild(i).GetComponent<BlockController>().material.name == material.name.Split()[0])
            {
                Vector3 blockpos = new Vector3(blocksParentOnLvl.GetChild(i).position.x, 0, blocksParentOnLvl.GetChild(i).position.z);
                float dist = Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.z), blockpos);
                if (minDistance > dist)
                {
                    minDistance = dist;
                    destinationPoint = blockpos;
                    blockChoosen = true;
                }
            }
        }
    }

    public void RotateWhileMoving()
    {
        if (direction != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(direction.normalized, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);

            foreach (BlockController block in blocks)
            {
                block.blockRotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
            }
        }
    }

    public int CheckIfPlayerHasBlocks()
    {
        return blocks.Count;
    }

    public void ChangeCurrentLevel()
    {
        if (!lvlChanged)
        {
            blockslvl++;
            blocksParentOnLvl = levels[blockslvl];
            slopesParentOnLvl = slopeParentsOnLvls[blockslvl];

            slopesOnLvl.Clear();
            for (int i = 0; i < slopesParentOnLvl.childCount; i++)
            {
                slopesOnLvl.Add(slopesParentOnLvl.GetChild(i));
            }
            lvlChanged = true;
        }

    }

    public void CollectBlock(BlockController block)
    {
        if (!block.isHeld)
        {
            if (blocks.Count == 0)
            {
                block.blockLocalPos = new Vector3(0, aiHeight / 2,
                    transform.position.z - capsuleCollider.radius * 2);
                block.transform.SetParent(transform);

                block.isHeld = true;
                blocks.Add(block);

            }
            else if (blocks.Count > 0)
            {
                block.blockLocalPos = new Vector3(0, (aiHeight / 2) + (block.sizeY / 4) * blocks.Count,
                    transform.position.z - capsuleCollider.radius * 2);
                block.transform.SetParent(transform);

                block.isHeld = true;
                blocks.Add(block);
            }

            if(blocksToCollect > blocks.Count)
                ChooseBlockToCollect();
        }
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
        for (int i = blocks.Count - 1; i >= 0; i--)
        {
            DropBlock(i);
        }

        Vector3 force = recoilForce * -hitDir;
        rb.AddForce(force);
        rb.velocity = Vector3.zero;

        isGoingToBuild = false;
        isBulding = false;

        blocksToCollect = Random.Range(blocksToCollectMin, blocksToCollectMax);
        ChooseBlockToCollect();
    }

    public void PlaceBlock(SlopeStepController step)
    {
        DropBlock(blocks.Count - 1);

        step.material = material;
        step.isTaken = true;
    }

    public void PushEnemy()
    {
        Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackPointRadius, enemyMask);

        if (hitEnemies.Length > 0)
        {
            foreach (Collider enemy in hitEnemies)
            {
                if(enemy.name == "Player")
                    enemy.GetComponent<PlayerLogic>().DropBlocks(direction);
                else
                    enemy.GetComponent<AIController>().DropBlocks(direction);
            }
        }
    }

    public void PushBack()
    {
        Vector3 force = recoilForce*3 * -direction.z * Vector3.forward;
        rb.AddForce(force);
        rb.velocity = Vector3.zero;
        direction = Vector3.zero;

        //choose different slope
        GoTryPlaceBlocks();
    }

    void Update()
    {
        DecideAction();
        UpdateDirection();
        Move();
        //check if player can push enemies while moving
        PushEnemy();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackPointRadius);
    }
}
