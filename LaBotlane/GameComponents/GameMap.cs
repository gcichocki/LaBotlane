using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;


namespace Labotlane.GameComponents
{
    /// <summary>
    /// Contient tous les objets de la map.
    /// </summary>
    public class GameMap
    {
        public struct Position
        {
            #region Variables
            /// <summary>
            /// Position en pourcentage de case (tiles).
            /// </summary>
            public Vector2 m_position;
            #endregion

            #region Properties
            /// <summary>
            /// Obtient ou définit la position en TILES dans l'espace de coordonnées de la map.
            /// </summary>
            public Vector2 MapSpace
            {
                get { return m_position; }
                set { m_position = value; }
            }

            /// <summary>
            /// Obtient ou définit la position en TILES dans l'espace de coordonnées de la map.
            /// </summary>
            public Point MapSpacePoint
            {
                get { return new Point((int)m_position.X, (int)m_position.Y); }
                set { m_position = new Vector2(value.X, value.Y); }
            }

            /// <summary>
            /// Obtient ou définit la position en PIXELS dans l'espace de coordonnées de la map.
            /// </summary>
            public Vector2 MapSpacePixel
            {
                get { return m_position * TileSize; }
                set { m_position = value / TileSize; }
            }

            /// <summary>
            /// Obtient ou définit la position en PIXELS. 
            /// Idem que MapSpacePixel.
            /// </summary>
            public Vector2 Pixels
            {
                get { return m_position * TileSize; }
                set { m_position = value / TileSize; }
            }
            /// <summary>
            /// Obtient ou définit la position en PIXELS dans l'espace de coordonnées de de l'écran.
            /// </summary>
            public Vector2 ScreenSpace
            {
                get
                {
                    return m_position * TileSize - LaBotlane.Instance.Scene.Map.ScrollingPixels;

                }
                set
                {
                    // ss = tiles * size - scrolling
                    // tiles = (ss + scrolling) / size.
                    m_position = (value + LaBotlane.Instance.Scene.Map.ScrollingPixels) / TileSize;
                }
            }
            #endregion

            #region Constructeurs
            /// <summary>
            /// Crée une nouvelle instance de position dans l'espace de la map.
            /// </summary>
            /// <param name="mapSpace"></param>
            private Position(Vector2 mapSpace)
            {
                m_position = mapSpace;
                
            }

            /// <summary>
            /// Retourne une position représentant la position en pixels dans l'espace de l'écran.
            /// </summary>
            public static Position FromScreenSpace(Vector2 screenSpace)
            {
                Position pos = new Position(Vector2.Zero);
                pos.ScreenSpace = screenSpace;
                return pos;
            }
            /// <summary>
            /// Retourne une position représentant la position en pixels dans l'espace de l'écran.
            /// </summary>
            public static Position FromScreenSpace(float x, float y)
            {
                Position pos = new Position(Vector2.Zero);
                pos.ScreenSpace = new Vector2(x, y);
                return pos;
            }

            /// <summary>
            /// Retourne une position représentant la position en pixels dans l'espace de l'écran.
            /// </summary>
            public static Position FromPixels(Vector2 screenSpace)
            {
                Position pos = new Position(Vector2.Zero);
                pos.MapSpacePixel = screenSpace;
                return pos;
            }

            /// <summary>
            /// Retourne une position représentant la position en pixels dans l'espace de l'écran.
            /// </summary>
            public static Position FromPixels(float x, float y)
            {
                Position pos = new Position(Vector2.Zero);
                pos.MapSpacePixel = new Vector2(x, y);
                return pos;
            }

            /// <summary>
            /// Retourne une position représentant la position en tiles dans l'espace de la map.
            /// </summary>
            public static Position FromMapSpacePoint(Point mapSpacePoint)
            {
                Position pos = new Position(Vector2.Zero);
                pos.MapSpacePoint = mapSpacePoint;
                return pos;
            }
            /// <summary>
            /// Retourne une position représentant la position donnée en tiles dans l'espace de la map.
            /// </summary>
            public static Position FromMapSpacePoint(int x, int y)
            {
                Position pos = new Position(Vector2.Zero);
                pos.MapSpacePoint = new Point(x, y);
                return pos;
            }
            /// <summary>
            /// Retourne une position représentant la position en pixels dans l'espace de la map.
            /// </summary>
            public static Position FromMapSpacePixel(Vector2 mapSpacePx)
            {
                Position pos = new Position(Vector2.Zero);
                pos.MapSpacePixel = mapSpacePx;
                return pos;
            }
            /// <summary>
            /// Retourne une position représentant la position en pixels dans l'espace de la map.
            /// </summary>
            public static Position FromMapSpacePixel(float x, float y)
            {
                Position pos = new Position(Vector2.Zero);
                pos.MapSpacePixel = new Vector2(x, y);
                return pos;
            }

