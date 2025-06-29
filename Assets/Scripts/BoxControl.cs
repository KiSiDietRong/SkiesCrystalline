using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;

public class BoxControl : MonoBehaviour
{
    public float rollDuration = 0.2f;
    public float gridSize = 25f; // lưới theo scale map
    public Button upButton;
    public Button downButton;
    public Button leftButton;
    public Button rightButton;

    private bool isMoving = false;
    private string currentBottomFace;
    private string previousBottomFace;
    private float initialY;
    private float halfHeight;
    private LevelManager levelManager;
    public enum CubeFace { Top, Bottom, Front, Back, Left, Right }

    void Start()
    {
        if (upButton != null)
            upButton.onClick.AddListener(() => OnMoveButtonPressed(Vector3.forward));
        if (downButton != null)
            downButton.onClick.AddListener(() => OnMoveButtonPressed(Vector3.back));
        if (leftButton != null)
            leftButton.onClick.AddListener(() => OnMoveButtonPressed(Vector3.left));
        if (rightButton != null)
            rightButton.onClick.AddListener(() => OnMoveButtonPressed(Vector3.right));

        initialY = transform.position.y;
        UpdateBottomFace();
        previousBottomFace = currentBottomFace;
        halfHeight = transform.localScale.y / 2f;
    }

    void Update()
    {
        if (isMoving) return;

        if (Input.GetKeyDown(KeyCode.UpArrow))
            StartCoroutine(Roll(Vector3.forward));
        if (Input.GetKeyDown(KeyCode.DownArrow))
            StartCoroutine(Roll(Vector3.back));
        if (Input.GetKeyDown(KeyCode.LeftArrow))
            StartCoroutine(Roll(Vector3.left));
        if (Input.GetKeyDown(KeyCode.RightArrow))
            StartCoroutine(Roll(Vector3.right));
    }

    public void OnMoveButtonPressed(Vector3 direction)
    {
        if (!isMoving)
            StartCoroutine(Roll(direction));
    }

    IEnumerator Roll(Vector3 direction)
    {
        isMoving = true;

        string preRollBottomFace = currentBottomFace;

        // Tính pivot chính xác để không xuyên floor
        Vector3 pivotOffset = direction * (gridSize / 2f) - Vector3.up * halfHeight;
        Vector3 pivot = transform.position + pivotOffset;

        Vector3 axis = Vector3.Cross(Vector3.up, direction);
        float totalAngle = 0f;

        while (totalAngle < 90f)
        {
            float angle = Mathf.Min(Time.deltaTime * (90f / rollDuration), 90f - totalAngle);
            transform.RotateAround(pivot, axis, angle);
            totalAngle += angle;
            yield return null;
        }

        // Snap position and rotation
        transform.position = RoundToGrid(transform.position);

        Vector3 fixedEuler = transform.eulerAngles;
        fixedEuler.x = Mathf.Round(fixedEuler.x / 90f) * 90f;
        fixedEuler.y = Mathf.Round(fixedEuler.y / 90f) * 90f;
        fixedEuler.z = Mathf.Round(fixedEuler.z / 90f) * 90f;
        transform.rotation = Quaternion.Euler(fixedEuler);

        UpdateBottomFace();

        if (preRollBottomFace == CubeFace.Left.ToString() ||
            preRollBottomFace == CubeFace.Right.ToString() ||
            preRollBottomFace == CubeFace.Front.ToString() ||
            preRollBottomFace == CubeFace.Back.ToString())
        {
            if (currentBottomFace == CubeFace.Top.ToString() ||
                currentBottomFace == CubeFace.Bottom.ToString())
            {
                Vector3 newPosition = transform.position;
                if (direction == Vector3.right)
                    newPosition.x += gridSize;
                else if (direction == Vector3.left)
                    newPosition.x -= gridSize;
                else if (direction == Vector3.forward)
                    newPosition.z += gridSize;
                else if (direction == Vector3.back)
                    newPosition.z -= gridSize;

                transform.position = RoundToGrid(newPosition);
            }
        }

        isMoving = false;
    }

    Vector3 RoundToGrid(Vector3 pos)
    {
        return new Vector3(
            Mathf.Round(pos.x / gridSize) * gridSize,
            Mathf.Round(pos.y / gridSize) * gridSize,
            Mathf.Round(pos.z / gridSize) * gridSize
        );
    }

    void UpdateBottomFace()
    {
        Vector3[] faceDirections = new Vector3[]
        {
            transform.up,
            -transform.up,
            transform.forward,
            -transform.forward,
            -transform.right,
            transform.right
        };

        CubeFace[] faces = new CubeFace[]
        {
            CubeFace.Top,
            CubeFace.Bottom,
            CubeFace.Front,
            CubeFace.Back,
            CubeFace.Left,
            CubeFace.Right
        };

        float minAngle = float.MaxValue;
        CubeFace bottomFace = CubeFace.Bottom;

        for (int i = 0; i < faceDirections.Length; i++)
        {
            float angle = Vector3.Angle(faceDirections[i], Vector3.down);
            if (angle < minAngle)
            {
                minAngle = angle;
                bottomFace = faces[i];
            }
        }

        currentBottomFace = bottomFace.ToString();

        // Cập nhật lại độ cao đúng mặt đáy
        if (currentBottomFace != previousBottomFace)
        {
            Vector3 newPosition = transform.position;
            if (bottomFace == CubeFace.Left || bottomFace == CubeFace.Right ||
                bottomFace == CubeFace.Front || bottomFace == CubeFace.Back)
            {
                newPosition.y = initialY - (gridSize / 2f); // nằm nghiêng
            }
            else if (bottomFace == CubeFace.Top || bottomFace == CubeFace.Bottom)
            {
                newPosition.y = initialY; // đứng thẳng
            }
            transform.position = RoundToGrid(newPosition);
        }

        previousBottomFace = currentBottomFace;
    }

    public string GetCurrentBottomFace()
    {
        return currentBottomFace;
    }
    public void SetLevelManager(LevelManager manager)
    {
        levelManager = manager;
    }
}
