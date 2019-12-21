/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

using System;
using UnityEngine;

public class UIArrow : MonoBehaviour
{
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private Sprite arrowOpen = null;
    [SerializeField] private Sprite arrowClosed = null;
    [SerializeField] private Sprite arrowDrop = null;

    private Collider2D collider2d;
    private SpriteRenderer spriteRenderer;
    private Color color;

    public int Column { get; set; }

    public bool IsOpen
    {
        get => collider2d.enabled;
        set
        {
            collider2d.enabled = value;
            spriteRenderer.sprite = value ? arrowOpen : arrowClosed;
        }
    }

    private void Awake()
    {
        collider2d = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        color = spriteRenderer.color;
    }

    private void OnMouseEnter()
    {
        spriteRenderer.sprite = arrowDrop;
        color.a = 1;
        spriteRenderer.color = color;
    }

    private void OnMouseExit()
    {
        spriteRenderer.sprite = arrowOpen;
    }

    private void OnMouseDown()
    {
        Click?.Invoke(Column);
    }

    private void Update()
    {
        if (IsOpen && spriteRenderer.sprite == arrowOpen)
        {
            color.a = Mathf.Cos(Time.time * Mathf.PI * 2 / fadeDuration)
                * 0.25f + 0.75f;
            spriteRenderer.color = color;
        }
    }

    public event Action<int> Click;
}
