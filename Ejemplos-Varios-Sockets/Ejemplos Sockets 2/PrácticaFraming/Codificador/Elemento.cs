// ******************************************************************************************
// Arroyo Auz Christian Xavier.                                                             *
// 01/06/2016.                                                                              *
// ******************************************************************************************


using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Text;
using System.IO;
using System;

namespace Codificador
{
    public class Elemento
    {
        public long numeroElemento;
        public String descripcion;
        public int cantidad;
        public int precio;
        public Boolean tieneDescuento;
        public Boolean enStock;

        //Creacion del constructor con sus diferentes variables e igualandolas a las variables locales
        public Elemento(long id, string descripcion, int cant, int precio, bool tieneDescuento, bool enStock)
        {
            numeroElemento = id;
            this.descripcion = descripcion;
            cantidad = cant;
            this.precio = precio;
            this.tieneDescuento = tieneDescuento;
            this.enStock = enStock;
        }

        //Resultados a imprimir
        public override string ToString()
        {
            String separador = "\n";
            String valor = "ID#=" + numeroElemento + separador + "Descripcion=" + descripcion + separador + "Cantidad=" + cantidad + separador + "Precio=" + precio + separador + "Precio Total=" + (cantidad * precio);

            //Analizando los valores de descuento y stock
            if (tieneDescuento)
            {
                valor += " (descuento)";
            }

            if (enStock)
            {
                valor += separador + "En Stock" + separador;
            }
            else
            {
                valor += separador + "No en Stock" + separador;
            }
            return valor;
        }

        public interface CodificadorElemento
        {
            //Codificador del elemento recibido
            byte[] Codificar(Elemento elemento);
        }

        public interface DecodificadorElemento
        {
            //Decodificador del elemento recivbido, en cadena de datos y un arreglo de bytes
            Elemento Decodificar(Stream dato);
            Elemento Decodificar(byte[] paquete);
        }
    }
}