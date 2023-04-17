using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TBDGame : MonoBehaviour
{
    public static string Username;
    public static string UserID;
    public static GameModes GameMode = GameModes.Classic;

    public GameObject Canvas;
    public GameObject Background;
    public GameObject SudokuGrid;
    public GameObject Highlight;
    public GameObject Tile;
    public GameObject Tile2;
    public GameObject TileSpawnPoint1;
    public GameObject TileSpawnPoint2;
    public GameObject TileSpawnPoint3;
    public ParticleSystem Explosion;
    public GameObject ScoreText;
    public GameObject MultiplierText;
    public GameObject GridTopLeft;
    public GameObject GridBottomRight;

    public Sprite Background1;
    public Sprite Background2;
    public Sprite Background3;

    ParentTile shape1;
    ParentTile shape2;
    ParentTile shape3;

    private Vector2[,] GridPoints = new Vector2[9, 9];
    private GameObject[,] CurrentTiles = new GameObject[9, 9];

    private float tileWidth;
    private float tileHeight;

    private int Score = 0; 

    public void RestartLevel()
    {
        Highscores.HighScoreList list = new Highscores.HighScoreList();
        if (PlayerPrefs.HasKey("Highscores" + GameMode.ToString()))
        {
            list = JsonUtility.FromJson<Highscores.HighScoreList>(PlayerPrefs.GetString("Highscores" + GameMode.ToString()));
        }

        if (list.highscores == null)
            list.highscores = new Highscores.HighScoreResponse[15];

        int lowestScoreIndex = 0;
        int lowestScore = -1;
        for (int i = 0; i < list.highscores.Length; i++)
        {
            if (list.highscores[i] == null)
                list.highscores[i] = new Highscores.HighScoreResponse();

            if (Score > list.highscores[i].score)
            {
                if (list.highscores[i].score > lowestScore)
                {
                    lowestScoreIndex = i;
                    lowestScore = list.highscores[i].score;
                }
            }
        }

        list.highscores[lowestScoreIndex] = new Highscores.HighScoreResponse()
        {
            username = Username,
            score = Score,
            dateAchieved = DateTime.Now,
            gameMode = (int)GameMode
        };

        PlayerPrefs.SetString("Highscores" + GameMode.ToString(), JsonUtility.ToJson(list));

        if (PlayerPrefs.HasKey("BoardState" + GameMode.ToString()))
        {
            PlayerPrefs.SetString("BoardState" + GameMode.ToString(), "");
            PlayerPrefs.DeleteKey("Shape1" + GameMode.ToString());
            PlayerPrefs.DeleteKey("Shape2" + GameMode.ToString());
            PlayerPrefs.DeleteKey("Shape3" + GameMode.ToString());
        }

        StartCoroutine(PostRequest("http://api.angryelfgames.com/AngryElf/InputHighScore", Score));

        SceneManager.LoadScene("TBDGame");
    }

    public void HighScores()
    {
        SceneManager.LoadScene("HighScores");
    }

    public void Home()
    {
        SceneManager.LoadScene("StartScreen");
    }

    // Start is called before the first frame update
    void Start()
    {
        if (GameMode == GameModes.Classic)
            Background.GetComponent<UnityEngine.UI.Image>().sprite = Background1;
        else if (GameMode == GameModes.Random)
            Background.GetComponent<UnityEngine.UI.Image>().sprite = Background2;
        else if (GameMode == GameModes.Twist)
            Background.GetComponent<UnityEngine.UI.Image>().sprite = Background3;


        float gridWidth = GridBottomRight.transform.position.x - GridTopLeft.transform.position.x;
        float gridHeight = GridTopLeft.transform.position.y - GridBottomRight.transform.position.y;

        tileWidth = gridWidth / 9.0f;
        tileHeight = gridHeight / 9.0f;

        float positionY = GridTopLeft.transform.position.y - (tileHeight / 2.0f);
        for (int y = 0; y < 9; y++)
        {
            float positionX = GridTopLeft.transform.position.x + (tileWidth / 2.0f);
            for (int x = 0; x < 9; x++)
            {
                GridPoints[x, y] = new Vector2(positionX, positionY);
                positionX += tileWidth;
            }
            positionY -= tileHeight;
        }
        
        if (PlayerPrefs.HasKey("Shape1" + GameMode.ToString()))
        {
            string savedShape = PlayerPrefs.GetString("Shape1" + GameMode.ToString());

            if (!string.IsNullOrEmpty(savedShape))
            {
                shape1 = new ParentTile();
                shape1.Parent = Instantiate(new GameObject(), new Vector3(TileSpawnPoint1.transform.position.x, TileSpawnPoint1.transform.position.y), Quaternion.identity);

                if (GameMode == GameModes.Classic)
                {
                    if (PlayerPrefs.HasKey("Shape1Quadrant" + GameMode.ToString()))
                        shape1.ShapeQuadrant = PlayerPrefs.GetInt("Shape1Quadrant" + GameMode.ToString());
                    shape1.ChosenShape = Convert.ToInt32(savedShape);
                    shape1.CreateChildrenFromShape(Tile, TileSpawnPoint1.transform.position, tileWidth, tileHeight);
                }
                else if (GameMode == GameModes.Twist)
                {
                    shape1.ChosenShape = Convert.ToInt32(savedShape);
                    shape1.CreateChildrenFromShape(Tile, TileSpawnPoint1.transform.position, tileWidth, tileHeight);
                }
                else if (GameMode == GameModes.Random)
                {
                    shape1.ChosenChildren = savedShape.Split(',').Select(x => Convert.ToInt32(x)).ToList();
                    shape1.CreateChildrenFromList(Tile2, TileSpawnPoint1.transform.position, tileWidth, tileHeight);
                }

                shape1.Parent.transform.localScale /= 2.0f;
            }
            else
            {
                shape1 = null;
            }
        }
        else
        {
            shape1 = InstantiateShape(TileSpawnPoint1.transform.position);
        }

        if (PlayerPrefs.HasKey("Shape2" + GameMode.ToString()))
        {
            string savedShape = PlayerPrefs.GetString("Shape2" + GameMode.ToString());

            if (!string.IsNullOrEmpty(savedShape))
            {
                shape2 = new ParentTile();
                shape2.Parent = Instantiate(new GameObject(), new Vector3(TileSpawnPoint2.transform.position.x, TileSpawnPoint2.transform.position.y), Quaternion.identity);

                if (GameMode == GameModes.Classic)
                {
                    if (PlayerPrefs.HasKey("Shape2Quadrant" + GameMode.ToString()))
                        shape2.ShapeQuadrant = PlayerPrefs.GetInt("Shape2Quadrant" + GameMode.ToString());
                    shape2.ChosenShape = Convert.ToInt32(savedShape);
                    shape2.CreateChildrenFromShape(Tile, TileSpawnPoint2.transform.position, tileWidth, tileHeight);
                }
                else if (GameMode == GameModes.Twist)
                {
                    shape2.ChosenShape = Convert.ToInt32(savedShape);
                    shape2.CreateChildrenFromShape(Tile, TileSpawnPoint2.transform.position, tileWidth, tileHeight);
                }
                else if (GameMode == GameModes.Random)
                {
                    shape2.ChosenChildren = savedShape.Split(',').Select(x => Convert.ToInt32(x)).ToList();
                    shape2.CreateChildrenFromList(Tile2, TileSpawnPoint2.transform.position, tileWidth, tileHeight);
                }

                shape2.Parent.transform.localScale /= 2.0f;
            }
            else
            {
                shape2 = null;
            }
        }
        else
        {
            shape2 = InstantiateShape(TileSpawnPoint2.transform.position);
        }

        if (PlayerPrefs.HasKey("Shape3" + GameMode.ToString()))
        {
            string savedShape = PlayerPrefs.GetString("Shape3" + GameMode.ToString());

            if (!string.IsNullOrEmpty(savedShape))
            {
                shape3 = new ParentTile();
                shape3.Parent = Instantiate(new GameObject(), new Vector3(TileSpawnPoint3.transform.position.x, TileSpawnPoint3.transform.position.y), Quaternion.identity);

                if (GameMode == GameModes.Classic)
                {
                    if (PlayerPrefs.HasKey("Shape3Quadrant" + GameMode.ToString()))
                        shape3.ShapeQuadrant = PlayerPrefs.GetInt("Shape3Quadrant" + GameMode.ToString());
                    shape3.ChosenShape = Convert.ToInt32(savedShape);
                    shape3.CreateChildrenFromShape(Tile, TileSpawnPoint3.transform.position, tileWidth, tileHeight);
                }
                else if (GameMode == GameModes.Twist)
                {
                    shape3.ChosenShape = Convert.ToInt32(savedShape);
                    shape3.CreateChildrenFromShape(Tile, TileSpawnPoint3.transform.position, tileWidth, tileHeight);
                }
                else if (GameMode == GameModes.Random)
                {
                    shape3.ChosenChildren = savedShape.Split(',').Select(x => Convert.ToInt32(x)).ToList();
                    shape3.CreateChildrenFromList(Tile2, TileSpawnPoint3.transform.position, tileWidth, tileHeight);
                }

                shape3.Parent.transform.localScale /= 2.0f;
            }
            else
            {
                shape3 = null;
            }
        }
        else
        {
            shape3 = InstantiateShape(TileSpawnPoint3.transform.position);
        }

        GetBoardState();

        ScoreText.GetComponent<TMPro.TextMeshProUGUI>().text = $"Score: {Score}";
        MultiplierText.GetComponent<TMPro.TextMeshProUGUI>().text = $"x{multiplier+1}";
    }

    int multiplier = 0;
    float timeForMultiplier = 0f;
    ParentTile draggedObject;
    GameObject highlightedTile;
    Vector3 originalPosition;
    Vector2 mouseStartPosition;
    Vector2 lastMousePosition;
    bool rotateShape = false;
    // Update is called once per frame
    void Update()
    {
        if (timeForMultiplier > 0)
            timeForMultiplier -= Time.deltaTime;

        if (multiplier > 0 && timeForMultiplier <= 0f)
        {
            multiplier = 0;
            MultiplierText.GetComponent<TMPro.TextMeshProUGUI>().text = $"x{multiplier + 1}";
        }

        if (Input.touchCount > 0)
        {
            Vector3 mousePositionOrig = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);

            if (draggedObject == null)
            {
                if (TileSpawnPoint1.GetComponent<BoxCollider2D>().OverlapPoint(mousePositionOrig))
                {
                    if (shape1 != null)
                    {
                        originalPosition = TileSpawnPoint1.transform.position;
                        draggedObject = shape1;
                    }
                }
                else if (TileSpawnPoint2.GetComponent<BoxCollider2D>().OverlapPoint(mousePositionOrig))
                {
                    if (shape2 != null)
                    {
                        originalPosition = TileSpawnPoint2.transform.position;
                        draggedObject = shape2;
                    }
                }
                else if (TileSpawnPoint3.GetComponent<BoxCollider2D>().OverlapPoint(mousePositionOrig))
                {
                    if (shape3 != null)
                    {
                        originalPosition = TileSpawnPoint3.transform.position;
                        draggedObject = shape3;
                    }
                }

                if (draggedObject != null)
                {
                    mouseStartPosition = mousePositionOrig;
                    draggedObject.Parent.transform.localScale *= 2.0f;
                }
            }
            
            if (draggedObject != null)
            {
                if (Input.GetTouch(0).phase == TouchPhase.Ended)
                {
                    if ((Vector2)mousePositionOrig == (Vector2)mouseStartPosition)
                    {
                        rotateShape = true;
                    }
                }

                Vector2 mousePosition = mousePositionOrig;
                mousePosition.y += (draggedObject.RelativeY + .5f);
                mousePosition.x += draggedObject.RelativeX;

                draggedObject.Parent.transform.position = new Vector3(mousePosition.x, mousePosition.y, -5);

                lastMousePosition = mousePosition;
            }
        }
        else
        {
            if (draggedObject != null)
            {
                if (!rotateShape)
                {
                    bool allChildrenOnGrid = true;
                    bool allChildrenCoveringBlank = true;
                    for (int i = 0; i < draggedObject.Parent.transform.childCount; i++)
                    {
                        if (draggedObject.Parent.transform.GetChild(i).position.x > GridTopLeft.transform.position.x && draggedObject.Parent.transform.GetChild(i).position.y < GridTopLeft.transform.position.y &&
                            draggedObject.Parent.transform.GetChild(i).position.x < GridBottomRight.transform.position.x && draggedObject.Parent.transform.GetChild(i).position.y > GridBottomRight.transform.position.y)
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

                        for (int i = 0; i < draggedObject.Parent.transform.childCount; i++)
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
                        draggedObject.Parent.transform.localScale /= 2.0f;
                    }
                }
                else
                {
                    draggedObject.Parent.transform.position = originalPosition;
                    RotateShape();
                    draggedObject.Parent.transform.localScale /= 2.0f;
                    rotateShape = false;
                }

                draggedObject = null;
            }
        }
    }

    ParentTile InstantiateShape(Vector3 position)
    {
        if (GameMode == GameModes.Random)
        {
            int tileCount = UnityEngine.Random.Range(1, 101);

            if (tileCount <= 10)
                tileCount = 1;
            else if (tileCount <= 30)
                tileCount = 2;
            else if (tileCount <= 60)
                tileCount = 3;
            else if (tileCount <= 75)
                tileCount = 4;
            else if (tileCount <= 85)
                tileCount = 5;
            else if (tileCount <= 93)
                tileCount = 6;
            else if (tileCount <= 97)
                tileCount = 7;
            else if (tileCount <= 99)
                tileCount = 8;
            else if (tileCount <= 100)
                tileCount = 9;

            ParentTile parent = new ParentTile();
            parent.Parent = Instantiate(new GameObject(), new Vector3(position.x, position.y), Quaternion.identity);
            parent.ChosenChildren = new List<int>();

            if (tileCount == 1)
            {
                parent.ChosenChildren.Add(8);
            }
            else
            {
                for (int i = 0; i < tileCount; i++)
                {
                    int child = UnityEngine.Random.Range(0, 9);

                    while (parent.ChosenChildren.Contains(child))
                    {
                        child = UnityEngine.Random.Range(0, 9);
                    }

                    parent.ChosenChildren.Add(child);
                }
            }

            parent.CreateChildrenFromList(Tile2, position, tileWidth, tileHeight);

            parent.Parent.transform.localScale /= 2.0f;
            return parent;
        }
        else if (GameMode == GameModes.Classic || GameMode == GameModes.Twist)
        {
            int shape = UnityEngine.Random.Range(0, Shapes.ShapesList.Count);

            ParentTile parent = new ParentTile();
            parent.Parent = Instantiate(new GameObject(), new Vector3(position.x, position.y), Quaternion.identity);
            parent.ChosenShape = shape;
            parent.CreateChildrenFromShape(Tile, position, tileWidth, tileHeight);
            parent.Parent.transform.localScale /= 2.0f;
            return parent;
        }
        else
        {
            ParentTile parent = new ParentTile();
            parent.Parent = Instantiate(new GameObject(), new Vector3(position.x, position.y), Quaternion.identity);
            parent.Parent.transform.localScale /= 2.0f;
            return parent;
        }
    }

    void RotateShape()
    {
        if (GameMode == GameModes.Random)
        {
            if (draggedObject.ChosenChildren.Count > 1)
            {
                var oldList = draggedObject.ChosenChildren;
                List<int> newList = new List<int>();

                foreach (var square in oldList)
                {
                    if (square <= 5)
                        newList.Add(square + 2);
                    else if (square == 6)
                        newList.Add(0);
                    else if (square == 7)
                        newList.Add(1);
                    else if (square == 8)
                        newList.Add(8);
                }

                for (int i = 0; i < draggedObject.Parent.transform.childCount; i++)
                {
                    Destroy(draggedObject.Parent.transform.GetChild(i).gameObject);
                }

                draggedObject.ChosenChildren = newList;
                draggedObject.CreateChildrenFromList(Tile2, originalPosition, tileWidth, tileHeight);
            }
        }
        else if (GameMode == GameModes.Twist)
        {
            if (Shapes.ShapesList[draggedObject.ChosenShape].Quadrants > 1)
            {
                draggedObject.ShapeQuadrant++;

                if (draggedObject.ShapeQuadrant >= Shapes.ShapesList[draggedObject.ChosenShape].Quadrants)
                    draggedObject.ShapeQuadrant = 0;

                for (int i = 0; i < draggedObject.Parent.transform.childCount; i++)
                {
                    Destroy(draggedObject.Parent.transform.GetChild(i).gameObject);
                }

                draggedObject.CreateChildrenFromShape(Tile, originalPosition, tileWidth, tileHeight);
            }
        }
    }

    void CleanupBoard()
    {
        List<GameObject> listDestroyed = new List<GameObject>();
        List<Vector2Int> listDestroyedGrid = new List<Vector2Int>();
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
                multiplier++;
                for (int x = 0; x < 9; x++)
                {
                    listDestroyed.Add(CurrentTiles[x, y]);
                    listDestroyedGrid.Add(new Vector2Int(x, y));
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
                multiplier++;
                for (int y = 0; y < 9; y++)
                {
                    if (!listDestroyed.Contains(CurrentTiles[x, y]))
                    {
                        listDestroyed.Add(CurrentTiles[x, y]);
                        listDestroyedGrid.Add(new Vector2Int(x, y));
                    }
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
                    multiplier++;
                    for (int y = i * 3; y < (i * 3) + 3; y++)
                    {
                        for (int x = j * 3; x < (j * 3) + 3; x++)
                        {
                            if (!listDestroyed.Contains(CurrentTiles[x, y]))
                            {
                                listDestroyed.Add(CurrentTiles[x, y]);
                                listDestroyedGrid.Add(new Vector2Int(x, y));
                            }
                        }
                    }
                }
            }
        }

        if (listDestroyed.Any())
        {
            Explosion.Play();

            Score += (listDestroyed.Count * multiplier);
            timeForMultiplier = 5f;
            ScoreText.GetComponent<TMPro.TextMeshProUGUI>().text = $"Score: {Score}";
        }

        foreach (var go in listDestroyed)
        {
            Destroy(go);
        }

        foreach (var v in listDestroyedGrid)
        {
            CurrentTiles[v.x, v.y] = null;
        }

        MultiplierText.GetComponent<TMPro.TextMeshProUGUI>().text = $"x{multiplier + 1}";
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

                    if (GameMode == GameModes.Classic || GameMode == GameModes.Twist)
                        CurrentTiles[x, y] = Instantiate(Tile, GridPoints[x, y], Quaternion.identity);
                    else if (GameMode == GameModes.Random)
                        CurrentTiles[x, y] = Instantiate(Tile2, GridPoints[x, y], Quaternion.identity);
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

        if (GameMode == GameModes.Classic)
        {
            PlayerPrefs.SetString("Shape1" + GameMode.ToString(), shape1 != null ? shape1.ChosenShape.ToString() : "");
            PlayerPrefs.SetInt("Shape1Quadrant" + GameMode.ToString(), shape1 != null ? shape1.ShapeQuadrant : -1);
            PlayerPrefs.SetString("Shape2" + GameMode.ToString(), shape2 != null ? shape2.ChosenShape.ToString() : "");
            PlayerPrefs.SetInt("Shape2Quadrant" + GameMode.ToString(), shape2 != null ? shape2.ShapeQuadrant : -1);
            PlayerPrefs.SetString("Shape3" + GameMode.ToString(), shape3 != null ? shape3.ChosenShape.ToString() : "");
            PlayerPrefs.SetInt("Shape3Quadrant" + GameMode.ToString(), shape3 != null ? shape3.ShapeQuadrant : -1);
        }
        else if (GameMode == GameModes.Random)
        {
            PlayerPrefs.SetString("Shape1" + GameMode.ToString(), shape1 != null ? string.Join(',', shape1.ChosenChildren) : "");
            PlayerPrefs.SetString("Shape2" + GameMode.ToString(), shape2 != null ? string.Join(',', shape2.ChosenChildren) : "");
            PlayerPrefs.SetString("Shape3" + GameMode.ToString(), shape3 != null ? string.Join(',', shape3.ChosenChildren) : "");
        }
        else if (GameMode == GameModes.Twist)
        {
            PlayerPrefs.SetString("Shape1" + GameMode.ToString(), shape1 != null ? shape1.ChosenShape.ToString() : "");
            PlayerPrefs.SetString("Shape2" + GameMode.ToString(), shape2 != null ? shape2.ChosenShape.ToString() : "");
            PlayerPrefs.SetString("Shape3" + GameMode.ToString(), shape3 != null ? shape3.ChosenShape.ToString() : "");
        }
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
        public int ChosenShape { get; set; }
        public int ShapeQuadrant { get; set; } = -1;
        public float RelativeX { get; set; } = 0;
        public float RelativeY { get; set; } = 0;
        public List<int> ChosenChildren { get; set; }

        public void CreateChildrenFromList(GameObject Tile, Vector3 position, float tileWidth, float tileHeight)
        {
            foreach (var child in ChosenChildren)
            {
                if (child == 0)
                    Instantiate(Tile, new Vector3(position.x - tileWidth, position.y + tileHeight, 0), Quaternion.identity, Parent.transform); // Bottom Left
                else if (child == 1)
                    Instantiate(Tile, new Vector3(position.x - tileWidth, position.y, 0), Quaternion.identity, Parent.transform); // Left 1
                else if (child == 2)
                    Instantiate(Tile, new Vector3(position.x - tileWidth, position.y - tileHeight, 0), Quaternion.identity, Parent.transform);  // Top Left
                else if (child == 3)
                    Instantiate(Tile, new Vector3(position.x, position.y - tileHeight, 0), Quaternion.identity, Parent.transform); // Up 1
                else if (child == 4)
                    Instantiate(Tile, new Vector3(position.x + tileWidth, position.y - tileHeight, 0), Quaternion.identity, Parent.transform); // Top Right
                else if (child == 5)
                    Instantiate(Tile, new Vector3(position.x + tileWidth, position.y, 0), Quaternion.identity, Parent.transform); // Right 1
                else if (child == 6)
                    Instantiate(Tile, new Vector3(position.x + tileWidth, position.y + tileHeight, 0), Quaternion.identity, Parent.transform); // Bottom Right
                else if (child == 7)
                    Instantiate(Tile, new Vector3(position.x, position.y + tileHeight, 0), Quaternion.identity, Parent.transform); // Bottom 1
                else if (child == 8)
                    Instantiate(Tile, new Vector3(position.x, position.y, 0), Quaternion.identity, Parent.transform); // center
                /*else if (child == 9)
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
                */
            }

            float sumX = 0;
            float sumY = 0;
            for (int i = 0; i < Parent.transform.childCount; i++)
            {
                sumX += Parent.transform.GetChild(i).position.x;
                sumY += Parent.transform.GetChild(i).position.y;
            }
            float centralX = sumX / Parent.transform.childCount;
            float centralY = sumY / Parent.transform.childCount;

            RelativeX = position.x - centralX;
            RelativeY = position.y - centralY;
        }

        public void CreateChildrenFromShape(GameObject Tile, Vector3 position, float tileWidth, float tileHeight)
        {
            if (ChosenShape >= Shapes.ShapesList.Count)
                ChosenShape %= Shapes.ShapesList.Count;

            if (ShapeQuadrant == -1)
            {
                if (GameMode == GameModes.Classic)
                    ShapeQuadrant = UnityEngine.Random.Range(0, Shapes.ShapesList[ChosenShape].Quadrants);
                else if (GameMode == GameModes.Twist)
                    ShapeQuadrant = 0;
            }

            var quadrant = Shapes.ShapesList[ChosenShape].GetQuadrant(ShapeQuadrant);
            for (int i = 0; i < quadrant.Count; i++)
            {
                float positionX = 0;
                float positionY = 0;

                positionX = (tileWidth * quadrant[i].Item1);
                positionY = (tileHeight * quadrant[i].Item2);

                Instantiate(Tile, new Vector3(position.x + positionX, position.y + positionY, 0), Quaternion.identity, Parent.transform);
            }

            float sumX = 0;
            float sumY = 0;
            for (int i = 0; i < Parent.transform.childCount; i++)
            {
                sumX += Parent.transform.GetChild(i).position.x;
                sumY += Parent.transform.GetChild(i).position.y;
            }
            float centralX = sumX / Parent.transform.childCount;
            float centralY = sumY / Parent.transform.childCount;

            RelativeX = position.x - centralX;
            RelativeY = position.y - centralY;
        }
    }

    [System.Serializable]
    public class SaveObject
    {
        public bool[] currentTiles;
    }
}
