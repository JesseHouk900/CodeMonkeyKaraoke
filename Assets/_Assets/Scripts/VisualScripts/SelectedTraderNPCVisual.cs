using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedTraderNPCVisual : MonoBehaviour, IShowHide
{

    [SerializeField] private TraderNPC traderNPC;
    [SerializeField] private GameObject[] visualGameObjectArray;
    private void Start()
    {
        Player.Instance.OnSelectedTraderNPCChanged += Player_OnSelectedTraderNPCChanged;
    }

    private void Player_OnSelectedTraderNPCChanged(object sender, Player.OnSelectedTraderNPCChangedEventArgs e)
    {
        if (traderNPC == e.selectedTraderNPC)
        {
            Show();
        }
        else
        {
            Hide();
        }
    }

    private void Show()
    {
        foreach (GameObject visualGameObject in visualGameObjectArray)
        {
            visualGameObject.SetActive(true);
        }
    }

    private void Hide()
    {
        foreach (GameObject visualGameObject in visualGameObjectArray)
        {
            visualGameObject.SetActive(false);
        }
    }
}
