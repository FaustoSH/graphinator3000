using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Practica_Final
{
    /// <summary>
    /// Lógica de interacción para DataConfigWindow.xaml
    /// </summary>
    /// 
    /*
    * ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    * Eventos y delegados
    * ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++         
    */
    //Tipo de evento para notificar a la ventana principal cuando se cambia el tamItem seleccionado
    public class SelectedIndexEventArgs : EventArgs
    {
        public int selectedIndex { get; set; }
        public SelectedIndexEventArgs(int sl) { selectedIndex = sl; }
    }
    //Event handler de ese evento
    public delegate void NewSelectedIndexEventHandler(Object sender, SelectedIndexEventArgs e);
    
    public partial class DataConfigWindow : Window
    {
        public event NewSelectedIndexEventHandler NewSelectedIndexControl;
        public delegate void NewTabItem();
        public delegate void RemoveTabItem(int tabIndex);
        public delegate void SortTable(int tabIndex);
        public delegate void NewMathsItems(float x0,float xF,float []dataArray, int tabIndex);
        public NewTabItem newTabItem;
        public RemoveTabItem removeTabItem;
        public SortTable sortTable;
        public NewMathsItems newMathsItems;

        public DataConfigWindow()
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
        private void AceptButton_Click(object sender, RoutedEventArgs e)
        {
            Hide();
        }


        /*
         * ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
         * Métodos destinados a la gestión de los tabItems
         * ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++         
         */
        //Llama al evento cuando se detecta que el tabControl ha cambiado de tabItem seleccionado
        void OnNewSelectedIndex(int sl)
        {
            if (NewSelectedIndexControl != null)
            {
                NewSelectedIndexControl(this, new SelectedIndexEventArgs(sl));
            }
        }
        //Detecta que se ha cambiado el tabItem seleccionado
        private void TablasTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            OnNewSelectedIndex(tablasTabControl.SelectedIndex);
        }
        //Crea un nuevo tabItem llamando al delegado
        private void NewTableButton_Click(object sender, RoutedEventArgs e)
        {
            newTabItem();
        }
        //Elimina el tabItem actual llamando al delegado
        private void trashButton_Click(object sender, RoutedEventArgs e)
        {
            if (tablasTabControl.Items.Count > 0 && tablasTabControl.SelectedIndex >= 0)
                removeTabItem(tablasTabControl.SelectedIndex);
        }


        /*
         * ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
         * Métodos destinados a la gestión fina de los datos
         * ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++         
         */
        //Ordena los valores actuales llamando al delegado
        private void sortButton_Click(object sender, RoutedEventArgs e)
        {
            int selectedIndex = tablasTabControl.SelectedIndex;
            sortTable(tablasTabControl.SelectedIndex);
            tablasTabControl.SelectedIndex = selectedIndex;
        }
        //Abre la ventana modal en la que se podrá insertar el polinomio y si se introduce uno válido inserta los datos llamando al delegado
        private void MathsButton_Click(object sender, RoutedEventArgs e)
        {
            DataMathWindow dcm = new DataMathWindow();
            dcm.Owner = this;
            dcm.ShowDialog();
            if (dcm.DialogResult == true)
            {
                CultureInfo cultura = (CultureInfo)CultureInfo.CurrentCulture.Clone();
                cultura.NumberFormat.NumberDecimalSeparator = ".";
                float xO = float.Parse(dcm.xO.Text, cultura);
                float xF = float.Parse(dcm.xF.Text, cultura);
                float[] dataArray = new float[dcm.dataStackPanel.Children.Count / 2];
                for (int i = 0; i < dcm.dataStackPanel.Children.Count / 2; i++)
                {
                    dataArray[i] = float.Parse(((TextBox)dcm.dataStackPanel.Children[i * 2]).Text);
                }
                if (xO < xF && tablasTabControl.Items.Count > 0 && tablasTabControl.SelectedIndex != -1)
                {
                    int selectedIndex = tablasTabControl.SelectedIndex;
                    newMathsItems(xO, xF, dataArray, selectedIndex);
                    tablasTabControl.SelectedIndex = selectedIndex;
                }
            }
        }
    }

 }
