using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragAndDrop : MonoBehaviour
{
    MinigameManager m_minigameManager;

    bool dragging = false; // Is the ingredient currently being dragged?
    new Collider2D collider;

    private void Awake() // Link to the necessary game objects / componenets
    {
        m_minigameManager = FindObjectOfType<MinigameManager>();
        collider = GetComponent<Collider2D>();
    }

    void Update()
    {
        //Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition); // Doesn't work with the canvas
        Vector2 mousePos = Input.mousePosition;

        if (Input.GetMouseButtonDown(0)) // If mouse down on an ingredient, you can drag it
        {
            if (collider == Physics2D.OverlapPoint(mousePos)) { dragging = true; }
        }
        if (dragging) { this.transform.position = mousePos; }

        if (Input.GetMouseButtonUp(0)) // When lifting the mouse, stop dragging and check for any potential actions
        {
            if (dragging)
            {
                dragging = false;

                // Some Console logs to check the bounds of the ingredient as percentages of the screen
                //Debug.Log("X1= " + (collider.bounds.center.x - collider.bounds.size.x / 2) / m_minigameManager.getScreenX());
                //Debug.Log("X2= " + (collider.bounds.center.x + collider.bounds.size.x / 2) / m_minigameManager.getScreenX());
                //Debug.Log("Y1= " + (collider.bounds.center.y - collider.bounds.size.y / 2) / m_minigameManager.getScreenY());
                //Debug.Log("Y2= " + (collider.bounds.center.y + collider.bounds.size.y / 2) / m_minigameManager.getScreenY());

                // If touching cutting board, tell the minigameManager to check the recipe list
                if (((collider.bounds.center.x - collider.bounds.size.x / 2) / m_minigameManager.getScreenX() >= m_minigameManager.cutbXbounds[0])
                 && ((collider.bounds.center.x + collider.bounds.size.x / 2) / m_minigameManager.getScreenX() <= m_minigameManager.cutbXbounds[1])
                 && ((collider.bounds.center.y - collider.bounds.size.y / 2) / m_minigameManager.getScreenY() >= m_minigameManager.cutbYbounds[0])
                 && ((collider.bounds.center.y + collider.bounds.size.y / 2) / m_minigameManager.getScreenY() <= m_minigameManager.cutbYbounds[1]))
                {
                    m_minigameManager.onCuttingBoard(this.name);
                }

                // If touching the sink, check if it can be filled with water
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
