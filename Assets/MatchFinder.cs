using System.Collections.Generic;
using UnityEngine;

public class MatchFinder : MonoBehaviour
{
    private BoardManager board;

    void Start()
    {
        board = GetComponent<BoardManager>();
    }

    public List<GameObject> FindMatches()
    {
        List<GameObject> matches = new List<GameObject>();

        for (int x = 0; x < board.width; x++)
        {
            for (int y = 0; y < board.height; y++)
            {
                GameObject current = board.GetCandyAt(x, y);
                if (current == null) continue;

                string type = current.name.Replace("(Clone)", "");

                // horizontal
                if (x < board.width - 2)
                {
                    var c1 = board.GetCandyAt(x + 1, y);
                    var c2 = board.GetCandyAt(x + 2, y);

                    if (c1 && c2 &&
                        c1.name.Contains(type) &&
                        c2.name.Contains(type))
                    {
                        Add(matches, current);
                        Add(matches, c1);
                        Add(matches, c2);
                    }
                }

                // vertical
                if (y < board.height - 2)
                {
                    var c1 = board.GetCandyAt(x, y + 1);
                    var c2 = board.GetCandyAt(x, y + 2);

                    if (c1 && c2 &&
                        c1.name.Contains(type) &&
                        c2.name.Contains(type))
                    {
                        Add(matches, current);
                        Add(matches, c1);
                        Add(matches, c2);
                    }
                }
            }
        }

        return matches;
    }

    void Add(List<GameObject> list, GameObject obj)
    {
        if (!list.Contains(obj))
            list.Add(obj);
    }
}