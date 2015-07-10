using System;
using System.Collections;
using System.Collections.Generic;
using IAChess.Pawn;
using AlphaBeta;
using UnityEngine;
namespace IAChess.IA{

	/// <summary>
	/// IA for a restricted version of chess (made for exam of "Artificial Intelligence")
	/// </summary>
	public class ExamIA : AChessIA{
		 
		private int[][] simplePawnEvaluate = new int[8][];
		private int[][] kingNormalEvaluate = new int[8][];
		private List<IPawn> computerTeam;
		private List<IPawn> playerTeam;
		private King computerKing;
		private King playerKing;
		private Dictionary<int, List<IPawn>> playerPattern = new Dictionary<int, List<IPawn>>(9);
		private Dictionary<int, List<IPawn>> computerPattern = new Dictionary<int, List<IPawn>>(9);
		private int passedPawnValue = 2;


		private int d = 2;
		private ChessNode oldRoot = new ChessNode();
		private Action cResult;
		private float maxTime = 10;
		private float elapsedTime = 0f;

		/// <summary>
		/// Initialize the IA
		/// </summary>
		protected override void Initialize(){
			maxTime = maxTime * _dif;
			for (int i = 0; i<8; i++){
				playerPattern.Add(i, new List<IPawn>());
				computerPattern.Add(i, new List<IPawn>());
			}

			/******************
			 * Initialize the reference of the teams and of kings (separately).
			 * ****************/
			computerTeam = _board.GetTeam(-1);
			playerTeam = _board.GetTeam(1);
			computerKing = _board.EnemyKing;
			playerKing = _board.PlayerKing;
			/*************************/

			/**************************
			 * Valutation table for the simple pawn. Gives a value for each pawn according to their positions.
			 * ************************/
			simplePawnEvaluate[0] = new int[]{100,100,100,100,100,100,100,100};
			simplePawnEvaluate[1] = new int[]{170,170,170,170,170,170,170,170};
			simplePawnEvaluate[2] = new int[]{150,150,150,150,150,150,150,150};
			simplePawnEvaluate[3] = new int[]{130,130,130,130,130,130,130,130};
			simplePawnEvaluate[4] = new int[]{125,130,130,130,130,130,130,125};
			simplePawnEvaluate[5] = new int[]{110,110,110,110,110,110,110,110};
			simplePawnEvaluate[6] = new int[]{110,110,110,80,80,110,110,110};
			simplePawnEvaluate[7] = new int[]{0,0,0,0,0,0,0,0};
			/**************************/


			/**************************
			 * Valutation table for the king. Gives a value for each pawn according to his positions.
			 * ************************/
			kingNormalEvaluate[0] = new int[]{0,0,0,0,0,0,0,0};
			kingNormalEvaluate[1] = new int[]{30,30,30, 30, 30,30,30,30};
			kingNormalEvaluate[2] = new int[]{40,40,40, 40, 40, 40,40,40};
			kingNormalEvaluate[3] = new int[]{40,40, 60, 60, 60, 60,40,40};
			kingNormalEvaluate[4] = new int[]{40,40, 60, 60, 60, 60,40,40};
			kingNormalEvaluate[5] = new int[]{40,40,40,45, 45, 40,40,40};
			kingNormalEvaluate[6] = new int[]{0,0, 0,0, 0, 0,0,0};
			kingNormalEvaluate[7] = new int[]{-60,-60,-60,-60,-60,-60,-60,-60};
			/*************************/
		}


		public override Action ChooseMove (){
			elapsedTime = Time.realtimeSinceStartup;

			while(true){

				computerTeam.Sort();
				ChessNode act = Search(oldRoot, d);

				if(Time.realtimeSinceStartup - elapsedTime > maxTime){
					break;
				}
				d++;
				cResult = FindNextAction(act);

				oldRoot = new ChessNode();

			}
			oldRoot = new ChessNode();
			d = 2;
			return cResult;
		}


		#region AlphaBeta
		ChessNode Search(ChessNode root, int level){
			return MaxSearch(root, level,int.MinValue, int.MaxValue);
		}

		ChessNode MaxSearch(ChessNode node, int level, int alpha, int beta){
			if(node.GetData() != null){
				if(_board.TerminalState(node.GetData())){
					node.SetUtility(node.Utility() - level);
					return node;
				}
				else if(level == 1){
					node.SetUtility(Util () + level);
					return node;
				}
			}

			if(Time.realtimeSinceStartup - elapsedTime > maxTime){
				return node;
			}

			int v = int.MinValue;
			ChessNode result = null;

			foreach(Action cAction in GetPossibleMove(-1)){
				ChessNode cNode = new ChessNode(node, cAction);
				_board.ApplyAction(cAction);
				result = Max (result, MinSearch(cNode, level-1, alpha, beta));
				_board.RevertAction(cAction);
				if(result != null)
					v = result.Utility();
				alpha = Math.Max(alpha, v);
				if(beta <= alpha){
					return result;
				}
			}
			return result;
		}

