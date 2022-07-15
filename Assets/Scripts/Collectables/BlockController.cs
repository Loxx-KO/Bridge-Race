using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockController : MonoBehaviour
{
    [Header("Components")]
    public Transform blocks;
    public MeshRenderer meshRenderer;
    public BoxCollider boxCollider;
    public Material material;

    [Header("Values")]
    public string blockLvl;
    public Vector3 blockLocalPos;
    public Quaternion blockRotation;
    public Vector3 defaultBlockPos;
    public bool isHeld = false;
    public float sizeY;

    void Start()
    {
        blockLocalPos = GetComponent<Transform>().position;
        defaultBlockPos = GetComponent<Transform>().position;
        material = GetComponent<MeshRenderer>().material;
        sizeY = GetComponent<BoxCollider>().size.y * GetComponent<Transform>().localScale.y * 2;
        meshRenderer = GetComponent<MeshRenderer>();
        boxCollider = GetComponent<BoxCollider>();
        blockLvl = blocks.name.Substring(blocks.name.Length-1, 1);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isHeld)
        {
            if (other.name == "Player")
            {
                //for player
                other.GetComponent<PlayerLogic>().CollectBlock(this);
                transform.position = blockLocalPos;
                Debug.Log("Collected");
            }
            //for ai
        }
    }
    private void OnTriggerExit(Collider other)
    {
        
    }

    void Update()
    {
        //when we return the block
        if(!isHeld && transform.position != defaultBlockPos)
        {
            transform.SetParent(blocks);
            transform.position = defaultBlockPos;
            meshRenderer.enabled = false;
            boxCollider.enabled = false;
        }
        else if(isHeld)
        {
            transform.rotation = blockRotation;
            transform.localPosition = new Vector3(0f, blockLocalPos.y, -0.4f);
        }
    }
}
