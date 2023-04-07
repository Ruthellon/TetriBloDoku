using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class TBDGame : MonoBehaviour
{
    public static string Username;
    public static string UserID;
    public static GameModes GameMode = GameModes.Classic;

    public GameObject SudokuGrid;
    public GameObject Highlight;
    public GameObject Tile;
    public GameObject TileSpawnPoint1;
    public GameObject TileSpawnPoint2;
    public GameObject TileSpawnPoint3;
    public ParticleSystem Explosion;
    public GameObject ScoreText;

    ParentTile shape1;
    ParentTile shape2;
    ParentTile shape3;

    private Vector2 defaultDimensions = new Vector2(1125, 2436);
    private Vector2 multFactor = new Vector2();

    private Vector2[,] GridPoints = new Vector2[9, 9];
    private GameObject[,] CurrentTiles = new GameObject[9, 9];

    private float tileWidth;
    private float tileHeight;

    Vector3 GridTopLeft = new Vector3();
    Vector3 GridBottomRight = new Vector3();

    private int Score = 0; 

    public void RestartLevel()
    {
        StartCoroutine(PostRequest("http://api.angryelfgames.com/AngryElf/InputHighScore", Score));

        if (PlayerPrefs.HasKey("BoardState" + GameMode.ToString()))
        {
            PlayerPrefs.SetString("BoardState" + GameMode.ToString(), "");
        }

        SceneManager.LoadScene("TBDGame");
    }

    public void HighScores()
    {
        SceneManager.LoadScene("HighScores");
    }

    // Start is called before the first frame update
    void Start()
    {
        ScoreText.GetComponent<TMPro.TextMeshProUGUI>().text = $"Score: {Score}";
        multFactor.x = Screen.width / defaultDimensions.x;
        multFactor.y = Screen.height / defaultDimensions.y;

        //Tile.transform.localScale = new Vector3(Tile.transform.localScale.x * multFactor.x, Tile.transform.localScale.y * multFactor.y, Tile.transform.localScale.z);

        Vector3 gridPosition = Camera.main.WorldToScreenPoint(SudokuGrid.transform.position);

        Vector3 screenBottomLeft = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0));
        Vector3 screenTopRight = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));

        float widthDiff = (Screen.width - (SudokuGrid.GetComponent<RectTransform>().rect.width * multFactor.x)) / 2.0f;
        GridTopLeft = Camera.main.ScreenToWorldPoint(new Vector3(widthDiff + 6.0f, (gridPosition.y + ((SudokuGrid.GetComponent<RectTransform>().rect.height * multFactor.y) / 2.0f)) - 6.0f));
        GridBottomRight = Camera.main.ScreenToWorldPoint(new Vector3((gridPosition.x + ((SudokuGrid.GetComponent<RectTransform>().rect.width * multFactor.x) / 2.0f)) - 6.0f, (gridPosition.y - ((SudokuGrid.GetComponent<RectTransform>().rect.height * multFactor.y) / 2.0f)) + 6.0f));

        float gridWidth = GridBottomRight.x - GridTopLeft.x;
        float gridHeight = GridTopLeft.y - GridBottomRight.y;

        tileWidth = gridWidth / 9.0f;
        tileHeight = gridHeight / 9.0f;

        float positionY = GridTopLeft.y - (tileHeight / 2.0f);
        for (int y = 0; y < 9; y++)
        {
            float positionX = GridTopLeft.x + (tileWidth / 2.0f);
            for (int x = 0; x < 9; x++)
            {
                GridPoints[x, y] = new Vector2(positionX, positionY);
                positionX += tileWidth;
            }
            positionY -= tileHeight;
        }

        shape1 = InstantiateShape(TileSpawnPoint1.transform.position);
        shape2 = InstantiateShape(TileSpawnPoint2.transform.position);
        shape3 = InstantiateShape(TileSpawnPoint3.transform.position);

        GetBoardState();
    }

    ParentTile draggedObject;
    GameObject highlightedTile;
    Vector3 originalPosition;
    Vector2 lastMousePosition;
    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount > 0)
        {
            Vector2 mousePositionOrig = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);

            if (draggedObject == null)
            {
                if (TileSpawnPoint1.GetComponent<BoxCollider2D>().OverlapPoint(mousePositionOrig))
                {
                    if (shape1 != null)
                    {
                        originalPosition = shape1.Parent.transform.position;
                        draggedObject = shape1;
                    }
                }
                else if (TileSpawnPoint2.GetComponent<BoxCollider2D>().OverlapPoint(mousePositionOrig))
                {
                    if (shape2 != null)
                    {
                        originalPosition = shape2.Parent.transform.position;
                        draggedObject = shape2;
                    }
                }
                else if (TileSpawnPoint3.GetComponent<BoxCollider2D>().OverlapPoint(mousePositionOrig))
                {
                    if (shape3 != null)
                    {
                        originalPosition = shape3.Parent.transform.position;
                        draggedObject = shape3;
                    }
                }

                if (draggedObject != null)
                {
                    draggedObject.Parent.transform.localScale *= 2.0f;
                }
            }
            
            if (draggedObject != null)
            {
                Vector2 mousePosition = mousePositionOrig;
                mousePosition.y += .5f;

                draggedObject.Parent.transform.position = new Vector3(mousePosition.x + draggedObject.RelativeX, mousePosition.y, -5);

                lastMousePosition = mousePosition;
            }
        }
        else
        {
            if (draggedObject != null)
            {
                bool allChildrenOnGrid = true;
                bool allChildrenCoveringBlank = true;

                for (int i = 0; i < draggedObject.Parent.transform.childCount; i++)
                {
                    if (draggedObject.Parent.transform.GetChild(i).position.x > GridTopLeft.x && draggedObject.Parent.transform.GetChild(i).position.y < GridTopLeft.y &&
                        draggedObject.Parent.transform.GetChild(i).position.x < GridBottomRight.x && draggedObject.Parent.transform.GetChild(i).position.y > GridBottomRight.y)
                        allChildrenOnGrid &= true;
                    else
                        allChildrenOnGrid &= false;

                    foreach (var space in CurrentTiles)
                    {
                        if (space != null && space.GetComponent<BoxCollider2D>().IsTouching(draggedObject.Parent.transform.GetChild(i).GetComponent<BoxCollider2D>()))
                            allChildrenCoveringBlank = false;
                    }
                }

                if (allChildrenOnGrid && allChildrenCoveringBlank)
                {
                    Vector2 tiletransform = new Vector2();
                    float nearestDistance = float.MaxValue;
                    int nearestX = 0, nearestY = 0;
                    for (int y = 0; y < 9; y++)
                    {
                        for (int x = 0; x < 9; x++)
                        {
                            float distance = Vector2.Distance(lastMousePosition, GridPoints[x, y]);
                            if (distance < nearestDistance)
                            {
                                nearestX = x;
                                nearestY = y;
                                nearestDistance = distance;
                                tiletransform = GridPoints[x, y];
                            }
                        }
                    }

                    if (GameMode == GameModes.Random)
                    {
                        for (int i = 0; i < draggedObject.ChosenChildren.Count; i++)
                        {
                            float nearestSpace = float.MaxValue;
                            int nearX = 0, nearY = 0;
                            for (int y = 0; y < 9; y++)
                            {
                                for (int x = 0; x < 9; x++)
                                {
                                    float distance = Vector2.Distance(draggedObject.Parent.transform.GetChild(i).position, GridPoints[x, y]);
                                    if (distance < nearestSpace)
                                    {
                                        nearX = x;
                                        nearY = y;
                                        nearestSpace = distance;
                                    }
                                }
                            }
                            draggedObject.Parent.transform.GetChild(i).transform.position = GridPoints[nearX, nearY];
                            CurrentTiles[nearX, nearY] = draggedObject.Parent.transform.GetChild(i).gameObject;
                        }
                    }
                    else if (GameMode == GameModes.Classic)
                    {
                        draggedObject.Parent.transform.position = new Vector3(tiletransform.x, tiletransform.y, 1.0f);
                        for (int i = 0; i < Shapes.ShapesList[draggedObject.ChosenShape].Count; i++)
                        {
                            CurrentTiles[nearestX + Shapes.ShapesList[draggedObject.ChosenShape][i].Item1, nearestY + Shapes.ShapesList[draggedObject.ChosenShape][i].Item2] = draggedObject.Parent.transform.GetChild(i).gameObject;
                        }
                    }

                    draggedObject.Parent.transform.DetachChildren();

                    if (draggedObject == shape1)
                    {
                        Destroy(draggedObject.Parent);
                        shape1 = null;
                    }
                    else if (draggedObject == shape2)
                    {
                        Destroy(draggedObject.Parent);
                        shape2 = null;
                    }
                    else if (draggedObject == shape3)
                    {
                        Destroy(draggedObject.Parent);
                        shape3 = null;
                    }

                    if (shape1 == null && shape2 == null && shape3 == null)
                    {
                        shape1 = InstantiateShape(TileSpawnPoint1.transform.position);
                        shape2 = InstantiateShape(TileSpawnPoint2.transform.position);
                        shape3 = InstantiateShape(TileSpawnPoint3.transform.position);
                    }

                    CleanupBoard();
                    SaveBoardState();
                }

                draggedObject.Parent.transform.position = originalPosition;
                draggedObject.Parent.transform.localScale /= 2.0f;
                draggedObject = null;
            }
        }
    }

    ParentTile InstantiateShape(Vector3 position)
    {
        if (GameMode == GameModes.Random)
        {
            int tileCount = Random.Range(1, 101);

            if (tileCount <= 5)
                tileCount = 1;
            else if (tileCount <= 20)
                tileCount = 2;
            else if (tileCount <= 40)
                tileCount = 3;
            else if (tileCount <= 60)
                tileCount = 4;
            else if (tileCount <= 80)
                tileCount = 5;
            else if (tileCount <= 90)
                tileCount = 6;
            else if (tileCount <= 96)
                tileCount = 7;
            else if (tileCount <= 99)
                tileCount = 8;
            else if (tileCount <= 100)
                tileCount = 9;


            ParentTile parent = new ParentTile();
            parent.Parent = Instantiate(new GameObject(), new Vector3(position.x, position.y), Quaternion.identity);
            parent.TileCount = tileCount;

            if (tileCount == 1)
            {
                Instantiate(Tile, new Vector3(position.x, position.y), Quaternion.identity, parent.Parent.transform);
                parent.ChosenChildren = new List<int>();
                parent.ChosenChildren.Add(4);

                parent.Parent.transform.localScale /= 2.0f; 
                return parent;
            }
            else
            {
                List<int> chosenChildren = new List<int>();
                for (int i = 0; i < tileCount; i++)
                {
                    int child = Random.Range(0, 16);

                    while (chosenChildren.Contains(child))
                    {
                        child = Random.Range(0, 16);
                    }

                    if (child == 0)
                        Instantiate(Tile, new Vector3(position.x - tileWidth, position.y + tileHeight), Quaternion.identity, parent.Parent.transform); // Bottom Left
                    else if (child == 1)
                        Instantiate(Tile, new Vector3(position.x, position.y + tileHeight), Quaternion.identity, parent.Parent.transform); // Bottom 1
                    else if (child == 2)
                        Instantiate(Tile, new Vector3(position.x + tileWidth, position.y + tileHeight), Quaternion.identity, parent.Parent.transform); // Bottom Right
                    else if (child == 3)
                        Instantiate(Tile, new Vector3(position.x - tileWidth, position.y), Quaternion.identity, parent.Parent.transform); // Left 1
                    else if (child == 4)
                        Instantiate(Tile, new Vector3(position.x, position.y), Quaternion.identity, parent.Parent.transform); // center
                    else if (child == 5)
                        Instantiate(Tile, new Vector3(position.x + tileWidth, position.y), Quaternion.identity, parent.Parent.transform); // Right 1
                    else if (child == 6)
                        Instantiate(Tile, new Vector3(position.x - tileWidth, position.y - tileHeight), Quaternion.identity, parent.Parent.transform);  // Top Left
                    else if (child == 7)
                        Instantiate(Tile, new Vector3(position.x, position.y - tileHeight), Quaternion.identity, parent.Parent.transform); // Up 1
                    else if (child == 8)
                        Instantiate(Tile, new Vector3(position.x + tileWidth, position.y - tileHeight), Quaternion.identity, parent.Parent.transform); // Top Right
                    else if (child == 9)
                        Instantiate(Tile, new Vector3(position.x, position.y + (tileHeight * 2)), Quaternion.identity, parent.Parent.transform); // Bottom 2
                    else if (child == 10)
                        Instantiate(Tile, new Vector3(position.x + tileWidth, position.y + (tileHeight * 2)), Quaternion.identity, parent.Parent.transform); // Bottom 2 Right 1
                    else if (child == 11)
                        Instantiate(Tile, new Vector3(position.x + (tileWidth * 2), position.y + (tileHeight * 2)), Quaternion.identity, parent.Parent.transform); // Bottom Right Corner
                    else if (child == 12)
                        Instantiate(Tile, new Vector3(position.x + (tileWidth * 2), position.y + tileHeight), Quaternion.identity, parent.Parent.transform); // Right 2 down 1
                    else if (child == 13)
                        Instantiate(Tile, new Vector3(position.x + (tileWidth * 2), position.y), Quaternion.identity, parent.Parent.transform); // Right 2
                    else if (child == 14)
                        Instantiate(Tile, new Vector3(position.x + (tileWidth * 2), position.y - tileHeight), Quaternion.identity, parent.Parent.transform);  // Right 2 up 1
                    else if (child == 15)
                        Instantiate(Tile, new Vector3(position.x - tileWidth, position.y + (tileHeight * 2)), Quaternion.identity, parent.Parent.transform); // Left 1 down 2


                    chosenChildren.Add(child);
                }

                float sumX = 0;
                for (int i = 0; i < chosenChildren.Count; i++)
                {
                    sumX += parent.Parent.transform.GetChild(i).position.x;
                }
                float centralX = sumX / chosenChildren.Count;

                parent.RelativeX = parent.Parent.transform.position.x - centralX;
                parent.Parent.transform.localScale /= 2.0f;
                parent.ChosenChildren = chosenChildren;
                return parent;
            }
        }
        else if (GameMode == GameModes.Classic)
        {
            int shape = Random.Range(0, Shapes.ShapesList.Count);

            ParentTile parent = new ParentTile();
            parent.Parent = Instantiate(new GameObject(), new Vector3(position.x, position.y), Quaternion.identity);
            parent.TileCount = Shapes.ShapesList.Count;
            parent.ChosenShape = shape;


            if (parent.TileCount == 1)
            {
                Instantiate(Tile, new Vector3(position.x, position.y), Quaternion.identity, parent.Parent.transform);
                parent.Parent.transform.localScale /= 2.0f;
                return parent;
            }
            else
            {
                List<int> chosenChildren = new List<int>();

                for (int i = 0; i < Shapes.ShapesList[shape].Count; i++)
                {
                    float positionX = 0;
                    float positionY = 0;

                    if (Shapes.ShapesList[shape][i].Item1 > 0)
                        positionX = tileWidth;
                    else if (Shapes.ShapesList[shape][i].Item1 < 0)
                        positionX = -tileWidth;

                    if (Shapes.ShapesList[shape][i].Item2 > 0)
                        positionY = tileHeight;
                    else if (Shapes.ShapesList[shape][i].Item2 < 0)
                        positionY = -tileHeight;

                    Instantiate(Tile, new Vector3(position.x + positionX, position.y - positionY), Quaternion.identity, parent.Parent.transform);
                }
                parent.Parent.transform.localScale /= 2.0f;
                return parent;
            }
        }
        else
        {
            ParentTile parent = new ParentTile();
            parent.Parent = Instantiate(new GameObject(), new Vector3(position.x, position.y), Quaternion.identity);
            parent.TileCount = 0;
            parent.Parent.transform.localScale /= 2.0f;
            return parent;
        }
    }

    void CleanupBoard()
    {
        List<GameObject> listDestroyed = new List<GameObject>();
        int multFactor = 0;
        for (int y = 0; y < 9; y++)
        {
            int rowCount = 0;
            for (int x = 0; x < 9; x++)
            {
                if (CurrentTiles[x,y] != null)
                {
                    rowCount++;
                }
                else
                {
                    break;
                }
            }

            if (rowCount == 9)
            {
                multFactor++;
                for (int x = 0; x < 9; x++)
                {
                    listDestroyed.Add(CurrentTiles[x, y]);
                }
            }
        }

        for (int x = 0; x < 9; x++)
        {
            int columnCount = 0;
            for (int y = 0; y < 9; y++)
            {
                if (CurrentTiles[x, y] != null)
                {
                    columnCount++;
                }
                else
                {
                    break;
                }
            }

            if (columnCount == 9)
            {
                multFactor++;
                for (int y = 0; y < 9; y++)
                {
                    if (!listDestroyed.Contains(CurrentTiles[x, y]))
                        listDestroyed.Add(CurrentTiles[x, y]);
                }
            }
        }

        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                int squareCount = 0;
                for (int y = i * 3; y < (i * 3) + 3; y++)
                {
                    for (int x = j * 3; x < (j * 3) + 3; x++)
                    {
                        if (CurrentTiles[x, y] != null)
                            squareCount++;
                        else
                            break;
                    }
                }

                if (squareCount == 9)
                {
                    multFactor++;
                    for (int y = i * 3; y < (i * 3) + 3; y++)
                    {
                        for (int x = j * 3; x < (j * 3) + 3; x++)
                        {
                            if (!listDestroyed.Contains(CurrentTiles[x, y]))
                                listDestroyed.Add(CurrentTiles[x, y]);
                        }
                    }
                }
            }
        }

        if (listDestroyed.Any())
        {
            Explosion.Play();

            Score += (listDestroyed.Count * multFactor);
            ScoreText.GetComponent<TMPro.TextMeshProUGUI>().text = $"Score: {Score}";
        }

        foreach (var go in listDestroyed)
        {
            Destroy(go);
        }
    }

    void GetBoardState()
    {
        if (PlayerPrefs.HasKey("BoardState" + GameMode.ToString()) && !string.IsNullOrEmpty(PlayerPrefs.GetString("BoardState" + GameMode.ToString())))
        {
            Score = PlayerPrefs.GetInt("Score" + GameMode.ToString(), 0);
            SaveObject save = JsonUtility.FromJson<SaveObject>(PlayerPrefs.GetString("BoardState" + GameMode.ToString()));

            for (int i = 0; i < save.currentTiles.Length; i++)
            {
                if (save.currentTiles[i])
                {
                    int x = i % 9;
                    int y = i / 9;
                    CurrentTiles[x, y] = Instantiate(Tile, GridPoints[x, y], Quaternion.identity);
                }
            }
        }
    }

    void SaveBoardState()
    {
        SaveObject save = new SaveObject();
        save.currentTiles = new bool[81];

        for (int y = 0; y < 9; y++)
        {
            for (int x = 0; x < 9; x++)
            {
                if (CurrentTiles[x, y] != null)
                    save.currentTiles[x + (y * 9)] = true;
                else
                    save.currentTiles[x + (y * 9)] = false;
            }
        }

        string json = JsonUtility.ToJson(save);

        PlayerPrefs.SetString("BoardState" + GameMode.ToString(), json);
        PlayerPrefs.SetInt("Score" + GameMode.ToString(), Score);
    }

    IEnumerator PostRequest(string uri, int score)
    {
        PostData postData = new PostData();
        postData.Username = Username;
        postData.Score = score;
        postData.GameMode = (int)GameMode;
        postData.UserID = UserID;
        postData.Secret = "06ec43b7-923f-4deb-b8b0-6f0c1b85cee7";

        string json = JsonUtility.ToJson(postData);

        using (UnityWebRequest webRequest = UnityWebRequest.Put(uri, json))
        {
            webRequest.method = "POST";
            webRequest.SetRequestHeader("Content-Type", "application/json");

            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();
        }
    }

    [System.Serializable]
    public class PostData
    {
        public string Username;
        public int Score;
        public int GameMode;
        public string UserID;
        public string Secret;
    }

    class ParentTile
    {
        public GameObject Parent { get; set; }
        public int TileCount { get; set; }
        public int ChosenShape { get; set; }
        public float RelativeX { get; set; } = 0;
        public List<int> ChosenChildren { get; set; }
    }

    [System.Serializable]
    public class SaveObject
    {
        public bool[] currentTiles;
    }
}
