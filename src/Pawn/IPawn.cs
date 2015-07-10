using System;
using System.Collections.Generic;
using IAChess.BoardUtil;
namespace IAChess.Pawn{

	/// <summary>
	/// Interface for a generic Pawn
	/// </summary>
	public interface IPawn : IComparable{

		int GetTeam();
		Board GetBoard();
		void SetBoard(Board board);
		int[] GetPosition();
		void SetPosition(int x, int y);
		bool CanMoveTo(int x,int y);
		bool CanEatIn(int x, int y);
		void Move(int[] pos);
		List<Action> GetPossibleMove();

	}
}

