using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Linq;
using System.Text;
using System.Net;
using System;

namespace ClienteUDP_2
{
    class Program
    {
        static void Main(string[] args)
        {
            bool terminado = false;
            string direccion;
            int puerto;

            //Direccion  de localhost
            //IPAddress servidorIP = IPAddress.Parse("127.0.0.1");

            //Direccion de la red local conectado
            //IPAddress servidorIP = IPAddress.Parse("172.30.81.254");

            //Ingreso manual de datos
            UdpClient cliente = new UdpClient();
            Console.Write("CLIENTE UDP 2.");
            Console.WriteLine("\n");
            Console.Write("Ingrese la direccion IP a la que deses enviar datos: ");
            direccion = Console.ReadLine();
            Console.WriteLine("\n");
            Console.Write("Ingrese el número de puerto por el cual desea conectarse: ");
            //Leyendo lo que se ingresa por consola, asosiando la direccion IP al servidorIP y su respectivo puerto
            puerto = Convert.ToInt16(Console.ReadLine());
            IPAddress servidorIP = IPAddress.Parse(direccion);
            //IPEndPoint puntoExtremo = new IPEndPoint(servidorIP, 11000);
            //Asignando un puerto por el cual enviar la informacion.
            IPEndPoint puntoExtremo = new IPEndPoint(servidorIP, puerto);
            Console.WriteLine("\n");
            Console.WriteLine("Ingrese el texto que se enviara mediante UDP al servidor {0}", servidorIP);
            Console.WriteLine("\n");
            Console.WriteLine("Si no desea envia nada, solo presione ENTER para finalizar...");
            while (!terminado)
            {
                Console.WriteLine("\n");
                Console.Write("Ingrese texto y presione ENTER para enviar: ");
                string textoParaEnvio = Console.ReadLine();
                if (textoParaEnvio.Length == 0)
                {
                    //Finalizando si no se ha ingresado texto a enviar
                    terminado = true;
                }
                else
                {
                    Console.WriteLine("\n");
                    Console.WriteLine("Se está enviando datos a la dirección IP: {0} puerto {1}", puntoExtremo.Address, puntoExtremo.Port);
                    //Codificando el texto ingresado a un arreglo de bytes
                    byte[] bufferTx = Encoding.ASCII.GetBytes(textoParaEnvio);
                    //Envienado el arreglo de byte, el tamaño de los bytes y el puerto por el cual se enviaron los datos asi como la direccion IP
                    cliente.Send(bufferTx, bufferTx.Length, puntoExtremo);
                }
            }
        }
    }
}
