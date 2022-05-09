using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookManager : MonoBehaviour
{

    #region Private fields

    private string[,] books = new string[,] { { "4", "6", "9", "10" } , { "3", "7", "8", "11" } , { "5", "8", "9", "12" }, { "5", "7", "9", "10" } };

    private string[] height = new string[] { "Up", "UpMiddle", "DownMiddle", "Down" };

    private int cpt = 0;
    #endregion

    #region Public Fields

    public GameObject[] racks; // 0, 1 : B - 2, 3 : A

    #endregion

    #region MonoBehaviour CallBacks


    void Update()
    {
        if(cpt == 1000)
        {
            Debug.Log("Move a book ? ");
            // Every n frames
            if (Random.Range(0, 101) < 10)
            {
                Debug.Log("Yes");
                // Get the random values
                int rack = Random.Range(0, 4);
                int floor = Random.Range(0, 4);
                int bookInFloor = Random.Range(0, 4);

                // Select the right column
                string column = rack < 2 ? "ColumnB" : "ColumnA";

                Transform book = racks[rack].transform.Find( column ).transform.Find( height[floor] ).transform.Find( books[floor, bookInFloor] );

                book.GetComponent<Rigidbody>().useGravity = true;
                book.GetComponent<BoxCollider>().enabled = true;

                Debug.Log("Moved a book");
            }
            cpt = 0;
        }
        cpt++;
    }


    #endregion

    #region Private methods

    #endregion

    #region Public methods

    #endregion
}
