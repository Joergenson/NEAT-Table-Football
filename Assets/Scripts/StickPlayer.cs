using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickPlayer : MonoBehaviour
{
    [SerializeField] private Stick _stick;

    private void OnCollisionEnter(Collision other)
    {
        _stick.AddInteraction();
    }
}
