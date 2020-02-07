// ******************************************************************************************
// Arroyo Auz Christian Xavier.                                                             *
// 01/06/2016.                                                                              *
// ******************************************************************************************


using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Linq;
using System.Text;
using System.Net;
using System;

namespace ServidorEcoConTimeout
{
    class Program
    {
        //Creacion de variables a usar
        //Tamaño del bufer
        private const int TAM_BUFFER = 32;
        //Tamaño de la cola en el ufer
        private const int TAM_COLA = 5;
        //tiempo de espera en milisegundos
        private const int LIMITE_ESPERA = 10000;

        static void Main(string[] args)
        {
            //Asignacion del numero del puerto que se usara
            int puerto = 8080;
            //Inicializando socket del servidor sin ningun valor
            Socket servidor = null;

            try
            {
                //Ingresando el tipo de socket, el tipo de protoco y el tipo de direcion que se usara
                servidor = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                //Asociando el socket con el EndPoint
                servidor.Bind(new IPEndPoint(IPAddress.Any, puerto));
                //Escuchando el bufer en cola del socket
                servidor.Listen(TAM_COLA);
            }
            catch (SocketException se)
            {
                //Mensaje de rror cuando se produce una excepcion, cuando el tiempo excede los 10 segundos.
                Console.WriteLine(se.ErrorCode + ": " + se.Message);
                Environment.Exit(se.ErrorCode);
            }

            byte[] buferRx = new byte[TAM_BUFFER];
            int cantBytesRecibidos;
            int totalBytesEnviados = 0;

            for (; ; )
            {
                //Revisando si los sockets estan libres
                Socket cliente = null;

                try
                {
                    //Aceptando la conexion del cliente
                    cliente = servidor.Accept();
                    //Iniciando el temporizador en base a la hora actual del PC
                    DateTime tiempoInicio = DateTime.Now;
                    cliente.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, LIMITE_ESPERA);
                    Console.Write("Gestionando al cliente: " + cliente.RemoteEndPoint + " - ");
                    totalBytesEnviados = 0;
                    while ((cantBytesRecibidos = cliente.Receive(buferRx, 0, buferRx.Length, SocketFlags.None)) > 0)
                    {
                        //Enviando el mensaje al cliente
                        cliente.Send(buferRx, 0, cantBytesRecibidos, SocketFlags.None);
                        totalBytesEnviados += cantBytesRecibidos;
                        TimeSpan tiempoTranscurrido = DateTime.Now - tiempoInicio;
                        //Revisando si el tiempo es menor al del temporizador
                        if (LIMITE_ESPERA - tiempoTranscurrido.TotalMilliseconds < 0)
                        {
                            Console.WriteLine("Terminando la conexión con el cliente debido al temporizador. Se han superado los " +
                                              LIMITE_ESPERA + "ms; se han enviado " + totalBytesEnviados + " bytes"); cliente.Close();
                            throw new SocketException(10060);
                        }
                        cliente.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout,
                                                (int)(LIMITE_ESPERA - tiempoTranscurrido.TotalMilliseconds));
                    }
                    Console.WriteLine("Se han enviado {0} bytes.", totalBytesEnviados);
                    //Cerrando el socket del cliente
                    cliente.Close();
                }
                catch (SocketException se)
                {
                    //Mensaje de error para tiempo mayor a 10 segundos
                    if (se.ErrorCode == 10060)
                    {
                        Console.WriteLine("Terminado la conexion debido al temporizador. Han transcurrido " + LIMITE_ESPERA + "ms; se han transmitido " + totalBytesEnviados + " bytes");
                    }
                    else
                    {
                        Console.WriteLine(se.ErrorCode + ": " + se.Message);
                    }
                    //Cerrando el socket del cliente
                    cliente.Close();
                }
            }
        }
    }
}