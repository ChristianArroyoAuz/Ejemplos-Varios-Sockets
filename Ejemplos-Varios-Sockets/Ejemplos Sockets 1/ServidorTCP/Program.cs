using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Linq;
using System.Text;
using System.Net;
using System;

namespace ServidorTCP
{
    class Program
    {
        static void Main(string[] args)
        {
            TcpClient manejoCliente; TcpListener servidor;
            //Asignando el tamaño del buffer en x numero de bytes
            byte[] bufferRx = new byte[50];
            //byte[] bufferRx = new byte[512];
            int datosLeidos;
            string datos;
            //Reciviendo de cualquier IP por el puerto designado
            IPEndPoint puntoLocal = new IPEndPoint(IPAddress.Any, 11000);
            servidor = new TcpListener(puntoLocal);
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("El servidor está escuchado...");
            servidor.Start();
            while (true)
            { 
                //Si en servidor ha iniciado, comiensa la conexion con los clientes
                manejoCliente = servidor.AcceptTcpClient();
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine("\n");
                Console.WriteLine("El servidor ha aceptado a un cliente...");
                //Comiensa la recepcion del blujo de bytes
                NetworkStream flujo = manejoCliente.GetStream();
                do
                { 
                    datosLeidos = flujo.Read(bufferRx, 0, bufferRx.Length);
                    if (datosLeidos > 0)
                    { 
                        //Se decodifican los bytes recibidos en la cadena de caracteres original
                        datos = Encoding.ASCII.GetString(bufferRx);
                        Console.Write("Mensaje Recibido. ");
                        Console.Write("Se recibio: ");
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write(datos);
                    }
                }
                while (datosLeidos > 0);
            } 
        }
    }
}