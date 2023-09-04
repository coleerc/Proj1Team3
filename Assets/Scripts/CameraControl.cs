using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraControl : MonoBehaviour
{
    public float sensX;
    public float sensY;

    public Transform orientation;

    private float xRotation;
    private float yRotation;
    // Start is called before the first frame update
    void Start()
    {
        CursorLockState();
    }

    // Update is called once per frame
    void Update()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * sensX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * sensY;

        yRotation += mouseX;
        xRotation -= mouseY;

        xRotation = Mathf.Clamp(xRotation, - 90f, 90f);

        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);
    }

    private void FixedUpdate()
    {
        CursorLockState();
    }
    private void CursorLockState()
    {
        if (SceneManager.GetActiveScene().name == "Tutorial" || SceneManager.GetActiveScene().name == "LevelOne" || SceneManager.GetActiveScene().name == "LevelTwo" || SceneManager.GetActiveScene().name == "LevelThree")
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        else if (SceneManager.GetActiveScene().name == "Lose" || SceneManager.GetActiveScene().name == "Win" || SceneManager.GetActiveScene().name == "MainMenu" || SceneManager.GetActiveScene().name == "Help" || SceneManager.GetActiveScene().name == "Credits" || SceneManager.GetActiveScene().name == "AssetsScene")
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
