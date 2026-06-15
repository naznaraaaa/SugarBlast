using UnityEngine;
using System.Collections;

public class Candy : MonoBehaviour
{
    private Vector2 firstTouchPosition;
    private Vector2 finalTouchPosition;

    public float swipeResist = 0.5f;
    public float moveSpeed = 8f;

    private BoardManager board;

    public int x;
    public int y;
    public int candyID;

    private bool isMoving = false;

    void Start()
    {
        board = FindObjectOfType<BoardManager>();
    }

    void OnMouseDown()
    {
        if (isMoving) return;

        firstTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    void OnMouseUp()
    {
        if (isMoving) return;

        finalTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        CalculateSwipe();
    }

    void CalculateSwipe()
    {
        float dx = finalTouchPosition.x - firstTouchPosition.x;
        float dy = finalTouchPosition.y - firstTouchPosition.y;

        if (Mathf.Abs(dx) > swipeResist || Mathf.Abs(dy) > swipeResist)
        {
            if (Mathf.Abs(dx) > Mathf.Abs(dy))
                StartCoroutine(MoveRoutine(dx > 0 ? 1 : -1, 0));
            else
                StartCoroutine(MoveRoutine(0, dy > 0 ? 1 : -1));
        }
    }

    IEnumerator MoveRoutine(int xDir, int yDir)
    {
        isMoving = true;

        int targetX = x + xDir;
        int targetY = y + yDir;

        GameObject other = board.GetCandyAt(targetX, targetY);

        if (other == null)
        {
            isMoving = false;
            yield break;
        }

        Candy otherCandy = other.GetComponent<Candy>();

        Vector2 myStart = transform.position;
        Vector2 otherStart = other.transform.position;

        int myOldX = x;
        int myOldY = y;

        int otherOldX = otherCandy.x;
        int otherOldY = otherCandy.y;

        // 🔄 SWAP ANIMASI (barengan)
        Coroutine c1 = StartCoroutine(SmoothMove(transform, otherStart));
        Coroutine c2 = StartCoroutine(SmoothMove(other.transform, myStart));

        yield return c1;
        yield return c2;

        // 🔄 UPDATE DATA GRID (manual, tanpa UpdateBoard)
        board.board[myOldX, myOldY] = other;
        board.board[targetX, targetY] = gameObject;

        x = targetX;
        y = targetY;

        otherCandy.x = myOldX;
        otherCandy.y = myOldY;

        // 🔍 CEK MATCH
        bool isMatch = board.ProcessMatches();

        if (!isMatch)
        {
            yield return new WaitForSeconds(0.1f);

            // 🔁 BALIKIN
            Coroutine r1 = StartCoroutine(SmoothMove(transform, myStart));
            Coroutine r2 = StartCoroutine(SmoothMove(other.transform, otherStart));

            yield return r1;
            yield return r2;

            // balikin data grid
            board.board[myOldX, myOldY] = gameObject;
            board.board[targetX, targetY] = other;

            x = myOldX;
            y = myOldY;

            otherCandy.x = otherOldX;
            otherCandy.y = otherOldY;
        }

        isMoving = false;
    }

    IEnumerator SmoothMove(Transform obj, Vector2 target)
    {
        while (Vector2.Distance(obj.position, target) > 0.01f)
        {
            obj.position = Vector2.MoveTowards(
                obj.position,
                target,
                moveSpeed * Time.deltaTime
            );
            yield return null;
        }

        obj.position = target;
    }
}