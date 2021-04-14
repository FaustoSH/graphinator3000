using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Practica_Final
{
    /*
    * ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    * Clase tabla donde contienen los tabItems y toda la información
    * ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++         
    */
    class Tabla
    {
        public delegate void DatosActualizados();
        public DatosActualizados datosActualizados;
        public TabItem tab { get; set; }
        public ScrollViewer sv { get; set; }
        public Grid g { get; set; }
        public StackPanel spx { get; set; }
        public StackPanel spy { get; set; }
        public Tabla(String tabName)
        {
            tab = new TabItem();
            sv = new ScrollViewer();
            g = new Grid();
            spx = new StackPanel();
            spy = new StackPanel();

            sv.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;

            spx.Margin = new Thickness(20, 0, 0, 0);
            spy.Margin = new Thickness(0, 0, 20, 0);

            g.ColumnDefinitions.Add(new ColumnDefinition());
            g.ColumnDefinitions.Add(new ColumnDefinition());
            g.RowDefinitions.Add(new RowDefinition());

            Grid.SetRow(spx, 0);
            Grid.SetRow(spy, 0);
            Grid.SetColumn(spx, 0);
            Grid.SetColumn(spy, 1);

            Label x = new Label();
            x.Content = "X";
            x.FontFamily = new FontFamily("Impact");
            x.FontSize = 30;
            x.HorizontalAlignment = HorizontalAlignment.Center;
            x.VerticalAlignment = VerticalAlignment.Center;
            Label y = new Label();
            y.Content = "Y";
            y.FontFamily = new FontFamily("Impact");
            y.FontSize = 30;
            y.HorizontalAlignment = HorizontalAlignment.Center;
            y.VerticalAlignment = VerticalAlignment.Center;

            spx.Children.Add(x);
            spy.Children.Add(y);

            ParDatos newParDatos = new ParDatos();
            newParDatos.insertDownD += insertDown;
            newParDatos.insertUpD += insertUp;
            newParDatos.deleteData += deleteData;
            newParDatos.datosActualizados += datosActualizadosTabla;
            spx.Children.Add(newParDatos.x);
            spy.Children.Add(newParDatos.y);

            g.Children.Add(spx);
            g.Children.Add(spy);

            sv.Content = g;

            tab.Header = tabName;
            tab.Content = sv;

        }

        //Inserta un nuevo parDato debajo
        private void insertDown(ParDatos parDatos)
        {
            ParDatos newParDatos = new ParDatos();
            newParDatos.insertDownD += insertDown;
            newParDatos.insertUpD += insertUp;
            newParDatos.deleteData += deleteData;
            newParDatos.datosActualizados += datosActualizadosTabla;
            spx.Children.Insert(spx.Children.IndexOf(parDatos.x)+1, newParDatos.x);
            spy.Children.Insert(spy.Children.IndexOf(parDatos.y)+1, newParDatos.y);
        }
        //Inserta un nuevo parDato arriba
        private void insertUp(ParDatos parDatos)
        {
            ParDatos newParDatos = new ParDatos();
            newParDatos.insertDownD += insertDown;
            newParDatos.insertUpD += insertUp;
            newParDatos.deleteData += deleteData;
            newParDatos.datosActualizados += datosActualizadosTabla;
            spx.Children.Insert(spx.Children.IndexOf(parDatos.x) , newParDatos.x);
            spy.Children.Insert(spy.Children.IndexOf(parDatos.y) , newParDatos.y);
        }
        //Borra el parDato seleccionado
        private void deleteData(ParDatos parDatos)
        {
            if (spx.Children.Count > 2&&spy.Children.Count>2)
            {
                spx.Children.Remove(parDatos.x);
                spy.Children.Remove(parDatos.y);
            }
        }
        //Añade un parDato a partir de dos valores
        public void NewMathsItem(float xValue, float yValue)
        {
            ParDatos newParDatos = new ParDatos(xValue, yValue);
            newParDatos.insertDownD += insertDown;
            newParDatos.insertUpD += insertUp;
            newParDatos.deleteData += deleteData;
            newParDatos.datosActualizados += datosActualizadosTabla;
            spx.Children.Add(newParDatos.x);
            spy.Children.Add(newParDatos.y);
        }
        //Añade un parDato a partir de dos valores en una posición determinada
        public void insertWithValues(int index, float xValue, float yValue)
        {
            ParDatos newParDatos = new ParDatos(xValue, yValue);
            newParDatos.insertDownD += insertDown;
            newParDatos.insertUpD += insertUp;
            newParDatos.deleteData += deleteData;
            newParDatos.datosActualizados += datosActualizadosTabla;
            spx.Children.Insert(index, newParDatos.x);
            spy.Children.Insert(index, newParDatos.y);
        }
        //LLamada de delegados hacia arriba para cambiar la gráfica
        public void datosActualizadosTabla()
        {
            datosActualizados();
        }

    }

    /*
    * ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    * Clase parDatos donde están los dos textBox que tienen los datos de x y de y
    * ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++         
    */
    class ParDatos
    {
        public delegate void InsertDataDown(ParDatos parDatos);
        public InsertDataDown insertDownD;
        public delegate void InsertDataUp(ParDatos parDatos);
        public InsertDataDown insertUpD;
        public delegate void DeleteData(ParDatos parDatos);
        public DeleteData deleteData;
        public delegate void DatosActualizados();
        public DatosActualizados datosActualizados;
        public TextBox x { get; set; }
        public TextBox y { get; set; }
        public ParDatos()
        {
            x = new TextBox();
            y = new TextBox();
            x.Text = "0";
            x.FontSize = 25;
            x.PreviewKeyDown += PreviewKeyDown;
            x.KeyDown += KeyDown;
            x.PreviewTextInput += PreviewTextInput;
            x.MouseRightButtonUp += MouseRightButtonUp;
            x.TextChanged += TextChanged;
            y.Text = "0";
            y.FontSize = 25;
            y.PreviewKeyDown += PreviewKeyDown;
            y.KeyDown += KeyDown;
            y.PreviewTextInput += PreviewTextInput;
            y.MouseRightButtonUp += MouseRightButtonUp;
            y.TextChanged += TextChanged;
        }

        public ParDatos(float xValue, float yValue)
        {
            x = new TextBox();
            y = new TextBox();
            x.Text = xValue.ToString();
            x.FontSize = 25;
            x.PreviewKeyDown += PreviewKeyDown;
            x.KeyDown += KeyDown;
            x.PreviewTextInput += PreviewTextInput;
            x.MouseRightButtonUp += MouseRightButtonUp;
            x.TextChanged += TextChanged;
            y.Text = yValue.ToString();
            y.FontSize = 25;
            y.PreviewKeyDown += PreviewKeyDown;
            y.KeyDown += KeyDown;
            y.PreviewTextInput += PreviewTextInput;
            y.MouseRightButtonUp += MouseRightButtonUp;
            y.TextChanged += TextChanged;
        }
        //Cambia la gráfica llamando a los delegados cuando se han cambiado los datos
        private void TextChanged(object sender, TextChangedEventArgs e)
        {
            datosActualizados();
        }
        //Controla los comandos por teclado
        private void KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                if (e.KeyboardDevice.Modifiers == System.Windows.Input.ModifierKeys.Shift)
                    insertUpD(this);
                else if (e.KeyboardDevice.Modifiers == System.Windows.Input.ModifierKeys.None)
                    insertDownD(this);
            }
            
        }
        //Controla los comandos por teclado que se tenga que modificar su preview
        private void PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Delete)
            {
                deleteData(this);
                return;
            }
            if (e.KeyboardDevice.Modifiers == System.Windows.Input.ModifierKeys.Control && e.Key == System.Windows.Input.Key.V)
            {
                if (Clipboard.ContainsText(TextDataFormat.Text))
                {
                    String text = Clipboard.GetText(TextDataFormat.Text);
                    var regex = new Regex(@"^[-]?[0-9]*(?:\.[0-9]*)?$");
                    if (regex.IsMatch(text))
                    {
                        TextBox tb = sender as TextBox;
                        if ((tb.Text.Contains(".") && text.Contains(".")) || (tb.Text.Contains("-") && text.Contains("-")))
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
        //Menú que salta al hacer click derecho
        private void MouseRightButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ContextMenu cm = new ContextMenu();
            MenuItem cortar = new MenuItem();
            cortar.Header = "Cortar (Ctrl+X)";
            cortar.Click += Cortar_Click;
            MenuItem copiar = new MenuItem();
            copiar.Header = "Copiar (Ctrl+C)";
            copiar.Click += Copiar_Click;
            MenuItem pegar = new MenuItem();
            pegar.Header = "Pegar (Ctrl+V)";
            pegar.Click += Pegar_Click;
            MenuItem insertUP = new MenuItem();
            insertUP.Header = "Insertar arriba (Shift+Enter)";
            insertUP.Click += InsertUP_Click;
            MenuItem insertDown = new MenuItem();
            insertDown.Header = "Insertar abajo (Enter)";
            insertDown.Click += InsertDown_Click;
            MenuItem delete = new MenuItem();
            delete.Header = "Eliminar fila (Supr)";
            delete.Click += Delete_Click;


            cm.Items.Add(cortar);
            cm.Items.Add(copiar);
            cm.Items.Add(pegar);
            cm.Items.Add(insertUP);
            cm.Items.Add(insertDown);
            cm.Items.Add(delete);

            TextBox aux = sender as TextBox;
            aux.ContextMenu = cm;
        }


        /*
         * ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
         * Métodos destinados a la gestión de las opciones del menú del click 
         * derecho
         * ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++         
         */
        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            deleteData(this);
        }

        private void InsertDown_Click(object sender, RoutedEventArgs e)
        {
            insertDownD(this);
        }

        private void InsertUP_Click(object sender, RoutedEventArgs e)
        {
            insertUpD(this);
        }

        private void Pegar_Click(object sender, RoutedEventArgs e)
        {
            if (Clipboard.ContainsText(TextDataFormat.Text))
            {
                String text = Clipboard.GetText(TextDataFormat.Text);
                var regex = new Regex(@"^[-]?[0-9]*(?:\.[0-9]*)?$");
                if (regex.IsMatch(text))
                {
                    if (x.ContextMenu != null)
                    {
                        if (sender.Equals(x.ContextMenu.Items[2]))
                        {
                            if (!((x.Text.Contains(".") && text.Contains(".")) || (x.Text.Contains("-") && text.Contains("-"))))
                            {
                                x.Text = x.Text.Insert(x.SelectionStart, text);
                            }
                        }
                    }
                    if (y.ContextMenu != null)
                    {
                        if (sender.Equals(y.ContextMenu.Items[2]))
                        {
                            if (!((x.Text.Contains(".") && text.Contains(".")) || (x.Text.Contains("-") && text.Contains("-"))))
                            {
                                y.Text = y.Text.Insert(y.SelectionStart, text);
                            }
                        }
                    }
                }
            }
        }

        private void Copiar_Click(object sender, RoutedEventArgs e)
        {
            if (x.ContextMenu != null)
            {
                if (sender.Equals(x.ContextMenu.Items[1]))
                {
                    if (!x.SelectedText.Equals(""))
                    {
                        Clipboard.SetText(x.SelectedText, TextDataFormat.Text);
                    }
                    else
                    {
                        Clipboard.SetText(x.Text, TextDataFormat.Text);
                    }
                }
            }
            if (y.ContextMenu != null)
            {
                if (sender.Equals(y.ContextMenu.Items[1]))
                {
                    if (!y.SelectedText.Equals(""))
                    {
                        Clipboard.SetText(y.SelectedText, TextDataFormat.Text);
                    }
                    else
                    {
                        Clipboard.SetText(y.Text, TextDataFormat.Text);
                    }
                }
            }
            
        }

        private void Cortar_Click(object sender, RoutedEventArgs e)
        {
            if (x.ContextMenu != null)
            {
                if (sender.Equals(x.ContextMenu.Items[0]))
                {
                    if (!x.SelectedText.Equals(""))
                    {
                        Clipboard.SetText(x.SelectedText, TextDataFormat.Text);
                        x.Text=x.Text.Remove(x.SelectionStart, x.SelectionLength);
                    }
                    else
                    {
                        Clipboard.SetText(x.Text, TextDataFormat.Text);
                        x.Text = "";
                    }
                }
            }
            if (y.ContextMenu != null)
            {
                if (sender.Equals(y.ContextMenu.Items[0]))
                {
                    if (!y.SelectedText.Equals(""))
                    {
                        Clipboard.SetText(y.SelectedText, TextDataFormat.Text);
                        y.Text=y.Text.Remove(y.SelectionStart, y.SelectionLength);
                    }
                    else
                    {
                        Clipboard.SetText(y.Text, TextDataFormat.Text);
                        y.Text = "";
                    }
                }
            }
        }



        //Gestiona lo que se puede escribir 
        private void PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
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
                    if (!tb.Text.Contains("-") && tb.SelectionStart == 0)
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
        
    }

}
