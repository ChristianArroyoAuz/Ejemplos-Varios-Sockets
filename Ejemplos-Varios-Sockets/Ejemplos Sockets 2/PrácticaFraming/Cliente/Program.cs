// ******************************************************************************************
// Arroyo Auz Christian Xavier.                                                             *
// 01/06/2016.                                                                              *
// ******************************************************************************************


using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Threading;
using Codificador;
using System.Linq;
using System.Text;
using System.Net;
using System;

namespace Cliente
{
    class Program : Codificar
    {
        static void Main(string[] args)
        {
            Thread.Sleep(500);
            IPAddress servidor = IPAddress.Parse("127.0.0.1");
            int puerto = 8080;
            //Direccion y puerto a usarse
            IPEndPoint extremo = new IPEndPoint(servidor, puerto);
            TcpClient cliente = new TcpClient();
            cliente.Connect(extremo);
            NetworkStream flujoRed = cliente.GetStream();
            //Creacion de los elementos a enviar segun lo establecido en el constructor
            Elemento elemento1 = new Elemento(1234567890987654L, "Cadena de Bicicleta", 18, 1000, true, false);
            Elemento elemento2 = new Elemento(1111111111111111L, "Pedal de Bicicleta", 18, 1000, false, true);
            //Codificando los elemento
            CodificadorTexto codificador = new CodificadorTexto();
            //Creando un arreglo de bytes de los elementos codificados
            byte[] datosCodificados1 = codificador.Codificar(elemento1);
            byte[] datosCodificados2 = codificador.Codificar(elemento2);
            //Imprimiendo los datos a enviar
            Console.WriteLine("Enviando elemento codificado en texto (" + datosCodificados1.Length + " bytes): ");
            Console.WriteLine("Enviando elemento codificado en texto (" + datosCodificados2.Length + " bytes): ");
            Console.WriteLine(elemento1);
            Console.WriteLine(elemento2);
            //Consiguiendo la cadena de datos codificados
            flujoRed.Write(datosCodificados1, 0, datosCodificados1.Length);
            //Decodificando en binario
            DecodificadorBinario decodificador = new DecodificadorBinario();
            Elemento elementoRecibido = decodificador.Decodificar(cliente.GetStream());
            //Imprimiendo los datos recividos
            Console.WriteLine("Se recibio un elemento codificado en formato binario:");
            Console.WriteLine(elementoRecibido);
            flujoRed.Close();
            cliente.Close();
            Console.ReadLine();
        }
    }
}