using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Labotlane.GameComponents
{
    /// <summary>
    /// Représente un joueur.
    /// Contient les données concernant les statistiques du joueur.
    /// </summary>
    public class Player
    {
        #region Variables
        private int m_id;
        private int m_gold;
        private int m_score;
        private string m_nickname = "Player";
        #endregion

        #region Properties
        /// <summary>
        /// Obtient ou définit l'id du joueur.
        /// Les id inférieurs à 0 sont réservés pour les entités neutres.
        /// </summary>
        public int Id
        {
            get { return m_id; }
            set { 
                if(value < 0) 
                {
                    throw new Exception("Id < 0 réservé pour les entités neutres.");
                 
                }
                 
                m_id = value;
            }
        }
        
        /// <summary>
        /// Obtient ou définit le score du joueur.
        /// </summary>
        public int Score
        {
            get { return m_score; }
            set { m_score = value; }
        }


        /// <summary>
        /// Obtient ou définit l'argent du joueur.
        /// </summary>
        public int Gold
        {
            get { return m_gold; }
            set { m_gold = value; }
        }

        /// <summary>
        /// Obtient ou définit le nom du joueur.
        /// </summary>
        public string Nickname
        {
            get { return m_nickname; }
            set { m_nickname = value; }
        }
        #endregion
    }
}
