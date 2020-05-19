using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleScript : MonoBehaviour
{
    [SerializeField] private GameObject player;

    [SerializeField] private GameObject instructions;

    [SerializeField] private int puzzleSize = 3;

    [SerializeField] private int numberOfPieces = 4;

    [SerializeField] private int numberOfNodes = 2;

    [SerializeField] private int numberOfBlockers = 1;

    private GameObject[,] blockers;

    private GameObject[,] pieces;

    private Vector2[] startingPiecePos;

    private Vector3[] startingPieceVector3;

    [SerializeField] private GameObject piecesParent;

    [SerializeField] private GameObject nodesParent;

    [SerializeField] private GameObject blockersParent;

    private GameObject[,] nodes;

    bool moving = false; //Returns true if pieces are currentl moving. Used to only allow further inputs until after all pieces are done repositioning

    private int n = 0; //Used in logic to stop infinte loops
    private int n2 = 0;

    public bool opensDoor = true;

    public DoorScript doorScript;

    private int ni;
    private int nj;

    private bool lockPuzzle = false; //Lock the puzzle for a small delay after win

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

        startingPiecePos = new Vector2[numberOfPieces]; //Initialize storing starting positions of pieces
        startingPieceVector3 = new Vector3[numberOfPieces]; //Initialize storing the starting world position of pieces

        for (int i = 0; i < numberOfPieces; i++)
        {
            Transform child = piecesParent.transform.GetChild(i);
            PuzzlePieceScript puzzlePieceScript = child.GetComponent<PuzzlePieceScript>();
            startingPiecePos[i] = puzzlePieceScript.position; //Store the starting position of all the pieces

            startingPieceVector3[i] = child.transform.position;
        }

        nodes = new GameObject[puzzleSize, puzzleSize]; //Intialize nodes array

        for (int i = 0; i < numberOfNodes; i++)
        {
            Transform child = nodesParent.transform.GetChild(i);
            PuzzlePieceScript puzzlePieceScript = child.GetComponent<PuzzlePieceScript>();
            nodes[(int)puzzlePieceScript.position.x, (int)puzzlePieceScript.position.y] = child.gameObject;
        }

        blockers = new GameObject[puzzleSize, puzzleSize]; //Intialize blockers array

        for (int i = 0; i < numberOfBlockers; i++)
        {
            Transform child = blockersParent.transform.GetChild(i);
            PuzzlePieceScript puzzlePieceScript = child.GetComponent<PuzzlePieceScript>();
            blockers[(int)puzzlePieceScript.position.x, (int)puzzlePieceScript.position.y] = child.gameObject;
        }

        CalcConnectivityFromNode();

        instructions.SetActive(false);
        
        this.enabled = false;
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

        if (Input.GetKeyDown("w") && !moving && !lockPuzzle) //Move Up
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

                            if (blockers[i, j + k] != null) //BlockerCheck
                            {
                                piecesAboveBlock = true;
                                checkAbove = false;
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
            n = 0;
            n2 = 0;
            ReArray();
            CalcConnectivityFromNode();
            calcWin();
        } //End of Move Up


        if (Input.GetKeyDown("s") && !moving && !lockPuzzle) //Move Down
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

                            if (blockers[i, j - k] != null) //BlockerCheck
                            {
                                piecesBelowBlock = true;
                                checkBelow = false;
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
            n = 0;
            n2 = 0;
            ReArray();
            CalcConnectivityFromNode();
            calcWin();
        } //End of Move Down


        if (Input.GetKeyDown("a") && !moving && !lockPuzzle) //Move Left
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

                            if (blockers[i - k, j] != null) //BlockerCheck
                            {
                                piecesLeftBlock = true;
                                checkLeft = false;
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
            n = 0;
            n2 = 0;
            ReArray();
            CalcConnectivityFromNode();
            calcWin();
        } //End of Move Left


        if (Input.GetKeyDown("d") && !moving && !lockPuzzle) //Move Right
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


                            if (blockers[i + k, j] != null) //BlockerCheck
                            {
                                piecesRightBlock = true;
                                checkRight = false;
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
            n = 0;
            n2 = 0;
            ReArray();
            CalcConnectivityFromNode();
            calcWin();
        } //End of Move Right

        //Quit puzzle
        if (Input.GetKeyDown("q") && !lockPuzzle)
        {
            QuitPuzzle();
        }

        //Restart puzzle
        if (Input.GetKeyDown("r") && !lockPuzzle)
        {
            Restart();
        }
    }


    public void TurnOnInstructions()
    {
        instructions.SetActive(true);
    }


    void QuitPuzzle()
    {
        PlayerMove playerMove = player.GetComponent<PlayerMove>();

        playerMove.inPuzzle = false;

        instructions.SetActive(false);

        this.enabled = false;
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


    void CalcConnectivityFromNode() //Determine if pieces are connected to nodes USED ONLY FOR VISUALS
    {
        //Set all pieces to unnconnected before determining connectivity (this does not produce visual artifacts because this is overridden on the same frame)
        for (int i = 0; i < puzzleSize; i++)
        {
            for (int j = 0; j < puzzleSize; j++)
            {
                if (pieces[i,j] != null)
                {
                    pieces[i, j].GetComponent<Renderer>().material = pieces[i, j].GetComponent<PuzzlePieceScript>().unconnected;
                }
            }
        }

        for (int i = 0; i < puzzleSize; i++) //Calc from node
        {
            for (int j = 0; j < puzzleSize; j++)
            {
                if (nodes[i,j] != null) //There is a node at i,j
                {
                    if (pieces[i,j] != null) //A piece is connected to this node
                    {
                        pieces[i, j].GetComponent<Renderer>().material = pieces[i, j].GetComponent<PuzzlePieceScript>().connected;

                        //Check up
                        if (j + 1 < puzzleSize) //Makes sure that we are not out of the bounds of the array
                        {
                            if (pieces[i, j + 1] != null) //if there is a piece k units above
                            {
                                //pieces[i, j + 1] is connected
                                pieces[i, j + 1].GetComponent<Renderer>().material = pieces[i, j + 1].GetComponent<PuzzlePieceScript>().connected;

                                CalcConnectivityPiece(i, j, i, j+1);
                            }
                        }
                        //Check down
                        if (j - 1 >= 0) //Makes sure that we are not out of the bounds of the array
                        {
                            if (pieces[i, j - 1] != null) //if there is a piece k units below
                            {
                                //pieces[i, j - 1] is connected
                                pieces[i, j - 1].GetComponent<Renderer>().material = pieces[i, j - 1].GetComponent<PuzzlePieceScript>().connected;

                                CalcConnectivityPiece(i, j, i, j - 1);
                            }
                        }

                        //Check left
                        if (i - 1 >= 0) //Makes sure that we are not out of the bounds of the array
                        {
                            if (pieces[i - 1, j] != null) //if there is a piece k units left
                            {
                                //pieces[i - 1, j] is connected
                                pieces[i - 1, j].GetComponent<Renderer>().material = pieces[i - 1, j].GetComponent<PuzzlePieceScript>().connected;

                                CalcConnectivityPiece(i, j, i - 1, j);
                            }
                        }

                        //Check right
                        if (i + 1 < puzzleSize) //Makes sure that we are not out of the bounds of the array
                        {
                            if (pieces[i + 1, j] != null) //if there is a piece k units left
                            {
                                //pieces[i + 1, j] is connected
                                pieces[i + 1, j].GetComponent<Renderer>().material = pieces[i + 1, j].GetComponent<PuzzlePieceScript>().connected;

                                CalcConnectivityPiece(i, j, i + 1, j);
                            }
                        }
                    }
                }
            }
        }
    }


    void CalcConnectivityPiece(int pI, int pJ, int i, int j) //pI is previous i, pJ is previous j, i and j are coordinates of this piece USED ONLY FOR VISUALS
    {
        if (n < numberOfPieces) //Makes sure that infinte loops don't occur
        {
            n++;

            //Check up
            if (j + 1 < puzzleSize && pJ != j + 1) //Makes sure that we are not out of the bounds of the array and that its not checking the previous piece
            {
                if (pieces[i, j + 1] != null) //if there is a piece k units above
                {
                    //pieces[i, j + 1] is connected
                    pieces[i, j + 1].GetComponent<Renderer>().material = pieces[i, j + 1].GetComponent<PuzzlePieceScript>().connected;

                    CalcConnectivityPiece(i, j, i, j + 1);
                }
            }
            //Check down
            if (j - 1 >= 0 && pJ != j - 1) //Makes sure that we are not out of the bounds of the array and that its not checking the previous piece
            {
                if (pieces[i, j - 1] != null) //if there is a piece k units below
                {
                    //pieces[i, j - 1] is connected
                    pieces[i, j - 1].GetComponent<Renderer>().material = pieces[i, j - 1].GetComponent<PuzzlePieceScript>().connected;

                    CalcConnectivityPiece(i, j, i, j - 1);
                }
            }

            //Check left
            if (i - 1 >= 0 && pI != i - 1) //Makes sure that we are not out of the bounds of the array and that its not checking the previous piece
            {
                if (pieces[i - 1, j] != null) //if there is a piece k units left
                {
                    //pieces[i + 1, j] is connected
                    pieces[i - 1, j].GetComponent<Renderer>().material = pieces[i - 1, j].GetComponent<PuzzlePieceScript>().connected;

                    CalcConnectivityPiece(i, j, i - 1, j);
                }
            }

            //Check right
            if (i + 1 < puzzleSize && pI != i + 1) //Makes sure that we are not out of the bounds of the array and that its not checking the previous piece
            {
                if (pieces[i + 1, j] != null) //if there is a piece k units left
                {
                    //pieces[i + 1, j] is connected
                    pieces[i + 1, j].GetComponent<Renderer>().material = pieces[i + 1, j].GetComponent<PuzzlePieceScript>().connected;

                    CalcConnectivityPiece(i, j, i + 1, j);
                }
            }
        }
    }


    void calcWin() //Like calcConnectivityFromNode exept this sets ni and nj to the position of the first node, and starts the checking cycle from the piece at the next node CALCULATES ACTUAL WIN CONDITION
    {
        bool firstNode = true;

        ni = -1;
        nj = -1;

        for (int i = 0; i < puzzleSize; i++)
        {
            for (int j = 0; j < puzzleSize; j++)
            {
                if (nodes[i,j] != null)
                {
                    if (pieces[i,j] != null)
                    {
                        if (firstNode)
                        {
                            ni = i; //Store position of first node
                            nj = j;
                            firstNode = false;
                            //Debug.Log(ni + "ni nj" + nj);
                        }
                        else if (firstNode == false) //Starts doing the checking, originating at the second node
                        {
                            //Debug.Log(i + "i j" + j);
                            //Check up
                            if (j + 1 < puzzleSize) //Makes sure that we are not out of the bounds of the array
                            {
                                if (pieces[i, j + 1] != null) //if there is a piece k units above
                                {
                                    //pieces[i, j + 1] is connected
                                    winCalcPiece(i, j, i, j + 1);
                                }
                            }
                            //Check down
                            if (j - 1 >= 0) //Makes sure that we are not out of the bounds of the array
                            {
                                if (pieces[i, j - 1] != null) //if there is a piece k units below
                                {
                                    //pieces[i, j - 1] is connected
                                    winCalcPiece(i, j, i, j - 1);
                                }
                            }

                            //Check left
                            if (i - 1 >= 0) //Makes sure that we are not out of the bounds of the array
                            {
                                if (pieces[i - 1, j] != null) //if there is a piece k units left
                                {
                                    //pieces[i - 1, j] is connected
                                    winCalcPiece(i, j, i - 1, j);
                                }
                            }

                            //Check right
                            if (i + 1 < puzzleSize) //Makes sure that we are not out of the bounds of the array
                            {
                                if (pieces[i + 1, j] != null) //if there is a piece k units left
                                {
                                    //pieces[i + 1, j] is connected
                                    winCalcPiece(i, j, i + 1, j);
                                }
                            }
                        }
                    }
                }
            }
        }
    }


    void winCalcPiece(int pI, int pJ, int i, int j) //Like calcConnectivityPiece ecxept like calcWin it starts at one position and doesn't affect the visuals CALCULATES ACTUAL WIN CONDITION
    {
        if (i == ni && j == nj) //This piece is on the first win node
        {
            IEnumerator winCo = WinCo();

            StartCoroutine(winCo);
        }

        if (n2 < numberOfPieces) //Makes sure that infinte loops don't occur
        {
            n2++;

            //Check up
            if (j + 1 < puzzleSize && pJ != j + 1) //Makes sure that we are not out of the bounds of the array and that its not checking the previous piece
            {
                if (pieces[i, j + 1] != null) //if there is a piece k units above
                {
                    //pieces[i, j + 1] is connected
                    winCalcPiece(i, j, i, j + 1);
                }
            }
            //Check down
            if (j - 1 >= 0 && pJ != j - 1) //Makes sure that we are not out of the bounds of the array and that its not checking the previous piece
            {
                if (pieces[i, j - 1] != null) //if there is a piece k units below
                {
                    //pieces[i, j - 1] is connected
                    winCalcPiece(i, j, i, j - 1);
                }
            }

            //Check left
            if (i - 1 >= 0 && pI != i - 1) //Makes sure that we are not out of the bounds of the array and that its not checking the previous piece
            {
                if (pieces[i - 1, j] != null) //if there is a piece k units left
                {
                    //pieces[i - 1, j] is connected
                    winCalcPiece(i, j, i - 1, j);
                }
            }

            //Check right
            if (i + 1 < puzzleSize && pI != i + 1) //Makes sure that we are not out of the bounds of the array and that its not checking the previous piece
            {
                if (pieces[i + 1, j] != null) //if there is a piece k units left
                {
                    //pieces[i + 1, j] is connected
                    winCalcPiece(i, j, i + 1, j);
                }
            }
        }
    }


    void Restart() //Restart the puzzle, return all pieces to original positions
    {
        for (int i = 0; i < puzzleSize; i++)
        {
            for (int j = 0; j < puzzleSize; j++)
            {
                pieces[i, j] = null; //Clear the pieces array
            }
        }

        for (int i = 0; i < numberOfPieces; i++)
        {
            Transform child = piecesParent.transform.GetChild(i);
            PuzzlePieceScript puzzlePieceScript = child.GetComponent<PuzzlePieceScript>();
            puzzlePieceScript.position = startingPiecePos[i]; //Reset internal position (per object)

            pieces[(int)puzzlePieceScript.position.x, (int)puzzlePieceScript.position.y] = child.gameObject; //Reset the position in the pieces[,] array

            child.transform.position = startingPieceVector3[i]; //Reset visual position
        }
    }


    IEnumerator WinCo() //Run when the player completes the puzzle
    {
        lockPuzzle = true;

        yield return new WaitForSeconds(.5f); //Slight delay for pieces to finish moving and to give the player a pause after completing

        if (opensDoor)
        {
            doorScript.triggerTouched = true;
        }

        lockPuzzle = false;

        QuitPuzzle();
    }
}