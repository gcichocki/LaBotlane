using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Labotlane.GameComponents
{
    /// <summary>
    /// Contient tous les algorithmes permettant la recherche de chemins.
    /// </summary>
    public class PathFinder
    {
        /// <summary>
        /// Exception levée lorsque le point de destination ne peut pas être atteint.
        /// </summary>
        public class AStarFailureException : Exception { }

        /// <summary>
        /// Contient tous les algorythmes permettant de gérer la structure de donnée "node".
        /// </summary>
        public class Node
        {
            #region Properties
            /// <summary>
            /// Obtient ou définit la position d'une node.
            /// </summary>
            public GameMap.Position Position
            {
                get;
                set;
            }

            /// <summary>
            /// Obtient ou définit le Gscore d'une node vers une position donnée
            /// </summary>
            public float GScore
            {
                get;
                set;
            }

            /// <summary>
            /// Obtient ou définit le FScore d'une node vers une posistion précise.
            /// </summary>
            public float FScore
            {
                get;
                set;
            }

            /// <summary>
            /// Créé une nouvelle instance de node.
            /// </summary>
            public Node(GameMap.Position position, float gScore, float fScore)
            {
                Position=position;
                GScore=gScore;
                FScore=fScore;
            }

            /// <summary>
            /// Créé une nouvelle instance de node.
            /// </summary>
            public Node()
            {

            }

            
            #endregion
        }

        public static List<GameMap.Position> GetNeighbours(GameMap.Position position)
        {
            List<GameMap.Position> voisins = new List<GameMap.Position>();
            Microsoft.Xna.Framework.Point mapSize = LaBotlane.Instance.Scene.Map.MapSize.MapSpacePoint;
            if(position.MapSpacePoint.X < mapSize.X - 1)
                voisins.Add( GameMap.Position.FromMapSpacePoint (position.MapSpacePoint.X+1, position.MapSpacePoint.Y));

            if (position.MapSpacePoint.Y < mapSize.Y - 1)
                voisins.Add(GameMap.Position.FromMapSpacePoint(position.MapSpacePoint.X, position.MapSpacePoint.Y+1));

            if (position.MapSpacePoint.X > 0)
                voisins.Add(GameMap.Position.FromMapSpacePoint(position.MapSpacePoint.X-1, position.MapSpacePoint.Y));

            if (position.MapSpacePoint.Y > 0)
                voisins.Add(GameMap.Position.FromMapSpacePoint(position.MapSpacePoint.X, position.MapSpacePoint.Y-1));

            return voisins;
        }

       
      

        /// <summary>
        /// Retourne la distance en pixels entre deux cases, à vol d'oiseau.
        /// 
        /// </summary>
        /// <param name="start">Noeud départ.</param>
        /// <param name="end">Noeud d'arrivée.</param>
        /// <returns></returns>
        public static float EstimateHeuristicCost(GameMap.Position start, GameMap.Position end)
        {
            return (float)(Math.Sqrt(Math.Pow(end.MapSpacePixel.X-start.MapSpacePixel.X,2) + 
                                     Math.Pow(end.MapSpacePixel.Y-start.MapSpacePixel.Y,2)));
        }

        /// <summary>
        /// Retourne la position de la case adjacente à la position courante
        /// dont le coût heuristique vers est l'objectif est le plus faible.
        /// </summary>
        /// <param name="current"></param>
        /// <param name="goal"></param>
        /// <returns></returns>
        public static GameMap.Position BestNeighbour(GameMap.Position current, GameMap.Position goal)
        {
            float min = float.MaxValue;
            GameMap.Position closest = GameMap.Position.FromMapSpacePixel(0,0);
            foreach(GameMap.Position voisin in GetNeighbours(current))
            {
                if(EstimateHeuristicCost(voisin, goal)<min && LaBotlane.Instance.Scene.Map.GetTileAt(voisin).IsCrossable)
                {
                    min = EstimateHeuristicCost(voisin, goal);
                    closest= voisin;
                }
            }
            return closest;
        }



        /// <summary>
        /// Retourne la position de la case adjacente à la position courante
        /// dont le coût heuristique vers est l'objectif est le plus faible qui appartient à la liste limitante.
        /// </summary>
        /// <param name="current"></param>
        /// <param name="goal"></param>
        /// <returns></returns>
        public static GameMap.Position BestNeighbourLimited(GameMap.Position current, GameMap.Position goal, IEnumerable<GameMap.Position> limitation)
        {
            float min = float.MaxValue;
            GameMap.Position closest = GameMap.Position.FromMapSpacePixel(0, 0);
            foreach (GameMap.Position voisin in GetNeighbours(current))
            {
                if ((limitation.Contains(voisin)) && (EstimateHeuristicCost(voisin, goal) < min) && LaBotlane.Instance.Scene.Map.GetTileAt(voisin).IsCrossable)
                {
                    min = EstimateHeuristicCost(voisin, goal);
                    closest = voisin;
                }
            }

            // BestNeighbourLimited(current, goal, limitation.Select(delegate(Node node) { return node.Position; }));
            return closest;
        }



        /// <summary>
        /// Retourne la trajectoire la plus optimisée entre deux positions, 
        /// à partir d'un graphe précis de cases, sous la forme d'une liste de position.
        /// </summary>
        /// <param name="start">Position initiale</param>
        /// <param name="goal">Position finale</param>z
        /// <param name="cameFrom">Liste des cases parrcourues par l'astar</param>
        /// <returns>Chemin optimisé entre start et goal</returns>
        public static List<GameMap.Position> ReconstructPath(GameMap.Position start, GameMap.Position goal, IEnumerable<GameMap.Position> cameFrom) 
        {
            List<GameMap.Position> trajectory;
            trajectory= new List<GameMap.Position>();
            trajectory.Add(goal);
            GameMap.Position current;
            current = start;
            while(! trajectory.Contains(start))
            {
                trajectory.Insert(0, BestNeighbourLimited(current, start, cameFrom));
                current = BestNeighbourLimited(current, start, cameFrom);
            }
            return trajectory;
        }

        /// <summary>
        /// Retourne la node avec le fscore le plus faible dans une liste de node.
        /// </summary>
        /// <param name="openset"></param>
        /// <returns></returns>
        public static Node LowestFScore(List<Node> openset)
        {
            if(openset.Count != 0)
            {
                float min = float.MaxValue;
                Node bestNode = null;
                foreach(Node node in openset)
                {
                    if(node.FScore<min)
                    {
                         min = node.FScore;
                         bestNode=node;
                    }
                 }
                 return bestNode;
            } else {
                    throw new Exception();
            }
            
        }





        public static List<GameMap.Position> Astar(GameMap.Position debut, GameMap.Position fin)
        {
            Microsoft.Xna.Framework.Point mapSize = LaBotlane.Instance.Scene.Map.MapSize.MapSpacePoint;
            Node[,] nodemap = new Node[mapSize.X, mapSize.Y];
            for(int x = 0; x < mapSize.X; x++)
            {
                for(int y = 0; y < mapSize.Y; y++)
                {
                    var position = GameMap.Position.FromMapSpace(x, y);
                    nodemap[x,y] = new Node(position,0,0);
                }
            }


            List<Node> closedset = new List<Node>();
            List<Node> openset = new List<Node>();
            List<Node> cameFrom = new List<Node>();
            Node current = new Node (debut, 0 ,EstimateHeuristicCost(debut, fin));
            nodemap[debut.MapSpacePoint.X, debut.MapSpacePoint.Y] = current;
            openset.Add(nodemap[debut.MapSpacePoint.X, debut.MapSpacePoint.Y]);
            float tentativeGScore =0;

            while(openset.Count != 0)
            {
                current = LowestFScore(openset);
                if(GameMap.Position.EqualsPoint(current.Position,  fin))
                {
                    return ReconstructPath(debut, fin, cameFrom.Select(delegate(Node node) { return node.Position; }));
                }
                openset.Remove(current);
                closedset.Add(current);
                IEnumerable<GameMap.Position> closedsetPosition = closedset.Select(delegate(Node node){ return node.Position;});
                IEnumerable<GameMap.Position> opensetPosition = openset.Select(delegate(Node node){ return node.Position;});
                List<GameMap.Position> neighbours = GetNeighbours(current.Position);
                foreach(GameMap.Position voisin in neighbours )
                {
                    if(closedsetPosition.Contains(voisin))
                    {
                        continue;
                    } 
                    tentativeGScore= current.GScore +1;
                    if(!opensetPosition.Contains(voisin) | tentativeGScore < nodemap[voisin.MapSpacePoint.X, voisin.MapSpacePoint.Y].GScore)
                    {
                        cameFrom.Add(current);
                        nodemap[voisin.MapSpacePoint.X, voisin.MapSpacePoint.Y].GScore = tentativeGScore;
                        nodemap[voisin.MapSpacePoint.X, voisin.MapSpacePoint.Y].FScore = nodemap[voisin.MapSpacePoint.X, voisin.MapSpacePoint.Y].GScore
                                                                                       + EstimateHeuristicCost(voisin, fin);
                        if (!openset.Contains(nodemap[voisin.MapSpacePoint.X, voisin.MapSpacePoint.Y]))
                        {
                            openset.Add(nodemap[voisin.MapSpacePoint.X, voisin.MapSpacePoint.Y]);
                        }  
                    }
                    
                }
                
            }
            
            throw new AStarFailureException();
            
        }


    }
}
