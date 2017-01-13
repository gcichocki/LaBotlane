﻿
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
namespace Labotlane.GameComponents.Graphics
{
    /// <summary>
    /// Représente une caméra : l'objet qui "voit" ce qu'il y a à l'écran.
    /// </summary>
    public class FirstPersonCameraV2
    {
        #region Variables
        /// <summary>
        /// Matrice "view" permettant la projection de l'image selon l'angle et la position de la caméra.
        /// </summary>
        Matrix m_view;
        /// <summary>
        /// Position de la caméra.
        /// </summary>
        Vector3 m_up = -Vector3.UnitZ; //L'axe Z du monde est situé vers le  bas
        Vector3 m_upRoll = -Vector3.UnitZ; //Axe "up" après rotation
        /// <summary>
        /// Cible de la caméra.
        /// </summary>
        Vector3 m_front = -Vector3.UnitX; //Repère de la caméra sens direct

        /// <summary>
        /// Position de la caméra.
        /// </summary>
        Vector3 m_position;
        /// <summary>
        /// Vaut vrai si View doit être mise à jour.
        /// </summary>
        bool m_needCompute = false;


        /// <summary>
        /// Obtient ou définit la position de la caméra.
        /// </summary>
        public Vector3 Position
        {
            get { return m_position; }
            set { m_position = value; m_needCompute = true; }
        }
        /// <summary>
        /// Obtient ou Modifie la direction "haut" de la caméra.
        /// </summary>
        public Vector3 Right
        {
            get { return Vector3.Cross(m_up, m_front); }
        }
        /// <summary>
        /// Obtient ou Modifie la direction "haut" de la caméra.
        /// </summary>
        public Vector3 Up
        {
            get { return m_upRoll; }
            set { m_upRoll = value; m_needCompute = true; }
        }
        /// <summary>
        /// Obtient ou modifie la direction "front" de la caméra.
        /// </summary>
        public Vector3 Front
        {
            get { return m_front; }
            set { m_front = value; m_needCompute = true; }
        }
        #endregion

        #region Properties
        /// <summary>
        /// Calcule la matrice View actuellement vue par la caméra.
        /// </summary>
        /// <returns></returns>
        public Matrix ComputeView()
        {
            return Matrix.CreateLookAt(m_position, m_position + m_front, m_upRoll);
        }
        /// <summary>
        /// Retourne la matrice "View" de la caméra.
        /// </summary>
        public Matrix View
        {
            get
            {
                if (m_needCompute)
                {
                    m_view = ComputeView();
                    m_needCompute = false;
                }
                return m_view;
            }
        }
        /// <summary>
        /// Fait effectuer une rotation de la caméra autour de l'axe "UP".
        /// </summary>
        /// <param name="value"></param>
        public void RotateSide(float value)
        {
            Matrix rotate = Matrix.CreateFromAxisAngle(new Vector3(0, 0, 1), -1 * value);
            m_front = Vector3.Normalize(Vector3.Transform(m_front, rotate));
            m_up = Vector3.Normalize(Vector3.Transform(m_up, rotate));
            m_upRoll = Vector3.Normalize(Vector3.Transform(m_upRoll, rotate));
            m_needCompute = true;
        }

        /// <summary>
        /// Fait effectuer une rotation de la caméra autour de l'axe "Front".
        /// </summary>
        /// <param name="value"></param>
        public void RotateRoll(float value)
        {
            Matrix rotate = Matrix.CreateFromAxisAngle(m_front, -value);
            m_upRoll = Vector3.Normalize(Vector3.Transform(m_up, rotate));

            m_needCompute = true;
        }

        /// <summary>
        /// Fait effectuer une rotation de la caméra de "haut en bas".
        /// </summary>
        /// <param name="value"></param>
        public void RotateUpDown(float value)
        {
            Vector3 side = Vector3.Cross(m_up, m_front);
            Matrix rotate = Matrix.CreateFromAxisAngle(side, -1 * value); //Convention trigo
            m_front = Vector3.Normalize(Vector3.Transform(m_front, rotate));
            m_up = Vector3.Normalize(Vector3.Transform(m_up, rotate));
            m_upRoll = Vector3.Normalize(Vector3.Transform(m_upRoll, rotate));
            m_needCompute = true;
        }
        /// <summary>
        /// Avance ou recule de la valeur désirée.
        /// </summary>
        /// <param name="value"></param>
        public void MoveForward(float value)
        {
            m_position += Vector3.Normalize(m_front) * value;
        }
        /// <summary>
        /// Avance ou recule de la valeur désirée.
        /// </summary>
        /// <param name="value"></param>
        public void MoveSide(float value)
        {
            Vector3 cameraSide = Vector3.Cross(m_up, m_front);
            m_position += Vector3.Normalize(cameraSide) * value;
        }
        /// <summary>
        /// Monde ou descend de la valeur désirée.
        /// </summary>
        /// <param name="value"></param>
        public void MoveUp(float value)
        {
            m_position += Vector3.Normalize(m_up) * value;
        }
        #endregion

    }
}
