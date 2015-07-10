using System;
using IAChess.Pawn;
namespace IAChess{
	/// <summary>
	/// Represents a chess move.
	/// </summary>
	public class Action{

		public IPawn pawn, eatedPawn;
		public int[] oldPosition = new int[2], position = new int[2], eatedPosition;

		public Action (IPawn pawn, int newPosX, int newPosY, int oldPositionX, int oldPositionY){
			this.pawn = pawn;
			this.position[0] = newPosX; this.position[1] = newPosY;
			this.oldPosition[0] = oldPositionX; this.oldPosition[1] = oldPositionY;
		}

		public Action (IPawn pawn, int newPosX, int newPosY, int oldPositionX, int oldPositionY, IPawn eatedPawn, int eatedPositionX, int eatedPositionY){
			this.pawn = pawn;
			this.position[0] = newPosX; this.position[1] = newPosY;
			this.oldPosition[0] = oldPositionX; this.oldPosition[1] = oldPositionY;
			this.eatedPawn = eatedPawn;
			this.eatedPosition = new int[]{eatedPositionX, eatedPositionY};
		}
	}
}

