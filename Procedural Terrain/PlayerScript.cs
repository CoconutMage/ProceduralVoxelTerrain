using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerScript : MonoBehaviour
{
    float movementSpeed;
    bool lockedCursor;
    TerrainGenerator generator;
    float time;
    GameObject canvasObject;

    void Start()
    {
        //canvasObject = GameObject.Find("Canvas");
        lockedCursor = false;
        movementSpeed = 0.3f;

        //canvasObject.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceCamera;
        //canvasObject.GetComponent<Canvas>().worldCamera = this.gameObject.GetComponent<Camera>();

        generator = GameObject.Find("Terrain Generator").GetComponent<TerrainGenerator>();
        TerrainGenerated();
    }
    void Update()
    {
        Vector3 movementDirection = new Vector3();

        RaycastHit hit;

        if (lockedCursor == true)
        {
            movementDirection = (Camera.main.transform.forward * Input.GetAxis("Vertical")) + (Camera.main.transform.right * Input.GetAxis("Horizontal")) + (Camera.main.transform.up * Input.GetAxis("UpDown"));

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            Vector3 temp = this.GetComponent<Transform>().eulerAngles;
            if (temp.x > 80 && temp.x < 250)
            {
                if (Input.GetAxis("Mouse Y") > 0) temp.x += -Input.GetAxis("Mouse Y");
            }
            else if (temp.x < 280 && temp.x > 120)
            {
                if (Input.GetAxis("Mouse Y") < 0) temp.x += -Input.GetAxis("Mouse Y");
            }
            else temp.x += -Input.GetAxis("Mouse Y");
            temp.y += Input.GetAxis("Mouse X");
            temp.z = 0;
            this.GetComponent<Transform>().eulerAngles = temp;

            this.GetComponent<CharacterController>().Move(movementDirection * movementSpeed);

            if (Physics.Raycast(transform.position, Camera.main.transform.forward, out hit, 5))
            {
                if (Input.GetMouseButton(0))
                {
                    if (hit.transform.tag == "Chunk" && (time == 0 || time - Time.time < -.25f))
                    {
                        time = Time.time;
                        Vector3 pos = hit.point;
                        if (hit.normal.x > 0) pos.x -= 0.1f;
                        if (hit.normal.y > 0) pos.y -= 0.1f;
                        if (hit.normal.z > 0) pos.z -= 0.1f;
                        generator.BreakBlock(pos);
                    }
                }
            }
        }
        else
        {
            //canvasObject.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            if (Physics.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Camera.main.transform.forward, out hit, 275))
            {
                if (Input.GetMouseButton(0))
                {
                    //GameObject.Find("Biome Map").GetComponent<BiomeMap>().DisplayClicked(hit.point);
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
        if (Input.GetKeyUp(KeyCode.L) && (time == 0 || time - Time.time < -.25f))
        {
            time = Time.time;
            lockedCursor = !lockedCursor;
        }
    }
    public void TerrainGenerated()
    {
        lockedCursor = true;
    }
    public void Reload()
    {
        SceneManager.LoadScene(0);
    }
}