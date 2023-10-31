using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Pixel_Art
{
    [Serializable]
    internal class Dibujo
    {
        public string Nombre { get; set; }
        public int NumColumnas { get; set; }
        public int NumFilas { get; set; }
        public Border[,] Borders { get; set; }

        public Dibujo(string nombre, int columnas, int filas, Border[,] borders)
        {
            Nombre = nombre;
            NumColumnas = columnas;
            NumFilas = filas;
            Borders = borders;
        }
    }
}
