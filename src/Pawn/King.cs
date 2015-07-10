using System;
using System.Collections.Generic;
using IAChess.BoardUtil;
using IAChess.Const;
namespace IAChess.Pawn
{
	public class King : IPawn{
		
		private Board _board;
		private int[] _position;
		private int _team;
		
		public King (int x, int y, int team){
			_position = new int[]{x,y};
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
			int absY = Math.Abs(y - _position[1]);
			int absX = Math.Abs(x - _position[0]);
			
			#region FalseCondition
			if(Types.OutOfBord(x,y)){
				//Fuori dalla scacchiera non ci si può muovere!
				return false;
			}
			if(x == _position[0] && y == _position[1]){
				//print ("Non puoi muovere il re nella stessa casella dov'è");
				return false;
			}
			if(absX > 1 || absY > 1){
				//print ("Il re può muoversi al massimo di una casella");
				return false;
			}
			IPawn cPawn = _board.GetPawn(x,y);
			if(cPawn != null && cPawn.GetTeam() == _team){
				//print ("Il re può mangiare solo pedoni del team avversario");
				return false;
			}

			List<IPawn> cEnemies = _board.GetTeam(_team*-1);
			foreach (IPawn cEnemy in cEnemies){
				if(cEnemy.CanEatIn(x,y)){
				//	print ("Il re non può muoversi in una casella che può essere 'mangiata' da un pedone nemico");
					return false;
				}
			}

			#endregion
			
			return true;
		}

		
		public bool CanEatIn(int x, int y){
			int absY = Math.Abs(y - _position[1]);
			int absX = Math.Abs(x - _position[0]);
			
			if(absY <= 1 && absX <= 1 && absX+absY != 0 ){
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
			int x = _position[0];
			int y = _position[1];
			for (int i = x-1; i < x+2; i++){
				for(int j = y-1; j< y+2; j++){
					if(i < Types.BOARD_SIZE && i >= 0 && j < Types.BOARD_SIZE && j>= 0){
						if(CanMoveTo(i,j)){
							fields.Add(GenerateAction(i,j));
						}
					}
				}
			}
			return fields;
		}

		private Action GenerateAction(int x, int y){
			Action act;
			IPawn pawn = _board.GetPawn(x,y);
			if(CanEatIn(x,y) && pawn != null){
				act = new Action(this, x,y, _position[0], _position[1], pawn, x,y);
			}else{
				act = new Action(this, x,y, _position[0], _position[1]);
			}

			return act;
		}

		public int CompareTo(object obj){
			return -1;
		}
	}
}

