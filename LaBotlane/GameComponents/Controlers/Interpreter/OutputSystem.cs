using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Labotlane.GameComponents.Controlers.Interpreter
{
    /// <summary>
    /// Interface représentant un système de sortie pour les commandes de l'interpréteur
    /// qui retourne juste une sortie.
    /// </summary>
    public interface OutputSystem
    {
        /// <summary>
        /// Ajoute le string s à la sortie du système.
        /// </summary>
        /// <param name="s"></param>
        void Print(string s);
        /// <summary>
        /// Ajoute le string s à la sortie d'erreur du système.
        /// </summary>
        /// <param name="s"></param>
        void PrintError(string s);
        /// <summary>
        /// Ajoute le string s à la sortie de statut du système.
        /// </summary>
        /// <param name="s"></param>
        void PrintStatus(string s);
        /// <summary>
        /// Envoie un objet vers ce système de sortie.
        /// </summary>
        /// <param name="o"></param>
        void PrintObject(object o);
    }
}
