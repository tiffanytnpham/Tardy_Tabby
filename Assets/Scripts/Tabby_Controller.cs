using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tabby_Controller : MonoBehaviour
{
    // Public variables to adjust character settings and link UI elements.
    public float speed = 5.0f;  // Default speed of the character.
    public float jumpForce = 2.0f;  // Force applied when jumping.
    public float wetSpeed = 2.5f;  // Speed when the character is wet.
    public GameObject levelClearedUI;  // UI element shown when level is cleared.
    public AudioSource jumpSound;  // Sound played on jump.
    public AudioSource wetSound;  // Sound played when character gets wet.
    public Button continueButton;  // Button to proceed after clearing a level.
    public GameObject endSceneImage;  // Image shown at the end of the game.

    // Private variables for internal mechanics.
    private Rigidbody2D rb;  // Rigidbody component for physics.
    private Animator animator;  // Animator for managing animations.
    private bool canJump = true;  // Determines if character can jump.
    private bool isWet = false;  // Tracks if the character is wet.
    private int jumpCount = 0;  // Counts number of jumps.
    private int maxJumpCount = 2;  // Double jump.
    private static readonly string isWalkingParam = "isWalking";  // Animator parameter.
    private static readonly string isJumpingParam = "isJumping";  // Animator parameter.

    // Initialize settings and add listeners on start.
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        // Initially hide UI elements.
        levelClearedUI.SetActive(false);
        continueButton.gameObject.SetActive(false);
        endSceneImage.SetActive(false);
        // Listener for the continue button.
        continueButton.onClick.AddListener(DisplayEndScene);
    }

    // Update called once per frame to handle input and level completion check.
    void Update()
    {
        HandleMovement();
        CheckForLevelCompletion();
    }

    // FixedUpdate is used for applying forces and movement.
    void FixedUpdate()
    {
        float currentSpeed = isWet ? wetSpeed : speed;
        float moveHorizontal = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(moveHorizontal * currentSpeed, rb.velocity.y);
    }

    // Detect character on ground to reset jumping capability.
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground") || collision.collider.CompareTag("WindowSill"))
        {
            canJump = true;
            jumpCount = 0;
            animator.SetBool(isJumpingParam, false);
        }
    }

    // Trigger detection for water and disable jumping.
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Puddle"))
        {
            isWet = true;
            canJump = false;
        }
    }

    // Reset the wet status when not in puddle.
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Puddle"))
        {
            isWet = false;
            canJump = true;
        }
    }

    // Handle movement input and initiate jumping.
    void HandleMovement()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        animator.SetBool(isWalkingParam, Mathf.Abs(moveHorizontal) > 0.01f);

        if (Input.GetKeyDown(KeyCode.UpArrow) && canJump && jumpCount < maxJumpCount && !isWet)
        {
            Jump();
        }
    }

    // Jump mechanics.
    void Jump()
    {
        rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
        animator.SetBool(isJumpingParam, true);
        jumpCount++;
        if (jumpSound != null)
            jumpSound.Play();
    }

    // Display the level cleared UI elements.
    void ShowLevelCleared()
    {
        levelClearedUI.SetActive(true);
        ShowContinueButton();
    }

    // Display the continue button.
    void ShowContinueButton()
    {
        if (continueButton != null)
            continueButton.gameObject.SetActive(true);
    }

    // Check if the character has reached the end of the level.
    void CheckForLevelCompletion()
    {
        Vector2 screenPosition = Camera.main.WorldToViewportPoint(transform.position);
        if (screenPosition.x > 1 && !levelClearedUI.activeSelf)
        {
            ShowLevelCleared();
        }
    }

    // Display the end scene and hide other UI elements.
    void DisplayEndScene()
    {
        endSceneImage.SetActive(true);
        levelClearedUI.SetActive(false);
        continueButton.gameObject.SetActive(false);
    }

    // Clean up event listeners.
    void OnDestroy()
    {
        continueButton.onClick.RemoveListener(DisplayEndScene);
    }
}
