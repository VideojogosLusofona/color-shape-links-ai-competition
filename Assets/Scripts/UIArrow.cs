/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

using System;
using UnityEngine;
using UnityEngine.Events;

public class UIArrow : MonoBehaviour
{
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] [Range(0f, 1f)] private float fadeMinTransparency = 0.2f;
    [SerializeField] private Sprite arrowOpen = null;
    [SerializeField] private Sprite arrowClosed = null;
    [SerializeField] private Sprite arrowDrop = null;

    private Collider2D collider2d;
    private SpriteRenderer spriteRenderer;
    private Color color;

    public int Column { get; set; }

    public bool Open
    {
        set
        {
            collider2d.enabled = value;
            spriteRenderer.sprite = value ? arrowOpen : arrowClosed;
        }
    }

    private void Awake()
    {
        Click = new IntEvent();
        collider2d = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        color = spriteRenderer.color;
    }

    private void OnEnable()
    {
        // When the arrow is enabled, make sure it doesn't get stuck with the
        // arrowDrop sprite
        if (spriteRenderer.sprite == arrowDrop)
        {
            spriteRenderer.sprite = arrowOpen;
        }
    }

    private void OnMouseEnter()
    {
        spriteRenderer.sprite = arrowDrop;
        color.a = 1;
        spriteRenderer.color = color;
    }

    private void OnMouseExit()
    {
        if (collider2d.enabled)
            spriteRenderer.sprite = arrowOpen;
    }

    private void OnMouseDown()
    {
        Click?.Invoke(Column);
    }

    private void Update()
    {
        if (collider2d.enabled && spriteRenderer.sprite == arrowOpen)
        {
            color.a = Mathf.Cos(Time.time * Mathf.PI * 2 / fadeDuration)
                * (1 - fadeMinTransparency) / 2
                + (1 + fadeMinTransparency) / 2;
            spriteRenderer.color = color;
        }
    }

    public IntEvent Click { get; private set; }

    [Serializable] public class IntEvent : UnityEvent<int> {}
}
