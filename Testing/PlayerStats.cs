using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    public float hunger;
    public float thirst;
    public float stamina;
    public float hungerRate;
    public float thirstRate;
    public float staminaDrain;
    GameObject hungerBar;
    GameObject thirstBar;
    GameObject staminaBar;
    public float xPos;
    public float yPos;
    public float zPos;
    public Vector2 chunkIn;
    Vector2 chunkWas;
    public int leftHandHolding;
    public int rightHandHolding;
    public int strength;
    public int endurance;
    public int plants;
    public byte direction;
    void Start()
    {
        strength = 1;
        endurance = 1;
        plants = 1;

        leftHandHolding = -1;
        rightHandHolding = -1;

        hunger = 100.00f;
        thirst = 100.00f;
        stamina = 100.00f;

        hungerRate = 0.001f;
        thirstRate = 0.001f;
        staminaDrain = -0.001f;

        hungerBar = GameObject.Find("Canvas").transform.GetChild(2).gameObject;
        thirstBar = GameObject.Find("Canvas").transform.GetChild(3).gameObject;
        staminaBar = GameObject.Find("Canvas").transform.GetChild(4).gameObject;
    }
    void Update()
    {
        xPos = this.gameObject.transform.position.x;
        yPos = this.gameObject.transform.position.y;
        zPos = this.gameObject.transform.position.z;

        if (chunkIn != new Vector2((int)xPos / 16, (int)zPos / 16))
        {
            if (chunkIn == new Vector2(0.0f, 0.0f))
            {
                chunkIn = new Vector2((int)xPos / 16, (int)zPos / 16);
                GameObject.Find(("Chunk:" + chunkIn.x + ":" + chunkIn.y)).GetComponent<ChunkData>().LoadChunk(3);
                GameObject.Find(("Chunk:" + (chunkIn.x - 1) + ":" + (chunkIn.y + 1))).GetComponent<ChunkData>().LoadChunk(3);
                GameObject.Find(("Chunk:" + (chunkIn.x) + ":" + (chunkIn.y + 1))).GetComponent<ChunkData>().LoadChunk(3);
                GameObject.Find(("Chunk:" + (chunkIn.x + 1) + ":" + (chunkIn.y + 1))).GetComponent<ChunkData>().LoadChunk(3);
                GameObject.Find(("Chunk:" + (chunkIn.x - 1) + ":" + (chunkIn.y))).GetComponent<ChunkData>().LoadChunk(3);
                GameObject.Find(("Chunk:" + (chunkIn.x + 1) + ":" + (chunkIn.y))).GetComponent<ChunkData>().LoadChunk(3);
                GameObject.Find(("Chunk:" + (chunkIn.x - 1) + ":" + (chunkIn.y - 1))).GetComponent<ChunkData>().LoadChunk(3);
                GameObject.Find(("Chunk:" + (chunkIn.x) + ":" + (chunkIn.y - 1))).GetComponent<ChunkData>().LoadChunk(3);
                GameObject.Find(("Chunk:" + (chunkIn.x + 1) + ":" + (chunkIn.y - 1))).GetComponent<ChunkData>().LoadChunk(3);
            }
            chunkWas = chunkIn;
            chunkIn = new Vector2((int)xPos / 16, (int)zPos / 16);
            if (chunkIn.x > chunkWas.x) direction = 2;
            else if (chunkIn.x < chunkWas.x) direction = 4;
            else if (chunkIn.y > chunkWas.y) direction = 1;
            else if (chunkIn.y < chunkWas.y) direction = 3;

            switch (direction)
            {
                case 1:
                    GameObject.Find(("Chunk:" + (chunkIn.x - 1) + ":" + (chunkIn.y + 1))).GetComponent<ChunkData>().LoadChunk(3);
                    GameObject.Find(("Chunk:" + (chunkIn.x) + ":" + (chunkIn.y + 1))).GetComponent<ChunkData>().LoadChunk(3);
                    GameObject.Find(("Chunk:" + (chunkIn.x + 1) + ":" + (chunkIn.y + 1))).GetComponent<ChunkData>().LoadChunk(3);

                    GameObject.Find(("Chunk:" + (chunkIn.x - 1) + ":" + (chunkIn.y - 2))).GetComponent<ChunkData>().UnLoadChunk();
                    GameObject.Find(("Chunk:" + (chunkIn.x) + ":" + (chunkIn.y - 2))).GetComponent<ChunkData>().UnLoadChunk();
                    GameObject.Find(("Chunk:" + (chunkIn.x + 1) + ":" + (chunkIn.y - 2))).GetComponent<ChunkData>().UnLoadChunk();
                    break;
                case 2:
                    GameObject.Find(("Chunk:" + (chunkIn.x + 1) + ":" + (chunkIn.y + 1))).GetComponent<ChunkData>().LoadChunk(3);
                    GameObject.Find(("Chunk:" + (chunkIn.x + 1) + ":" + (chunkIn.y))).GetComponent<ChunkData>().LoadChunk(3);
                    GameObject.Find(("Chunk:" + (chunkIn.x + 1) + ":" + (chunkIn.y - 1))).GetComponent<ChunkData>().LoadChunk(3);

                    GameObject.Find(("Chunk:" + (chunkIn.x - 2) + ":" + (chunkIn.y + 1))).GetComponent<ChunkData>().UnLoadChunk();
                    GameObject.Find(("Chunk:" + (chunkIn.x - 2) + ":" + (chunkIn.y))).GetComponent<ChunkData>().UnLoadChunk();
                    GameObject.Find(("Chunk:" + (chunkIn.x - 2) + ":" + (chunkIn.y - 1))).GetComponent<ChunkData>().UnLoadChunk();
                    break;
                case 3:
                    GameObject.Find(("Chunk:" + (chunkIn.x - 1) + ":" + (chunkIn.y - 1))).GetComponent<ChunkData>().LoadChunk(3);
                    GameObject.Find(("Chunk:" + (chunkIn.x) + ":" + (chunkIn.y - 1))).GetComponent<ChunkData>().LoadChunk(3);
                    GameObject.Find(("Chunk:" + (chunkIn.x + 1) + ":" + (chunkIn.y - 1))).GetComponent<ChunkData>().LoadChunk(3);

                    GameObject.Find(("Chunk:" + (chunkIn.x - 1) + ":" + (chunkIn.y + 2))).GetComponent<ChunkData>().UnLoadChunk();
                    GameObject.Find(("Chunk:" + (chunkIn.x) + ":" + (chunkIn.y + 2))).GetComponent<ChunkData>().UnLoadChunk();
                    GameObject.Find(("Chunk:" + (chunkIn.x + 1) + ":" + (chunkIn.y + 2))).GetComponent<ChunkData>().UnLoadChunk();
                    break;
                case 4:
                    GameObject.Find(("Chunk:" + (chunkIn.x - 1) + ":" + (chunkIn.y + 1))).GetComponent<ChunkData>().LoadChunk(3);
                    GameObject.Find(("Chunk:" + (chunkIn.x - 1) + ":" + (chunkIn.y))).GetComponent<ChunkData>().LoadChunk(3);
                    GameObject.Find(("Chunk:" + (chunkIn.x - 1) + ":" + (chunkIn.y - 1))).GetComponent<ChunkData>().LoadChunk(3);

                    GameObject.Find(("Chunk:" + (chunkIn.x + 2) + ":" + (chunkIn.y + 1))).GetComponent<ChunkData>().UnLoadChunk();
                    GameObject.Find(("Chunk:" + (chunkIn.x + 2) + ":" + (chunkIn.y))).GetComponent<ChunkData>().UnLoadChunk();
                    GameObject.Find(("Chunk:" + (chunkIn.x + 2) + ":" + (chunkIn.y - 1))).GetComponent<ChunkData>().UnLoadChunk();
                    break;
            }
            
        }

        if (xPos < 0) chunkIn.x -= 1;
        if (yPos < 0) chunkIn.y -= 1;

        hungerBar.GetComponent<Slider>().value = hunger;
        thirstBar.GetComponent<Slider>().value = thirst;
        staminaBar.GetComponent<Slider>().value = stamina;
        hunger -= hungerRate;
        thirst -= thirstRate;
    }
    public bool IsHolding(int id)
    {
        if (leftHandHolding == id || rightHandHolding == id)
        {
            return true;
        }
        return false;
    }
}