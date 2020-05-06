using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzlePieceScript : MonoBehaviour
{
    [SerializeField] private int puzzleSize = 3;

    [SerializeField] private float speed = 0.1f;

    public bool isNode = false;

    //Materials
    [SerializeField] private Material unconnected;
    [SerializeField] private Material connected;

    public bool moving = false;

    public Vector2 position;


    public IEnumerator MoveUp()
    {
        position.y++;

        float distance = 0.6f / puzzleSize;

        Vector3 startPos = transform.position;

        Vector3 endPos = new Vector3(transform.position.x, transform.position.y + distance, transform.position.z);

        moving = true;

        for (float i = 0; i < 1 + speed; i+=speed)
        {
            transform.position = Vector3.Lerp(startPos, endPos, i);

            yield return new WaitForEndOfFrame();
        }

        moving = false;
    }

    public IEnumerator MoveDown()
    {
        position.y--;

        float distance = 0.6f / puzzleSize;

        Vector3 startPos = transform.position;

        Vector3 endPos = new Vector3(transform.position.x, transform.position.y - distance, transform.position.z);

        moving = true;

        for (float i = 0; i < 1 + speed; i += speed)
        {
            transform.position = Vector3.Lerp(startPos, endPos, i);

            yield return new WaitForEndOfFrame();
        }

        moving = false;
    }

    public IEnumerator MoveLeft()
    {
        position.x--;

        float distance = 0.6f / puzzleSize;

        Vector3 startPos = transform.position;

        Vector3 endPos = new Vector3(transform.position.x - distance, transform.position.y, transform.position.z);

        moving = true;

        for (float i = 0; i < 1 + speed; i += speed)
        {
            transform.position = Vector3.Lerp(startPos, endPos, i);

            yield return new WaitForEndOfFrame();
        }

        moving = false;
    }

    public IEnumerator MoveRight()
    {
        position.x++;

        float distance = 0.6f / puzzleSize;

        Vector3 startPos = transform.position;

        Vector3 endPos = new Vector3(transform.position.x + distance, transform.position.y, transform.position.z);

        moving = true;

        for (float i = 0; i < 1 + speed; i += speed)
        {
            transform.position = Vector3.Lerp(startPos, endPos, i);

            yield return new WaitForEndOfFrame();
        }

        moving = false;
    }
}