            /// <summary>
            /// Retourne une position représentant la position en tiles dans l'espace de la map.
            /// </summary>
            public static Position FromMapSpace(Vector2 mapSpace)
            {
                Position pos = new Position(Vector2.Zero);
                pos.MapSpace = mapSpace;
                return pos;
            }

            /// <summary>
            /// Retourne une position représentant la position en tiles dans l'espace de la map donnée.
            /// </summary>
            public static Position FromMapSpace(float x, float y)
            {
                Position pos = new Position(Vector2.Zero);
                pos.MapSpace = new Vector2(x, y);
                return pos;
            }

            /// <summary>
            /// Retourne le carré de la distance en pixels entre deux positions données.
            /// </summary>
            public static float DistanceSquaredPixels(Position pos1, Position pos2)
            {
                return (float)(Math.Pow((pos2.MapSpacePixel.X - pos1.MapSpacePixel.X), 2) +
                               Math.Pow((pos2.MapSpacePixel.Y - pos1.MapSpacePixel.Y), 2));
            }

            /// <summary>
            /// Retourne la distance en pixels entre les deux positions données.
            /// </summary>
            /// <param name="pos1"></param>
            /// <param name="po2"></param>
            /// <returns></returns>
            public static float DistancePixels(Position pos1, Position pos2)
            {
                return (float)Math.Sqrt(DistanceSquaredPixels(pos1, pos2));
            }

            /// <summary>
            /// Retourne le carré de la distance en tiles entre deux positions données.
            /// </summary>
            public static float DistanceSquared(Position pos1, Position pos2)
            {
                return (float)(Math.Pow((pos2.MapSpace.X - pos1.MapSpace.X), 2) +
                               Math.Pow((pos2.MapSpace.Y - pos1.MapSpace.Y), 2));
            }

            /// <summary>
            /// Retourne la distance en tiles entre les deux positions données.
            /// </summary>
            /// <param name="pos1"></param>
            /// <param name="po2"></param>
            /// <returns></returns>
            public static float Distance(Position pos1, Position pos2)
            {
                return (float)Math.Sqrt(DistanceSquaredPixels(pos1, pos2));
            }
            #endregion

            #region Methods
            /// <summary>
            /// Retourne une valeur indiquant si la position indique un lieu à l'intérieur de la map, non
            /// si ce lieu est hors des bords.
            /// </summary>
            /// <returns></returns>
            public bool IsInMap()
            {
                Point mapSize = LaBotlane.Instance.Scene.Map.MapSize.MapSpacePoint;
                return ((m_position.X > 0 && m_position.Y > 0 && m_position.X <= mapSize.X - 1 && m_position.Y <= mapSize.Y - 1));
            }

            /// <summary>
            /// Transforme la position actuelle pour qu'elle soit contenue dans la map.
            /// </summary>
            public void MakeInRange()
            {
                Point mapSize = LaBotlane.Instance.Scene.Map.MapSize.MapSpacePoint;
                m_position.X = Math.Max(0, Math.Min(mapSize.X - 1, m_position.X));
                m_position.Y = Math.Max(0, Math.Min(mapSize.Y - 1, m_position.Y));
            }

            /// <summary>
            /// Retourne la position actuelle corrigée afin qu'elle soit contenue dans la map.
            /// </summary>
            public Position InRange()
            {
                Point mapSize = LaBotlane.Instance.Scene.Map.MapSize.MapSpacePoint;
                return new Position(new Vector2(Math.Max(0, Math.Min(mapSize.X - 1, m_position.X)), Math.Max(0, Math.Min(mapSize.Y - 1, m_position.Y))));
            }

