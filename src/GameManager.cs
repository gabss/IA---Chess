using UnityEngine;
using System.Collections;
using IAChess;
using IAChess.Const;
using IAChess.BoardUtil;
using IAChess.IA;
using IAChess.Pawn;

/// <summary>
/// The class manages all the game. It creates a link between unity object and the classes belonging to 'chess'
/// </summary>
public class GameManager : MonoBehaviour {
	
	//Identfy the position in the array of gameobject (filled in unity) of all the differents pawns
	public enum PawnIndex{
		King = 0,
		SimplePawn = 1
	}
	//Reference to the main camera, used for identify the position of the mouse click
	public Camera mainCamera;

	//Reference to GUI elements
	public GameObject gui;
	public GameObject winNotice;
	public GameObject loseNotice;
	/***************************/

	//Reference all of the pawns in the white and black team (GameObject)
	public GameObject[] White;
	public GameObject[] Black;
	/*********************************/

	//Graphic to show, when selecting a pawn, on the possible move square
	public GameObject possibeMoveHover;
	//The position of the (0,0) square
	public Vector3 startingPosition = new Vector3(-3.5f,3.5f,0);
	//Graphic to show, when selecting a pawn, on the selected pawn
	public Transform selectedHover;



	private Vector3 initialSelectedHoverPosition;
	private ArrayList possibleMoveHoverInstances = new ArrayList();
	private IPawn[][] pawns = new IPawn[IAChess.Const.Types.BOARD_SIZE][];
	private GameObject[][] pawnsGO = new GameObject[IAChess.Const.Types.BOARD_SIZE][];
	private bool turn = false, wait;
	private Action computerAction;
	private int evaluatedAction = 0;
	private Board board = new Board();
	private IPawn selectedPawn;
	private AChessIA _ia;
	private Director _manager;

	/// <summary>
	/// Initialize all the components of the game. 
	/// </summary>
	void Start(){
		_manager = GameObject.FindGameObjectWithTag("GameController").GetComponent<Director>();;
		initialSelectedHoverPosition = selectedHover.localPosition;
		for(int i=0; i<IAChess.Const.Types.BOARD_SIZE; i++){
			pawns[i] = new IPawn[IAChess.Const.Types.BOARD_SIZE];
			pawnsGO[i] = new GameObject[IAChess.Const.Types.BOARD_SIZE];
		}
		CreateEnemyTeam((_manager.team == "White")?Black:White);
		CreatePlayerTeam((_manager.team == "White")?White:Black);
		if(_manager.team == "Black") turn = true;
		board.SetPawns(pawns);
		_ia = AChessIA.GetIA(board, _manager.difficulty);
	}

	/// <summary>
	/// At each update, we check, if all the past actions are evaluated, whose turn is it and delegate the move to
	/// the relative manage function (PlayerTurn or ComputerTurn.
	/// </summary>
	void Update(){
		if(board.GetActionsCount() - evaluatedAction == 0){
			if(!turn){
				PlayerTurn();
			}else{
				ComputerTurn();
			}
		}
		Repaint();
	}

	/// <summary>
	/// Check if there's still an action to evaluate (in graphical sense). If  there is one, than repaint the 
	/// chess board.
	/// </summary>
	void Repaint(){
		if(board.GetActionsCount() - evaluatedAction == 1){

			Action lastAction = board.GetLastAction();
			if(lastAction.eatedPosition != null){
				GameObject.Destroy(pawnsGO[lastAction.eatedPosition[0]][lastAction.eatedPosition[1]]);
			}
			pawnsGO[lastAction.oldPosition[0]][lastAction.oldPosition[1]].transform.position = new Vector3(startingPosition.x + lastAction.position[1], startingPosition.y - lastAction.position[0]);
			pawnsGO[lastAction.position[0]][lastAction.position[1]] = pawnsGO[lastAction.oldPosition[0]][lastAction.oldPosition[1]];
			evaluatedAction++;
			if(board.TerminalState(lastAction)){
				gui.SetActive(true);
				if(lastAction.pawn.GetTeam() == 1){
					winNotice.SetActive(true);
				}else{
					loseNotice.SetActive(true);
				}
			}
		}
	}

