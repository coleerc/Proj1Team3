using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerBehaviour : MonoBehaviour
{
    #region game variables
    [SerializeField] public float spirit;
    [SerializeField] private int gum;
    [SerializeField] private GameObject gumProjectile;
    [SerializeField] private Camera playerCam;
    private int candyCollected;
    private bool finalCandyCollected;
    private float minSpirit;
    private float maxSpirit;
    private float decayPerSecond;
    private bool ghostTouched;

    [SerializeField] GameObject finalCandy;
    private string sceneName;

    [SerializeField] private GameObject bottomCandy, middleCandy, topCandy;
    [SerializeField] private AudioController playerAudio;
    #endregion
    #region movement variables
    [Header("Movement")]
    [SerializeField] private float moveSpeed;

    [SerializeField] private float groundDrag;

    [SerializeField] private float jumpForce;
    [SerializeField] private float jumpCooldown;
    [SerializeField] private float airMultiplier;
    private bool readyToJump;

    [HideInInspector] private float walkSpeed;

    [Header("Keybinds")]
    [SerializeField] private KeyCode jumpKey = KeyCode.Space;
    [SerializeField] private KeyCode addGum = KeyCode.G;
    [SerializeField] private KeyCode addSpirit = KeyCode.H;
    [SerializeField] private KeyCode moveToNextLevel = KeyCode.N;
    [SerializeField] private KeyCode moveToPrevLevel = KeyCode.P;

    public Transform orientation;

    private float horizontalInput;
    private float verticalInput;

    private Vector3 moveDirection;

    private Rigidbody rb;
    #endregion
    #region ui variables

    [SerializeField] private Slider spiritBar;
    [SerializeField] private Image spiritFill;
    [SerializeField] private TMP_Text gumText;
    private bool spiritDecayIsRunning = false;

    private Vector3 currentPosition;
    private Vector3 lastKnownPosition;
    public bool isWalking;

    #endregion
    private void Start()
    {
        isWalking = false;
        spirit = 50;
        minSpirit = 0;
        maxSpirit = 150;
        decayPerSecond = -0.5f;
        candyCollected = 0;
        gum = 0;
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        playerAudio.GetComponent<AudioController>();

        readyToJump = true;
        finalCandyCollected = false;
        finalCandy.SetActive(false);
        sceneName = SceneManager.GetActiveScene().name;

        bottomCandy.SetActive(false);
        middleCandy.SetActive(false);
        topCandy.SetActive(false);
        spiritBar.value = spirit;
        gumText.text = "x0";

        CheatCodes();
    }

    private void Awake()
    {
        isWalking = false;
        currentPosition = transform.position;
        lastKnownPosition = currentPosition;
    }
    private void Update()
    {
        PlayerInput();
        SpeedControl();
        if (Input.GetMouseButtonDown(0) && gum > 0)
        {
            ShootGum();
        }

        if (ghostTouched)
        {
            decayPerSecond = -2;
        }
        else
        {
            decayPerSecond = -0.5f;
        }
        spirit = Mathf.Clamp(spirit + decayPerSecond * Time.deltaTime, minSpirit, maxSpirit);
    }

    private void FixedUpdate()
    {
        currentPosition = transform.position;
        MovePlayer();
        CheckLoss(spirit);
        SetFinalCandyActive();
        UpdateSpiritBar(spirit);

        if (Vector3.Distance(currentPosition, lastKnownPosition) > 0.001f)
        {
            isWalking = true;
            Debug.Log(isWalking + " walking");
        }
        else
        {
            isWalking = false;
            Debug.Log(isWalking + " not walking");
        }
        lastKnownPosition = currentPosition;
    }

    private void SetFinalCandyActive()
    {
        if(sceneName == "Tutorial" && candyCollected == 1)
        {
            finalCandy.SetActive(true);
        }
        else if (sceneName == "LevelOne" && candyCollected == 6)
        {
            finalCandy.SetActive(true);
        }
        else if (sceneName == "LevelTwo" && candyCollected == 8)
        {
            finalCandy.SetActive(true);
        }
    }

    #region movement behavior
    private void PlayerInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        // when to jump
        if (Input.GetKeyDown(jumpKey) && readyToJump) 
        {
            readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }

    }

    private void MovePlayer()
    {
        // calculate movement direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        // limit velocity if needed
        if (flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

    private void Jump()
    {
        // reset y velocity
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }
    private void ResetJump()
    {
        readyToJump = true;
    }
    #endregion
    #region collisions
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "enemy")
        {
            spirit -= 10;
            ghostTouched = true;

            if (!playerAudio.ghostTouched.isPlaying)
            {
                playerAudio.PlayGhostTouched();
            }
            UpdateSpiritBar(spirit);
            CheckLoss(spirit);
        }
        else if (other.gameObject.tag == "spiritcandy")
        {
            spirit +=10;
            UpdateSpiritBar(spirit);
            candyCollected++;
            playerAudio.PlayCrinkle();
            RaiseCandyLevelInBucket();
            Destroy(other.gameObject);
        }
        else if (other.gameObject.tag == "gum")
        {
            gum++;
            UpdateGumCount(gum);
            playerAudio.PlayCrinkle();
            Destroy(other.gameObject);
        }
        else if (other.gameObject.tag == "finalcandy")
        {
            finalCandyCollected = true;
            CheckWin();
            other.gameObject.SetActive(false);
        }
        else if (other.gameObject.tag == "witchesbrew")
        {
            moveSpeed = 2.0f;
        }
        else if (other.gameObject.tag == "car")
        {
            StartCoroutine(GameObject.FindObjectOfType<SceneFader>().FadeAndLoadScene(SceneFader.FadeDirection.In, "LevelTwo"));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.tag == "witchesbrew")
        {
            moveSpeed = 4.0f;
        }
        else if(other.gameObject.tag == "enemy")
        {
            ghostTouched = false;
        }
    }
    #endregion

    #region projectile shooting
    private void ShootGum()
    {
        gum--;
        UpdateGumCount(gum);
        GameObject gumProj = Instantiate(gumProjectile);
        gumProj.transform.position = playerCam.transform.position + playerCam.transform.forward;
        gumProj.transform.forward = playerCam.transform.forward;
    }

    #endregion
    #region check win/loss
    private void CheckLoss(float hp)
    {
        if (hp == 0)
        {
            StartCoroutine(GameObject.FindObjectOfType<SceneFader>().FadeAndLoadScene(SceneFader.FadeDirection.In, "Lose"));
        }

        else { return; }
    }

    private void CheckWin()
    {
        if (sceneName == "Tutorial" && finalCandyCollected)
        {
            StartCoroutine(GameObject.FindObjectOfType<SceneFader>().FadeAndLoadScene(SceneFader.FadeDirection.In, "LevelOne"));
        }
        else if (sceneName == "LevelOne" && finalCandyCollected)
        {
            StartCoroutine(GameObject.FindObjectOfType<SceneFader>().FadeAndLoadScene(SceneFader.FadeDirection.In, "LevelTwo")); 
        }
        else if (sceneName == "LevelTwo" && finalCandyCollected)
        {
            StartCoroutine(GameObject.FindObjectOfType<SceneFader>().FadeAndLoadScene(SceneFader.FadeDirection.In, "Win"));
        }
    }

    #endregion

    #region ui updating
    private void UpdateSpiritBar(float currentSpirit)
    {
        spiritBar.value = currentSpirit;
    }

    private void UpdateGumCount(int currentGum)
    {
        gumText.text = "x" + gum;
    }
    private void RaiseCandyLevelInBucket()
    {
        if (sceneName == "Tutorial" && candyCollected == 1)
        {
            topCandy.SetActive(true);
        }
        else if (sceneName == "LevelOne" && candyCollected == 1)
        {
            bottomCandy.SetActive(true);
        }
        else if (sceneName == "LevelOne" && candyCollected == 3)
        {
            middleCandy.SetActive(true);
        }
        else if (sceneName == "LevelOne" && candyCollected == 6)
        {
            topCandy.SetActive(true);
        }
        else if (sceneName == "LevelTwo" && candyCollected == 2)
        {
            bottomCandy.SetActive(true);
        }
        else if (sceneName == "LevelTwo" && candyCollected == 4)
        {
            middleCandy.SetActive(true);
        }
        else if (sceneName == "LevelTwo" && candyCollected == 8)
        {
            topCandy.SetActive(true);
        }
    }
    #endregion

    private void CheatCodes()
    {
        if (Input.GetKeyDown(addGum))
        {
            gum++;
            UpdateGumCount(gum);
        }
        if (Input.GetKeyDown(addSpirit))
        {
            spirit += 10;
            UpdateSpiritBar(spirit);
        }
        if (Input.GetKeyDown(moveToNextLevel) && sceneName == "Tutorial")
        {
            StartCoroutine(GameObject.FindObjectOfType<SceneFader>().FadeAndLoadScene(SceneFader.FadeDirection.In, "LevelOne"));
        }
        else if (Input.GetKeyDown(moveToNextLevel) && sceneName == "LevelOne")
        {
            StartCoroutine(GameObject.FindObjectOfType<SceneFader>().FadeAndLoadScene(SceneFader.FadeDirection.In, "LevelTwo"));
        }
        else if (Input.GetKeyDown(moveToPrevLevel) && sceneName == "LevelTwo")
        {
            StartCoroutine(GameObject.FindObjectOfType<SceneFader>().FadeAndLoadScene(SceneFader.FadeDirection.In, "LevelOne"));
        }
        else if (Input.GetKeyDown(moveToPrevLevel) && sceneName == "LevelOne")
        {
            StartCoroutine(GameObject.FindObjectOfType<SceneFader>().FadeAndLoadScene(SceneFader.FadeDirection.In, "Tutorial"));
        }
    }
}