            /// <summary>
            /// Retourne la position actuelle corrigée afin qu'elle soit contenue dans l'écran.
            /// </summary>
            public Position InScreenRange()
            {
                Point screenSize = new Point(Scenes.Scene.ResolutionWidth, Scenes.Scene.ResolutionHeight);
                Vector2 posScreenSpace = ScreenSpace;
                return Position.FromScreenSpace(Math.Max(0, Math.Min(screenSize.X, posScreenSpace.X)), 
                    Math.Max(0, Math.Min(screenSize.Y, posScreenSpace.Y)));
            }

            
            #endregion

            #region Operators
            /// <summary>
            /// Ajoute deux positions.
            /// </summary>
            public static Position operator +(Position p1, Position p2)
            {
                return new Position(p1.m_position + p2.m_position);
            }

            /// <summary>
            /// Retourne l'opposé de la position passée en paramètre.
            /// </summary>
            public static Position operator -(Position p1)
            {
                return new Position(-p1.m_position);
            }

            /// <summary>
            /// Soustrait p1 à p2.
            /// </summary>
            public static Position operator -(Position p1, Position p2)
            {
                return new Position(p1.m_position - p2.m_position);
            }

            /// <summary>
            /// Multiplie p1 par le scalaire factor.
            /// </summary>

            public static Position operator *(Position p1, float factor)
            {
                return new Position(p1.m_position * factor);
            }

            /// <summary>
            /// Divise p1 par le scalaire div.
            /// </summary>
            public static Position operator /(Position p1, float div)
            {
                return new Position(p1.m_position / div);
            }

            /// <summary>
            /// Retourne une valeur indiquant si les deux positions sont égales.
            /// </summary>
            public static bool operator ==(Position p1, Position p2)
            {
                return p1.m_position == p2.m_position;
            }

            /// <summary>
            /// Retourne une valeur indiquant si les deux positions sont différentes.
            /// </summary>
            public static bool operator !=(Position p1, Position p2)
            {
                return !(p1 == p2);
            }

            /// <summary>
            /// Retourne un string représentant l'objet position actuel.
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                return "<X=" + ((int)m_position.X).ToString() + ",Y=" + ((int)m_position.Y).ToString() + ">";
            }

            /// <summary>
            /// Retourne une valeur indiquant si les deux positions en "point" sont égales.
            /// </summary>
            public static bool EqualsPoint(Position p1, Position p2)
            {
                return p1.MapSpacePoint == p2.MapSpacePoint;
            }
            #endregion
        }
        /// <summary>
        /// Taille des tiles.
        /// </summary>
        public const int TileSize = 32;
        /// <summary>
        /// Taille horizontale de l'écran en tiles.
        /// </summary>
        public static int ScreenWidth = 27;
        /// <summary>
        /// Taille verticale de l'écran en tiles.
        /// </summary>
        public static int ScreenHeight = 20;
        /// <summary>
        /// Vitesse de scrolling en px/sec.
        /// </summary>
        public const float ScrollSpeed = 1024.0f;
        #region Variables
        /// <summary>
        /// Représente une tilemap.
        /// </summary>
        private Tile[,] m_tilemap;
        /// <summary>
        /// Représente la zone de la carte affichée à l'écran.
        /// Unité : pixel
        /// </summary>
        private Rectangle m_gameScreen;
        /// <summary>
        /// Texture représentant le tileset.
        /// </summary>
        private Texture2D m_tilesetTexture;
        /// <summary>
        /// Contient la liste des entités dynamiques.
        /// </summary>
        private List<Entities.Entity> m_dynamicEntities;
        /// <summary>
        /// Contient la liste des entités à supprimer à la fin d'une frame de jeu.
        /// </summary>
        private List<Entities.Entity> m_entitiesToRemove;
        #endregion

        #region Properties
        /// <summary>
        /// Retourne la liste des entités dynamiques, en lecture seule.
        /// </summary>
        public IEnumerable<Entities.Entity> DynamicEntities
        {
            get { return m_dynamicEntities; }
        }
        /// <summary>
        /// Retourne la taille de la map.
        /// </summary>
        public Position MapSize
        {
            get { return Position.FromMapSpacePoint(m_tilemap.GetLength(0), m_tilemap.GetLength(1)); }
            
        }
        /// <summary>
        /// Obtient la texture du tileset.
        /// </summary>
        public Texture2D TilesetTexture
        {
            get { return m_tilesetTexture; }
        }

