using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragAndDrop : MonoBehaviour
{
    bool canMove;
    bool dragging;
    new Collider2D collider;
    Collider2D endCollide;
    bool win;

    void Start()
    {
        collider = GetComponent<Collider2D>();
        canMove = false;
        dragging = false;
        win = false;
        endCollide = GameObject.Find("EndGoal").GetComponent<Collider2D>(); //target object
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (Input.GetMouseButtonDown(0))
        {
            if (collider == Physics2D.OverlapPoint(mousePos))
            {
                canMove = true;
            }
            else
            {
                canMove = false;
            }
            if (canMove)
            {
                dragging = true;
            }


        }
        if (dragging)
        {
            this.transform.position = mousePos;
        }
        if (Input.GetMouseButtonUp(0))
        {
            canMove = false;
            dragging = false;
            if(endCollide.bounds.Intersects(collider.bounds)){ //checks that object is dropped in the right place
              win = true;
              //whatever needs to be done if dropped correctly
            }
        }
    }

}
