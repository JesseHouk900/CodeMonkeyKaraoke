using System;
using UnityEngine;

public class FakePlayerInput : MonoBehaviour
{
    // Input events
    public event Action<Vector2> OnMoveInput;
    public event Action OnSwitchCamera;

    private void Update()
    {
        Vector2 inputVector = new Vector2(0, 0);
        if (Input.GetKey(KeyCode.W))
            inputVector.y += 1f;
        if (Input.GetKey(KeyCode.S))
            inputVector.y -= 1f;
        if (Input.GetKey(KeyCode.A))
            inputVector.x -= 1f;
        if (Input.GetKey(KeyCode.D))
            inputVector.x += 1f;

        if (inputVector != Vector2.zero)
            OnMoveInput?.Invoke(inputVector.normalized);

        if (Input.GetKeyDown(KeyCode.F))
            OnSwitchCamera?.Invoke();
    }
}