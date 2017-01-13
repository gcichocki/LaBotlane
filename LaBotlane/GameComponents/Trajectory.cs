using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace Labotlane.GameComponents
{
    /// <summary>
    /// Représente une trajectoire.
    /// Contient également des moyens de stockage de la position actuelle par rapport à la trajectoire.
    /// </summary>
    public class Trajectory
    {
        /// <summary>
        /// Liste des points de passage de la trajectoire.
        /// Deux points consécutifs doivent être séparés d'une distance de 1 !
        /// </summary>
        private List<GameMap.Position> m_trajectoryUnits;

        /// <summary>
        /// Indique la distance parcourue depuis le début de la trajectoire en pourcentage de case.
        /// 0 : case de départ
        /// [taille de TrajectoryUnit] : case d'arrivée.
        /// </summary>
        private float m_currentDistance;

        #region Properties
        /// <summary>
        /// Obtient ou définit la liste des points de passage de la trajectoire.
        /// Deux points consécutifs doivent être séparés d'une distance de 1 !
        /// </summary>
        public List<GameMap.Position> TrajectoryUnits
        {
            get
            {
                return m_trajectoryUnits;
            }
            set
            {
                // Vérifie que la distance entre 2 points consécutifs est de 1.
                bool ok = true;
                if (value.Count <= 1)
                    m_trajectoryUnits = value;
                else
                {
                    int i = 0;
                    while((i < value.Count -1) && ok)
                    {
                        ok = ((Math.Abs(value[i].MapSpace.X - value[i + 1].MapSpace.X) + Math.Abs(value[i].MapSpace.Y- value[i+1].MapSpace.Y)) == 1);
                        i++;
                    }
                    if (ok)
                        m_trajectoryUnits = value;
                    else
                        throw new Exception("La distance entre les points de la trajectoire n'est pas toujours égale à 1");
                }
                
            }
        }
        #endregion

        #region Public
        /// <summary>
        /// Crée une nouvelle instance de Trajectory avec une trajectoire vide.
        /// </summary>
        public Trajectory()
        {
            TrajectoryUnits = new List<GameMap.Position>();
        }

        /// <summary>
        /// Retourne la position finale de la trajectoire.
        /// </summary>
        /// <returns></returns>
        public GameMap.Position LastPosition()
        {
            return TrajectoryUnits.Last();
        }
        /// <summary>
        /// Crée une nouvelle instance de Trajectory avec la trajectoire passée en paramètre.
        /// /!\ Chaque point de la trajectoire doit être séparé d'une seule unité par rapport à son voisin.
        /// </summary>
        /// <param name="points"></param>
        public Trajectory(List<GameMap.Position> points)
        {
            TrajectoryUnits = points;
        }
        /// <summary>
        /// Avance d'une étape discrète dans la trajectoire.
        /// </summary>
        /// <param name="speed">Distance parcourue au cours de cette étape en pourcentage de case.</param>
        public void NextStep(float speed)
        {
            m_currentDistance += speed;
            
        }
        /// <summary>
        /// Retourne vrai si la position du dernier élément de la trajectoire est atteinte.
        /// </summary>
        /// <returns></returns>
        public bool IsEnded
        {
            get { return m_currentDistance >= m_trajectoryUnits.Count - 1; }
        }

        /// <summary>
        /// Retourne la position actuelle dans la trajectoire, en pourcentage de cases.
        /// </summary>
        public GameMap.Position Position
        {
            get 
            {
                if (IsEnded)
                    return GameMap.Position.FromMapSpace(m_trajectoryUnits.Last().MapSpace.X, m_trajectoryUnits.Last().MapSpace.Y);
                else
                    return lerp(m_trajectoryUnits[(int)m_currentDistance].MapSpacePoint,
                        m_trajectoryUnits[(int)m_currentDistance+1].MapSpacePoint,
                        m_currentDistance % 1);
            }
        }

        #endregion


        #region Utils
        /// <summary>
        /// Interpolation linéaire de from à to, avec l'avancement a.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="a"></param>
        /// <returns></returns>
        float lerp(float from, float to, float a)
        {
            return from * (1 - a) + to * a;
        }
        /// <summary>
        /// Interpolation linéaire entre deux vecteurs.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="a"></param>
        /// <returns></returns>
        GameMap.Position lerp(Point from, Point to, float a)
        {
            return GameMap.Position.FromMapSpace(lerp(from.X, to.X, a), lerp(from.Y, to.Y, a));
        }
        #endregion
    }
}
