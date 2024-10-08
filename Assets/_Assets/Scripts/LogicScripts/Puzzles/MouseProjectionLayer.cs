using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static GameInput;

public class MouseProjectionLayer : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameInput gameInput;

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("Mouse Projection Layer pointer enter");
        gameInput.OnPointerEnterBallObstacleMouseProjectionLayer(this, new OnPointerHoverStatusChangeEventArgs()
        {
            isInside = true
        });
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("Mouse Projection Layer pointer Exit");
        gameInput.OnPointerExitBallObstacleMouseProjectionLayer(this, new OnPointerHoverStatusChangeEventArgs()
        {
            isInside = false
        });
    }
}
