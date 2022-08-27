using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class loadFriend : MonoBehaviour
{

    public Transform friendsPosition;

    private float distance = 10;
    [SerializeField]
    private float scaleLetter = 1;

    void Start()
    {
        int nFriends = 5;//Acces friend list

        //List<playerInfo> friends;  Acces info players

        for(int i = 0; i < nFriends; i++)
        {
            GameObject friend = new GameObject();

            friend.transform.parent = friendsPosition;

            friend.transform.position = new Vector3(friendsPosition.position.x, friendsPosition.position.y - distance * i, friendsPosition.position.z);

            TextMeshProUGUI friendText = friend.AddComponent<TextMeshProUGUI>();
            //friend.GetComponent<RectTransform>().localScale = new Vector3(scaleLetter, scaleLetter, scaleLetter);
            friendText.text = "ANTONIO"+i;
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