		ChessNode MinSearch(ChessNode node, int level, int alpha, int beta){
			if(node.GetData() != null){
				if(_board.TerminalState(node.GetData())){
					node.SetUtility(node.Utility() + level);
					return node;
				}
				else if(level == 1){
					node.SetUtility(Util () - level);
					return node;
				}
			}

			if(Time.realtimeSinceStartup - elapsedTime > maxTime){
				return node;
			}

			int v = int.MaxValue;
			ChessNode result = null;

			foreach(Action cAction in GetPossibleMove(1)){
				ChessNode cNode = new ChessNode(node, cAction);
				_board.ApplyAction(cAction);
				result = Min (result, MaxSearch(cNode, level-1,alpha, beta));
				_board.RevertAction(cAction);
				if(result != null)
					v = result.Utility();
				beta = Math.Min(beta, v);
				if(beta <= alpha){
					return result;
				}
			}

			return result;
		}

		private ChessNode Max(ChessNode a, ChessNode b){
			if(a==null) return b;
			if(b==null) return a;
			return (a.Utility()>=b.Utility())?a:b;
		}
		
		private ChessNode Min(ChessNode a, ChessNode b){
			if(a==null) return b;
			if(b==null) return a;
			return (a.Utility()<=b.Utility())?a:b;
		}
		#endregion


		/// <summary>
		/// Gives all the possible moves of the requested team
		/// </summary>
		/// <returns>The list of all possible actions</returns>
		/// <param name="team">Team to check</param>
		List<Action> GetPossibleMove(int team){
			List<IPawn> cTeam = (team==1)?playerTeam:computerTeam;
			List<Action> result = new List<Action>();
			foreach(IPawn pwn in cTeam){
				result.AddRange(pwn.GetPossibleMove());
			}
			return result;
		}

		/*************************
		 * Utility function
		 ************************/
		private int Util(){
			int pcPoint = 0;
			int playerPoint = 0;

			foreach (IPawn pawn in playerTeam){
				if(pawn.GetType() == typeof(SimplePawn)){
					playerPoint += simplePawnEvaluate[pawn.GetPosition()[0]][pawn.GetPosition()[1]];
				}else{
					playerPoint += kingNormalEvaluate[pawn.GetPosition()[0]][pawn.GetPosition()[1]];
				}
				List<IPawn> pawns;
				if(playerPattern.TryGetValue(pawn.GetPosition()[1], out pawns)){
					pawns.Add(pawn);
				}
			}

			foreach (IPawn pawn in computerTeam){
				if(pawn.GetType() == typeof(SimplePawn)){
					pcPoint += simplePawnEvaluate[7-pawn.GetPosition()[0]][pawn.GetPosition()[1]];
				}else{
					pcPoint += kingNormalEvaluate[7-pawn.GetPosition()[0]][pawn.GetPosition()[1]];
				}
				List<IPawn> pawns;
				if(computerPattern.TryGetValue(pawn.GetPosition()[1], out pawns)){
					pawns.Add(pawn);
				}
			}

			playerPoint += EvaluatePossiblePlayerPatterns();
			pcPoint += EvaluatePossibleComputerPatterns();

			foreach(KeyValuePair<int, List<IPawn>> entry in playerPattern){
				entry.Value.Clear();
			}
			foreach(KeyValuePair<int, List<IPawn>> entry in computerPattern){
				entry.Value.Clear();
			}
			return pcPoint*computerTeam.Count - playerPoint*playerTeam.Count;
		}

