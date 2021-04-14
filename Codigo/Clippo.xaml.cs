using System;
using System.Collections.Generic;
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
    /// Lógica de interacción para Clippo.xaml
    /// </summary>
    public partial class Clippo : Window
    {
        private string[] consejosStrings = new string[9];
        public Clippo()
        {
            consejosStrings[0] = "Pulsa sobre la carpeta y elige tu proyecto .inator";
            consejosStrings[1] = "Pulsa sobre el papel azul con el + y se creará uno nuevo";
            consejosStrings[2] = "Pulsa sobre el disquete y selecciona la ruta y el nombre de tu archivo .inator";
            consejosStrings[3] = "Pulsa sobre la caja con la flecha y selecciona la ruta y el nombre de tu archivo .png";
            consejosStrings[4] = "Pulsa sobre la tabla y añade todas las tablas o filas dentro de una tabla que quieras.\nTambién puedes introducir los datos usando una fórmula pulsando sobre la pizarra en esa misma ventana";
            consejosStrings[5] = "Pulsa el icono de la gráfica y en esa ventana podrás cambiar el tipo de gráfica y su color respectivamente";
            consejosStrings[6] = "Pero si ya estás en la ayuda.\n...\nSi necesitas más ayuda deberías hablar con un amigo\n...\nA menos que yo sea tu único amigo...  :(";
            consejosStrings[7] = "Es el grandioso Fausto Sánchez Hoya. \nFaustosanchezhoya@gmail.com\nfaustosanchezhoya.com\nlinkedin.com/in/faustosh/\ngithub.com/FaustoSH";
            consejosStrings[8] = "Eeeeeeeeeeeh...\n...\n...\n...\nNo sé si tengo permitido contarte esto";
            InitializeComponent();
            this.Loaded += Clippo_Loaded;
        }

        /*
         * ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
         * Métodos destinados a la gestión de la ventana
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
         * Métodos destinados a escribir los diferentes consejos en el textBlock
         * ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++         
         */
        //Cuando carga la ventana escribe el primer consejo
        private void Clippo_Loaded(object sender, RoutedEventArgs e)
        {
            
            EscribirConsejo();
        }
        //Escribe el consejo correspondiente con la duda seleccionada
        private void EscribirConsejo()
        {
            consejos.Text = consejosStrings[consejosCombobox.SelectedIndex];
        }
        //Cuando cambia de duda reescribe el consejo
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(this.IsLoaded)
                EscribirConsejo();
        }
    }
}
