using System;
using System.Collections.Generic;
using AlphaBeta;

namespace IAChess.IA
{
	/// <summary>
	/// Implementation of INode, with Action
	/// </summary>
	public class ChessNode : INode<Action>{

		private Action _action;
		private int _util;
		private INode<Action> _father;
		private List<INode<Action>> _childrens = new List<INode<Action>>();


		public ChessNode(){}

		public ChessNode(INode<Action> father, Action action){
			_action = action;
			_father = father;
			_util = -100000 * action.pawn.GetTeam(); //max
			_father.Childrens().Add(this);
		}
		public int Utility(){
			return _util;
		}
		public void SetUtility(int utl){
			_util = utl;
		}
		public INode<Action> Father(){
			return _father;
		}
		public List<INode<Action>> Childrens(){
			return _childrens;
		}
		public Action GetData(){
			return _action;
		}

	}
}

