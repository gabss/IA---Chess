using System;
namespace AlphaBeta{
	/// <summary>
	/// *DEPRECATED*
	/// This class compute the alphabeta algorithm on a passed node (that is the root of a tree).
	/// The generic type T, identfy the type of the value that a node has.
	/// </summary>
	public class AlphaBeta<T>{

		public AlphaBeta(){}

		public INode<T> AlphaBetaSearch(INode<T> node){
			return MaxValue(node, int.MinValue, int.MaxValue);
		}

		public INode<T> MaxValue(INode<T> node, int alpha, int beta){
			if(TerminalState(node)){
				return node;
			}
			INode<T> result = null;
			int v = int.MinValue;
			foreach(INode<T> child in node.Childrens()){
				result = Max (result, MinValue(child, alpha, beta));
				v = result.Utility();
				if(v >= beta){
					return result;
				}
				alpha = Math.Max(alpha, v);
			}
			return result;
		}

		public INode<T> MinValue(INode<T> node, int alpha, int beta){
			if(TerminalState(node)){
				return node;
			}

			INode<T> result = null;
			int v = int.MaxValue;
			foreach(INode<T> child in node.Childrens()){
				result = Min (result, MaxValue(child, alpha, beta));
				v = result.Utility();
				if(v <= alpha){
					return result;
				}
				beta = Math.Min(beta, v);
			}
			return result;
		}

		public bool TerminalState(INode<T> node){
			if(node.Childrens().Count <= 0)
				return true;
			return false;
		}

		private INode<T> Max(INode<T> a, INode<T> b){
			if(a==null) return b;
			if(b==null) return a;
			return (a.Utility()>=b.Utility())?a:b;
		}

		private INode<T> Min(INode<T> a, INode<T> b){
			if(a==null) return b;
			if(b==null) return a;
			return (a.Utility()<=b.Utility())?a:b;
		}
		

	}
}

