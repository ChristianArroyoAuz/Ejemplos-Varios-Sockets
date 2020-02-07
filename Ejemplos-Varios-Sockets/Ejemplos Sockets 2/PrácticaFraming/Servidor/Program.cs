// ******************************************************************************************
// Arroyo Auz Christian Xavier.                                                             *
// 01/06/2016.                                                                              *
// ******************************************************************************************


using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Sockets;
using Codificador;
using System.Linq;
using System.Text;
using System.Net;
using System;

namespace Servidor
{
    class Program : Codificar
    {
        static void Main(string[] args)
        {
            //Estableciendo el pueto por el cual se va a transmitir
            int puerto = 8080;
            //Recibiendo los datos desde cualquier direcion IP por el pueto establecido
            TcpListener socketEscucha = new TcpListener(IPAddress.Any, puerto);
            //Iniciando el socket
            socketEscucha.Start();
            //Acceptando la comunicacion por el puerto
            TcpClient cliente = socketEscucha.AcceptTcpClient();
            //Decodificando los datos
            DecodificadorTexto decodificador = new DecodificadorTexto();
            Elemento elemento = decodificador.Decodificar(cliente.GetStream());
            //Imprimiendo los datos
            Console.WriteLine("Se recibio un elemento codificado en texto:");
            Console.WriteLine(elemento);
            CodificadorBinario codificador = new CodificadorBinario();
            elemento.precio += 10;
            Console.Write("Enviando elemento en binario...");
            //Haciendo un arreglo de bytes para el envido
            byte[] bytesParaEnviar = codificador.Codificar(elemento);
            Console.WriteLine("(" + bytesParaEnviar.Length + " bytes): ");
            cliente.GetStream().Write(bytesParaEnviar, 0, bytesParaEnviar.Length);
            //Cerrando y deteniendo el socket que se esta usando
            cliente.Close();
            socketEscucha.Stop();
            Console.ReadLine();
        }
    }
}