using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragAndDrop : MonoBehaviour
{
    MinigameManager m_minigameManager;

    //bool canMove;
    bool dragging;
    new Collider2D collider;
    new Collider2D cutCollider;
    new Collider2D sinkCollider;

    private void Awake()
    {
        m_minigameManager = FindObjectOfType<MinigameManager>();
    }

    void Start()
    {
        collider = GetComponent<Collider2D>();
        //cutCollider = m_minigameManager.getIngr(0).GetComponent<Collider2D>(); // Cutting Board
        //sinkCollider = m_minigameManager.getIngr(1).GetComponent<Collider2D>(); // Sink
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

            // if touching cutting board, send n, xy position and xy scale (4 elemnt array?)
                // minigame manager will carry out the 
            // else, send xy = -1f -1f -1f -1f

            if (collider.bounds.Intersects(m_minigameManager.getIngr(1).GetComponent<Collider2D>().bounds))
            {
                m_minigameManager.touchingSink(this.name);
            }
            // If touching cutting board, check for other objects it can be combined with
                // If successfully combined, load a new ingredient! At the current transform's x/y
            // If touching sink, check if it can be filled with water
        }
    }

}
