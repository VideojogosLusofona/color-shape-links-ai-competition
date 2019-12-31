/// @file
/// @brief This file contains the ::HumanMoveButton class.
///
/// @author Nuno Fachada
/// @date 2019
/// @copyright [MPLv2](http://mozilla.org/MPL/2.0/)

using System;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Script that controls a UI button for performing human moves.
/// </summary>
public class HumanMoveButton : MonoBehaviour
{
    /// <summary>Duration of button fade in and out effect.</summary>
    [SerializeField] private float fadeDuration = 1f;

    /// <summary>Minimum transparency for fade in and out effect.</summary>
    [SerializeField] [Range(0f, 1f)] private float fadeMinTransparency = 0.2f;

    /// <summary>Reference to the open arrow sprite.</summary>
    [SerializeField] private Sprite arrowOpen = null;

    /// <summary>Reference to the closed arrow sprite.</summary>
    [SerializeField] private Sprite arrowClosed = null;

    /// <summary>Reference to the drop arrow sprite.</summary>
    [SerializeField] private Sprite arrowDrop = null;

    // Collider used for detecting mouse events
    private Collider2D collider2d;

    // Sprite renderer for this arrow button
    private SpriteRenderer spriteRenderer;

    // Sprite color, used for manipulating the button transparency
    private Color color;

    /// <summary>Board column to which the button is associated.</summary>
    /// <value>
    /// Value between 0 and <see cref="SessionController.cols"/> - 1.
    /// </value>
    public int Column { get; set; }

    /// <summary>
    /// Open or close the arrow button. An open button can be clicked, usually
    /// meaning that the associated column still has space for at least one
    /// more piece. A closed button cannot be clicked, and usually implies that
    /// the associated column is full.
    /// </summary>
    /// <value>
    /// `true` if the button is to be opened, `false` if the button is
    /// to be closed.
    /// </value>
    public bool Open
    {
        set
        {
            // Basically, the collider being enable or not defines whether the
            // button is open or closed
            collider2d.enabled = value;

            // Set the sprite according to the button state
            spriteRenderer.sprite = value ? arrowOpen : arrowClosed;
        }
    }

    // Awake is called when the script instance is being loaded.
    private void Awake()
    {
        // Initialize the Unity event used for clicks
        Click = new IntEvent();

        // Get reference to the associated collider
        collider2d = GetComponent<Collider2D>();

        // Get reference to the associated sprite renderer
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Get the current sprite color
        color = spriteRenderer.color;
    }

    // This function is called when the object becomes enabled and active
    private void OnEnable()
    {
        // When the arrow is enabled, make sure it doesn't get stuck with the
        // arrowDrop sprite
        if (spriteRenderer.sprite == arrowDrop)
        {
            spriteRenderer.sprite = arrowOpen;
        }
    }

    // Called when the mouse enters the collider
    private void OnMouseEnter()
    {
        spriteRenderer.sprite = arrowDrop;
        color.a = 1;
        spriteRenderer.color = color;
    }

    // Called when the mouse leaves the collider
    private void OnMouseExit()
    {
        if (collider2d.enabled)
            spriteRenderer.sprite = arrowOpen;
    }

    // Called when the user has pressed the mouse button while over the
    // collider
    private void OnMouseDown()
    {
        // Notify listeners that a piece is to be dropped in this column
        Click?.Invoke(Column);
    }

    // Update is called every frame, if the MonoBehaviour is enabled
    private void Update()
    {
        // Apply fade in/out effect if button is open and not hovered by mouse
        if (collider2d.enabled && spriteRenderer.sprite == arrowOpen)
        {
            color.a = Mathf.Cos(Time.time * Mathf.PI * 2 / fadeDuration)
                * (1 - fadeMinTransparency) / 2
                + (1 + fadeMinTransparency) / 2;
            spriteRenderer.color = color;
        }
    }

    // Unity event to be invoked when a button is clicked
    public IntEvent Click { get; private set; }

    // Type of Unity event that accepts an int, used for indicating the column
    // associated with the clicked button
    [Serializable] public class IntEvent : UnityEvent<int> {}
}
