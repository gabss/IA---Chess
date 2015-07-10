using System.Collections.Generic;
namespace AlphaBeta{
	/// <summary>
	/// Interface for nodes (utilized from the alphabeta algorithm)
	/// </summary>
	public interface INode<T>{

		///<summary>
		/// <returns> The utility of the node</returns>
		/// </summary>
		int Utility();
	
		///<summary>
		/// <returns> The father of the node</returns>
		/// </summary>
		INode<T> Father();

		///<summary>
		/// <returns> A list of the childrens of the node</returns>
		/// </summary>
		List<INode<T>> Childrens();

		///<summary>
		/// <returns> The data (of type T) in the node </returns>
		/// </summary>
		T GetData();
	}
}
