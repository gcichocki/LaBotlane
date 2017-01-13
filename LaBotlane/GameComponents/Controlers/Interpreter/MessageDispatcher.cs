using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Labotlane.GameComponents.Controlers.Interpreter
{
    /// <summary>
    /// Représente un système qui envoie des évènements lorsque des messages sont ajoutés.
    /// </summary>
    public class MessageDispatcher : OutputSystem
    {
        /// <summary>
        /// Type de fonction représentant un dispatch de message.
        /// </summary>
        /// <param name="s"></param>
        public delegate void MessageDispatchDelegate(string s);
        public delegate void ObjectDispatchDelegate(object o);

        public event MessageDispatchDelegate OnStatusMessage;
        public event MessageDispatchDelegate OnErrorMessage;
        public event MessageDispatchDelegate OnMessage;
        public event ObjectDispatchDelegate OnObject;
        /// <summary>
        /// Crée une nouvelle instance de MessageDispatcher.
        /// </summary>
        public MessageDispatcher()
        {

        }

        /// <summary>
        /// Crée une nouvelle instance de MessageDispatcher associée à l'interpréteur passé en argument.
        /// </summary>
        /// <param name="intepreter"></param>
        public MessageDispatcher(CommandInterpreter intepreter)
        {
            intepreter.OutputSystems.Add(this);
        }
        #region Output System
        public void Print(string s)
        {
            if (OnMessage != null)
                OnMessage(s);
        }

        public void PrintError(string s)
        {
            if (OnErrorMessage != null)
                OnErrorMessage(s);
        }

        public void PrintStatus(string s)
        {
            if (OnStatusMessage != null)
                OnStatusMessage(s);
        }

        public void PrintObject(object o)
        {
            if (OnObject != null)
                OnObject(o);
        }
        #endregion
    }
}
