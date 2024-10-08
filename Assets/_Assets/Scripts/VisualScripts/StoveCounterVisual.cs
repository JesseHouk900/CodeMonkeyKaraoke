using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoveCounterVisual : MonoBehaviour
{

    [SerializeField] private StoveCounter stoveCounter;
    [SerializeField] private GameObject stoveOnVisualGameObject;
    [SerializeField] private GameObject sizzlingParticlesGameObject;

    private void Start()
    {
        stoveCounter.OnStateChanged += StoveCounter_OnStateChanged;
    }

    private void StoveCounter_OnStateChanged(object sender, StoveCounter.OnStateChangedEventArgs e)
    {
        bool showVisual = e.state != StoveCounter.State.Idle;
        stoveOnVisualGameObject.SetActive(showVisual);
        sizzlingParticlesGameObject.SetActive(showVisual);
    }
}
