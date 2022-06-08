using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MatchUpController : MonoBehaviour
{
    Board board;

    public List<Gem> FoundGemsList = new List<Gem>(); // e�le�en m�cevherler i�in liste olu�turuyoruz.

    private void Awake()
    {
        board = Object.FindObjectOfType<Board>();

    }

    public void FindMatchUp()
    {
        FoundGemsList.Clear(); //Listeye s�rekli ekleme yap�lmamas� i�in en ba�ta temizliyoruz.


        for (int x = 0; x < board.width; x++)
        {
            for (int y = 0; y < board.height; y++)
            {
                Gem validGem = board.allGems[x, y];

                if(validGem != null)
                {

                    //x s�ras�ndaki e�le�meler kontrol edildi
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
                    //y s�ras�ndaki e�le�meler kontrol edildi
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
        }//d�ng�ler bitti

        if(FoundGemsList.Count > 0)
        {
            FoundGemsList = FoundGemsList.Distinct().ToList(); //Ayn� m�cevher listede 2 kere bulunmamas� i�in d�zeltme yapt�k.
        }

    }
}
