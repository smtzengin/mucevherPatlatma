using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gem : MonoBehaviour
{
    [HideInInspector] public Vector2Int posIndex;

    [HideInInspector] public Board board;


    public Vector2 firstTouchPos;
    public Vector2 lastTouchPos;

    bool isMousePressed;
    float dragAngle;

    Gem otherGem;

    public bool isMatched;

    Vector2Int firstPos;


    public enum GemType
    {
        blue,
        pink,
        yellow,
        peaGreen,
        darkGreen,
        bomb
       
    };

    public GemType type;

    public GameObject gemEffect;

    public int bombVolume;
    
    private void Update()
    {
        if(Vector2.Distance(transform.position,posIndex) > .01f)
        {
            transform.position = Vector2.Lerp(transform.position, posIndex, board.gemSpeed * Time.deltaTime);
        }
        else
        {
            transform.position = new Vector3(posIndex.x, posIndex.y, 0f);
        }


        if (isMousePressed && Input.GetMouseButtonUp(0))
        {
            isMousePressed = false;
            if (board.validState == Board.BoardState.moving)
            {
                lastTouchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                CalculateAngle();
            }
            
        }
    }


    public void EditGem(Vector2Int pos,Board theboard)
    {
        posIndex = pos;
        board = theboard;
    }

    private void OnMouseDown()
    {
        if(board.validState == Board.BoardState.moving)
        {
            firstTouchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            isMousePressed = true;
        }
        
    }

    void CalculateAngle()
    {
        float dx = lastTouchPos.x - firstTouchPos.x;
        float dy = lastTouchPos.y - firstTouchPos.y;

        dragAngle = Mathf.Atan2(dy, dx); //arctan'� hesaplad�k.

        dragAngle = dragAngle * 180 / Mathf.PI; //Radyant� dereceye �evirdik.

        if (Vector3.Distance(firstTouchPos,lastTouchPos) > 0.5f)
        {
            MoveTile();
        }
    }

    void MoveTile()
    {
        firstPos = posIndex;

        if(dragAngle<45 && dragAngle > -45 && posIndex.x < board.width-1)
        {
            //i�aretledi�imiz m�cevher 0,0 ise di�er yan�ndaki m�cevher 1,0
            otherGem = board.allGems[posIndex.x + 1, posIndex.y];
            otherGem.posIndex.x--; // di�er m�cevheri yer de�i�tirece�imiz i�in azaltt�k.
            posIndex.x++; // ilk m�cevheeri de bir artt�rd�k.
        }
        else if (dragAngle > 45 && dragAngle <= 135 && posIndex.y < board.height - 1)
        {
            //i�aretledi�imiz m�cevher 0,0 ise di�er yan�ndaki m�cevher 1,0
            otherGem = board.allGems[posIndex.x , posIndex.y+1];
            otherGem.posIndex.y--;
            posIndex.y++; 
        }
        else if (dragAngle >= -135 && dragAngle < -45 && posIndex.y >0)
        {
            //i�aretledi�imiz m�cevher 0,0 ise di�er yan�ndaki m�cevher 1,0
            otherGem = board.allGems[posIndex.x, posIndex.y - 1];
            otherGem.posIndex.y++; 
            posIndex.y--; 
        }
        else if (dragAngle > 135 || dragAngle < -135 && posIndex.x > 0)
        {
            //i�aretledi�imiz m�cevher 0,0 ise di�er yan�ndaki m�cevher 1,0
            otherGem = board.allGems[posIndex.x-1, posIndex.y];
            otherGem.posIndex.x++;
            posIndex.x--; 
        }

        board.allGems[posIndex.x, posIndex.y] = this;
        board.allGems[otherGem.posIndex.x, otherGem.posIndex.y] = otherGem;

        StartCoroutine(ControlMovement());
    }

    public IEnumerator ControlMovement()
    {
        board.validState = Board.BoardState.waiting;

        //E�ER M�CEVHERLER E�LE�MED�YSE M�CEVHER�N OYNATILMASINI ENGELL�YORUZ.

        yield return new WaitForSeconds(.5f);

        board.matchUpController.FindMatchUp();

        if(otherGem != null)
        {
            if (!isMatched && !otherGem.isMatched) // e�le�mediyse
            {
                otherGem.posIndex = posIndex;
                posIndex = firstPos;

                board.allGems[posIndex.x, posIndex.y] = this;
                board.allGems[otherGem.posIndex.x, otherGem.posIndex.y] = otherGem;

                yield return new WaitForSeconds(.5f);
                board.validState = Board.BoardState.moving;

            }
            else
            {
                board.AllDestroyMatchedGems();
            }
        }
    }
}
