using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Practica_Final
{
    class ViewModel
    {
        /*
         * ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
         * Delegados
         * ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++         
         */
        public delegate void DatosActualizados();
        public DatosActualizados datosActualizados;
        /*
         * ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
         * Variables para el funcionamiento del sistema
         * ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++         
         */
        public ObservableCollection<TabItem> tabControlIS { get; set; }

        /*
         * ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
         * Constructor principal en el que crea la lista de tabItems y le añade uno nuevo
         * ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++         
         */
        public ViewModel()
        {
            tabControlIS = new ObservableCollection<TabItem>();
            NewItem();
        }

        /*
         * ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
         * Métodos destinados a exportar e importar datos como un fichero .inator
         * ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++         
         */
        //Guarda los datos con el formato necesario
        public int saveData(String fileName)
        {
            try
            {
                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                }
                using (FileStream fs = File.Create(fileName))
                {
                    int i;
                    for (i = 0; i < tabControlIS.Count - 1; i++)
                    {
                        ScrollViewer sv = tabControlIS.ElementAt(i).Content as ScrollViewer;
                        Grid g = sv.Content as Grid;
                        StackPanel spx = g.Children[0] as StackPanel;
                        StackPanel spy = g.Children[1] as StackPanel;
                        for (int j = 1; j < spx.Children.Count; j++)
                        {
                            String s = ((TextBox)spx.Children[j]).Text + "\t" + ((TextBox)spy.Children[j]).Text + "\n";
                            Byte[] parDatos = new UTF8Encoding(true).GetBytes(s);
                            fs.Write(parDatos, 0, parDatos.Length);
                        }
                        Byte[] separacion = new UTF8Encoding(true).GetBytes("_\n");
                        fs.Write(separacion, 0, separacion.Length);
                    }
                    ScrollViewer sv2 = tabControlIS.ElementAt(i).Content as ScrollViewer;
                    Grid g2 = sv2.Content as Grid;
                    StackPanel spx2 = g2.Children[0] as StackPanel;
                    StackPanel spy2 = g2.Children[1] as StackPanel;
                    for (int j = 1; j < spx2.Children.Count; j++)
                    {
                        String s = ((TextBox)spx2.Children[j]).Text + "\t" + ((TextBox)spy2.Children[j]).Text + "\n";
                        Byte[] parDatos = new UTF8Encoding(true).GetBytes(s);
                        fs.Write(parDatos, 0, parDatos.Length);
                    }
                    fs.Close();
                }
                return 0;
            }
            catch
            {
                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                }
                return -1;
            }
            
        }
        //Abre los datos haciendo primero un parseo de esto
        public int importData(String fileName)
        {
            try
            {
                tabControlIS.Clear();
                using (StreamReader sr = File.OpenText(fileName))
                {

                    String s = "";
                    String data = "";
                    while ((s = sr.ReadLine()) != null)
                    {
                        data += s + "\n";
                    }
                    String[] tables = data.Split('_');
                    for (int i = 0; i < tables.Length; i++)
                    {
                        String tabName = "Tabla nº: " + i;
                        Tabla newTabla = new Tabla(tabName);
                        newTabla.datosActualizados += DatosActualizadosVM;
                        newTabla.spx.Children.RemoveAt(newTabla.spx.Children.Count - 1);
                        newTabla.spy.Children.RemoveAt(newTabla.spy.Children.Count - 1);
                        String[] parDatos = tables[i].Split('\n');
                        for (int j = 0; j < parDatos.Length; j++)
                        {
                            String[] datos = parDatos[j].Split('\t');
                            try
                            {
                                if (!parDatos[j].Equals(""))
                                {
                                    float x = float.Parse(datos[0]);
                                    float y = float.Parse(datos[1]);
                                    newTabla.NewMathsItem(x, y);
                                }
                            }
                            catch 
                            {
                                tabControlIS.Clear();
                                return -1;
                            }
                        }
                        tabControlIS.Add(newTabla.tab);
                    }
                    sr.Close();
                    return 0;
                }
            }
            catch
            {
                tabControlIS.Clear();
                return -1;
            }
            
        }

        /*
         * ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
         * Métodos destinados a la gestión fina de los datos
         * ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++         
         */
        //Añade un nuevo tabItem vacío
        public void NewItem()
        {
            String tabName = "Tabla nº: " + tabControlIS.Count.ToString();
            Tabla newTabla = new Tabla(tabName);
            newTabla.datosActualizados += DatosActualizadosVM;
            tabControlIS.Add(newTabla.tab);
        }
        //Elimina el tabItem actual
        public void RemoveItem(int tabIndex)
        {
            tabControlIS.RemoveAt(tabIndex);
            for (int i = 0; i < tabControlIS.Count; i++)
            {
                String tabName = "Tabla nº: " + i;
                tabControlIS.ElementAt(i).Header = tabName;
            }
            DatosActualizadosVM();
        }
        //Ordena los datos del tabItem actual de menor a mayor en función de x
        public void SortTable(int tabIndex)
        {
            ScrollViewer sv = tabControlIS.ElementAt(tabIndex).Content as ScrollViewer;
            Grid g = sv.Content as Grid;
            StackPanel spx = g.Children[0] as StackPanel;
            StackPanel spy = g.Children[1] as StackPanel;
            List<float> spxList = new List<float>();
            List<float> spxListAUX = new List<float>();
            List<float> spyList = new List<float>();
            for (int i = 0; i < spx.Children.Count-1; i++)
            {
                spxList.Add(float.Parse(((TextBox)spx.Children[i + 1]).Text, CultureInfo.InvariantCulture));
                spxListAUX.Add(float.Parse(((TextBox)spx.Children[i + 1]).Text, CultureInfo.InvariantCulture));
                spyList.Add(float.Parse(((TextBox)spy.Children[i + 1]).Text, CultureInfo.InvariantCulture));
            }
            String tabName = "Tabla nº: " + tabIndex;
            Tabla newTabla = new Tabla(tabName);
            newTabla.datosActualizados += DatosActualizadosVM;
            newTabla.spx.Children.RemoveAt(newTabla.spx.Children.Count - 1);
            newTabla.spy.Children.RemoveAt(newTabla.spy.Children.Count - 1);
            spxList.Sort();
            for (int i = 0; i < spx.Children.Count-1; i++)
            {
                int index = spxListAUX.IndexOf(spxList[i]);
                newTabla.NewMathsItem(spxList[i], spyList[index]);
                spyList.RemoveAt(index);
                spxListAUX.RemoveAt(index);
            }
            tabControlIS.RemoveAt(tabIndex);
            tabControlIS.Insert(tabIndex, newTabla.tab);
            DatosActualizadosVM();
        }
        //Vacía el tabItem seleccionado y lo rellena de pares de datos en función del polinomio
        public void NewMathsItemfloat (float x0,float xF, float []dataArray, int tabIndex)
        {
            String tabName = "Tabla nº: " + tabIndex;
            Tabla newTabla = new Tabla(tabName);
            newTabla.datosActualizados += DatosActualizadosVM;
            newTabla.spx.Children.RemoveAt(newTabla.spx.Children.Count - 1);
            newTabla.spy.Children.RemoveAt(newTabla.spy.Children.Count - 1);
            for (float i =x0; i <= xF; i=i+0.5f)
            {
                float result = 0;
                for (float j = 0; j < dataArray.Length; j++)
                {
                    result += dataArray[(int)j] * (float)Math.Pow(i, j);
                }
                newTabla.NewMathsItem(i, result);
            }
            tabControlIS.RemoveAt(tabIndex);
            tabControlIS.Insert(tabIndex, newTabla.tab);

        }
        //Delegado que se va llamando hacia arriba: modelo->view model->main window Para que se cambie la gráfica cuando se acutaliza un dato
        public void DatosActualizadosVM()
        {
            datosActualizados();
        }

    }
}
