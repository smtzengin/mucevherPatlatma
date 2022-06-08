using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MatchUpController : MonoBehaviour
{
    Board board;

    public List<Gem> FoundGemsList = new List<Gem>(); // eþleþen mücevherler için liste oluþturuyoruz.

    private void Awake()
    {
        board = Object.FindObjectOfType<Board>();

    }

    public void FindMatchUp()
    {
        FoundGemsList.Clear(); //Listeye sürekli ekleme yapýlmamasý için en baþta temizliyoruz.


        for (int x = 0; x < board.width; x++)
        {
            for (int y = 0; y < board.height; y++)
            {
                Gem validGem = board.allGems[x, y];

                if(validGem != null)
                {

                    //x sýrasýndaki eþleþmeler kontrol edildi
                    if(x > 0 && x < board.width-1)
                    {
                        Gem leftGem = board.allGems[x - 1, y];
                        Gem rightGem = board.allGems[x + 1, y];

                        if(leftGem != null && rightGem != null)
                        {
                            if(leftGem.type == validGem.type && rightGem.type == validGem.type)
                            {
                                validGem.isMatched = true;
                                leftGem.isMatched = true;
                                rightGem.isMatched = true;

                                FoundGemsList.Add(validGem);
                                FoundGemsList.Add(leftGem);
                                FoundGemsList.Add(rightGem);
                            }
                        }
                    }
                    //y sýrasýndaki eþleþmeler kontrol edildi
                    if (y > 0 && y < board.height - 1)
                    {
                        Gem downGem = board.allGems[x , y-1];
                        Gem upGem = board.allGems[x, y+1];

                        if (downGem != null && upGem != null)
                        {
                            if (downGem.type == validGem.type && upGem.type == validGem.type)
                            {
                                validGem.isMatched = true;
                                downGem.isMatched = true;
                                upGem.isMatched = true;

                                FoundGemsList.Add(validGem);
                                FoundGemsList.Add(downGem);
                                FoundGemsList.Add(upGem);
                            }
                        }
                    }
                }
            }
        }//döngüler bitti

        if(FoundGemsList.Count > 0)
        {
            FoundGemsList = FoundGemsList.Distinct().ToList(); //Ayný mücevher listede 2 kere bulunmamasý için düzeltme yaptýk.
        }
        FindBomb();
    }

    //bombayý buluyoruz
    public void FindBomb()
    {
        for (int i = 0; i < FoundGemsList.Count; i++)
        {
            Gem gem = FoundGemsList[i];
            int x = gem.posIndex.x;
            int y = gem.posIndex.y;

            if(gem.posIndex.x > 0)
            {
                if (board.allGems[x-1,y] != null)
                {
                    if(board.allGems[x-1,y].type == Gem.GemType.bomb)
                    {
                        MarkTheBombSite(new Vector2Int(x - 1, y), board.allGems[x-1,y]);
                    }
                }
            }


            if (gem.posIndex.x < board.width-1)
            {
                if (board.allGems[x + 1, y] != null)
                {
                    if (board.allGems[x + 1, y].type == Gem.GemType.bomb)
                    {
                        MarkTheBombSite(new Vector2Int(x + 1, y), board.allGems[x + 1, y]);
                    }
                }
            }

            if (gem.posIndex.y > 0)
            {
                if (board.allGems[x, y-1] != null)
                {
                    if (board.allGems[x, y-1].type == Gem.GemType.bomb)
                    {
                        MarkTheBombSite(new Vector2Int(x , y-1), board.allGems[x, y-1]);
                    }
                }
            }

            if (gem.posIndex.y < board.height-1)
            {
                if (board.allGems[x, y + 1] != null)
                {
                    if (board.allGems[x, y + 1].type == Gem.GemType.bomb)
                    {
                        MarkTheBombSite(new Vector2Int(x, y + 1), board.allGems[x, y + 1]);
                    }
                }
            }
        }

    }

    public void MarkTheBombSite(Vector2Int bombPos,Gem bomb)
    {
        for (int x = bombPos.x-bomb.bombVolume; x <= bombPos.x+bomb.bombVolume; x++)
        {
            for (int y = bombPos.y-bomb.bombVolume; y <= bombPos.y+bomb.bombVolume; y++)
            {
                if(x>0 && x<board.width-1 && y>=0 && y < board.height - 1)
                {
                    if(board.allGems[x,y] != null)
                    {
                        board.allGems[x, y].isMatched = true;
                        FoundGemsList.Add(board.allGems[x, y]);

                    }
                }
            }
        }
        if (FoundGemsList.Count > 0)
        {
            FoundGemsList = FoundGemsList.Distinct().ToList(); //Ayný mücevher listede 2 kere bulunmamasý için düzeltme yaptýk.
        }

    }
}
