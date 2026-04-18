using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Collider2D))]
public class DragNDrop : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private Vector3 _originPosition;
    private Camera _cam;
    private bool _isDragged;
    
    [SerializeField] private float followSpeed;
    [SerializeField] private float acceleration;
    
    public UnityEvent onSuccessfulDrop;
    
    private void Start()
    {
        _originPosition = transform.position;
        _cam = Camera.main;
    }

    private void Update()
    {
        if (!_isDragged) return;
        Vector2 mousePos = _cam.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        var distance = Vector2.Distance(transform.position, mousePos);
        var speedMultiplier = acceleration * distance * Time.deltaTime;
        var direction = (mousePos - (Vector2)transform.position).normalized;
        
        transform.Translate(followSpeed * speedMultiplier * direction);
        transform.position =  new Vector3(transform.position.x, transform.position.y, _originPosition.z - 0.1f);
    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
        _isDragged = true;
        Debug.Log("OnPointerDown " + gameObject.name);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _isDragged = false;
        if (Camera.main == null) return;
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(eventData.position);
        var hit = Physics2D.Raycast(mousePos, Vector2.zero, 100f, LayerMask.GetMask("DropArea"));

        if (hit.collider == null)
        {
            ResetPosition();
            return;
        }
        
        onSuccessfulDrop?.Invoke();
    }

    public void ResetPosition()
    {
        transform.position = _originPosition;
    }
}
