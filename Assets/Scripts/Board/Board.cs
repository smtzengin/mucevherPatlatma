using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Board : MonoBehaviour
{
    public int width, height;

    public GameObject tilePrefab;

    public Gem[] gems;

    public Gem[,] allGems;

    public float gemSpeed;

    public MatchUpController matchUpController;

    public enum BoardState
    {
        waiting,
        moving
    };
    public BoardState validState = BoardState.moving;

    public Gem bomb;
    public float chanceOfBomb = 2f;

    private void Start()
    {
        allGems = new Gem[width, height];

        EditFN();
    }

    private void Awake()
    {
        matchUpController = Object.FindObjectOfType<MatchUpController>();
    }


    void EditFN()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector2 pos = new Vector2(x, y);

                GameObject bgTile = Instantiate(tilePrefab, pos, Quaternion.identity);

                bgTile.transform.parent = this.transform;

                bgTile.name = "BG Tile - " + x + " , " + y;

                int randomGem = Random.Range(0,gems.Length);

                int counter = 0;
                while (isThereAMatch(new Vector2Int(x, y), gems[randomGem]) && counter <100)
                {
                    randomGem = Random.Range(0, gems.Length);
                    counter++;
                    if (counter > 0)
                    {
                        print(counter);
                    }
                }

                CreateGem(new Vector2Int(x,y), gems[randomGem]);
            }
        }
    }

    void CreateGem(Vector2Int pos,Gem newGems)
    {
        if(Random.Range(0f,100f)< chanceOfBomb)
        {
            newGems = bomb;
        }
        Gem gem = Instantiate(newGems, new Vector3(pos.x, pos.y+height, 0f), Quaternion.identity);
        gem.transform.parent = this.transform;
        gem.name = "Mucevher -" + pos.x + ", " + pos.y;

        allGems[pos.x, pos.y] = gem;

        gem.EditGem(pos, this);
    }

    bool isThereAMatch(Vector2Int posControl,Gem controlGem)
    {
        if(posControl.x>1)
        {
            if(allGems[posControl.x-1, posControl.y].type == controlGem.type 
            && allGems[posControl.x-2,posControl.y].type == controlGem.type)
            {
                return true;
            }
        }
        if (posControl.y > 1)
        {
            if (allGems[posControl.x , posControl.y - 1].type == controlGem.type
            && allGems[posControl.x, posControl.y - 2].type == controlGem.type)
            {
                return true;
            }
        }

        return false;
    }

    void DestroyMatchedGems(Vector2Int pos)
    {
        if (allGems[pos.x,pos.y] != null)
        {
            if (allGems[pos.x, pos.y].isMatched)
            {
                if (allGems[pos.x,pos.y].type == Gem.GemType.bomb)
                {
                    SoundManager.instance.MakeExplosionSound();
                }
                else
                {
                    SoundManager.instance.MakeGemSound();
                }

                Instantiate(allGems[pos.x, pos.y].gemEffect, new Vector2(pos.x, pos.y),Quaternion.identity);
                Destroy(allGems[pos.x, pos.y].gameObject);
                allGems[pos.x, pos.y] = null;
            }
        }
    }

    public void AllDestroyMatchedGems()
    {
        for (int i = 0; i < matchUpController.FoundGemsList.Count; i++)
        {
            if(matchUpController.FoundGemsList[i] != null)
            {
                UIManager.instance.IncreaseScore(matchUpController.FoundGemsList[i].scoreValue);

                DestroyMatchedGems(matchUpController.FoundGemsList[i].posIndex);
            }
   
        }
        StartCoroutine(ScrollToBottom());
    }

    IEnumerator ScrollToBottom()
    {
        yield return new WaitForSeconds(0.2f);

        int emptyCounter = 0;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (allGems[x,y] == null)
                {
                    emptyCounter++;
                }
                else if(emptyCounter>0){
                    allGems[x, y].posIndex.y -= emptyCounter;
                    allGems[x, y - emptyCounter] = allGems[x, y];
                    allGems[x, y] = null;
                }
            }

            emptyCounter = 0;
        }
        StartCoroutine(FillTheBoard());
    }

    IEnumerator FillTheBoard()
    {
        yield return new WaitForSeconds(.5f);

        FillTheBoardFNC();

        yield return new WaitForSeconds(.5f);

        matchUpController.FindMatchUp();

        if (matchUpController.FoundGemsList.Count > 0)
        {
            yield return new WaitForSeconds(1.5f);
            AllDestroyMatchedGems();
        }
        else
        {
            yield return new WaitForSeconds(.5f);
            validState = BoardState.moving;
        }
    }

    void FillTheBoardFNC()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (allGems[x, y] == null)
                {
                    int randomGem = Random.Range(0, gems.Length);

                    CreateGem(new Vector2Int(x, y), gems[randomGem]);
                }

            }
            CheckForMisplacements();
        }
    }

    void CheckForMisplacements()
    {
        List<Gem> FoundGemList = new List<Gem>();

        FoundGemList.AddRange(FindObjectsOfType<Gem>());

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (FoundGemList.Contains(allGems[x,y]))
                {
                    FoundGemList.Remove(allGems[x,y]);
                }

            }

        }

        foreach (Gem gem in FoundGemList)  
        {
            Destroy(gem.gameObject);
        }
    }

    public void MixTheBoard()
    {
        if(validState != BoardState.waiting)
        {
            validState = BoardState.waiting;

            List<Gem> gemsOnTheSceneList = new List<Gem>();

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    //sahnedeki mücevherleri bir listeye döküp tüm mücevherleri tutan listeyi null'a çektik
                    gemsOnTheSceneList.Add(allGems[x, y]);
                    allGems[x, y] = null;
                }
            }

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    //burada sahnedeki tüm mücevherlerin tutulduðu listeyi karýþtýrýp tekrardan sahneye süreceðiz.
                    int gemsToUse = Random.Range(0, gemsOnTheSceneList.Count);

                    int counter = 0;

                    while(isThereAMatch(new Vector2Int(x, y), gemsOnTheSceneList[gemsToUse]) && counter <100 && gemsOnTheSceneList.Count>1)
                    {
                        gemsToUse = Random.Range(0, gemsOnTheSceneList.Count);
                        counter++;
                    }


                    gemsOnTheSceneList[gemsToUse].EditGem(new Vector2Int(x, y), this);
                    allGems[x, y] = gemsOnTheSceneList[gemsToUse];
                    gemsOnTheSceneList.RemoveAt(gemsToUse);

                    StartCoroutine(ScrollToBottom());
                }
            }
        }

    }

 }

