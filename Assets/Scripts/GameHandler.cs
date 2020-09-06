using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameHandler : MonoBehaviour
{
    public GameObject card1;
    public GameObject handArea;

    void Start()
    {
        for(int i = 0; i < 5; i++)
        {
            GameObject playerCard = Instantiate(card1, new Vector3(0, 0, 0), Quaternion.identity);
            playerCard.transform.SetParent(handArea.transform, false);
        }
    }
}
