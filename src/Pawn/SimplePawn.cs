using System;
using System.Linq;
using System.Collections.Generic;
using IAChess.BoardUtil;
using IAChess.Const;
using UnityEngine;
namespace IAChess.Pawn
{
	public class SimplePawn : IPawn{

		private Board _board;
		private int[] _position;
		private int _team;
		private int[] _initialPosition;

		public SimplePawn (int x, int y, int team){
			_position = new int[]{x,y};
			_initialPosition = new int[]{x,y};
			_team = team;
		}

		public int GetTeam(){
			return _team;
		}

		public Board GetBoard(){
			return _board;
		}
		public void SetBoard(Board board){
			_board = board;
		}
		public int[] GetPosition(){
			return _position;
		}
		public void SetPosition(int x, int y){
			_position[0] = x;
			_position[1] = y;
		}
		public bool CanMoveTo(int x, int y){
			bool moved = !(_initialPosition.SequenceEqual(_position));
			int absY = Math.Abs(y - _position[1]);
			int absX = Math.Abs(x - _position[0]);

			#region FalseCondition
			if(IAChess.Const.Types.OutOfBord(x,y)){
				//Fuori dalla scacchiera non ci si può muovere!
				return false;
			}

			if(x == _position[0] && y == _position[1]){
				//Debug.Log ("Non puoi muovere il pedone nella stessa casella dov'è");
				return false;
			}
			if( x*_team> _position[0]*_team ){
				//Debug.Log ("Il pedone non può tornare indietro");
				return false;
			}
			if( (moved && absX > 1) || (!moved && absX > 2)){
				//Debug.Log ("Il pedone può muoversi al massimo di una casella in avanti, ad eccezione del primo turno");
				return false;
			}
			if(absX == 2 && _board.GetPawn(x + _team, y) != null){
				//Debug.Log ("Non puoi saltare una pedina!");
				return false;
			}
			if(x == _position[0] && y != _position[1]){
				//Debug.Log ("Il pedone non può muoversi in orizzontale");
				return false;
			}
			if(absY > 1){
				//Debug.Log ("Il pedone può spostarsi orizzontalmente al max di 1, quando mangia");
				return false;
			}
			IPawn psbPawn = _board.GetPawn(x,y);
			
			if(psbPawn != null && (absY != 1 || psbPawn.GetTeam() == _team)){
				//Debug.Log ("Il pedone può mangiare solo se si muove in diagonale di una casella");
				return false;
			}
			if((psbPawn == null && !CanEnpasse(x,y)) && absY != 0){
				//Debug.Log ("Il pedone può spostarsi in diagonale solo se mangia!");
				return false;
			}

			#endregion
			
			return true;
		}

		private bool CanEnpasse(int x, int y){
			if(Math.Abs(_position[1] - y) == 1){
				x = x + _team;
				if(!IAChess.Const.Types.OutOfBord(x,y)){
					IPawn cPawn = _board.GetPawn(x,y);
					Action lastAction = _board.GetLastAction();
					if(cPawn!= null && lastAction != null && cPawn == lastAction.pawn && cPawn.GetTeam() != _team){
						if(Math.Abs(lastAction.position[0] - lastAction.oldPosition[0]) == 2){
							return true;
						}
					}
				}
			}
			return false;
		}

		public bool CanEatIn(int x, int y){
			int absY = Math.Abs(y - _position[1]);
			int absX = Math.Abs(x - _position[0]);
			
			if((x*_team < _position[0]*_team ) && absY == 1 && absX == 1){
				return true;
			}


			return false;
		}

		public void Move(int[] pos){
			if(CanMoveTo(pos[0], pos[1])){
				_board.ApplyAction(GenerateAction(pos[0], pos[1]));
			}
		}

		public List<Action> GetPossibleMove(){
			List<Action> fields = new List<Action>();
			int row = _position[0] - _team;
			for(int i = -1; i<2; i++){
				int j = _position[1]+i;
				if(!IAChess.Const.Types.OutOfBord(row,j)){
					if(CanMoveTo(row, j)){
						fields.Add(GenerateAction(row,j));
					}
				}
			}
			if(_initialPosition.SequenceEqual(_position)){
				int j = row - _team;
				if(CanMoveTo(j, _position[1])){
					fields.Add(GenerateAction(j, _position[1]));
				}
			}
			return fields;
		}

		private Action GenerateAction(int x, int y){
			Action act;
			if(CanEatIn(x, y)){
				if(CanEnpasse(x,y)){
					act = new Action(this, x,y, _position[0], _position[1], _board.GetPawn(x+_team, y), x+_team,y);
				}else{
					act = new Action(this, x,y, _position[0], _position[1], _board.GetPawn(x, y), x,y);
				}
			}else{
				act = new Action(this, x,y, _position[0], _position[1]);
			}
			return act;
		}

		public int CompareTo(object obj){
			IPawn pwn = (IPawn) obj;

			if(pwn == null) return -1;
			if(obj.GetType() == typeof(King)) return 1;

			return _team * _position[0].CompareTo(pwn.GetPosition()[0]);

		}
	}
}

