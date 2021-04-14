using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Practica_Final
{
    /// <summary>
    /// Lógica de interacción para GraphicsSettingsWindow.xaml
    /// </summary>
    public partial class GraphicsSettingsWindow : Window
    {
        /*
         * ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
         * Delegados
         * ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++         
         */
        public delegate void CambiarTipoGrafica();
        public delegate void CambiarColorGrafica(Color newColor);
        public CambiarTipoGrafica cambiarTipoGrafica;
        public CambiarColorGrafica cambiarColorGrafica;

        /*
         * ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
         * Variables destinadas al funcionamiento general de la ventana
         * ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++         
         */
        public Color color { get; set; }
        private int[] colores = new int[16];


        public GraphicsSettingsWindow()
        {
            InitializeComponent();
        }
        
        /*
         * ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
         * Métodos destinados a la gestión general de la ventana
         * ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++         
         */
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }

        /*
         * ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
         * Métodos destinados a la gestión de los botones de la ventana
         * ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++         
         */
        //Llama a la función de la ventana principal de cambiar el tipo de gráfica a través de un delegado
        private void graficsButton_Click(object sender, RoutedEventArgs e)
        {
            cambiarTipoGrafica();
        }
        //Esconde la ventana
        private void aceptButton_Click(object sender, RoutedEventArgs e)
        {
            Hide();
        }
        //Abre un color dialog y si se acepta el color llama mediante un delegado a la función de cambio de color de la ventana principal
        private void colorButton_Click(object sender, RoutedEventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();
            colorDialog.AnyColor = true;
            colorDialog.FullOpen = true;
            colorDialog.SolidColorOnly = false;
            colorDialog.Color = System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B);
            colorDialog.CustomColors = colores;
            if (colorDialog.ShowDialog()==System.Windows.Forms.DialogResult.OK)
            {
                int[] coloresAux = (int[])colorDialog.CustomColors.Clone();
                for (int i = 0; i < 16; i++)
                {
                    colores[i] = coloresAux[i];
                }
                if (color != Color.FromArgb(colorDialog.Color.A, colorDialog.Color.R, colorDialog.Color.G, colorDialog.Color.B))
                {
                    color = Color.FromArgb(colorDialog.Color.A, colorDialog.Color.R, colorDialog.Color.G, colorDialog.Color.B);
                    cambiarColorGrafica(color);
                }
            }
        }

    }
}
