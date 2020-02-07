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
    public class Entramar
    {
        //Iniciando el proceso de entramar los paquetes de bytes
        public static byte[] SiguienteToken(Stream datosEntrada, byte[] delimitador)
        {
            int siguienteByte;

            //Comienza la lectura de los paquetes
            if ((siguienteByte = datosEntrada.ReadByte()) == -1)
            {
                return null;
            }

            //Inicializando la memoria del buffer
            MemoryStream bufer = new MemoryStream();

            do
            {
                //Escribiendo en la memoria del bufer
                bufer.WriteByte((byte)siguienteByte);
                byte[] tokenActual = bufer.ToArray();

                if (TerminaCon(tokenActual, delimitador))
                {
                    //Dando valor a la longitud del token
                    int longitudToken = tokenActual.Length - delimitador.Length;
                    byte[] token = new byte[longitudToken];
                    Array.Copy(tokenActual, 0, token, 0, longitudToken);
                    //retornando el token
                    return token;
                }
            }

            while ((siguienteByte = datosEntrada.ReadByte()) != -1);
            return bufer.ToArray();
        }

        private static Boolean TerminaCon(byte[] valor, byte[] sufijo)
        {
            //Terminando con el conteo
            if (valor.Length < sufijo.Length)
            {
                return false;
            }

            //Devolviendo el tamaño de los bytes
            for (int offset = 1; offset <= sufijo.Length; offset++)
            {
                if (valor[valor.Length - offset] != sufijo[sufijo.Length - offset])
                {
                    return false;
                }
            }
            return true;
        }
    }
}