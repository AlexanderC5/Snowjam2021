using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragAndDrop : MonoBehaviour
{
    //bool canMove;
    bool dragging;
    new Collider2D collider;
    //Collider2D endCollide;
    //bool win;

    void Start()
    {
        collider = GetComponent<Collider2D>();
        //canMove = false;
        dragging = false;
        //win = false;
        //endCollide = GameObject.Find("EndGoal").GetComponent<Collider2D>(); //target object
    }

    // Update is called once per frame
    void Update()
    {
        //Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePos = Input.mousePosition;

        if (Input.GetMouseButtonDown(0))
        {
            if (collider == Physics2D.OverlapPoint(mousePos))
            {
                dragging = true;
            }
            else
            {
                //dragging = false;
            }
        }
        if (dragging)
        {
            this.transform.position = mousePos;
        }
        if (Input.GetMouseButtonUp(0))
        {
            dragging = false;

            // If touching cutting board, check for other objects it can be combined with
                // If successfully combined, load a new ingredient! At the current transform's x/y
            // If touching sink, check if it can be filled with water
        }
    }

}
