using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Lógica de interacción para DataMathWindow.xaml
    /// </summary>
    public partial class DataMathWindow : Window
    {
        /*
         * ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
         * Variables destinadas al funcionamiento general de la ventana
         * ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++         
         */
        public int grado = 0;

        public DataMathWindow()
        {
            InitializeComponent();
            xO.PreviewTextInput += PreviewTextInput;
            xF.PreviewTextInput += PreviewTextInput;
            xO.PreviewKeyDown += PreviewKeyDown;
            xF.PreviewKeyDown += PreviewKeyDown;
            xO.PreviewMouseRightButtonUp += PreviewMouseRightButtonUp;
            xF.PreviewMouseRightButtonUp += PreviewMouseRightButtonUp;
            grado = int.Parse((string)gradoLB.Content);
            Label label = new Label();
            label.Content = "x" + grado;
            label.Foreground = Brushes.Black;
            label.FontSize = 20;
            label.FontFamily = new FontFamily("Impcat");
            label.VerticalAlignment = VerticalAlignment.Center;
            TextBox textBox = new TextBox();
            textBox.Text = "0";
            textBox.FontSize = 20;
            textBox.VerticalContentAlignment = VerticalAlignment.Center;
            textBox.Margin = new Thickness(10);
            textBox.PreviewTextInput += PreviewTextInput;
            textBox.PreviewMouseRightButtonUp += PreviewMouseRightButtonUp;
            textBox.PreviewKeyDown += PreviewKeyDown;
            dataStackPanel.Children.Add(textBox);
            dataStackPanel.Children.Add(label);
        }


        /*
         * ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
         * Métodos destinados a la gestión general de la ventana
         * ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++         
         */
        //Si se acepta el polinomio y cumple los requerimientos pone el dialogResult a true
        private void AceptButton_Click(object sender, RoutedEventArgs e)
        {
            if (xO.Text.Length > 0)
            {
                if (xO.Text.Equals("-") || xO.Text.Equals("."))
                    xO.Text = "0";
            }
            else
                xO.Text = "0";
            if (xF.Text.Length > 0)
            {
                if (xF.Text.Equals("-") || xF.Text.Equals("."))
                    xF.Text = "0";
            }
            else
                xF.Text = "0";
            for (int i = 0; i < dataStackPanel.Children.Count; i = i + 2)
            {
                if (((TextBox)dataStackPanel.Children[i]).Text.Length > 0)
                {
                    if (((TextBox)dataStackPanel.Children[i]).Text.Equals("-") || ((TextBox)dataStackPanel.Children[i]).Text.Equals("."))
                        ((TextBox)dataStackPanel.Children[i]).Text = "0";
                }
                else
                    ((TextBox)dataStackPanel.Children[i]).Text = "0";
            }
            DialogResult = true;
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (DialogResult == null)
            {
                DialogResult = false;
            }
        }

        /*
         * ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
         * Métodos destinados a la gestión de lo que se puede copiar, pegar y 
         * escribir dentro de los cuadros de texto
         * ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++         
         */
        //Evita que pueda salir el menú del click derecho
        private new void PreviewMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            //El click derecho en esta ventana está desactivado a propósito
            e.Handled = true;
        }
        //Restringe el pegado mediante ctrl+v
        private new void PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyboardDevice.Modifiers == System.Windows.Input.ModifierKeys.Control && e.Key == System.Windows.Input.Key.V)
            {
                if (Clipboard.ContainsText(TextDataFormat.Text))
                {
                    String text = Clipboard.GetText(TextDataFormat.Text);
                    var regex = new Regex(@"^[-]?[0-9]*(?:\.[0-9]*)?$");
                    if (regex.IsMatch(text))
                    {
                        TextBox tb = sender as TextBox;
                        if ((tb.Text.Contains(".") && text.Contains("."))||(tb.Text.Contains("-") && text.Contains("-")))
                        {
                            e.Handled = true;
                        }
                        else
                        {
                            e.Handled = false;
                        }
                    }
                    else
                        e.Handled = true;
                }
            }
        }
        //Restringe los carácteres que se pueden introducir en el textbox
        private new void PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            TextBox tb = sender as TextBox;
            switch (e.Text)
            {
                case "0":
                case "1":
                case "2":
                case "3":
                case "4":
                case "5":
                case "6":
                case "7":
                case "8":
                case "9":
                    e.Handled = false;
                    break;
                case ".":
                    if (!tb.Text.Contains("."))
                    {
                        e.Handled = false;
                    }
                    else
                        e.Handled = true;
                    break;
                case "-":
                    if (!tb.Text.Contains("-")&&tb.SelectionStart==0)
                    {
                        e.Handled = false;
                    }
                    else
                        e.Handled = true;
                    break;
                default:
                    e.Handled = true;
                    break;
            }
        }

        /*
         * ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
         * Métodos destinados a la gestión del grado del polinomio
         * ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++         
         */
        //Aumenta el grado
        private void plusButton_Click(object sender, RoutedEventArgs e)
        {
            grado++;
            gradoLB.Content = grado;
            Label label = new Label();
            label.Content = "x" + grado;
            label.Foreground = Brushes.Black;
            label.FontSize = 20;
            label.FontFamily = new FontFamily("Impcat");
            label.VerticalAlignment = VerticalAlignment.Center;
            TextBox textBox = new TextBox();
            textBox.Text = "0";
            textBox.FontSize = 20;
            textBox.VerticalContentAlignment = VerticalAlignment.Center;
            textBox.Margin = new Thickness(10);
            textBox.PreviewTextInput += PreviewTextInput;
            textBox.PreviewMouseRightButtonUp += PreviewMouseRightButtonUp;
            textBox.PreviewKeyDown += PreviewKeyDown;
            dataStackPanel.Children.Add(textBox);
            dataStackPanel.Children.Add(label);
        }
        //Baja el grado
        private void minusButton_Click(object sender, RoutedEventArgs e)
        {
            if (grado > 0)
            {
                dataStackPanel.Children.RemoveAt(grado*2+1);
                dataStackPanel.Children.RemoveAt(grado * 2);
                grado--;
                gradoLB.Content = grado;
            }
            
        }
    }
}
