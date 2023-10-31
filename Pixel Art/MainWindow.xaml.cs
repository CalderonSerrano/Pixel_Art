using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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
using System.Xml.Serialization;

namespace Pixel_Art
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Color selectedColor;
        private bool isFirstTime = true;
        private Dibujos dibujos;
        private int dimensiones;

        public MainWindow()
        {
            InitializeComponent();
            dibujos = new Dibujos();
        }

        private void createPanel(int size)
        {
            pixelPanelGrid.Children.Clear();
            dimensiones = size;
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    Border element = new Border();
                    element.Style = (Style)Resources["borderPixelArt"];
                    pixelPanelGridBorder.BorderThickness = new Thickness(3.0);
                    pixelPanelGridBorder.BorderBrush = Brushes.Black;
                    pixelPanelGrid.Margin = new Thickness(0.0);
                    pixelPanelGrid.Children.Add(element);
                }
            }
        }

        private bool isPanelEmpty()
        {
            foreach (Border child in pixelPanelGrid.Children)
            {
                if (child.Background.ToString() != "#FFFFFFFF")
                    return false;
            }
            return true;
        }


        private void resizePanel(object sender, RoutedEventArgs e)
        {
            int size = int.Parse(((FrameworkElement)sender).Tag.ToString());
            if (!isFirstTime && !isPanelEmpty())
            {
                if (MessageBox.Show("¿Seguro que quieres perder tu dibujo?", "Nuevo dibujo", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
                    return;
                createPanel(size);
            }
            else
            {
                createPanel(size);
                isFirstTime = false;
            }
        }

        private void radioButtonColor_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton radioButton = (RadioButton)sender;

            if (radioButton.Tag.ToString() == "Personalizado")
            {
                colorPersonalizadoTextBox.IsEnabled = true;
            }
            else
            {
                selectedColor = (Color)ColorConverter.ConvertFromString(radioButton.Tag.ToString());
            }
        }

        private void personalizadoRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            colorPersonalizadoTextBox.IsEnabled = true;
        }

        private void personalizadoRadioButton_Unchecked(object sender, RoutedEventArgs e)
        {
            colorPersonalizadoTextBox.IsEnabled = false;
            personalizadoRadioButton.Foreground = Brushes.Black;
        }

        private void colorPersonalizadoTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                try
                {
                    selectedColor = (Color)ColorConverter.ConvertFromString(colorPersonalizadoTextBox.Text);
                }
                catch (FormatException ex)
                {
                    MessageBox.Show("Código de color " + colorPersonalizadoTextBox.Text + " no válido.", "Error", MessageBoxButton.OK, MessageBoxImage.Hand);
                }
            }
        }

        private void bordeBorder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SolidColorBrush solidColorBrush = new SolidColorBrush(selectedColor);
            Border border = (Border)sender;
            border.Background = solidColorBrush;
            e.Handled = true; // Marcar el evento como "manejado" para evitar la propagación.
        }

        private void bordeBorder_MouseEnter(object sender, MouseEventArgs e)
        {
            Border border = (Border)sender;
            if (Mouse.LeftButton != MouseButtonState.Pressed)
                return;
            SolidColorBrush solidColorBrush = new SolidColorBrush(selectedColor);
            border.Background = solidColorBrush;
            e.Handled = true; // Marcar el evento como "manejado" para evitar la propagación.
        }

        private void rellenarButton_Click(object sender, RoutedEventArgs e)
        {
            if (selectedColor != null)
            {
                SolidColorBrush brush = new SolidColorBrush(selectedColor);
                foreach (Border border in pixelPanelGrid.Children)
                {
                    border.Background = brush;
                }
            }
        }

        private void panelPersonalizadoTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Return)
                return;
            try
            {
                int num = int.Parse(panelPersonalizadoTextBox.Text);
                if ((num < 1 || num > 100) && errorNumero.Visibility == Visibility.Collapsed)
                {
                    errorNumero.Visibility = Visibility.Visible;
                }
                else
                {
                    errorNumero.Visibility = Visibility.Collapsed;
                    createPanel(int.Parse(panelPersonalizadoTextBox.Text));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Número '" + panelPersonalizadoTextBox.Text + "' no válido.\n Introduce un número entero.", "Error", MessageBoxButton.OK, MessageBoxImage.Hand);
            }
        }

        private void panelPersonalizadoTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            panelPersonalizadoTextBox.Foreground = Brushes.Black;
            panelPersonalizadoTextBox.Text = "";
        }

        private void panelPersonalizadoTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            panelPersonalizadoTextBox.Foreground = Brushes.LightGray;
            if (string.IsNullOrEmpty(panelPersonalizadoTextBox.Text))
                panelPersonalizadoTextBox.Text = "ej. 60";
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (((FrameworkElement)sender).ToolTip.ToString() == "Borde")
                pixelPanelGridBorder.BorderThickness = new Thickness(3.0);
            else
                pixelPanelGridBorder.BorderThickness = new Thickness(0.0);
        }

        //------------------------------ EXTRAS ------------------------------------

        private void GuardarDibujo(object sender, RoutedEventArgs e)
        {
            // Pide al usuario que ingrese un nombre para el dibujo.
            string nombreDibujo = PromptForDibujoNombre();

            if (string.IsNullOrEmpty(nombreDibujo))
            {
                MessageBox.Show("Nombre de dibujo no válido.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Verifica si ya existe un dibujo con el mismo nombre.
            if (dibujos.listaDibujos.Any(d => d.Nombre == nombreDibujo))
            {
                MessageBox.Show("Ya existe un dibujo con el mismo nombre. Por favor, elige otro nombre.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Crea una matriz para almacenar los Border actuales.
            Border[,] borders = new Border[dimensiones, dimensiones];

            // Recorre el UniformGrid y guarda los colores de fondo de los Border en la matriz.
            int index = 0;
            foreach (var child in pixelPanelGrid.Children)
            {
                if (child is Border border)
                {
                    SolidColorBrush brush = border.Background as SolidColorBrush;
                    Color color = brush.Color;
                    int row = index / dimensiones;
                    int col = index % dimensiones;
                    borders[row, col] = new Border { Background = new SolidColorBrush(color) };
                    index++;
                }
            }

            // Crea un objeto Dibujo y agrégalo a la lista de dibujos.
            Dibujo dibujo = new Dibujo(nombreDibujo, dimensiones, dimensiones, borders);
            dibujos.listaDibujos.Add(dibujo);
            listaDibujosListBox.Items.Add(nombreDibujo);
            MessageBox.Show("Dibujo guardado con éxito.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private string PromptForDibujoNombre()
        {
            // Abre un cuadro de diálogo para que el usuario ingrese un nombre para el dibujo.
            string nombre = Microsoft.VisualBasic.Interaction.InputBox("Ingresa un nombre para el dibujo:", "Guardar Dibujo", "");

            return nombre;
        }

        private string PromptForDibujoBorrar(string enunciado)
        {
            // Abre un cuadro de diálogo para que el usuario ingrese un nombre para el dibujo.
            string nombre = Microsoft.VisualBasic.Interaction.InputBox(enunciado, "Borrar Dibujo", "");

            return nombre;
        }

        private void VerDibujos_Click(object sender, RoutedEventArgs e)
        {
            // Llenar el ListBox con los nombres de los dibujos.
            listaDibujosListBox.Items.Clear();
            foreach (Dibujo dibujo in dibujos.listaDibujos)
            {
                listaDibujosListBox.Items.Add(dibujo.Nombre);
            }
        }

        private void CargarDibujo_Click(object sender, RoutedEventArgs e)
        {
            string nombre = PromptForDibujoNombre();

            // Buscar el dibujo en la lista por nombre.
            Dibujo dibujo = dibujos.BuscarDibujoPorNombre(nombre);

            if (dibujo != null)
            {
                // Cargar el dibujo en el lienzo actual.
                CargarDibujoEnLienzo(dibujo);
            }
            else
            {
                MessageBox.Show("No se encontró un dibujo con ese nombre.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void CargarDibujoEnLienzo(Dibujo dibujo)
        {
            // Limpia el lienzo actual.
            pixelPanelGrid.Children.Clear();

            // Obtén las dimensiones del dibujo.
            int numFilas = dibujo.NumFilas;
            int numColumnas = dibujo.NumColumnas;

            // Verifica si las dimensiones coinciden con el tamaño del lienzo actual.
            if (numFilas != dimensiones || numColumnas != dimensiones)
            {
                MessageBoxResult result = MessageBox.Show("Las dimensiones del dibujo no coinciden con el tamaño del lienzo actual. ¿Desea redimensionar el lienzo para cargar el dibujo?", "Error de Dimensiones", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    // Redimensiona el lienzo actual con las dimensiones del dibujo.
                    dimensiones = numFilas;
                    dimensiones = numColumnas;

                    // Recorre la matriz de Border y agrega los elementos al lienzo.
                    for (int i = 0; i < numFilas; i++)
                    {
                        for (int j = 0; j < numColumnas; j++)
                        {
                            Border elemento = dibujo.Borders[i, j];
                            pixelPanelGrid.Children.Add(elemento);
                        }
                    }
                }
            }
            else
            {
                // Las dimensiones coinciden, así que simplemente carga el dibujo en el lienzo.
                dimensiones = numFilas;
                dimensiones = numColumnas;

                // Recorre la matriz de Border y agrega los elementos al lienzo.
                for (int i = 0; i < numFilas; i++)
                {
                    for (int j = 0; j < numColumnas; j++)
                    {
                        Border elemento = dibujo.Borders[i, j];
                        pixelPanelGrid.Children.Add(elemento);
                    }
                }
            }
        }


        private void BorrarDibujo_Click(object sender, RoutedEventArgs e)
        {
            string nombre = PromptForDibujoBorrar("Nombre del dibujo a borrar");

            // Buscar el dibujo en la lista por nombre.
            Dibujo dibujo = dibujos.BuscarDibujoPorNombre(nombre);

            if (dibujo != null)
            {
                // Confirmar con el usuario antes de borrar el dibujo.
                MessageBoxResult result = MessageBox.Show("¿Desea borrar el dibujo '" + nombre + "'?", "Confirmar Borrado", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    // Borrar el dibujo de la lista.
                    dibujos.listaDibujos.Remove(dibujo);
                    listaDibujosListBox.Items.Remove(nombre);
                }
            }
            else
            {
                MessageBox.Show("No se encontró un dibujo con ese nombre.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}