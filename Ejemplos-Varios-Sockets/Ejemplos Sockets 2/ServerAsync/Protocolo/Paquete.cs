// ******************************************************************************************
// Arroyo Auz Christian Xavier.                                                             *
// 01/06/2016.                                                                              *
// ******************************************************************************************

using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Text;
using System;


namespace Protocolo
{
    // --------------------------
    // Estructura del Paquete
    // --------------------------  
    // Descripcion     -> |idDato|longitud|long del mensaje|   nombre   |    mensaje   |
    // Tamaño en bytes -> |  4   |   4    |       4        |  
    public enum IdentificadorDato
    {
        Mensaje, Conectado, Desconectado, Null
    }

    public class Paquete
    {
        private IdentificadorDato idDato;
        private string nombre;
        private string mensaje;

        //Geters y Seters de la variables creadas
        public IdentificadorDato IdentificadorChat
        {
            get { return idDato; }
            set { idDato = value; }
        }
        public string NombreChat
        {
            get { return nombre; }
            set { nombre = value; }
        }
        public string MensajeChat
        {
            get { return mensaje; }
            set { mensaje = value; }
        }

        public Paquete()
        {
            this.idDato = IdentificadorDato.Null;
            this.mensaje = null;
            this.nombre = null;
        }

        //Estableciendo el tamaño del paquete y de sus respectivos elementos
        //Revisando que los elemntos tengan una longitud mayor a cero
        public Paquete(byte[] arregloBytes)
        {
            this.idDato = (IdentificadorDato)BitConverter.ToInt32(arregloBytes, 0);
            int longitudNombre = BitConverter.ToInt32(arregloBytes, 4);
            int longitudMensaje = BitConverter.ToInt32(arregloBytes, 8);

            if (longitudNombre > 0)
            {
                this.nombre = Encoding.UTF8.GetString(arregloBytes, 12, longitudNombre);
            }
            else
            {
                this.nombre = null;
            }

            if (longitudMensaje > 0)
            {
                this.mensaje = Encoding.UTF8.GetString(arregloBytes, 12 + longitudNombre, longitudMensaje);
            }
            else
            {
                this.mensaje = null;
            }
        }

        //Revisando que exista cadena de caracteres a convertir, de no haberlas lanzara un mensaje de error
        public byte[] ObtenerArregloBytes()
        {
            List<Byte> arregloBytes = new List<Byte>();
            arregloBytes.AddRange(BitConverter.GetBytes((int)this.idDato));

            if (this.nombre != null)
            {
                arregloBytes.AddRange(BitConverter.GetBytes(this.nombre.Length));
            }
            else
            {
                arregloBytes.AddRange(BitConverter.GetBytes(0));
            }

            if (this.mensaje != null)
            {
                arregloBytes.AddRange(BitConverter.GetBytes(this.mensaje.Length));
            }
            else
            {
                arregloBytes.AddRange(BitConverter.GetBytes(0));
            }

            if (this.nombre != null)
            {
                arregloBytes.AddRange(Encoding.UTF8.GetBytes(this.nombre));
            }

            if (this.mensaje != null)
            {
                arregloBytes.AddRange(Encoding.UTF8.GetBytes(this.mensaje));
            }

            return arregloBytes.ToArray();
        }
    }
}