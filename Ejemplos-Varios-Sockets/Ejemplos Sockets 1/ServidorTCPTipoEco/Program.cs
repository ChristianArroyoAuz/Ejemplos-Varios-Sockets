using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Linq;
using System.Text;
using System.Net;
using System;

namespace ServidorTCPTipoEco
{
    class Program
    {
        static void Main(string[] args)
        {
            //Inicializando instancians de TCP
            TcpClient manejoCliente;
            TcpListener servidor;
            //Dando un tamaño al buffer de datos de la recepcion
            //byte[] bufferRx = new byte[512];
            byte[] bufferRx = new byte[32];
            byte[] bufferTx;
            int datosLeidos;
            string datos;
            //Inicializando las instancias de la direccion IP y el puerto a usarse
            IPEndPoint puntoLocal = new IPEndPoint(IPAddress.Any, 11000);
            servidor = new TcpListener(puntoLocal);
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("El servidor está escuchado...");
            servidor.Start(5);
            while (true)
            {
                //Si el servido ha empezado puede aceptar la recepcion de los datos
                manejoCliente = servidor.AcceptTcpClient();
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write("\n");
                Console.Write("El servidor ha aceptado a un cliente...");
                //Comenzando la recepcion del flujo de bytes
                NetworkStream flujo = manejoCliente.GetStream();
                do
                {
                    //Leyendo el flujo de bytes, el tamaño de los datos
                    datosLeidos = flujo.Read(bufferRx, 0, bufferRx.Length);
                    if (datosLeidos > 0)
                    {
                        //Decodificando los datos que se hallan en el buffer de datos
                        datos = Encoding.ASCII.GetString(bufferRx);
                        Console.ForegroundColor = ConsoleColor.Gray;
                        Console.Write("\n");
                        Console.Write("Mensaje Recibido. ");
                        Console.Write("Se recibio: ");
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write(datos);
                        Console.ForegroundColor = ConsoleColor.Gray;
                        Console.WriteLine("\n");
                        Console.Write("Mensaje Enviado");
                        //Codificando los datos en el buffer para enviar
                        bufferTx = Encoding.ASCII.GetBytes(datos);
                        flujo.Write(bufferTx, 0, bufferTx.Length);
                    }
                }
                while (datosLeidos > 0);
            }
        }
    }
}