        /// <summary>
        /// Obtient ou définit la zone de la carte affichée à l'écran.
        /// Unité : pixels, dans la MapSpace.
        /// </summary>
        public Rectangle GameScreenPixel
        {
            get 
            {
                // TODO : limiter la position de l'écran aux limites de la map.
                return m_gameScreen;
            }
            private set 
            { 
                m_gameScreen = value;
                m_gameScreen.X = Math.Max(0, Math.Min(m_gameScreen.X, MapSize.MapSpacePoint.X * TileSize - m_gameScreen.Width));
                m_gameScreen.Y = Math.Max(0, Math.Min(m_gameScreen.Y, MapSize.MapSpacePoint.Y * TileSize - m_gameScreen.Height));
            }
        }

        /// <summary>
        /// Obtient le début de la zone de la carte affichée à l'écran.
        /// </summary>
        public Position Scrolling
        {
            get { return Position.FromScreenSpace(m_gameScreen.X, m_gameScreen.Y); }
        }

        /// <summary>
        /// Obtient le début de la zone de la carte affichée à l'écran, en pixels.
        /// </summary>
        public Vector2 ScrollingPixels
        {
            get { return new Vector2(m_gameScreen.X, m_gameScreen.Y); }
        }
        /// <summary>
        /// Obtient ou définit la zone de la carte affichée à l'écran.
        /// Unité : tiles.
        /// </summary>
        public Rectangle GameScreenTiles
        {
            get
            {
                return new Rectangle(
                    Math.Min(Math.Max(0, GameScreenPixel.Left / TileSize - 1), MapSize.MapSpacePoint.X-ScreenWidth),
                    Math.Min(Math.Max(0, GameScreenPixel.Top / TileSize - 1), MapSize.MapSpacePoint.Y-ScreenHeight),
                    ScreenWidth,
                    ScreenHeight
                    );
                
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Crée une nouvelle instance de GameMap.
        /// </summary>
        public GameMap()
        {
            m_tilesetTexture = LaBotlane.Instance.Content.Load<Texture2D>("textures\\tileset");
            m_dynamicEntities = new List<Entities.Entity>();
            m_entitiesToRemove = new List<Entities.Entity>();
            // ----- TEST

            
            /*Random rand = new Random();
            for(int i = 0; i < 100; i++)
            {
                for(int j = 0; j < 100; j++)
                {
                    m_tilemap[i, j] = new Tile();
                    SpawnEntity(new Entities.GroundEntity(), Position.FromMapSpacePoint(i, j));
                    if(rand.Next(10) == 0)
                    {
                        Entities.SoldierEntity newEntity = new Entities.SoldierEntity();
                        newEntity.Debug_Variant = rand.Next(6);
                        newEntity.OwnerId = rand.Next(2);
                        SpawnEntity(newEntity, Position.FromMapSpacePoint(i, j));

                        Position pos = Position.FromMapSpace(rand.Next(i), rand.Next(j));
                        if(CanMoveTo(newEntity, pos))
                            MoveEntity(newEntity, pos);
                    }
                }
            }*/
        }

        #region Movement

        /// <summary>
        /// Retourne une valeur indiquant si l'entité source peut se déplacer vers l'entité destination.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool CanMoveTo(Entities.Entity src, GameMap.Position pt2)
        {
            return GetTrajectoryTo(src.EndOfQueuePosition, pt2).Count <= src.RemainingMovePoints;
        }


        /// <summary>
        /// Retourne une liste de points contenant les points par lesquels il faut passer
        /// pour attendre pt2 partant de pt1, en prenant LE PLUS COURT CHEMIN.
        /// </summary>
        /// <param name="pt1"></param>
        /// <param name="pt2"></param>
        /// <returns></returns>
        public List<Position> GetTrajectoryTo(GameMap.Position pt1, GameMap.Position pt2)
        {
            // List<Position> positions = PathFinder.Astar(pt1, pt2);
            // /*
            List<Position> positions = new List<Position>();
            int dx = Math.Sign(pt2.MapSpacePoint.X - pt1.MapSpacePoint.X);
            int dy = Math.Sign(pt2.MapSpacePoint.Y - pt1.MapSpacePoint.Y);

            int x = pt1.MapSpacePoint.X;
            int y = pt1.MapSpacePoint.Y;
            while(x != pt2.MapSpacePoint.X)
            {
                positions.Add(Position.FromMapSpacePoint(x, y));
                x += dx;
            }
            while (y != pt2.MapSpacePoint.Y)
            {
                positions.Add(Position.FromMapSpacePoint(x, y));
                y += dy;
            }

            // Dernier point.
            positions.Add(Position.FromMapSpacePoint(x, y)); // */

            

            return positions;
        }
        #endregion

        #region Entities
        /// <summary>
        /// Trouve les unités en filtrant avec le type donné et l'id du joueur.
        /// </summary>
        /// <param name="playerId"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public List<Entities.Entity> FindEntityByType(int playerId, Entities.TargetType type)
        {
            List<Entities.Entity> matchedEntity = new List<Entities.Entity>();
            foreach(Entities.Entity entity in DynamicEntities)
            {
                if(entity.OwnerId == playerId && ((type & entity.Type) == type))
                {
                    matchedEntity.Add(entity);
                }
            }
            return matchedEntity;
        }
        /// <summary>
        /// Obtient le tile présent à la position passée en paramètre.
        /// </summary>
        public Tile GetTileAt(Position position)
        {
            position = position.InRange();
            return m_tilemap[position.MapSpacePoint.X, position.MapSpacePoint.Y];
        }
        /// <summary>
        /// Obtient l'entité controlable par le joueur dont l'id est playerId à la position donnée.
        /// 
        /// Retourne null si aucune entité n'est trouvée.
        /// </summary>
        /// <param name="position">Position en cases où chercher l'entité.</param>
        /// <param name="playerId"></param>
        /// <returns></returns>
        public Entities.Entity GetEntityAt(Position position, int playerId)
        {
            
            Tile tile = m_tilemap[position.MapSpacePoint.X, position.MapSpacePoint.Y];
            foreach(Entities.Entity entity in tile.Entities)
            {
                if (playerId == entity.OwnerId)
                    return entity;
            }
            return null;
        }

        /// <summary>
        /// Envoie une signal aux entités se trouvant à la position donnée indiquant que l'entité src 
        /// les a traversées.
        /// </summary>
        public void SendEntityCrossSignal(Position position, Entities.Entity src)
        {
            Tile tile = m_tilemap[position.MapSpacePoint.X, position.MapSpacePoint.Y];
            foreach(Entities.Entity entity in tile.Entities)
            {
                entity.OnCrossedBy(src);
            }
        }
        /// <summary>
        /// Fait apparaître une entité à la position définie en argument.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="position">Position où faire apparaître l'entité.</param>
        public void SpawnEntity(Entities.Entity entity, Position position)
        {
            // Vérification de la position.
            if (!position.IsInMap())
                new Entities.InvalidInteractionException("Impossible de faire apparaître une entité à la position : " +
                    position.MapSpacePoint.ToString() + ". En dehors de la map.");

            entity.Position = position;
            entity.EndOfQueuePosition = position;
            m_tilemap[position.MapSpacePoint.X, position.MapSpacePoint.Y].Entities.Add(entity);

            // Ajoute l'entité à la liste des entités dynamiques.
            if (entity.IsDynamic)
                m_dynamicEntities.Add(entity);
        }
        /// <summary>
        /// Déplace une unité de sa position actuelle vers la position passée en paramètre.
        /// 
        /// Lève une exception si l'unité ne peut pas se déplacer à la position donnée.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="newPosition">Nouvelle position pour l'unité.</param>
        public void MoveEntity(Entities.Entity entity, Position newPosition)
        {
            Position currentPosition = entity.EndOfQueuePosition;

            // Vérification de la possibilité de l'action.
            List<Position> trajPositions = GetTrajectoryTo(currentPosition, newPosition);
            if (trajPositions.Count > entity.RemainingMovePoints)
            {
                throw new Entities.InvalidInteractionException("Pas assez de move points pour effectuer le déplacement.");
            }
            else if(!newPosition.IsInMap())
            {
                throw new Entities.InvalidInteractionException("Impossible de faire déplacer une entité à la position : " +
                    newPosition.MapSpacePoint.ToString() + ". En dehors de la map.");
            }


            // Déplace l'entité dans la tilemap.
            m_tilemap[currentPosition.MapSpacePoint.X, currentPosition.MapSpacePoint.Y].Entities.Remove(entity);
            m_tilemap[newPosition.MapSpacePoint.X, newPosition.MapSpacePoint.Y].Entities.Add(entity);
            entity.EndOfQueuePosition = newPosition;

            // Déclenche l'animation
            entity.RemainingMovePoints -= trajPositions.Count;
            entity.ApplyInteraction(Entities.Interaction.MoveInteraction(entity, new Trajectory(trajPositions)));
        }

        /// <summary>
        /// Supprime une entité du jeu.
        /// </summary>
        /// <param name="entity">Entité à supprimer</param>
        public void RemoveEntity(Entities.Entity entity)
        {
            m_entitiesToRemove.Add(entity);
        }
        #endregion

        #region Scrolling
        /// <summary>
        /// Centre la caméra sur la tile passée en argument.
        /// </summary>
        /// <param name="entity"></param>
        public void CenterOnTile(Point tile)
        {
            /*            if (newMousePos.ScreenSpace.X <= 1)
                LaBotlane.Instance.Scene.Map.ScrollLeft(time);
            else if (newMousePos.ScreenSpace.X >= Scenes.Scene.ResolutionWidth - 1)
                LaBotlane.Instance.Scene.Map.ScrollRight(time);
            if (newMousePos.ScreenSpace.Y <= 1)
                LaBotlane.Instance.Scene.Map.ScrollUp(time);
            else if (newMousePos.ScreenSpace.Y >= Scenes.Scene.ResolutionHeight - 1)
                LaBotlane.Instance.Scene.Map.ScrollDown(time);*/
            GameScreenPixel = new Rectangle(
                (int)Math.Max(0, Math.Min(MapSize.Pixels.X - Scenes.Scene.ResolutionWidth, (tile.X - ScreenWidth/2)*TileSize)),
                (int)Math.Max(0, Math.Min(MapSize.Pixels.X - Scenes.Scene.ResolutionHeight, (tile.Y-ScreenHeight/2)*TileSize)),
                ScreenWidth,
                ScreenHeight);
        }

        /// <summary>
        /// Centre la caméra sur la tile passée en argument.
        /// </summary>
        /// <param name="entity"></param>
        public void CenterOnPosition(GameMap.Position tile)
        {
            /*            if (newMousePos.ScreenSpace.X <= 1)
                LaBotlane.Instance.Scene.Map.ScrollLeft(time);
            else if (newMousePos.ScreenSpace.X >= Scenes.Scene.ResolutionWidth - 1)
                LaBotlane.Instance.Scene.Map.ScrollRight(time);
            if (newMousePos.ScreenSpace.Y <= 1)
                LaBotlane.Instance.Scene.Map.ScrollUp(time);
            else if (newMousePos.ScreenSpace.Y >= Scenes.Scene.ResolutionHeight - 1)
                LaBotlane.Instance.Scene.Map.ScrollDown(time);*/
            GameScreenPixel = new Rectangle(
                (int)Math.Max(0, Math.Min(MapSize.Pixels.X - Scenes.Scene.ResolutionWidth, tile.MapSpacePixel.X - (ScreenWidth / 2) * TileSize)),
                (int)Math.Max(0, Math.Min(MapSize.Pixels.X - Scenes.Scene.ResolutionHeight, tile.MapSpacePixel.Y - (ScreenHeight / 2) * TileSize)),
                ScreenWidth,
                ScreenHeight);
        }
        /// <summary>
        /// Centre la caméra sur la tile passée en argument.
        /// </summary>
        /// <param name="entity"></param>
        public void CenterOnTile(GameMap.Position tile)
        {
            CenterOnTile(tile.MapSpacePoint);
        }

        /// <summary>
        /// Effectue un mouvement vertical bas de la caméra, à la vitesse par défaut.
        /// </summary>
        /// <param name="time"></param>
        public void ScrollDown(GameTime time)
        {
            Rectangle gameScreenPixel = GameScreenPixel;
            GameScreenPixel = new Rectangle(gameScreenPixel.X, gameScreenPixel.Y + (int)((float)time.ElapsedGameTime.TotalSeconds * ScrollSpeed),
                Scenes.Scene.ResolutionWidth, Scenes.Scene.ResolutionHeight);
        }

        /// <summary>
        /// Effectue un mouvement vertical haut de la caméra, à la vitesse par défaut.
        /// </summary>
        /// <param name="time"></param>
        public void ScrollUp(GameTime time)
        {
            Rectangle gameScreenPixel = GameScreenPixel;
            GameScreenPixel = new Rectangle(gameScreenPixel.X, gameScreenPixel.Y - (int)((float)time.ElapsedGameTime.TotalSeconds * ScrollSpeed),
                Scenes.Scene.ResolutionWidth, Scenes.Scene.ResolutionHeight);
        }

        /// <summary>
        /// Effectue un mouvement horitontal droit de la caméra, à la vitesse par défaut.
        /// </summary>
        /// <param name="time"></param>
        public void ScrollRight(GameTime time)
        {
            Rectangle gameScreenPixel = GameScreenPixel;
            GameScreenPixel = new Rectangle(gameScreenPixel.X + (int)((float)time.ElapsedGameTime.TotalSeconds * ScrollSpeed), gameScreenPixel.Y,
                Scenes.Scene.ResolutionWidth, Scenes.Scene.ResolutionHeight);
        }

        /// <summary>
        /// Effectue un mouvement horitontal gauche de la caméra, à la vitesse par défaut.
        /// </summary>
        /// <param name="time"></param>
        public void ScrollLeft(GameTime time)
        {
            Rectangle gameScreenPixel = GameScreenPixel;
            GameScreenPixel = new Rectangle(gameScreenPixel.X - (int)((float)time.ElapsedGameTime.TotalSeconds * ScrollSpeed), gameScreenPixel.Y,
                Scenes.Scene.ResolutionWidth, Scenes.Scene.ResolutionHeight);
        }
        #endregion

        #region Updates
        /// <summary>
        /// Mets à jour la carte.
        /// </summary>
        /// <param name="time"></param>
        public void Update(GameTime time)
        {
            UpdateScrolling(time);
            UpdateDynamicEntities(time);
        }
        /// <summary>
        /// Mets à jour les entités dynamiques.
        /// </summary>
        /// <param name="time"></param>
        void UpdateDynamicEntities(GameTime time)
        {
            // Mets à jour les entités dynamiques.
            foreach(Entities.Entity entity in m_dynamicEntities)
            {
                entity.Update(time);
            }

            // Supprime les entités dans la liste des entités à supprimer.
            foreach(Entities.Entity entity in m_entitiesToRemove)
            {
                Position pos = entity.TilemapPosition;
                m_tilemap[pos.MapSpacePoint.X, pos.MapSpacePoint.Y].Entities.Remove(entity);
                if (entity.IsDynamic)
                    m_dynamicEntities.Remove(entity);
            }
        }
        /// <summary>
        /// Mets à jour le scrolling de la map.
        /// </summary>
        /// <param name="time"></param>
        public void UpdateScrolling(GameTime time)
        {
            //m_gameScreen = new Rectangle(0, 0, ScreenWidth * TileSize, ScreenHeight * TileSize);
            /*GameScreenPixel = new Rectangle((int)(time.TotalGameTime.TotalSeconds*256), 
                20 + (int)(Math.Sin(time.TotalGameTime.TotalSeconds)*15), ScreenWidth * TileSize, ScreenHeight * TileSize);*/
        }
        #endregion

        #region Draw
        /// <summary>
        /// Appelle les fonctions Draw de chacune des cellules.
        /// </summary>
        /// <param name="batch"></param>
        public void Draw(SpriteBatch batch)
        {
            Rectangle screenSize = GameScreenTiles;

            // Highlight du curseur.
            Position cursorPos = Position.FromScreenSpace(Input.GetMouseState().X, Input.GetMouseState().Y).InRange();
            m_tilemap[cursorPos.MapSpacePoint.X, cursorPos.MapSpacePoint.Y].Highlight();
            
            // Dessine la tilemap
            int left = Math.Max(0, screenSize.Left);
            int top = Math.Max(0, screenSize.Top);
            int right = Math.Min(screenSize.Right+1, m_tilemap.GetLength(0));
            int bottom = Math.Min(screenSize.Bottom+1, m_tilemap.GetLength(1));
            for(int x = left; x < right; x++)
            {
                for(int y = top; y < bottom; y++)
                {
                    m_tilemap[x, y].Draw(this);
                }
            }

            // Entités dynamiques
            foreach(Entities.Entity entity in m_dynamicEntities)
            {
                entity.Draw(this);
            }

            
        }

        #endregion

        #region Turns
        /// <summary>
        /// Méthode appeler lorsque le tour d'un joueur démarre.
        /// </summary>
        public void TurnStarted(int playerId)
        {
            foreach(Entities.Entity entity in m_dynamicEntities)
            {
                if (entity.OwnerId == playerId)
                {
                    entity.ResetMovePoints();
                    entity.ActionDone = false;
                }
            }
        }

        #endregion

        #region Loading
        /// <summary>
        /// Charge la map depuis le fichier dont le nom est passé en paramètre.
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public void LoadFromFile(string filename)
        {
            FileStream fileStream = new FileStream(filename, FileMode.Open);
            StreamReader reader = new StreamReader(fileStream);

            // Récupère le nom de la map.
            string mapName = reader.ReadLine();

            // Récupère le nombre de joueurs.
            int playersCount = Int32.Parse(reader.ReadLine());


            // Initilialise la tilemap à partir des données présente dans le fichier donné en argument.
            string size = reader.ReadLine();
            string[] splitSize = size.Split(' ');

            int sizeX = Int32.Parse(splitSize[0]);
            int sizeY = Int32.Parse(splitSize[1]);

            m_tilemap = CreateTileMap(sizeX, sizeY);
            for (int y = 0; y < sizeY; y++)
            {
                string line = reader.ReadLine();
                for (int x = 0; x < sizeX; x++)
                {
                    char charEntity = line[x];
                    Tile entitile = ConvertToTile(charEntity);

                    // Fait apparaître les entités contenues dans le tile sur la map.
                    foreach (Entities.Entity entity in entitile.Entities)
                    {
                        SpawnEntity(entity, Position.FromMapSpacePoint(x, y));
                    }

                }
            }

            // DEBUG
            Entities.FactoryEntity facto = new Entities.FactoryEntity();
            facto.OwnerId = 1;
            SpawnEntity(facto, Position.FromMapSpacePoint(sizeX - 5, 15));
        }
        /// <summary>
        /// Convertit un caractère issu d'un fichier texte en tile.
        /// </summary>
        private Tile ConvertToTile(char charEntity)
        {
            Tile tile = new Tile();
            switch (charEntity)
            {
                case 'P':
                    tile.Entities.Add(new Entities.GroundEntity());
                    break;
                case 'M':
                    tile.Entities.Add(new Entities.GroundEntity());
                    tile.Entities.Add(new Entities.MountainEntity());
                    break;
                case 'W':
                    tile.Entities.Add(new Entities.WaterEntity());
                    break;
                case 'G':
                    tile.Entities.Add(new Entities.GroundEntity());
                    tile.Entities.Add(new Entities.GoldEntity());
                    break;
                case 'T':
                    tile.Entities.Add(new Entities.GroundEntity());
                    tile.Entities.Add(new Entities.FactoryEntity() { OwnerId = 0 });
                    break;
                case 'F':
                    tile.Entities.Add(new Entities.ForestEntity());

                    break;
                case 'S':
                    tile.Entities.Add(new Entities.SwampEntity());
                    break;
                case 'B':
                    tile.Entities.Add(new Entities.BonusEntity());
                    break;
                default:
                    break;
            }
            return tile;
        }

        /// <summary>
        /// Crée et retourne une tilemap contenant des tiles initialisés vides.
        /// </summary>
        /// <param name="w">Largeur de la tilemap.</param>
        /// <param name="h">Hauteur de la tilemap</param>
        /// <returns></returns>
        private Tile[,] CreateTileMap(int w, int h)
        {
            Tile[,] tile = new Tile[w, h];
            for (int x = 0; x < w; x++)
            {
                for (int y = 0; y < h; y++)
                {
                    tile[x, y] = new Tile();
                    tile[x, y].Position = Position.FromMapSpacePoint(x, y);
                }
            }
            return tile;
        }
        #endregion


 

        #endregion
    }
}
