using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class Classic : MonoBehaviour
{
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

        if (PlayerPrefs.HasKey("BoardStateClassic"))
        {
            PlayerPrefs.SetString("BoardStateClassic", "");
        }

        SceneManager.LoadScene("Classic");
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
            Vector2 mousePosition = mousePositionOrig;
            mousePosition.y += .5f;
            if (draggedObject == null)
            {
                if (shape1 != null)
                {
                    if (TileSpawnPoint1.GetComponent<BoxCollider2D>().OverlapPoint(mousePositionOrig))
                    {
                        originalPosition = shape1.Parent.transform.position;
                        draggedObject = shape1;
                    }
                }

                if (shape2 != null)
                {
                    if (TileSpawnPoint2.GetComponent<BoxCollider2D>().OverlapPoint(mousePositionOrig))
                    {
                        originalPosition = shape2.Parent.transform.position;
                        draggedObject = shape2;
                    }
                }

                if (shape3 != null)
                {
                    if (TileSpawnPoint3.GetComponent<BoxCollider2D>().OverlapPoint(mousePositionOrig))
                    {
                        originalPosition = shape3.Parent.transform.position;
                        draggedObject = shape3;
                    }
                }
            }
            else if (draggedObject != null)
            {
                if (mousePosition.x > GridTopLeft.x && mousePosition.y < GridTopLeft.y &&
                    mousePosition.x < GridBottomRight.x && mousePosition.y > GridTopLeft.y)
                {
                    Vector2 tiletransform;
                    float nearestDistance = float.MaxValue;
                    for (int y = 0; y < 9; y++)
                    {
                        for (int x = 0; x < 9; x++)
                        {
                            float distance = Vector2.Distance(mousePosition, GridPoints[x, y]);
                            if (distance < nearestDistance)
                            {
                                nearestDistance = distance;
                                tiletransform = GridPoints[x, y];
                            }
                        }
                    }
                }

                draggedObject.Parent.transform.position = new Vector3(mousePosition.x, mousePosition.y, -5);
            }

            lastMousePosition = mousePosition;
        }
        else
        {
            if (draggedObject != null)
            {
                if (lastMousePosition.x > GridTopLeft.x && lastMousePosition.y < GridTopLeft.y &&
                    lastMousePosition.x < GridBottomRight.x && lastMousePosition.y > GridBottomRight.y)
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

                        draggedObject.Parent.transform.position = new Vector3(tiletransform.x, tiletransform.y, 1.0f);

                        //CurrentTiles[nearestX, nearestY] = draggedObject.Parent.transform.GetChild(0).gameObject;

                        for (int i = 0; i < Shapes.ShapesList[draggedObject.ChosenShape].Count; i++)
                        {
                            CurrentTiles[nearestX + Shapes.ShapesList[draggedObject.ChosenShape][i].Item1, nearestY - Shapes.ShapesList[draggedObject.ChosenShape][i].Item2] = draggedObject.Parent.transform.GetChild(i).gameObject;
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
                    else
                    {
                        draggedObject.Parent.transform.position = originalPosition;
                    }
                }
                else
                {
                    draggedObject.Parent.transform.position = originalPosition;
                }

                draggedObject = null;
            }
        }
    }

    ParentTile InstantiateShape(Vector3 position)
    {
        int shape = Random.Range(0, Shapes.ShapesList.Count);

        ParentTile parent = new ParentTile();
        parent.Parent = Instantiate(new GameObject(), new Vector3(position.x, position.y), Quaternion.identity);
        parent.TileCount = Shapes.ShapesList.Count;
        parent.ChosenShape = shape;


        if (parent.TileCount == 1)
        {
            Instantiate(Tile, new Vector3(position.x, position.y), Quaternion.identity, parent.Parent.transform);
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

                Instantiate(Tile, new Vector3(position.x + positionX, position.y + positionY), Quaternion.identity, parent.Parent.transform);
            }
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
        if (PlayerPrefs.HasKey("BoardStateClassic") && !string.IsNullOrEmpty(PlayerPrefs.GetString("BoardStateClassic")))
        {
            Score = PlayerPrefs.GetInt("ScoreClassic", 0);
            SaveObject save = JsonUtility.FromJson<SaveObject>(PlayerPrefs.GetString("BoardStateClassic"));

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

        PlayerPrefs.SetString("BoardStateClassic", json);
        PlayerPrefs.SetInt("ScoreClassic", Score);
    }

    IEnumerator PostRequest(string uri, int score)
    {
        PostData postData = new PostData();
        postData.Username = RandomGame.Username;
        postData.Score = score;
        postData.GameMode = 2;
        postData.UserID = RandomGame.UserID;
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
    }

    [System.Serializable]
    public class SaveObject
    {
        public bool[] currentTiles;
    }
}
