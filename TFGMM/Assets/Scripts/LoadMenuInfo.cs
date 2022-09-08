using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class LoadMenuInfo : MonoBehaviour
{
    public TextMeshProUGUI topLeftName;

    public Text midButtonName;

    public TextMeshProUGUI roomCode;

    public 
    // Start is called before the first frame update
    void Start()
    {
        topLeftName.text = ComInfo.getPlayerName() + ComInfo.getPlayerID();

        midButtonName.text = ComInfo.getPlayerName();

        int room = ComInfo.getSalaKey();
        if (room != -1)
        {
            roomCode.text = "RoomCode: " + room;
        }
    }
}
