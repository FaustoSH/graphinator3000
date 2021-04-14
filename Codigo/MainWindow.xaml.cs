using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Practica_Final
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Window
    {
        /*
         * ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
         * Variables destinadas al funcionamiento del easter egg
         * ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++         
         */
        private int contadorDeCasitas = 0;
        private Boolean pisitoEnLaPlaya = false;


        /*
         * ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
         * Variables destinadas al funcionamiento general de la ventana
         * ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++         
         */
        private ViewModel listaTablas=null;
        private int tablaActual=0;
        private DataConfigWindow dcw=null;
        private GraphicsSettingsWindow gsw = null;
        private Clippo clippo = null;
        private ScaleTransform sc;
        private TranslateTransform tt;
        private TransformGroup tg;
        private bool polilineaActivada = true;
        private Polyline graficaPolilinea;
        private List<Line> barras;
        private List<float> datosGraficaX;
        private List<float> datosGraficaY;
        private Color color= Color.FromRgb(255,0,0);
        private Line x;
        private Line y;
        private Polyline rectangulo;
        private double thickness;
        private bool haciendoRectangulo=false;
        private double ancho, alto;

        public MainWindow()
        {
            System.Globalization.CultureInfo customCulture = (System.Globalization.CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
            customCulture.NumberFormat.NumberDecimalSeparator = ".";

            System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
            
            listaTablas = new ViewModel();
            listaTablas.datosActualizados += DibujarGrafica;
            sc = new ScaleTransform();
            tt = new TranslateTransform();
            tg = new TransformGroup();
            tg.Children.Add(tt);
            tg.Children.Add(sc);
            graficaPolilinea = null;
            x = null;
            y = null;
        }

        /*
         * ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
         * Métodos relacionados con la gestión de la ventana y los cambios en ella
         * ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++         
         */
        //Inicia la formación de los ejes del gráfico cuando se ha cargado la ventana
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            IniciarGrafico();
            
        }
        //Gestiona los tamaños de las letras cuando se ha cambiado el tamaño de la ventana
        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Label numTabla = tableNumLabel;
            Label leyenda = legendLabel;
            Grid gr = mainGrid;

            numTabla.FontSize = (int)((gr.ActualWidth * 0.3 + gr.ActualHeight * 0.7) / 30);
            legendLabel.FontSize = (int)((gr.ActualWidth * 0.3 + gr.ActualHeight * 0.7) / 30);
            if (legendStackPanel.Children.Count > 1)
            {
                for (int i = 1; i < legendStackPanel.Children.Count; i++)
                {
                    ((Label)legendStackPanel.Children[i]).FontSize = (int)((gr.ActualWidth * 0.3 + gr.ActualHeight * 0.7) / 35);
                }
            }
        }


        /*
         * ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
         * Métodos relacionados con los botones de la ventana principal
         * ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++         
         */
        //Abre un dialog para extraer el archivo del que sacar los datos y llama a la función para hacerlo
        private void openButton_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();
            ofd.CheckFileExists = true;
            ofd.CheckPathExists = true;
            ofd.DefaultExt = ".inator";
            ofd.Filter = "Tablas de datos (.inator)|*.inator";
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                String fileName = ofd.FileName;
                if (listaTablas.importData(fileName) == -1)
                    System.Windows.Forms.MessageBox.Show("Archivo corrupto", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                else
                {
                    for (int i = legendStackPanel.Children.Count - 1; i >= 1; i--)
                    {
                        legendStackPanel.Children.RemoveAt(i);
                    }
                    DibujarGrafica();
                }  
            }
        }      
        //Vacia todos los datos avisando antes con un dialog de que se van a borrar
        private void NewButton_Click(object sender, RoutedEventArgs e)
        {

            if (System.Windows.Forms.MessageBox.Show("Se perderán todos los datos que hay actualmente", "Atención", System.Windows.Forms.MessageBoxButtons.OKCancel, System.Windows.Forms.MessageBoxIcon.Warning) == System.Windows.Forms.DialogResult.OK)
            {
                listaTablas = new ViewModel();
                listaTablas.datosActualizados += DibujarGrafica;
                if (dcw != null)
                {
                    dcw.tablasTabControl.ItemsSource = listaTablas.tabControlIS;
                }
                for (int i = legendStackPanel.Children.Count - 1; i >= 1; i--)
                {
                    legendStackPanel.Children.RemoveAt(i);
                }
                DibujarGrafica();
            }
            
        }
        //Abre un dialog para especificar la ruta en la que se van a guardar los datos y llama a la función que lo hace
        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.SaveFileDialog sfd = new System.Windows.Forms.SaveFileDialog();
            sfd.Filter = "Tablas de datos (.inator)|*.inator";
            sfd.DefaultExt = ".inator";
            sfd.CheckFileExists = false;
            if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                String fileName = sfd.FileName;
                if (!fileName.Contains(".inator"))
                {
                    fileName += ".inator";
                }
                if (listaTablas.saveData(fileName) == -1)
                {
                    System.Windows.Forms.MessageBox.Show("No se ha podido guardar", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                }
            }
        }
        //Abre un dialog para especificar la rua en la que se va a guardar un png del gráfico
        private void exportButton_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.SaveFileDialog sfd = new System.Windows.Forms.SaveFileDialog();
            sfd.Filter = "Tablas de datos (.png)|*.png";
            sfd.DefaultExt = ".png";
            sfd.CheckFileExists = false;
            if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                String fileName = sfd.FileName;
                if (!fileName.Contains(".png"))
                {
                    fileName += ".png";
                }
                //El siguiente código para exportar como png lo he sacado de: 
                // https://jasonkemp.ca/blog/how-to-save-xaml-as-an-image/
                Rect rect = new Rect(grafico.RenderSize);
                RenderTargetBitmap rtb = new RenderTargetBitmap((int)rect.Right,
                  (int)rect.Bottom, 96d, 96d, System.Windows.Media.PixelFormats.Default);
                rtb.Render(grafico);
                //endcode as PNG
                BitmapEncoder pngEncoder = new PngBitmapEncoder();
                pngEncoder.Frames.Add(BitmapFrame.Create(rtb));

                //save to memory stream
                System.IO.MemoryStream ms = new System.IO.MemoryStream();

                pngEncoder.Save(ms);
                ms.Close();
                System.IO.File.WriteAllBytes(fileName, ms.ToArray());

            }

        }
        //Abre una ventana no modal en la que se podrán gestionar los tabItems y los diferentes datos en cada uno
        private void DataConfigButton_Click(object sender, RoutedEventArgs e)
        {
            if (dcw == null)
            {
                dcw = new DataConfigWindow();
                dcw.Owner = this;
                dcw.tablasTabControl.ItemsSource = listaTablas.tabControlIS;
                dcw.tablasTabControl.SelectedIndex = tablaActual;
                dcw.NewSelectedIndexControl += Dlg_newSelectedIndex;
                dcw.newTabItem += NewTabItem;
                dcw.removeTabItem += RemoveTamItem;
                dcw.newMathsItems += NewMathsItems;
                dcw.sortTable += SortTable;
                dcw.Show();
            }
            else
            {
                dcw.Show();
            }


        }
        //Abre una ventana no modal en la que se podrá gestionar tanto el color como el tipo de gráfica
        private void graficsButton_Click(object sender, RoutedEventArgs e)
        {
            if (gsw == null)
            {
                gsw = new GraphicsSettingsWindow();
                gsw.Owner = this;
                gsw.color = color;
                gsw.cambiarTipoGrafica += CambiarTipoGrafica;
                gsw.cambiarColorGrafica += CambiarColorGrafica;
                gsw.Show();
            }
            else
            {
                gsw.Show();
            }

        }
        //Abre una ventana no modal en la que se podrá obtener ayuda de nuestro amigo Clippo
        private void helpButton_Click(object sender, RoutedEventArgs e)
        {
            if (clippo == null)
            {
                clippo = new Clippo();
                clippo.Owner = this;
                clippo.Show();
            }
            else
            {
                clippo.Show();
            }
        }



        /*
         * ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
         * Métodos relacionados con la gestión del gráfico: 
         * ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++         
         */
        //Dibuja los ejes principales del gráfico
        private void IniciarGrafico()
        {
            rectangulo = new Polyline();
            //Código extraido de Studium
            grafico.Children.Clear();
            x = new Line();
            y = new Line();

            x.Stroke = Brushes.Black;
            x.StrokeThickness = 20 / ancho;
            x.X1 = -10;
            x.Y1 = 0;
            x.X2 = 10;
            x.Y2 = 0;

            x.RenderTransform = tg;

            grafico.Children.Add(x);

            y.Stroke = Brushes.Black;
            y.StrokeThickness = 20 / ancho;
            y.X1 = 0;
            y.Y1 = -10;
            y.X2 = 0;
            y.Y2 = 10;

            y.RenderTransform = tg;

            grafico.Children.Add(y);
            thickness = 100 / ancho;

        }
        //Dibuja la gráfica a partir de todos los valores del tabItem seleccionado
        private void DibujarGrafica()
        {
            if (listaTablas.tabControlIS.Count == 0 || tablaActual >= listaTablas.tabControlIS.Count)
            {
                IniciarGrafico();
                return;
            }
            graficaPolilinea = new Polyline();
            barras = new List<Line>();
            TabItem tabItem = listaTablas.tabControlIS[tablaActual];
            ScrollViewer sv = tabItem.Content as ScrollViewer;
            Grid g = sv.Content as Grid;
            StackPanel spx = g.Children[0] as StackPanel;
            StackPanel spy = g.Children[1] as StackPanel;
            datosGraficaX = new List<float>();
            datosGraficaY = new List<float>();
            for (int i = 1; i < spx.Children.Count; i++)
            {
                if (!(((TextBox)spx.Children[i]).Text.Equals("")) && !(((TextBox)spx.Children[i]).Text.Equals("-")) && !(((TextBox)spx.Children[i]).Text.Equals(".")))
                {
                    datosGraficaX.Add(float.Parse(((TextBox)spx.Children[i]).Text));
                }
                else
                {
                    datosGraficaX.Add(0);
                }
                if (!(((TextBox)spy.Children[i]).Text.Equals("")) && !(((TextBox)spy.Children[i]).Text.Equals("-")) && !(((TextBox)spy.Children[i]).Text.Equals(".")))
                {
                    datosGraficaY.Add(float.Parse(((TextBox)spy.Children[i]).Text));
                }
                else
                {
                    datosGraficaY.Add(0);
                }
            }
            double recorridoX = 0;
            double recorridoY = 0;
            float mayorX = datosGraficaX.First();
            float menorX = datosGraficaX.First();
            float mayorY = datosGraficaY.First();
            float menorY = datosGraficaY.First();
            for (int i = 1; i < datosGraficaX.Count; i++)
            {
                if (datosGraficaY[i] > mayorY)
                    mayorY = datosGraficaY[i];
                if (datosGraficaY[i] < menorY)
                    menorY = datosGraficaY[i];
                if (datosGraficaX[i] > mayorX)
                    mayorX = datosGraficaX[i];
                if (datosGraficaX[i] < menorX)
                    menorX = datosGraficaX[i];
            }
            if (datosGraficaX.Count == 1)
            {
                recorridoX = datosGraficaX[0];
                recorridoY = datosGraficaY[0];
            }
            else
            {
                if (menorX <= 0)
                    recorridoX = mayorX - menorX;
                else
                    recorridoX = mayorX;

                if (menorY <= 0)
                    recorridoY = mayorY - menorY;
                else
                    recorridoY = mayorY;
            }

            IniciarGrafico();
            for (int i = 0; i < datosGraficaX.Count; i++)
            {
                Line line = new Line();
                Point p = new Point();
                if (recorridoX == 0 && datosGraficaX.Count == 1)
                {
                    line.X1 = 0;
                    line.X2 = 0;
                    p.X = 0;
                }
                else if (recorridoX != 0 && datosGraficaX.Count == 1)
                {
                    if (recorridoX >= 0)
                    {
                        line.X1 = 10;
                        line.X2 = 10;
                        p.X = 10;
                    }
                    else
                    {
                        line.X1 = -10;
                        line.X2 = -10;
                        p.X = -10;
                    }

                }
                else if (recorridoX == 0 && datosGraficaX.Count > 1)
                {
                    recorridoX = 1;
                    line.X1 = datosGraficaX[i] * 10 / recorridoX;
                    line.X2 = datosGraficaX[i] * 10 / recorridoX;
                    p.X = datosGraficaX[i] * 10 / recorridoX;
                }
                else
                {
                    line.X1 = datosGraficaX[i] * 10 / recorridoX;
                    line.X2 = datosGraficaX[i] * 10 / recorridoX;
                    p.X = datosGraficaX[i] * 10 / recorridoX;
                }
                line.Y1 = 0;
                if (recorridoY == 0 && datosGraficaY.Count == 1)
                {
                    line.Y2 = 0;
                    p.Y = 0;
                }
                else if (recorridoY != 0 && datosGraficaY.Count == 1)
                {
                    if (recorridoY >= 0)
                    {
                        line.Y2 = -10;
                        p.Y = -10;
                    }
                    else
                    {
                        line.Y2 = 10;
                        p.Y = 10;
                    }

                }
                else if (recorridoY == 0 && datosGraficaY.Count > 1)
                {
                    recorridoY = 1;
                    line.Y2 = datosGraficaY[i] * -10 / recorridoY;
                    p.Y = datosGraficaY[i] * -10 / recorridoY;

                }
                else
                {
                    line.Y2 = datosGraficaY[i] * -10 / recorridoY;
                    p.Y = datosGraficaY[i] * -10 / recorridoY;
                }
                line.StrokeThickness = thickness;
                line.Stroke = new SolidColorBrush(color);
                line.RenderTransform = tg;
                barras.Add(line);
                graficaPolilinea.Points.Add(p);
                if (!polilineaActivada)
                    grafico.Children.Add(line);
            }
            if (polilineaActivada)
            {
                graficaPolilinea.StrokeThickness = thickness;
                graficaPolilinea.Stroke = new SolidColorBrush(color);
                graficaPolilinea.RenderTransform = tg;
                grafico.Children.Add(graficaPolilinea);
            }
        }
        //Redibuja la gráfica multiplicando todas sus coordenadas por 1.1 para simular un zoom in
        private void zoomInButton_Click(object sender, RoutedEventArgs e)
        {
            for (int i = grafico.Children.Count - 1; i > 1; i--)
            {
                grafico.Children.RemoveAt(i);
            }
            if (polilineaActivada)
            {
                ((Line)grafico.Children[0]).Y1 = ((Line)grafico.Children[0]).Y1 * 1.1;
                ((Line)grafico.Children[0]).Y2 = ((Line)grafico.Children[0]).Y1;
                ((Line)grafico.Children[1]).X1 = ((Line)grafico.Children[1]).X1 * 1.1;
                ((Line)grafico.Children[1]).X2 = ((Line)grafico.Children[1]).X1;
            }
            if (datosGraficaX != null)
            {
                if (polilineaActivada)
                {
                    for (int i = 0; i < graficaPolilinea.Points.Count; i++)
                    {
                        Point p = new Point();

                        p.X = graficaPolilinea.Points[i].X * 1.1;
                        p.Y = graficaPolilinea.Points[i].Y * 1.1;
                        graficaPolilinea.Points.RemoveAt(i);
                        graficaPolilinea.Points.Insert(i, p);


                    }
                    graficaPolilinea.StrokeThickness = thickness;
                    graficaPolilinea.Stroke = new SolidColorBrush(color);
                    graficaPolilinea.RenderTransform = tg;
                    grafico.Children.Add(graficaPolilinea);
                }
                else
                {
                    for (int i = 0; i < barras.Count; i++)
                    {
                        Line l = new Line();
                        l.X1 = barras[i].X1 * 1.1;
                        l.X2 = barras[i].X2 * 1.1;
                        l.Y1 = barras[i].Y1 * 1.1;
                        l.Y2 = barras[i].Y2 * 1.1;
                        l.StrokeThickness = thickness;
                        l.Stroke = new SolidColorBrush(color);
                        l.RenderTransform = tg;
                        barras.RemoveAt(i);
                        barras.Insert(i, l);
                        grafico.Children.Add(l);
                    }
                }
            }
        }
        //Redibuja la gráfica dividiendo todas sus coordenadas entre 1.1 para simular un zoom out
        private void zoomOutButton_Click(object sender, RoutedEventArgs e)
        {
            for (int i = grafico.Children.Count - 1; i > 1; i--)
            {
                grafico.Children.RemoveAt(i);
            }
            if (polilineaActivada)
            {
                ((Line)grafico.Children[0]).Y1 = ((Line)grafico.Children[0]).Y1 / 1.1;
                ((Line)grafico.Children[0]).Y2 = ((Line)grafico.Children[0]).Y1;
                ((Line)grafico.Children[1]).X1 = ((Line)grafico.Children[1]).X1 / 1.1;
                ((Line)grafico.Children[1]).X2 = ((Line)grafico.Children[1]).X1;
            }
            if (datosGraficaX != null)
            {
                if (polilineaActivada)
                {
                    for (int i = 0; i < graficaPolilinea.Points.Count; i++)
                    {
                        Point p = new Point();

                        p.X = graficaPolilinea.Points[i].X / 1.1;
                        p.Y = graficaPolilinea.Points[i].Y / 1.1;
                        graficaPolilinea.Points.RemoveAt(i);
                        graficaPolilinea.Points.Insert(i, p);


                    }
                    graficaPolilinea.StrokeThickness = thickness;
                    graficaPolilinea.Stroke = new SolidColorBrush(color);
                    graficaPolilinea.RenderTransform = tg;
                    grafico.Children.Add(graficaPolilinea);
                }
                else
                {
                    for (int i = 0; i < barras.Count; i++)
                    {
                        Line l = new Line();
                        l.X1 = barras[i].X1 / 1.1;
                        l.X2 = barras[i].X2 / 1.1;
                        l.Y1 = barras[i].Y1 / 1.1;
                        l.Y2 = barras[i].Y2 / 1.1;
                        l.StrokeThickness = thickness;
                        l.Stroke = new SolidColorBrush(color);
                        l.RenderTransform = tg;
                        barras.RemoveAt(i);
                        barras.Insert(i, l);
                        grafico.Children.Add(l);
                    }
                }
            }
        }
        //Redibuja la gráfica ajustando sus valores a la posición del ratón. A parte también inicia el rectángulo que purga los datos
        private void grafico_MouseMove(object sender, MouseEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                if (polilineaActivada)
                {
                    for (int i = grafico.Children.Count - 1; i > 1; i--)
                    {
                        grafico.Children.RemoveAt(i);
                    }
                }
                Point posicion = e.GetPosition(grafico);

                double xAdaptado = (posicion.X * 20 / ancho) - 10;
                double yAdaptado = (posicion.Y * 20 / alto) - 10;
                double diferenciaX = xAdaptado - y.X1;
                double diferenciaY = yAdaptado - x.Y1;
                if (polilineaActivada)
                {
                    x.Y1 = yAdaptado;
                    x.Y2 = yAdaptado;
                    y.X1 = xAdaptado;
                    y.X2 = xAdaptado;
                }
                if (datosGraficaX != null && polilineaActivada)
                {
                    for (int i = 0; i < graficaPolilinea.Points.Count; i++)
                    {
                        Point p = new Point();

                        p.X = graficaPolilinea.Points[i].X + diferenciaX;
                        p.Y = graficaPolilinea.Points[i].Y + diferenciaY;
                        graficaPolilinea.Points.RemoveAt(i);
                        graficaPolilinea.Points.Insert(i, p);


                    }
                    if (polilineaActivada)
                    {
                        graficaPolilinea.StrokeThickness = thickness;
                        graficaPolilinea.Stroke = new SolidColorBrush(color);
                        graficaPolilinea.RenderTransform = tg;
                        grafico.Children.Add(graficaPolilinea);
                    }
                }
            }
            if (Mouse.RightButton == MouseButtonState.Pressed)
            {
                if (haciendoRectangulo == false)
                {
                    grafico.Children.Remove(rectangulo);
                    haciendoRectangulo = true;
                    rectangulo.Points.Clear();
                    Point posicion = e.GetPosition(grafico);
                    rectangulo.Points.Add(new Point((posicion.X * 20 / ancho) - 10, (posicion.Y * 20 / alto) - 10));
                    rectangulo.Points.Add(new Point((posicion.X * 20 / ancho) - 10, (posicion.Y * 20 / alto) - 10));
                    rectangulo.Points.Add(new Point((posicion.X * 20 / ancho) - 10, (posicion.Y * 20 / alto) - 10));
                    rectangulo.Points.Add(new Point((posicion.X * 20 / ancho) - 10, (posicion.Y * 20 / alto) - 10));
                    rectangulo.Points.Add(new Point((posicion.X * 20 / ancho) - 10, (posicion.Y * 20 / alto) - 10));
                    rectangulo.RenderTransform = tg;
                    rectangulo.StrokeThickness = 50 / ancho;
                    rectangulo.Stroke = Brushes.BlueViolet;
                    grafico.Children.Add(rectangulo);
                }
                else
                {
                    grafico.Children.Remove(rectangulo);
                    Point posicion = e.GetPosition(grafico);

                    rectangulo.Points.RemoveAt(1);
                    rectangulo.Points.Insert(1, new Point((posicion.X * 20 / ancho) - 10, rectangulo.Points[0].Y));
                    rectangulo.Points.RemoveAt(2);
                    rectangulo.Points.Insert(2, new Point((posicion.X * 20 / ancho) - 10, (posicion.Y * 20 / alto) - 10));
                    rectangulo.Points.RemoveAt(3);
                    rectangulo.Points.Insert(3, new Point(rectangulo.Points[0].X, (posicion.Y * 20 / alto) - 10));

                    rectangulo.RenderTransform = tg;
                    rectangulo.StrokeThickness = 50 / ancho;
                    rectangulo.Stroke = Brushes.BlueViolet;
                    grafico.Children.Add(rectangulo);
                }
            }

        }
        //Finaliza el rectángulo que purga los datos y hace las comparaciones necesarias para eliminar los puntos y linas que no se encontraban dentro
        private void grafico_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (rectangulo.Points.Count == 5)
            {
                double xMayor = rectangulo.Points.First().X;
                double xMenor = rectangulo.Points.First().X;
                double yMayor = rectangulo.Points.First().Y;
                double yMenor = rectangulo.Points.First().Y;
                for (int i = 0; i < 5; i++)
                {
                    if (rectangulo.Points[i].X < xMenor)
                        xMenor = rectangulo.Points[i].X;
                    if (rectangulo.Points[i].X > xMayor)
                        xMayor = rectangulo.Points[i].X;
                    if (rectangulo.Points[i].Y < yMenor)
                        yMenor = rectangulo.Points[i].Y;
                    if (rectangulo.Points[i].Y > yMayor)
                        yMayor = rectangulo.Points[i].Y;

                }
                if (polilineaActivada && graficaPolilinea != null)
                {
                    Polyline recortada = new Polyline();
                    for (int i = 0; i < graficaPolilinea.Points.Count; i++)
                    {
                        if (graficaPolilinea.Points[i].X <= xMayor && graficaPolilinea.Points[i].X >= xMenor && graficaPolilinea.Points[i].Y <= yMayor && graficaPolilinea.Points[i].Y >= yMenor)
                        {
                            recortada.Points.Add(graficaPolilinea.Points[i]);
                        }
                    }
                    graficaPolilinea = recortada;
                    grafico.Children.RemoveAt(2);
                    recortada.StrokeThickness = thickness;
                    recortada.Stroke = new SolidColorBrush(color);
                    recortada.RenderTransform = tg;
                    grafico.Children.Add(recortada);
                }
                else if (barras != null)
                {
                    List<Line> barrasRecortadas = new List<Line>();
                    for (int i = 0; i < barras.Count; i++)
                    {
                        Line l = new Line();
                        l = barras[i];
                        if (l.X1 <= xMayor && l.X1 >= xMenor && l.Y1 <= yMayor && l.Y2 <= yMayor && l.Y1 >= yMenor && l.Y2 >= yMenor)
                        {
                            l.StrokeThickness = thickness;
                            l.Stroke = new SolidColorBrush(color);
                            l.RenderTransform = tg;
                            barrasRecortadas.Add(l);
                        }
                    }
                    for (int i = grafico.Children.Count - 1; i > 1; i--)
                    {
                        grafico.Children.RemoveAt(i);
                    }
                    for (int i = 0; i < barrasRecortadas.Count; i++)
                    {
                        grafico.Children.Add(barrasRecortadas[i]);
                    }
                    barras = barrasRecortadas;
                }
                haciendoRectangulo = false;
                rectangulo.Points.Clear();
                grafico.Children.Remove(rectangulo);
            }
        }
        //Gestiona el scale transport y el translate transform para que las gráficas siempre estén en proporción con el canvas
        private void grafico_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ancho = grafico.ActualWidth;
            alto = grafico.ActualHeight;

            tt.X = 10;
            tt.Y = 10;

            sc.ScaleX = ancho / 20;
            sc.ScaleY = alto / 20;
        }
        //Cambia el típo de gráfica entre polilinea y de barras
        private void CambiarTipoGrafica()
        {
            if (polilineaActivada)
                polilineaActivada = false;
            else
                polilineaActivada = true;
            DibujarGrafica();
        }
        //Adapta el color de la gráfica al color seleccionado
        private void CambiarColorGrafica(Color newColor)
        {
            color = newColor;
            DibujarGrafica();
        }


        /*
         * ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
         * Métodos relacionados con la gestión de los tabItems
         * ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++         
         */
        //Cuando se detecta que se ha cambiado la colección de datos actualiza el número de la tabla y redibuja la gráfica
        private void Dlg_newSelectedIndex(object sender, SelectedIndexEventArgs e)
        {
            if (e.selectedIndex >= 0)
            {
                tablaActual = e.selectedIndex;
                tableNumLabel.Content = "Tabla nº" + tablaActual;
                
            }
            else
            {
                tableNumLabel.Content = "Tabla nºX";
                
            }
            DibujarGrafica();
        }
        //Llama al ViewModel para que añada una nueva colección de datos
        private void NewTabItem()
        {
            if (dcw != null&&listaTablas!=null)
            {
                listaTablas.NewItem();
            }
        }
        //Llama al ViewModel para que elimine la colección de datos actual
        private void RemoveTamItem(int tabIndex)
        {
            if (dcw != null&&listaTablas != null)
            {
                listaTablas.RemoveItem(tabIndex);
                if (legendStackPanel.Children.Count > 1)
                {
                    legendStackPanel.Children.RemoveAt(2 * tabIndex + 1);
                    legendStackPanel.Children.RemoveAt(2 * tabIndex + 1);
                }
            }
        }


        /*
         * ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
         * Métodos relacionados con la gestión de los datos
         * ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++         
         */
        //LLama al viewModel para que ordene los valores de menor a mayor en función de x
        private void SortTable(int tabindex)
        {
            if (dcw != null && listaTablas != null)
            {
                listaTablas.SortTable(tabindex);
            }
        }
        //Llama al ViewModel para que añada varios datos en función de un polinomio y refleja este en la leyenda
        public void NewMathsItems(float x0, float xF, float[] dataArray, int tabIndex)
        {
            listaTablas.NewMathsItemfloat(x0, xF, dataArray, tabIndex);
            Label formula = new Label();
            formula.FontSize = 20;
            formula.FontFamily = new FontFamily("Impact");
            formula.HorizontalAlignment = HorizontalAlignment.Left;
            formula.VerticalAlignment = VerticalAlignment.Center;
            String content = "";
            int i;
            for (i = 0; i < dataArray.Length - 1; i++)
            {
                content += dataArray[i] + "*x^" + i + "+";
            }
            content += dataArray[i] + "*x^" + i;
            formula.Content = content;
            Label bounds = new Label();
            bounds.FontSize = 20;
            bounds.FontFamily = new FontFamily("Impact");
            bounds.HorizontalAlignment = HorizontalAlignment.Left;
            bounds.VerticalAlignment = VerticalAlignment.Center;
            bounds.Content = x0 + " --> " + xF;
            legendStackPanel.Children.Insert(2 * tablaActual + 1, formula);
            legendStackPanel.Children.Insert(2 * tablaActual + 2, bounds);

        }


        /*
         * ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
         * Métodos relacionados con el easter egg
         * ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++         
         */
        //Cuenta el número de clicks sobre la label TablaNº
        private void tableNumLabel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            contadorDeCasitas++;
            if (contadorDeCasitas == 10)
            {
                pisitoEnLaPlaya = true;
                if (grafico.Children.Count > 2)
                {
                    for (int i = grafico.Children.Count - 1; i > 1; i--)
                    {
                        grafico.Children.RemoveAt(i);
                    }
                    DibujarCasita();
                }
                else
                {
                    DibujarCasita();
                }
            }
            else if (contadorDeCasitas >= 10 && pisitoEnLaPlaya)
            {
                pisitoEnLaPlaya = false;
                contadorDeCasitas = 0;
            }
        }
        //Dibuja una increíble casita
        private void DibujarCasita()
        {
            Polyline graficaCasita = new Polyline();
            graficaCasita.Points.Add(new Point(-3, 0));
            graficaCasita.Points.Add(new Point(-0.25, 0));
            graficaCasita.Points.Add(new Point(-0.25, -0.75));
            graficaCasita.Points.Add(new Point(0.25, -0.75));
            graficaCasita.Points.Add(new Point(0.25, 0));
            graficaCasita.Points.Add(new Point(3, 0));
            graficaCasita.Points.Add(new Point(3, -5));
            graficaCasita.Points.Add(new Point(0, -8));
            graficaCasita.Points.Add(new Point(-3, -5));
            graficaCasita.Points.Add(new Point(3, -5));
            graficaCasita.Points.Add(new Point(3, -4));
            graficaCasita.Points.Add(new Point(-3, -4));
            graficaCasita.Points.Add(new Point(-3, -3));
            graficaCasita.Points.Add(new Point(3, -3));
            graficaCasita.Points.Add(new Point(3, -4));
            graficaCasita.Points.Add(new Point(1, -4));
            graficaCasita.Points.Add(new Point(1, -4.75));
            graficaCasita.Points.Add(new Point(0.5, -4.75));
            graficaCasita.Points.Add(new Point(0.5, -4));
            graficaCasita.Points.Add(new Point(-0.5, -4));
            graficaCasita.Points.Add(new Point(-0.5, -4.75));
            graficaCasita.Points.Add(new Point(-1, -4.75));
            graficaCasita.Points.Add(new Point(-1, -4));
            graficaCasita.Points.Add(new Point(-3, -4));
            graficaCasita.Points.Add(new Point(-3, -5));
            graficaCasita.Points.Add(new Point(-3, 0));
            graficaCasita.Points.Add(new Point(5, 0));
            graficaCasita.Points.Add(new Point(5, -2));
            graficaCasita.Points.Add(new Point(4.5, -5));
            graficaCasita.Points.Add(new Point(5, -8));
            graficaCasita.Points.Add(new Point(5.5, -5));
            graficaCasita.Points.Add(new Point(5, -2));

            graficaCasita.Stroke = Brushes.Black;
            graficaCasita.StrokeThickness = 40 / ancho;
            graficaCasita.RenderTransform = tg;
            grafico.Children.Add(graficaCasita);
        }
    }
}
