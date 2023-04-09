using Assets.Scripts;
using System;
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
            PlayerPrefs.DeleteKey("Shape1" + GameMode.ToString());
            PlayerPrefs.DeleteKey("Shape2" + GameMode.ToString());
            PlayerPrefs.DeleteKey("Shape3" + GameMode.ToString());
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
        
        if (PlayerPrefs.HasKey("Shape1" + GameMode.ToString()))
        {
            string savedShape = PlayerPrefs.GetString("Shape1" + GameMode.ToString());

            if (!string.IsNullOrEmpty(savedShape))
            {
                shape1 = new ParentTile();
                shape1.Parent = Instantiate(new GameObject(), new Vector3(TileSpawnPoint1.transform.position.x, TileSpawnPoint1.transform.position.y), Quaternion.identity);

                if (GameMode == GameModes.Classic)
                {
                    shape1.ChosenShape = Convert.ToInt32(savedShape);
                    shape1.CreateChildrenFromShape(Tile, TileSpawnPoint1.transform.position, tileWidth, tileHeight);
                }
                else if (GameMode == GameModes.Random)
                {
                    shape1.ChosenChildren = savedShape.Split(',').Select(x => Convert.ToInt32(x)).ToList();
                    shape1.CreateChildrenFromList(Tile, TileSpawnPoint1.transform.position, tileWidth, tileHeight);
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
                    shape2.ChosenShape = Convert.ToInt32(savedShape);
                    shape2.CreateChildrenFromShape(Tile, TileSpawnPoint2.transform.position, tileWidth, tileHeight);
                }
                else if (GameMode == GameModes.Random)
                {
                    shape2.ChosenChildren = savedShape.Split(',').Select(x => Convert.ToInt32(x)).ToList();
                    shape2.CreateChildrenFromList(Tile, TileSpawnPoint2.transform.position, tileWidth, tileHeight);
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
                    shape3.ChosenShape = Convert.ToInt32(savedShape);
                    shape3.CreateChildrenFromShape(Tile, TileSpawnPoint3.transform.position, tileWidth, tileHeight);
                }
                else if (GameMode == GameModes.Random)
                {
                    shape3.ChosenChildren = savedShape.Split(',').Select(x => Convert.ToInt32(x)).ToList();
                    shape3.CreateChildrenFromList(Tile, TileSpawnPoint3.transform.position, tileWidth, tileHeight);
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

                draggedObject = null;
            }
        }
    }

    ParentTile InstantiateShape(Vector3 position)
    {
        if (GameMode == GameModes.Random)
        {
            int tileCount = UnityEngine.Random.Range(1, 101);

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
            parent.ChosenChildren = new List<int>();

            for (int i = 0; i < tileCount; i++)
            {
                int child = UnityEngine.Random.Range(0, 9);

                while (parent.ChosenChildren.Contains(child))
                {
                    child = UnityEngine.Random.Range(0, 9);
                }

                parent.ChosenChildren.Add(child);
            }

            parent.CreateChildrenFromList(Tile, position, tileWidth, tileHeight);

            parent.Parent.transform.localScale /= 2.0f;
            return parent;
        }
        else if (GameMode == GameModes.Classic)
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

    void CleanupBoard()
    {
        List<GameObject> listDestroyed = new List<GameObject>();
        List<Vector2Int> listDestroyedGrid = new List<Vector2Int>();
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
                multFactor++;
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
                    multFactor++;
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

            Score += (listDestroyed.Count * multFactor);
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

        if (GameMode == GameModes.Classic)
        {
            PlayerPrefs.SetString("Shape1" + GameMode.ToString(), shape1 != null ? shape1.ChosenShape.ToString() : "");
            PlayerPrefs.SetString("Shape2" + GameMode.ToString(), shape2 != null ? shape2.ChosenShape.ToString() : "");
            PlayerPrefs.SetString("Shape3" + GameMode.ToString(), shape3 != null ? shape3.ChosenShape.ToString() : "");
        }
        else if (GameMode == GameModes.Random)
        {
            PlayerPrefs.SetString("Shape1" + GameMode.ToString(), shape1 != null ? string.Join(',', shape1.ChosenChildren) : "");
            PlayerPrefs.SetString("Shape2" + GameMode.ToString(), shape2 != null ? string.Join(',', shape2.ChosenChildren) : "");
            PlayerPrefs.SetString("Shape3" + GameMode.ToString(), shape3 != null ? string.Join(',', shape3.ChosenChildren) : "");
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
        public float RelativeX { get; set; } = 0;
        public List<int> ChosenChildren { get; set; }

        public void CreateChildrenFromList(GameObject Tile, Vector3 position, float tileWidth, float tileHeight)
        {
            foreach (var child in ChosenChildren)
            {
                if (child == 0)
                    Instantiate(Tile, new Vector3(position.x - tileWidth, position.y + tileHeight), Quaternion.identity, Parent.transform); // Bottom Left
                else if (child == 1)
                    Instantiate(Tile, new Vector3(position.x, position.y + tileHeight), Quaternion.identity, Parent.transform); // Bottom 1
                else if (child == 2)
                    Instantiate(Tile, new Vector3(position.x + tileWidth, position.y + tileHeight), Quaternion.identity, Parent.transform); // Bottom Right
                else if (child == 3)
                    Instantiate(Tile, new Vector3(position.x - tileWidth, position.y), Quaternion.identity, Parent.transform); // Left 1
                else if (child == 4)
                    Instantiate(Tile, new Vector3(position.x, position.y), Quaternion.identity, Parent.transform); // center
                else if (child == 5)
                    Instantiate(Tile, new Vector3(position.x + tileWidth, position.y), Quaternion.identity, Parent.transform); // Right 1
                else if (child == 6)
                    Instantiate(Tile, new Vector3(position.x - tileWidth, position.y - tileHeight), Quaternion.identity, Parent.transform);  // Top Left
                else if (child == 7)
                    Instantiate(Tile, new Vector3(position.x, position.y - tileHeight), Quaternion.identity, Parent.transform); // Up 1
                else if (child == 8)
                    Instantiate(Tile, new Vector3(position.x + tileWidth, position.y - tileHeight), Quaternion.identity, Parent.transform); // Top Right
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
            for (int i = 0; i < ChosenChildren.Count; i++)
            {
                sumX += Parent.transform.GetChild(i).position.x;
            }
            float centralX = sumX / ChosenChildren.Count;

            RelativeX = Parent.transform.position.x - centralX;
        }

        public void CreateChildrenFromShape(GameObject Tile, Vector3 position, float tileWidth, float tileHeight)
        {
            for (int i = 0; i < Shapes.ShapesList[ChosenShape].Count; i++)
            {
                float positionX = 0;
                float positionY = 0;

                positionX = (tileWidth * Shapes.ShapesList[ChosenShape][i].Item1);
                positionY = (tileHeight * Shapes.ShapesList[ChosenShape][i].Item2);

                Instantiate(Tile, new Vector3(position.x + positionX, position.y + positionY), Quaternion.identity, Parent.transform);
            }
        }
    }

    [System.Serializable]
    public class SaveObject
    {
        public bool[] currentTiles;
    }
}
