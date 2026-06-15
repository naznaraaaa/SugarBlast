using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public int width = 5;
    public int height = 5;
    public float spacing = 1f;

    public GameObject[] candies;
    public GameObject[,] board;

    public Transform candyParent; // 🔥 TAMBAHAN

    private MatchFinder matchFinder;
    private bool[,] validTiles;

    void Start()
    {
        board = new GameObject[width, height];
        matchFinder = GetComponent<MatchFinder>();

        InitGridShape();
        SetupBoard();

        StartCoroutine(ClearStartMatches());
    }

    void InitGridShape()
    {
        if (width == 5 && height == 5)
        {
            validTiles = null;
            return;
        }

        if (width == 7 && height == 7)
        {
            validTiles = new bool[,]
            {
                {false, true, true, true, true, true, false},
                {true, true, true, true, true, true, true},
                {true, true, true, true, true, true, true},
                {true, true, true, true, true, true, true},
                {true, true, true, true, true, true, true},
                {true, true, true, true, true, true, true},
                {false, true, true, true, true, true, false}
            };
            return;
        }

        if (width == 9 && height == 9)
        {
            validTiles = new bool[,]
            {
                {false,true,false,false,false,false,false,true,false},
                {true,true,true,false,false,false,true,true,true},
                {false,true,true,true,true,true,true,true,false},
                {false,false,true,true,true,true,true,false,false},
                {false,false,true,true,true,true,true,false,false},
                {false,false,true,true,true,true,true,false,false},
                {false,true,true,true,true,true,true,true,false},
                {true,true,true,false,false,false,true,true,true},
                {false,true,false,false,false,false,false,true,false}
            };
        }
    }

    void SetupBoard()
    {
        float offsetX = (width - 1) / 2f;
        float offsetY = (height - 1) / 2f;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (validTiles != null && !validTiles[x, y])
                    continue;

                SpawnCandy(x, y, offsetX, offsetY);
            }
        }
    }

    void SpawnCandy(int x, int y, float offsetX, float offsetY)
    {
        int rand = Random.Range(0, candies.Length);

        Vector2 localPos = new Vector2(
            (x - offsetX) * spacing,
            (y - offsetY) * spacing
        );

        Vector2 worldPos = localPos + (Vector2)transform.position;

        // 🔥 MASUK KE CANDIES HOLDER
        GameObject obj = Instantiate(candies[rand], worldPos, Quaternion.identity, candyParent);

        Candy c = obj.GetComponent<Candy>();
        c.x = x;
        c.y = y;

        board[x, y] = obj;
    }

    public GameObject GetCandyAt(int x, int y)
    {
        if (x >= 0 && x < width && y >= 0 && y < height)
        {
            if (validTiles != null && !validTiles[x, y])
                return null;

            return board[x, y];
        }
        return null;
    }

    public bool ProcessMatches()
    {
        List<GameObject> matches = matchFinder.FindMatches();

        if (matches.Count < 3)
            return false;

        if (GameManager.instance.sfxSource != null &&
            GameManager.instance.matchSound != null)
        {
            GameManager.instance.sfxSource.PlayOneShot(
                GameManager.instance.matchSound
            );
}

        int matchCount = matches.Count;
        int score = 5 + (matchCount - 3) * 2;

        GameManager.instance.AddScore(score);

        foreach (GameObject obj in matches)
        {
            if (obj != null)
            {
                Candy c = obj.GetComponent<Candy>();
                board[c.x, c.y] = null;
                Destroy(obj);
            }
        }

        StartCoroutine(FillBoardRoutine());
        return true;
    }

    IEnumerator FillBoardRoutine()
    {
        yield return new WaitForSeconds(0.2f);

        Collapse();
        yield return new WaitForSeconds(0.2f);

        Refill();
        yield return new WaitForSeconds(0.2f);

        ProcessMatches();
    }

    IEnumerator ClearStartMatches()
    {
        yield return new WaitForSeconds(0.1f);

        while (true)
        {
            List<GameObject> matches = matchFinder.FindMatches();

            if (matches.Count < 3)
                break;

            foreach (GameObject obj in matches)
            {
                if (obj != null)
                {
                    Candy c = obj.GetComponent<Candy>();
                    board[c.x, c.y] = null;
                    Destroy(obj);
                }
            }

            yield return new WaitForSeconds(0.2f);

            Collapse();
            Refill();

            yield return new WaitForSeconds(0.2f);
        }
    }

    void Collapse()
    {
        float offsetX = (width - 1) / 2f;
        float offsetY = (height - 1) / 2f;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (validTiles != null && !validTiles[x, y])
                    continue;

                if (board[x, y] == null)
                {
                    for (int i = y + 1; i < height; i++)
                    {
                        if (validTiles != null && !validTiles[x, i])
                            continue;

                        if (board[x, i] != null)
                        {
                            GameObject obj = board[x, i];
                            board[x, y] = obj;
                            board[x, i] = null;

                            Candy c = obj.GetComponent<Candy>();
                            c.y = y;

                            Vector2 localPos = new Vector2(
                                (x - offsetX) * spacing,
                                (y - offsetY) * spacing
                            );

                            obj.transform.position = localPos + (Vector2)transform.position;

                            break;
                        }
                    }
                }
            }
        }
    }

    void Refill()
    {
        float offsetX = (width - 1) / 2f;
        float offsetY = (height - 1) / 2f;

        for (int x = 0; x < width; x++)
        {
            for (int y = height - 1; y >= 0; y--)
            {
                if (validTiles != null && !validTiles[x, y])
                    continue;

                if (board[x, y] == null)
                {
                    SpawnCandy(x, y, offsetX, offsetY);
                }
            }
        }
    }
}