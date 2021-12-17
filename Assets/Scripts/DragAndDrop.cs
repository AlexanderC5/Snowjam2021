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
            //Debug.Log(mousePos);

            if (dragging)
            {
                dragging = false;
                //Debug.Log("Y1= " + (collider.bounds.center.y - collider.bounds.size.y / 2) / m_minigameManager.getScreenY());
                //Debug.Log("Y2= " + (collider.bounds.center.y + collider.bounds.size.y / 2) / m_minigameManager.getScreenY());

                // if touching cutting board, tell the minigameManager to check the recipe list
                if (((collider.bounds.center.x - collider.bounds.size.x / 2) / m_minigameManager.getScreenX() >= m_minigameManager.cutbXbounds[0])
                 && ((collider.bounds.center.x + collider.bounds.size.x / 2) / m_minigameManager.getScreenX() <= m_minigameManager.cutbXbounds[1])
                 && ((collider.bounds.center.y - collider.bounds.size.y / 2) / m_minigameManager.getScreenY() >= m_minigameManager.cutbYbounds[0])
                 && ((collider.bounds.center.y + collider.bounds.size.y / 2) / m_minigameManager.getScreenY() <= m_minigameManager.cutbYbounds[1]))
                {
                    m_minigameManager.onCuttingBoard(this.name);
                }

                // If touching sink, check if it can be filled with water
                if (((collider.bounds.center.x - collider.bounds.size.x / 2) / m_minigameManager.getScreenX() >= m_minigameManager.sinkXbounds[0])
                 && ((collider.bounds.center.x + collider.bounds.size.x / 2) / m_minigameManager.getScreenX() <= m_minigameManager.sinkXbounds[1])
                 && ((collider.bounds.center.y - collider.bounds.size.y / 2) / m_minigameManager.getScreenY() >= m_minigameManager.sinkYbounds[0])
                 && ((collider.bounds.center.y + collider.bounds.size.y / 2) / m_minigameManager.getScreenY() <= m_minigameManager.sinkYbounds[1]))
                {
                    m_minigameManager.touchingSink(this.name);
                }



                // OLD attempts at trying to use colliders to define the sink/cut_board bounds. Didn't work
                // because they would sometimes jump in front of the other ingredients and prevent the other
                // ingredients from being dragged, so I switched to using coordinate bounds:

                /*
                if (collider.bounds.Intersects(m_minigameManager.getIngr(0).GetComponent<Collider2D>().bounds))
                {
                    m_minigameManager.onCuttingBoard(this.name, true);
                }
                else m_minigameManager.onCuttingBoard(this.name, false);
                */

                /*
                if (collider.bounds.Intersects(m_minigameManager.getIngr(1).GetComponent<Collider2D>().bounds))
                {
                    m_minigameManager.touchingSink(this.name);
                }
                */
            }
        }
    }

}
