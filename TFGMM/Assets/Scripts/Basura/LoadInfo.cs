using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadInfo : MonoBehaviour
{
    public static int HeroExp { get; set; } //Number in the array of brawler we need info

    public GameObject HeroName;
    // Start is called before the first frame update
    

    private void OnEnable()
    {
        HeroName.GetComponent<TMPro.TextMeshProUGUI>().text = "Hero Name";
    }
}
