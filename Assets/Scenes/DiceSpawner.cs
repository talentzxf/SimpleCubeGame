using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class DiceSpawner : MonoBehaviour
{
    public int n = 3;

    public GameObject buttonPrefab;
    public GameObject canvas;
    public GameObject buttonArrayPositon;
    public GameObject scoreText;

    private TextMeshProUGUI scoreTextInput;
    
    private float offset = 4.0f;
    private float buttonOffset = 30.0f;

    // initial/starting die in the gallery
    private string galleryDie = "d6-red";

    private bool isRotating = false;

    private Die[,] dieArray;
    private System.Random rnd = new System.Random();
    
    private void setupDice()
    {
        string[] dieName = galleryDie.Split("-");

        dieArray = new Die[n, n];

        for (int i = 0; i < n; i++) // the ith row
        {
            for (int j = 0; j < n; j++) // the jth column
            {
                Vector3 offset = new Vector3(i * this.offset, 0.0f, j * this.offset);
                GameObject dieGO = Dice.prefab(dieName[0], this.transform.position + offset,
                    this.transform.rotation.eulerAngles,
                    new Vector3(1.4f, 1.4f, 1.4f), galleryDie);

                dieGO.GetComponent<Rigidbody>().isKinematic = true;

                dieArray[i, j] = (Die)dieGO.GetComponent(typeof(Die));
            }
        }
    }

    IEnumerator rotateColumn(int colId)
    {
        isRotating = true;
        float rotateAngle = 0.0f;

        while (rotateAngle < 90.0f)
        {
            rotateAngle += 1.0f;
            for (int i = 0; i < n; i++)
            {
                dieArray[colId, i].transform.Rotate(Vector3.forward, 1.0f, Space.World);
            }
            
            yield return null;
        }

        isRotating = false;
    }
    
    IEnumerator rotateRow(int rowId)
    {
        isRotating = true;
        float rotateAngle = 0.0f;
        
        while (rotateAngle < 90.0f)
        {
            rotateAngle += 1.0f;
            for (int i = 0; i < n; i++)
            {
                dieArray[i, rowId].transform.Rotate(Vector3.left, 1.0f, Space.World);
            }
            
            yield return null;
        }

        isRotating = false;
    }
    
    private void setupUI()
    {
        RectTransform buttonArrayRectTransform = buttonArrayPositon.GetComponent<RectTransform>();

        //TODO: Merge these two for-loop in one to avoid code duplication.
        for (int i = 0; i < n; i++) // the ith column
        {
            GameObject buttonGO = Instantiate(buttonPrefab);
            buttonGO.transform.SetParent(buttonArrayPositon.transform);

            buttonGO.transform.localPosition = new Vector3();
            RectTransform buttonRectTransform = buttonGO.GetComponent<RectTransform>();
            buttonRectTransform.localPosition = new Vector2((i + 1) * buttonOffset, -buttonOffset );

            TextMeshProUGUI text = buttonGO.GetComponentInChildren<TextMeshProUGUI>();
            text.text = (i + 1).ToString();

            Button btn = buttonGO.GetComponent<Button>();

            int columnId = i;
            btn.onClick.AddListener(delegate { if(!isRotating) { StartCoroutine(rotateColumn(columnId));} });
        }

        for (int j = 0; j < n; j++) // The jth row
        {
            GameObject buttonGO = Instantiate(buttonPrefab);
            buttonGO.transform.SetParent(buttonArrayPositon.transform);

            buttonGO.transform.localPosition = new Vector3();
            RectTransform buttonRectTransform = buttonGO.GetComponent<RectTransform>();
            buttonRectTransform.localPosition = new Vector2(0.0f, j * this.buttonOffset);

            TextMeshProUGUI text = buttonGO.GetComponentInChildren<TextMeshProUGUI>();
            text.text = (j + 1).ToString();
            
            Button btn = buttonGO.GetComponent<Button>();

            int rowId = j;
            btn.onClick.AddListener(delegate { if(!isRotating){ StartCoroutine(rotateRow(rowId));} });
        }

        scoreTextInput = scoreText.GetComponent<TextMeshProUGUI>();
    }

    void randomShuffleDice()
    {
        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                int leftCount = rnd.Next() % 4;
                dieArray[i, j].transform.Rotate(Vector3.left, 90.0f * leftCount, Space.World);
                int forwardCount = rnd.Next() % 4;
                dieArray[i, j].transform.Rotate(Vector3.forward, 90.0f * forwardCount, Space.World);
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        setupDice();
        randomShuffleDice();
        setupUI();
    }

    // Update is called once per frame
    void Update()
    {
        int score = 0;

        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                score += dieArray[i, j].value;
            }
        }

        scoreTextInput.text = "Score:" + score;
    }
}