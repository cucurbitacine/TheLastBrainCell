using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    [SerializeField] private float parallaxEffectMultiplier = 1f;

    private Transform _cameraTransform;
    private Transform _transform;

    private Vector3 _lastCameraPosition;

    private void Awake()
    {
        _cameraTransform = Camera.main.transform;
        _transform = transform;
    }

    private void Start()
    {
        _lastCameraPosition = _cameraTransform.position;
    }

    private void LateUpdate()
    {
        Vector3 currentCameraPosition = _cameraTransform.position;
        Vector3 deltaMovement = currentCameraPosition - _lastCameraPosition;
        _transform.position += deltaMovement * parallaxEffectMultiplier;
        _lastCameraPosition = currentCameraPosition;
    }
}
