using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pixel_Art
{
    internal class Dibujos
    {
        public List<Dibujo> listaDibujos { get; set; }

        public Dibujos()
        {
            listaDibujos = new List<Dibujo>();
        }

        public void AgregarDibujo(Dibujo dibujo)
        {
            listaDibujos.Add(dibujo);
        }

        public Dibujo BuscarDibujoPorNombre(string nombre)
        {
            return listaDibujos.FirstOrDefault(d => d.Nombre == nombre);
        }
    }
}
