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
    public List<Material> playersMaterials;

    [Header("Values")]
    public string blockLvl;
    public Vector3 blockLocalPos;
    public Quaternion blockRotation;
    public Vector3 defaultBlockPos;
    public bool isHeld = false;
    public float sizeY;

    private void Awake()
    {
        blockLocalPos = GetComponent<Transform>().position;
        defaultBlockPos = GetComponent<Transform>().position;
        sizeY = GetComponent<BoxCollider>().size.y * GetComponent<Transform>().localScale.y * 2;
        meshRenderer = GetComponent<MeshRenderer>();
        ChangeMaterial();
        boxCollider = GetComponent<BoxCollider>();
        blockLvl = blocks.name.Substring(blocks.name.Length - 1, 1);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isHeld)
        {
            if (other.GetComponent<MeshRenderer>().material.name.Split()[0] == material.name.Split()[0])
            {
                other.GetComponent<IBlockCollector>().CollectBlock(this);
                transform.position = blockLocalPos;
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        
    }

    private void ChangeMaterial()
    {
        int colorIndex = Random.Range(0, playersMaterials.Count);
        material = playersMaterials[colorIndex];
        meshRenderer.material = material;
    }

    void Update()
    {
        //when we return the block
        if(!isHeld && transform.position != defaultBlockPos)
        {
            transform.SetParent(blocks);
            transform.position = defaultBlockPos;
            transform.rotation = new Quaternion(0, 0, 0, transform.rotation.w);
            ChangeMaterial();
        }
        else if(isHeld)
        {
            transform.rotation = blockRotation;
            transform.localPosition = new Vector3(0f, blockLocalPos.y, -0.4f);
        }
    }
}
