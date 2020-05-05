using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleScript : MonoBehaviour
{
    [SerializeField] private int puzzleSize = 3;

    [SerializeField] private int numberOfPieces = 4;

    private GameObject[,] pieces;

    [SerializeField] private GameObject piecesParent;

    bool moving = false; //Returns true if pieces are currentl moving. Used to only allow further inputs until after all pieces are done repositioning


    // Start is called before the first frame update   
    void Start()
    {
        pieces = new GameObject[puzzleSize, puzzleSize]; //Intialize pieces array

        for (int i = 0; i < numberOfPieces; i++) //For loop to get gameObject piece[i] and set it to the pieces[,] array where the terms in the array are the pieces' position on the puzzle grid
        {
            Transform child = piecesParent.transform.GetChild(i);
            PuzzlePieceScript puzzlePieceScript = child.GetComponent<PuzzlePieceScript>();
            pieces[(int)puzzlePieceScript.position.x, (int)puzzlePieceScript.position.y] = child.gameObject;
        }
    }


    // Update is called once per frame
    void Update()
    {
        //When moving = true
        for (int i = 0; i < puzzleSize && moving; i++) //Checks from leftmost column and up, then one column right and up, etc. and sets moving = true if any puzzlePieces are moving
        {
            for (int j = 0; j < puzzleSize && moving; j++)
            {
                if (pieces[i, j] != null)
                {
                    PuzzlePieceScript puzzlePieceScript = pieces[i, j].GetComponent<PuzzlePieceScript>();

                    moving = puzzlePieceScript.moving;
                }
            }
        }
        //When moving = false
        for (int i = 0; i < puzzleSize && !moving; i++) //Checks from leftmost column and up, then one column right and up, etc. and sets moving = true if any puzzlePieces are moving
        {
            for (int j = 0; j < puzzleSize && !moving; j++)
            {
                if (pieces[i,j] != null)
                {
                    PuzzlePieceScript puzzlePieceScript = pieces[i, j].GetComponent<PuzzlePieceScript>();

                    moving = puzzlePieceScript.moving;
                }
            }
        }

        if (Input.GetKeyDown("w") && !moving) //Move Up
        {
            for (int i = 0; i < puzzleSize; i++)
            {
                for (int j = 0; j < puzzleSize; j++)
                {
                    if (pieces[i,j] != null) //Search through all pieces, starting in the leftmost column and going up, then right 1 column, etc. If there is a piece in the position, this if statement returns true
                    {
                        //Debug.Log("i" + i + "j" + j);

                        bool boundaryBlocked; //true if piece [i,j] blocked by the boundary of the puzzle grid

                        bool checkAbove = true; //Determines if k loop keeps checking; false if there is an empty space above a piece

                        bool piecesAboveBlock = false; //True if pieces above block movement of piece[i,j] (set to false to fix compiler error of local unassigned variable as a result of this variable being set in a nested if statement)

                        //First check is if this piece will move out of the boundary of the puzzle grid
                        if (j < puzzleSize - 1)
                        {
                            boundaryBlocked = false;
                        }
                        else
                        {
                            boundaryBlocked = true;
                        }

                        //Debug.Log("i" + i + "j" + j + "boundaryBlocked = " + boundaryBlocked);

                        //Second check is if there is a piece above, Third check is if the piece above will move out of the boundary of the puzzle grid; Repeat second and third checks for higher j values
                        for (int k = 1; k < puzzleSize && checkAbove && !boundaryBlocked; k++) 
                        {
                            if (pieces[i, j + k] != null) //Second check
                            {
                               if (j + k < puzzleSize - 1) //Third check
                               {
                                    piecesAboveBlock = false;
                               }
                               else
                               {
                                    piecesAboveBlock = true;
                                    checkAbove = false;
                               }

                                //Debug.Log("i" + i + "j+k" + j + k + "piecesAboveBlock = " + piecesAboveBlock);
                            }
                            else
                            {
                                checkAbove = false;
                                piecesAboveBlock = false;
                            }
                        }

                        //The big if statement that moves the piece if it and all the pieces immediately above it are not blocked by the boundary of the puzzle
                        if (!boundaryBlocked && !piecesAboveBlock)
                        {
                            //Debug.Log("MoveUp" + "i" + i + "j" + j);

                            PuzzlePieceScript puzzlePieceScript = pieces[i, j].GetComponent<PuzzlePieceScript>();

                            IEnumerator moveUp = puzzlePieceScript.MoveUp();

                            StartCoroutine(moveUp);
                        }
                    }
                }
            }
            ReArray();
        } //End of Move Up


        if (Input.GetKeyDown("s") && !moving) //Move Down
        {
            for (int i = 0; i < puzzleSize; i++)
            {
                for (int j = 0; j < puzzleSize; j++)
                {
                    if (pieces[i, j] != null) //Search through all pieces, starting in the leftmost column and going up, then right 1 column, etc. If there is a piece in the position, this if statement returns true
                    {
                        //Debug.Log("i" + i + "j" + j);

                        bool boundaryBlocked; //true if piece [i,j] blocked by the boundary of the puzzle grid

                        bool checkBelow = true; //Determines if k loop keeps checking; false if there is an empty space below a piece

                        bool piecesBelowBlock = false; //True if pieces below block movement of piece[i,j] (set to false to fix compiler error of local unassigned variable as a result of this variable being set in a nested if statement)

                        //First check is if this piece will move out of the boundary of the puzzle grid
                        if (j > 0)
                        {
                            boundaryBlocked = false;
                        }
                        else
                        {
                            boundaryBlocked = true;
                        }

                        //Debug.Log("i" + i + "j" + j + "boundaryBlocked = " + boundaryBlocked);

                        //Second check is if there is a piece below, Third check is if the piece below will move out of the boundary of the puzzle grid; Repeat second and third checks for lower j values 
                        for (int k = 1; k < puzzleSize && checkBelow && !boundaryBlocked; k++)
                        {
                            if (pieces[i, j - k] != null) //Second check
                            {
                                if (j - k > 0) //Third check
                                {
                                    piecesBelowBlock = false;
                                }
                                else
                                {
                                    piecesBelowBlock = true;
                                    checkBelow = false;
                                }

                                //Debug.Log("i" + i + "j-k" + j + k + "piecesBelowBlock = " + piecesBelowBlock);
                            }
                            else
                            {
                                checkBelow = false;
                                piecesBelowBlock = false;
                            }
                        }

                        //The big if statement that moves the piece if it and all the pieces immediately below it are not blocked by the boundary of the puzzle
                        if (!boundaryBlocked && !piecesBelowBlock)
                        {
                            //Debug.Log("MoveDown" + "i" + i + "j" + j);

                            PuzzlePieceScript puzzlePieceScript = pieces[i, j].GetComponent<PuzzlePieceScript>();

                            IEnumerator moveDown = puzzlePieceScript.MoveDown();

                            StartCoroutine(moveDown);
                        }
                    }
                }
            }
            ReArray();
        } //End of Move Down


        if (Input.GetKeyDown("a") && !moving) //Move Left
        {
            for (int i = 0; i < puzzleSize; i++)
            {
                for (int j = 0; j < puzzleSize; j++)
                {
                    if (pieces[i, j] != null) //Search through all pieces, starting in the leftmost column and going up, then right 1 column, etc. If there is a piece in the position, this if statement returns true
                    {
                        //Debug.Log("i" + i + "j" + j);

                        bool boundaryBlocked; //true if piece [i,j] blocked by the boundary of the puzzle grid

                        bool checkLeft = true; //Determines if k loop keeps checking; false if there is an empty space to the left of a piece

                        bool piecesLeftBlock = false; //True if pieces to the left block movement of piece[i,j] (set to false to fix compiler error of local unassigned variable as a result of this variable being set in a nested if statement)

                        //First check is if this piece will move out of the boundary of the puzzle grid
                        if (i > 0)
                        {
                            boundaryBlocked = false;
                        }
                        else
                        {
                            boundaryBlocked = true;
                        }

                        //Debug.Log("i" + i + "j" + j + "boundaryBlocked = " + boundaryBlocked);

                        //Second check is if there is a piece left, Third check is if the piece left will move out of the boundary of the puzzle grid; Repeat second and third checks for lower i values
                        for (int k = 1; k < puzzleSize && checkLeft && !boundaryBlocked; k++)
                        {
                            if (pieces[i - k, j] != null) //Second check
                            {
                                if (i - k > 0) //Third check
                                {
                                    piecesLeftBlock = false;
                                }
                                else
                                {
                                    piecesLeftBlock = true;
                                    checkLeft = false;
                                }

                                //Debug.Log("i-k" + i + k + "j" + j  + "piecesLeftBlock = " + piecesLeftBlock);
                            }
                            else
                            {
                                checkLeft = false;
                                piecesLeftBlock = false;
                            }
                        }

                        //The big if statement that moves the piece if it and all the pieces immediately left of it are not blocked by the boundary of the puzzle
                        if (!boundaryBlocked && !piecesLeftBlock)
                        {
                            //Debug.Log("MoveLeft" + "i" + i + "j" + j);

                            PuzzlePieceScript puzzlePieceScript = pieces[i, j].GetComponent<PuzzlePieceScript>();

                            IEnumerator moveLeft = puzzlePieceScript.MoveLeft();

                            StartCoroutine(moveLeft);
                        }
                    }
                }
            }
            ReArray();
        } //End of Move Down


        if (Input.GetKeyDown("d") && !moving) //Move Right
        {
            for (int i = 0; i < puzzleSize; i++)
            {
                for (int j = 0; j < puzzleSize; j++)
                {
                    if (pieces[i, j] != null) //Search through all pieces, starting in the leftmost column and going up, then right 1 column, etc. If there is a piece in the position, this if statement returns true
                    {
                        //Debug.Log("i" + i + "j" + j);

                        bool boundaryBlocked; //true if piece [i,j] blocked by the boundary of the puzzle grid

                        bool checkRight = true; //Determines if k loop keeps checking; false if there is an empty space to the right of a piece

                        bool piecesRightBlock = false; //True if pieces to the right block movement of piece[i,j] (set to false to fix compiler error of local unassigned variable as a result of this variable being set in a nested if statement)

                        //First check is if this piece will move out of the boundary of the puzzle grid
                        if (i < puzzleSize - 1)
                        {
                            boundaryBlocked = false;
                        }
                        else
                        {
                            boundaryBlocked = true;
                        }

                        //Debug.Log("i" + i + "j" + j + "boundaryBlocked = " + boundaryBlocked);

                        //Second check is if there is a piece right, Third check is if the piece right will move out of the boundary of the puzzle grid; Repeat second and third checks for higher i values
                        for (int k = 1; k < puzzleSize && checkRight && !boundaryBlocked; k++)
                        {
                            if (pieces[i + k, j] != null) //Second check
                            {
                                if (i + k < puzzleSize - 1) //Third check
                                {
                                    piecesRightBlock = false;
                                }
                                else
                                {
                                    piecesRightBlock = true;
                                    checkRight = false;
                                }

                                //Debug.Log("i+k" + i + k + "j" + j  + "piecesRightBlock = " + piecesRightBlock);
                            }
                            else
                            {
                                checkRight = false;
                                piecesRightBlock = false;
                            }
                        }

                        //The big if statement that moves the piece if it and all the pieces immediately right of it are not blocked by the boundary of the puzzle
                        if (!boundaryBlocked && !piecesRightBlock)
                        {
                            //Debug.Log("MoveRight" + "i" + i + "j" + j);

                            PuzzlePieceScript puzzlePieceScript = pieces[i, j].GetComponent<PuzzlePieceScript>();

                            IEnumerator moveRight = puzzlePieceScript.MoveRight();

                            StartCoroutine(moveRight);
                        }
                    }
                }
            }
            ReArray();
        } //End of Move Down
    }


    void ReArray() //Reinitialize the array with updated positions for all pieces, call after each button press event
    {
        for (int i = 0; i < puzzleSize; i++)
        {
            for (int j = 0; j < puzzleSize; j++)
            {
                pieces[i, j] = null; //Clear the array
            }
        }

        //Reinitialize array with updated positions
        for (int i = 0; i < numberOfPieces; i++) //For loop to get gameObject piece[i] and set it to the pieces[,] array where the terms in the array are the pieces' position on the puzzle grid
        {
            Transform child = piecesParent.transform.GetChild(i);
            PuzzlePieceScript puzzlePieceScript = child.GetComponent<PuzzlePieceScript>();
            pieces[(int)puzzlePieceScript.position.x, (int)puzzlePieceScript.position.y] = child.gameObject;
        }
    }
}