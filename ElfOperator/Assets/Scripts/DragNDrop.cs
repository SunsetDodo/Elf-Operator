using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Collider2D))]
public class DragNDrop : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private Vector2 originPosition;
    
    [SerializeField] private float followSpeed;
    [SerializeField] private float acceleration;

    public bool isDragged;
    Camera cam;
    
    public List<string> whitelistTags = new List<string>();
    public UnityEvent<GameObject> onSuccessfulDrop;
    
    private void Start()
    {
        originPosition = transform.position;
        cam = Camera.main;
    }

    private void Update()
    {
        if (!isDragged) return;
        Vector2 mousePos = cam.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        var distance = Vector2.Distance(transform.position, mousePos);
        var speedMultiplier = acceleration * distance * Time.deltaTime;
        var direction = (mousePos - (Vector2)transform.position).normalized;
        
        transform.Translate(followSpeed * speedMultiplier * direction);
    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
        isDragged = true;
        Debug.Log("OnPointerDown " + gameObject.name);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isDragged = false;
        if (Camera.main == null) return;
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(eventData.position);
        var hit = Physics2D.Raycast(mousePos, Vector2.zero);

        if (hit.collider == null || !whitelistTags.Contains(hit.transform.tag)) return;
        
        onSuccessfulDrop?.Invoke(gameObject);
    }

    public void ResetPosition()
    {
        transform.position = originPosition;
    }
}
