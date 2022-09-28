using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class roomControl : MonoBehaviour
{
    public GameObject createRoom;

    public GameObject inputField;

    public GameObject inputFieldText;

    public GameObject joinRoom;

    public GameObject roomText;

    public GameObject leaveRoom;

    void Start()
    {
        if(ComInfo.getSalaKey() != -1)
        {
            //Change buttons
            createRoom.SetActive(false);
            inputField.SetActive(false);
            joinRoom.SetActive(false);
            roomText.SetActive(true);
            leaveRoom.SetActive(true);

            roomText.GetComponent<TMPro.TextMeshProUGUI>().text = ComInfo.getSalaKey().ToString();
        }
    }

    public void createRoomFunction()
    {
        //Change buttons
        createRoom.SetActive(false);
        inputField.SetActive(false);
        joinRoom.SetActive(false);
        roomText.SetActive(true);
        leaveRoom.SetActive(true);

        //Create room
        int random = Random.Range(10000, 99999);

        roomText.GetComponent<Text>().text = random.ToString();

        ComInfo.setSalaKey(random, true);
    }
    public void joinRoomFunction()
    {
        if((12345-10000) >= 0) //If the room have 5 digits Dont change well the text to int
        {
            //Change buttons
            createRoom.SetActive(false);
            inputField.SetActive(false);
            joinRoom.SetActive(false);
            roomText.SetActive(true);
            leaveRoom.SetActive(true);


            //int sala = int.Parse(inputFieldText.GetComponent<TMPro.TextMeshProUGUI>().text); Da error ns porque
            int sala = int.Parse("12345");

            //roomText.GetComponent<TMPro.TextMeshProUGUI>().text = inputFieldText.GetComponent<TMPro.TextMeshProUGUI>().text;
            roomText.GetComponent<Text>().text = sala.ToString();

            ComInfo.setSalaKey(sala, false);

            Debug.Log("La sala es " + ComInfo.getSalaKey());

            SceneManager.UnloadSceneAsync("addFriends");
            SceneManager.UnloadSceneAsync("Lobby");
            SceneManager.LoadScene("Lobby", LoadSceneMode.Single);
        }
       
    }

    public void leaveRoomFunction()
    {
        //Change buttons
        createRoom.SetActive(true);
        inputField.SetActive(true);
        joinRoom.SetActive(true);
        roomText.SetActive(false);
        leaveRoom.SetActive(false);


        ComInfo.setSalaKey(-1, true);
    }

    public void startWritting()
    {
        Debug.Log("Sale KeyBoard");
        TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default);
    }
}
