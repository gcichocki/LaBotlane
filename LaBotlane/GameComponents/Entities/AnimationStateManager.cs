using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Labotlane.GameComponents.Entities
{
    public enum AnimationState
    {
        Idle, 
        Running
    }
    /// <summary>
    /// Permet de dire si oui ou non le passage à un état donné est envisageable.
    /// Frame est le numéro de la frame pour l'animation actuelle.
    /// </summary>
    /// <param name="evt"></param>
    /// <returns></returns>
    public delegate bool AnimationStatePredicate(Entity evt, int frame);
    /// <summary>
    /// Représente un noeud d'animation.
    /// </summary>
    public class AnimationStateNode
    {
        /// <summary>
        /// Retourne l'état attaché à ce noeud.
        /// </summary>
        public AnimationState State { get; set; }
        /// <summary>
        /// Liens de cet état d'animation vers les états voisins.
        /// </summary>
        public List<AnimationStateNode> Links { get; set; }
        /// <summary>
        /// Prédicats permettant de déterminer si on peut passer à l'état donné.
        /// </summary>
        public List<AnimationStatePredicate> Predicates { get; set; }
        /// <summary>
        /// Priorités des états voisins.
        /// </summary>
        public List<int> Priorities { get; set; }

        /// <summary>
        /// Ajoute un nouveau lien.
        /// </summary>
        public void AddLink(AnimationStateNode node, AnimationStatePredicate predicate, int priority)
        {
            Priorities.Add(priority);
            Predicates.Add(predicate);
            Links.Add(node);
        }
        /// <summary>
        /// Crée un nouveau noeud d'animation.
        /// </summary>
        public AnimationStateNode(AnimationState state)
        {
            Links = new List<AnimationStateNode>();
            Predicates = new List<AnimationStatePredicate>();
            Priorities = new List<int>();
            State = state;
            
            
        }
    }
    /// <summary>
    /// Permet de gérer les états d'animation du héros sous la forme d'un graphe.
    /// </summary>
    public class AnimationStateManager
    {
        #region Variables
        /// <summary>
        /// Représente le noeud en cours.
        /// </summary>
        AnimationStateNode m_currentNode;
        /// <summary>
        /// Frame du noeud en cours.
        /// </summary>
        int m_currentFrame = 0;
        #endregion


        #region Properties
        /// <summary>
        /// Retourne l'état d'animation actuel.
        /// </summary>
        public AnimationState CurrentState
        {
            get { return m_currentNode.State; }
        }
        #endregion

        /// <summary>
        /// Crée une nouvelle instance de AnimationStateManager.
        /// </summary>
        public AnimationStateManager()
        {
           /* AnimationStateNode idle = new AnimationStateNode(AnimationState.Idle);
            AnimationStateNode running = new AnimationStateNode(AnimationState.Running);*/
            
            /*
            // -- IDLE -- //
            // Idle -> Idle
            idle.Links.Add(idle);
            idle.Priorities.Add(0);
            {
                return true;
            }));
            // Idle -> Running
            idle.Links.Add(running);
            idle.Priorities.Add(1);
            idle.Predicates.Add(new AnimationStateNode.AnimationStatePredicate(delegate(Entity evt, int frame)
            {
                Player player = (Player)evt;
                return player.IsRunning;
            }));
            
            // -- RUNNING -- //
            // Running -> Running
            AnimationStateNode.AnimationStatePredicate runToRun = new AnimationStateNode.AnimationStatePredicate(delegate(GameEvent evt, int frame)
            {
                Player player = (Player)evt;
                return player.IsRunning;
            });
            running.AddLink(running, runToRun, 0);
            // Running -> Idle
            AnimationStateNode.AnimationStatePredicate runToIdle = new AnimationStateNode.AnimationStatePredicate(delegate(GameEvent evt, int frame)
            {
                Player player = (Player)evt;
                return player.IsIdle;
            });
            running.AddLink(idle, runToIdle, 1);
            
            m_currentNode = idle;*/
        }

        /// <summary>
        /// Mets à jour l'état d'animation.
        /// </summary>
        /// <param name="evt"></param>
        public void Update(Entity evt)
        {
            // Sélectionne les noeuds éligibles.
            List<int> okPredicates = new List<int>();
            int nodeId = -1;
            int maxPriority = int.MinValue;
            for(int i = 0; i < m_currentNode.Links.Count; i++)
            {
                if(m_currentNode.Predicates[i].Invoke(evt, m_currentFrame))
                {
                    if (m_currentNode.Priorities[i] > maxPriority)
                    {
                        maxPriority = m_currentNode.Priorities[i];
                        nodeId = i;
                    }
                }
            }

            // Si aucun noeud sélectionné, on reste sur l'actuel.
            AnimationStateNode newNode;
            if (nodeId == -1)
                newNode = m_currentNode;
            else
                newNode = m_currentNode.Links[nodeId];

            // On récupère les vieux et nouveau états d'animation.
            AnimationState oldAnimState = CurrentState;
            m_currentNode = newNode;
            AnimationState newAnimState = CurrentState;

            // Mise à jour de la frame.
            if (oldAnimState == newAnimState)
                m_currentFrame++;
            else
                m_currentFrame = 0;
        }
    }
}