		int EvaluatePossiblePlayerPatterns(){
			List<IPawn> pawns;
			int points = 0;
			foreach(IPawn pawn in playerTeam){
				if(pawn.GetType() == typeof(SimplePawn)){

					#region PawnPosition
					if(playerPattern.TryGetValue(pawn.GetPosition()[1]+1, out pawns) && pawns.Count > 0 ){
						foreach(IPawn friendPawn in pawns){
							if(friendPawn.GetType() != typeof(King)){
								if(friendPawn.GetPosition()[0] == pawn.GetPosition()[0]){
									points += 6;
								}else if(friendPawn.GetPosition()[0] == pawn.GetPosition()[0]+1 || friendPawn.GetPosition()[0] == pawn.GetPosition()[0]-1){
									points+=3;
								}
							}
						}
					}
					if(playerPattern.TryGetValue(pawn.GetPosition()[1]-1, out pawns) && pawns.Count > 0 ){
						foreach(IPawn friendPawn in pawns){
							if(friendPawn.GetType() != typeof(King)){
								if(friendPawn.GetPosition()[0] == pawn.GetPosition()[0]){
									points += 6;
								}else if(friendPawn.GetPosition()[0] == pawn.GetPosition()[0]+1 || friendPawn.GetPosition()[0] == pawn.GetPosition()[0]-1){
									points+=3;
								}
							}
						}
					}
					#endregion

					#region PassedPawn (must be the last one)
					bool passed = true;
					if(computerPattern.TryGetValue(pawn.GetPosition()[1], out pawns) && pawns.Count > 0 ){
						foreach(IPawn enemyPawn in pawns){
							if(enemyPawn.GetPosition()[0] < pawn.GetPosition()[0]){
								passed = false;
								break;
							}
						}
					}
					if(!passed) continue;
					if(computerPattern.TryGetValue(pawn.GetPosition()[1]-1, out pawns) && pawns.Count > 0 ){
						foreach(IPawn enemyPawn in pawns){
							if(enemyPawn.GetPosition()[0] < pawn.GetPosition()[0]){
								passed = false;
								break;
							}
						}
					}
					if(!passed) continue;
					if(computerPattern.TryGetValue(pawn.GetPosition()[1]+1, out pawns) && pawns.Count > 0 ){
						foreach(IPawn enemyPawn in pawns){
							if(enemyPawn.GetPosition()[0] < pawn.GetPosition()[0]){
								passed = false;
								break;
							}
						}
					}
					if(!passed) continue;

					//Square rule
					if(Mathf.Abs(computerKing.GetPosition()[1]-pawn.GetPosition()[1]) <= 7-pawn.GetPosition()[0])
						continue;


					points += (int)Mathf.Pow(passedPawnValue, 7-pawn.GetPosition()[0]);
					#endregion
				}
			}

			return points;
		}

		int EvaluatePossibleComputerPatterns(){
			List<IPawn> pawns;
			int points = 0;
			foreach(IPawn pawn in computerTeam){
				if(pawn.GetType() == typeof(SimplePawn)){
					#region PawnPosition
					if(computerPattern.TryGetValue(pawn.GetPosition()[1]+1, out pawns) && pawns.Count > 0 ){
						foreach(IPawn friendPawn in pawns){
							if(friendPawn.GetType() != typeof(King)){
								if(friendPawn.GetPosition()[0] == pawn.GetPosition()[0]){
									points += 6;
								}else if(friendPawn.GetPosition()[0] == pawn.GetPosition()[0]+1 || friendPawn.GetPosition()[0] == pawn.GetPosition()[0]-1){
									points+=3;
								}
							}
						}
					}
					if(computerPattern.TryGetValue(pawn.GetPosition()[1]-1, out pawns) && pawns.Count > 0 ){
						foreach(IPawn friendPawn in pawns){
							if(friendPawn.GetType() != typeof(King)){
								if(friendPawn.GetPosition()[0] == pawn.GetPosition()[0]){
									points += 6;
								}else if(friendPawn.GetPosition()[0] == pawn.GetPosition()[0]+1 || friendPawn.GetPosition()[0] == pawn.GetPosition()[0]-1){
									points+=3;
								}
							}
						}
					}
					#endregion
					#region PassedPawn (must be the last one)
					bool passed = true;
					if(playerPattern.TryGetValue(pawn.GetPosition()[1], out pawns) && pawns.Count > 0 ){
						foreach(IPawn enemyPawn in pawns){
							if(enemyPawn.GetPosition()[0] > pawn.GetPosition()[0]){
								passed = false;
								break;
							}
						}
					}
					if(!passed) continue;
					if(playerPattern.TryGetValue(pawn.GetPosition()[1]-1, out pawns) && pawns.Count > 0 ){
						foreach(IPawn enemyPawn in pawns){
							if(enemyPawn.GetPosition()[0] > pawn.GetPosition()[0]){
								passed = false;
								break;
							}
						}
					}
					if(!passed) continue;
					if(playerPattern.TryGetValue(pawn.GetPosition()[1]+1, out pawns) && pawns.Count > 0 ){
						foreach(IPawn enemyPawn in pawns){
							if(enemyPawn.GetPosition()[0] > pawn.GetPosition()[0]){
								passed = false;
								break;
							}
						}
					}
					if(!passed) continue;

					//Square rule
					if(Mathf.Abs(playerKing.GetPosition()[1]-pawn.GetPosition()[1]) <= pawn.GetPosition()[0])
						continue;


					points += (int)Mathf.Pow(passedPawnValue, pawn.GetPosition()[0]);
					#endregion
				}
			}
			return points;
		}

		Action FindNextAction(ChessNode node){
			while(node.Father().Father() != null){
				node = (ChessNode)node.Father();
			}

			return node.GetData();
		}

	}
}