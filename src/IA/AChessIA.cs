using System;
using System.Collections.Generic;
using IAChess.BoardUtil;
using IAChess.Pawn;
namespace IAChess.IA{

	/// <summary>
	/// Abstract class for IA
	/// </summary>
	public abstract class AChessIA{


		protected Board _board;
		protected static int _dif;

		public static AChessIA GetIA(Board board, int dif, int team = 0){
			AChessIA _ia;
			_ia =  new ExamIA();
			_ia._board = board;
			_dif = dif;
			_ia.Initialize();
			return _ia;
		}

		//Functions to implement

		/// <summary>
		/// Initialize the IA
		/// </summary>
		protected abstract void Initialize();

		/// <summary>
		/// Return the nex action to do
		/// </summary>
		/// <returns>The choosen move</returns>
		public abstract Action ChooseMove();

	}
}

