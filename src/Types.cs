using System;
namespace IAChess.Const
{
	/// <summary>
	/// The class contain some static util and information about the chess world
	/// </summary>
	public static class Types{

		//The size of the square field
		public const int BOARD_SIZE = 8;

		/// <summary>
		/// Util function for check if a position is out of board
		/// </summary>
		/// <returns><c>true</c>, if the point (x,y) is out of the board, <c>false</c> otherwise.</returns>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		public static bool OutOfBord(int x, int y){
			if(x < 0 || x > 7 || y < 0 || y > 7) return true;
			return false;
		}
	}


}