	/// <summary>
	/// Manages the player turn and then the selection of the pawn and the computation of the possible move
	/// </summary>
	void PlayerTurn(){
		if(Input.GetMouseButtonDown(0)){
			int[] pos = LocateMouseClick();
			if(!IAChess.Const.Types.OutOfBord(pos[0], pos[1])){
				if(selectedPawn == null){
					selectedPawn = board.GetPawn(pos[0], pos[1]);
					if(selectedPawn != null && selectedPawn.GetTeam() == -1){
						selectedPawn = null;
					}
					if(selectedPawn != null){
						CheckIfNeedToMoveKing();
						selectedHover.localPosition = new Vector3(-3.5f+selectedPawn.GetPosition()[1],3.5f-selectedPawn.GetPosition()[0],0);
						foreach(Action cAction in selectedPawn.GetPossibleMove()){
							possibleMoveHoverInstances.Add((GameObject)GameObject.Instantiate(possibeMoveHover, new Vector3(-3.5f+cAction.position[1],3.5f-cAction.position[0],0), Quaternion.identity));
						}
					}
				}else{
					if(selectedPawn.CanMoveTo(pos[0], pos[1])){
						selectedPawn.Move(pos);
						turn = true;
						selectedPawn = null;
					}
					selectedPawn = null;
					selectedHover.localPosition = initialSelectedHoverPosition;
					foreach(object obj in possibleMoveHoverInstances){
						GameObject.Destroy((GameObject)obj);
					}
					possibleMoveHoverInstances.Clear();
				}
			}
		}
	}
	/// <summary>
	/// Manages the computer turn. It's responsible to contact the IA for ask for the next move.
	/// </summary>
	void ComputerTurn(){
		Action act = _ia.ChooseMove();
		act.pawn.Move(act.position);
		turn = false;
	}

	/// <summary>
	/// Util function that check if the player must move the king (because it's under check)
	/// </summary>
	void CheckIfNeedToMoveKing(){
		int[] kingPos = board.PlayerKing.GetPosition();
		foreach(IPawn cPawn in board.GetTeam(-1)){
			if(cPawn.CanEatIn(kingPos[0], kingPos[1])){
				bool found = false;
				foreach(IPawn pwn in board.GetTeam(1)){
					if(pwn.CanEatIn(cPawn.GetPosition()[0], cPawn.GetPosition()[1]) && selectedPawn == pwn){
						found = true;
						break;
					}
				}
				if(!found)selectedPawn = board.PlayerKing;
				break;
			}
		}
	}

	/// <summary>
	/// Locates the mouse click and transform the position in a bidimensional array of two integer values that
	/// gives the coordinates x,y of the clicked square
	/// </summary>
	/// <returns>The mouse click.</returns>
	int[] LocateMouseClick(){
		Vector2 clickPosition = (Vector2)transform.TransformPoint(mainCamera.ScreenToWorldPoint(Input.mousePosition));
		int[] result = new int[2];
		result[0] = Mathf.Abs((int)(clickPosition.y - 4));
		result[1] = (int)(clickPosition.x + 4);
		return result;
	}

	/// <summary>
	/// Initialization function the creates the computer team
	/// </summary>
	/// <param name="team">Team. The list of gameobjects that represents the computer team</param>
	void CreateEnemyTeam(GameObject[] team){
		int i = 1;
		GameObject pawn;
		Vector3 cPosition;
		for (int j = 0; j<IAChess.Const.Types.BOARD_SIZE; j++){
			cPosition = new Vector3(startingPosition.x + j, startingPosition.y - i,0);
			pawn = (GameObject)GameObject.Instantiate(team[(int)PawnIndex.SimplePawn], cPosition, Quaternion.identity);
			pawn.transform.parent = transform;
			pawnsGO[i][j] = pawn;
			pawns[i][j] = new SimplePawn(i,j, -1);
		}

		i = 0;
		int column = (_manager.team == "White")?4:3;
		cPosition = new Vector3(startingPosition.x + column, startingPosition.y - i,0);
		pawn = (GameObject)GameObject.Instantiate(team[(int)PawnIndex.King], cPosition, Quaternion.identity);
		pawn.transform.parent = transform;
		pawnsGO[i][column] = pawn;
		pawns[i][column] = new King(i,column, -1);


	}

	/// <summary>
	/// Initialization function the creates the player team
	/// </summary>
	/// <param name="team">Team. The list of gameobjects that represents the player team</param>
	void CreatePlayerTeam(GameObject[] team){
		int i = 6;
		GameObject pawn;
		Vector3 cPosition;
		for (int j = 0; j<IAChess.Const.Types.BOARD_SIZE; j++){
			cPosition = new Vector3(startingPosition.x + j, startingPosition.y - i,0);
			pawn = (GameObject)GameObject.Instantiate(team[(int)PawnIndex.SimplePawn], cPosition, Quaternion.identity);
			pawn.transform.parent = transform;
			pawnsGO[i][j] = pawn;
			pawns[i][j] = new SimplePawn(i,j, 1);
		}
		i = 7;
		int column = (_manager.team == "White")?4:3;
		cPosition = new Vector3(startingPosition.x + column, startingPosition.y - i,0);
		pawn = (GameObject)GameObject.Instantiate(team[(int)PawnIndex.King], cPosition, Quaternion.identity);
		pawn.transform.parent = transform;
		pawnsGO[i][column] = pawn;
		pawns[i][column] = new King(i,column, 1);

	}

	/// <summary>
	/// Reloads the game.
	/// </summary>
	public void Reload(){
		gui.SetActive(false);
		winNotice.SetActive(false);
		loseNotice.SetActive(false);
		Application.LoadLevel("init");
	}

}
