using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Labotlane.GameComponents.Controlers.Interpreter
{
    /// <summary>
    /// Représente un rapport d'erreurs.
    /// </summary>
    public class ErrorReport : OutputSystem
    {
        /// <summary>
        /// Message de l'erreur.
        /// </summary>
        public List<string> Messages { get; set; }
        /// <summary>
        /// Obtient ou définit le script original à partir duquel le report est créé.
        /// </summary>
        public string OriginalScript { get; set; }

        #region OutputSystem Implementation
        public void PrintError(string s)
        {
            Messages.Add(s);
        }

        public void PrintStatus(string s)
        {
            Messages.Add(s);
        }

        public void Print(string s)
        {
            Messages.Add(s);
        }

        public void PrintObject(object o)
        {

        }
        #endregion

        /// <summary>
        /// Crée une nouvelle instance de ErrorReport.
        /// </summary>
        public ErrorReport()
        {
            Messages = new List<string>();
        }

        /// <summary>
        /// Affiche le rapport d'erreur dans une Windows Form.
        /// </summary>
        public void Display()
        {
            // Affichage des erreurs.
            System.Windows.Forms.Form form = new System.Windows.Forms.Form();
            form.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            form.Size = new System.Drawing.Size(640, 480);
            form.Text = "Rapport d'erreurs Script.";
            form.Padding = new System.Windows.Forms.Padding(5);
            form.BackColor = System.Drawing.Color.White;
            System.Windows.Forms.TextBox text = new System.Windows.Forms.TextBox();
            text.Dock = System.Windows.Forms.DockStyle.Fill;
            text.BackColor = System.Drawing.Color.Black;
            text.ForeColor = System.Drawing.Color.Green;
            text.Multiline = true;
            text.Margin = new System.Windows.Forms.Padding(5);
            text.Font = new System.Drawing.Font("Courier New", 9.0f, System.Drawing.FontStyle.Regular);
            text.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            //text.Font = System.Drawing.SystemFonts.DialogFont;
            form.Controls.Add(text);


            text.Text = GetErrorReportText();
            form.Show();
        }
        /// <summary>
        /// Retourne un string représentant le rapport d'erreur tel qu'il peut être affiché
        /// à un humain.
        /// </summary>
        /// <param name="originalScript"></param>
        /// <returns></returns>
        public string GetErrorReportText()
        {
            StringBuilder b = new StringBuilder();
            b.AppendLine("Exécution du script : ");
            b.AppendLine("-----------------------");
            b.AppendLine(Tools.StringUtils.PutLineNumbers(OriginalScript));
            b.AppendLine("-----------------------");
            b.AppendLine("Erreurs : ");
            foreach (string error in Messages)
            {
                b.AppendLine(error);
            }
            return b.ToString();
        }

    }
}
