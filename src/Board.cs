using System;
using System.Collections.Generic;
using IAChess.Pawn;
using IAChess.Const;
using UnityEngine;
namespace IAChess.BoardUtil{
	/// <summary>
	/// It represents the chess board. Have reference to all the pawns of the two teams and their positions.
	/// Mantains the stack of all the move.
	/// </summary>
	public class Board{

		private IPawn[][] _board;
		private Stack<Action> _actions = new Stack<Action>();
		private List<IPawn>[] _teams = new List<IPawn>[2];
		public King EnemyKing;
		public King PlayerKing;


		public Board (){}

		//Fill their reference
		public void SetPawns(IPawn[][] board){
			_board = board;
			_teams[0] = new List<IPawn>();
			_teams[1] = new List<IPawn>();

			int i;
			/* Computer kings*/
			i=0;

			foreach(IPawn cPawn in _board[i]){
				if(cPawn != null){
					EnemyKing = (King)cPawn;
					_teams[0].Add(cPawn);
					cPawn.SetBoard(this);
				}
			}
			/*Computer pawns*/
			i=1;
			foreach(IPawn cPawn in _board[i]){
				if(cPawn != null){
					_teams[0].Add(cPawn);
					cPawn.SetBoard(this);
				}
			}
			/*Player King*/
			i=7;
			foreach(IPawn cPawn in _board[i]){
				if(cPawn != null){
					PlayerKing = (King)cPawn;
					_teams[1].Add(cPawn);
					cPawn.SetBoard(this);
				}
			}
			/*Player pawns*/
			i=6;
			foreach(IPawn cPawn in _board[i]){
				if(cPawn != null){
					_teams[1].Add(cPawn);
					cPawn.SetBoard(this);
				}
			}


		}
		/// <summary>
		/// Returns the pawn in the passed coordinates
		/// </summary>
		/// <returns>The pawn.</returns>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		public IPawn GetPawn(int x, int y){
			return _board[x][y];
		}

		/// <summary>
		/// Gets the team.
		/// </summary>
		/// <returns>The list of pawn of the requested teams.</returns>
		/// <param name="team">Team.</param>
		public List<IPawn> GetTeam(int team){
			int index = (team == 1)?1:0;
			return _teams[index];
		}

		/// <summary>
		/// Applies the action to the board.
		/// </summary>
		/// <param name="act">Act. The action to apply</param>
		public void ApplyAction(Action act){
			_board[act.oldPosition[0]][act.oldPosition[1]] = null;
			_board[act.position[0]][act.position[1]] = act.pawn;
			act.pawn.SetPosition(act.position[0], act.position[1]);
			if(act.eatedPosition != null){
				if(act.eatedPosition[0] != act.position[0] || act.eatedPosition[1] != act.position[1])
					_board[act.eatedPosition[0]][act.eatedPosition[1]] = null;
				act.eatedPawn.SetPosition(-10,-10);
				_teams[(act.eatedPawn.GetTeam() == 1)?1:0].Remove(act.eatedPawn);
			}
			_actions.Push(act);
		}

		/// <summary>
		/// Reverts the action.
		/// </summary>
		/// <param name="act">Act. The action to revert</param>
		public void RevertAction(Action act){
			_board[act.oldPosition[0]][act.oldPosition[1]] = act.pawn;
			_board[act.position[0]][act.position[1]] = null;
			act.pawn.SetPosition(act.oldPosition[0], act.oldPosition[1]);
			if(act.eatedPosition != null){
				_board[act.eatedPosition[0]][act.eatedPosition[1]] = act.eatedPawn;
				act.eatedPawn.SetPosition(act.eatedPosition[0], act.eatedPosition[1]);
				_teams[(act.eatedPawn.GetTeam() == 1)?1:0].Add(act.eatedPawn);
			}
			_actions.Pop();
		}

		/// <summary>
		/// Gets the last action.
		/// </summary>
		/// <returns>The last action.</returns>
		public Action GetLastAction(){
			if(_actions.Count <= 0) return null;
			return _actions.Peek();
		}

		/// <summary>
		/// Gets the actions count.
		/// </summary>
		/// <returns>The actions count.</returns>
		public int GetActionsCount(){
			return _actions.Count;
		}

		/// <summary>
		/// Test if the passed action lead to a terminal test
		/// </summary>
		/// <returns><c>true</c>, if state will be terminal, <c>false</c> otherwise.</returns>
		/// <param name="act">Act. The action</param>
		public bool TerminalState(Action act){
			if(act.pawn.GetType() == typeof(SimplePawn)){
				if(act.pawn.GetTeam() == -1 && act.position[0] == 7){
					return true;
				}
				if(act.pawn.GetTeam() == 1 && act.position[0] == 0){
					return true;
				}
			}
			if( IAChess.Const.Types.OutOfBord(EnemyKing.GetPosition()[0], EnemyKing.GetPosition()[1]))
				return true;
			return false;
		}
	}
	
}

