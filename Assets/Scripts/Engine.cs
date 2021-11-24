using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;

public class Engine : MonoBehaviour
{
    const float MAX_X = -6f;
    const float MIN_X = -9f;
    const float MAX_Y = 2.0f;
    const float MIN_Y = -4f;

    const float WIN_LIGHT_INTENCITY = 40f;
    const float LIGHT_INTENCITY_SPEED = 0.7f;

    const float ROTATE_EPSILON = 0.02f;
    const float MOVE_EPSILON = 0.2f;

    const string MUSIC_PREF_NAME = "music";
    const string SCORE_PREF_NAME = "score";

    const float MOUSE_ROTATE_MULTIPLICATOR = 10f;
    const float MOUSE_MOVE_MULTIPLICATOR = 0.2f;


    public Camera mainCamera;
    private GameObject target;

    public GameObject soundtrackAudio;
    public GameObject winAudio;

    public GameObject lightObject;

    public List<GameObject> objects;

    public GameObject helpObject;

    public GameObject nextLevelButton;

    public int level;
    public string help;


    private bool completed = false;
    private bool isLeftMousePressed = false;
    private bool isRightMousePressed = false;

    void Start()
    {
        if (PlayerPrefs.GetInt(MUSIC_PREF_NAME) != 1)
        {
            soundtrackAudio.SetActive(false);
            winAudio.SetActive(false);
        }
        helpObject.GetComponent<Text>().text = help;
    }

    void FixedUpdate()
    {
        if (!completed)
        {
            return;
        }
        if (lightObject.GetComponent<Light>().intensity < WIN_LIGHT_INTENCITY)
        {
            lightObject.GetComponent<Light>().intensity += LIGHT_INTENCITY_SPEED;
        }
    }

    void Update()
    {
        if (completed)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            isLeftMousePressed = true;

            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hitInfo))
            {
                if (hitInfo.collider.gameObject != null && objects.Contains(hitInfo.collider.gameObject))
                {
                    target = hitInfo.collider.gameObject;
                }
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            isLeftMousePressed = false;
        }

        if (Input.GetMouseButtonDown(1))
        {
            isRightMousePressed = true;

            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hitInfo))
            {
                if (hitInfo.collider.gameObject != null && objects.Contains(hitInfo.collider.gameObject))
                {
                    target = hitInfo.collider.gameObject;
                }
            }
        }
        if (Input.GetMouseButtonUp(1))
        {
            isRightMousePressed = false;
        }

        Rotate();
        CheckShadow();

        if (completed)
            onComlete();
    }

    void onComlete()
    {
        if (winAudio.activeSelf)
            winAudio.GetComponent<AudioSource>().Play();

        if (PlayerPrefs.GetInt(SCORE_PREF_NAME) < level)
            PlayerPrefs.SetInt(SCORE_PREF_NAME, level);

        nextLevelButton.SetActive(true);
    }

    void CheckShadow()
    {
        if (level == 1)
            completed = Check1Level();
        else if (level == 2)
            completed = Check2Level();
        else if (level == 3)
            completed = Check3Level();
        else if (level == 4)
            completed = Check4Level();
    }

    void Rotate()
    {
        if (!target)
            return;

        bool[] activities; // hirizontal, vertical, move
        if (level == 1)
            activities = new bool[] {true, false, false};
        else if (level == 2)
            activities = new bool[] {true, true, false};
        else if (level == 3)
            activities = new bool[] {true, true, true};
        else if (level == 4)
            activities = new bool[] {true, true, true};
        else
            activities = new bool[] {false, false, false};

        if (isLeftMousePressed)
        {            
            float yRot = activities[0] ? Input.GetAxis("Mouse X") * MOUSE_ROTATE_MULTIPLICATOR : 0f;
            float zRot = activities[1] ? Input.GetAxis("Mouse Y") * MOUSE_ROTATE_MULTIPLICATOR : 0f;

            target.transform.Rotate(0f, -yRot, -zRot, Space.World);

        }
        else if (isRightMousePressed)
        {            
            float xMov = activities[2] ? Input.GetAxis("Mouse X") * MOUSE_MOVE_MULTIPLICATOR : 0f;
            float yMov = activities[2] ? Input.GetAxis("Mouse Y") * MOUSE_MOVE_MULTIPLICATOR : 0f;

            float nextX = target.transform.position.x + xMov;
            float nextY = target.transform.position.y + yMov;

            if (nextX < MIN_X || nextX > MAX_X)
                xMov = 0f;
            if (nextY < MIN_Y || nextY > MAX_Y)
                yMov = 0f;

            target.transform.Translate(xMov, yMov, 0f, Space.World);
        }
    }

    bool checkPosition(Vector3 need, Vector3 current)
    {
        Vector3 diff = need - current;
        return diff.magnitude < MOVE_EPSILON;
    }

    bool isForward(Vector3 vec)
    {
        return 1.0f - vec.normalized.z < ROTATE_EPSILON;
    }
    bool isBack(Vector3 vec)
    {
        return 1.0f + vec.normalized.z < ROTATE_EPSILON;
    }
    bool isRight(Vector3 vec)
    {
        return 1.0f - vec.normalized.x < ROTATE_EPSILON;
    }
    bool isLeft(Vector3 vec)
    {
        return 1.0f + vec.normalized.x < ROTATE_EPSILON;
    }
    bool isUp(Vector3 vec)
    {
        return 1.0f - vec.normalized.y < ROTATE_EPSILON;
    }
    bool isDown(Vector3 vec)
    {
        return 1.0f + vec.normalized.y < ROTATE_EPSILON;
    }

    // Teapot
    bool Check1Level()
    {
        return isForward(objects[0].transform.forward) || isBack(objects[0].transform.forward);
    }

    // Elephant
    bool Check2Level()
    {
        return isForward(objects[0].transform.up) || isBack(objects[0].transform.up);
    }

    // Globe
    bool Check3Level()
    {
        bool earthRot = isForward(objects[0].transform.up) || isBack(objects[0].transform.up);
        bool baseRot = isForward(objects[1].transform.up) || isBack(objects[1].transform.up);

        Vector3 positionDiff = objects[0].transform.position - objects[1].transform.position;
        positionDiff.z = 0f;

        Vector3 needPositionDiff = new Vector3(0.0f, 0.0f, 0.0f);

        return checkPosition(needPositionDiff, positionDiff) && earthRot && baseRot;
    }

    // 42
    bool Check4Level()
    {
        bool rot4 = isBack(objects[1].transform.forward) && isUp(objects[1].transform.up);
        bool rot2 = isBack(objects[0].transform.forward) && (isUp(objects[0].transform.up) || isDown(objects[0].transform.up) );

        Vector3 positionDiff = objects[0].transform.position - objects[1].transform.position;
        positionDiff.z = 0f;

        Vector3 needPositionDiff = new Vector3(1.5f, 0.2f, 0.0f);

        return checkPosition(needPositionDiff, positionDiff) && rot4 && rot2;
    }

    public void backToMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void nextLevel()
    {
        int nextScene = level + 1;
        if (nextScene > 4)
            nextScene = 0;

        SceneManager.LoadScene(nextScene);
    }
}
