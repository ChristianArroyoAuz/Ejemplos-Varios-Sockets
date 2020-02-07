// ******************************************************************************************
// Arroyo Auz Christian Xavier.                                                             *
// 01/06/2016.                                                                              *
// ******************************************************************************************


using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Threading;
using System.Linq;
using System.Text;
using System.Net;
using System;

namespace ServerHilosIPv4
{
    class Program
    {
        //Declaracion de objetos de forma global.
        //Tipo de protocolo a usarse y el tipo de socket
        Socket socketEscucha = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        Socket socketCliente;

        static void Main(string[] args)
        {
            //Iniciando la consola
            new Program();
            Console.Read();
        }

        public Program()
        {
            //Consiguiendo la direccion IP de la red a la cual se encuentra conectado la PC
            IPAddress[] direccionesIP = Dns.GetHostAddresses(Dns.GetHostName());
            IPAddress direccionServidor = direccionesIP[0];
            Console.WriteLine("Direcciones IP: ");

            foreach (IPAddress ip in direccionesIP)
            {
                //Imprimiendo las direcciones IP
                Console.WriteLine(" * {0}", ip);
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    //Si no esta conectado a ninguna red imprimir la direccion de hostlocal
                    if (!ip.Equals("127.0.0.1"))
                    {
                        direccionServidor = ip;
                        //Informando del puerto que esta usando
                        Console.WriteLine("El servidor está escuchando en la dirección: {0} puerto: 8080", ip);
                    }
                }
            }
            IPEndPoint ipServidor = new IPEndPoint(direccionServidor, 8080);
            socketEscucha.Bind(ipServidor);
            Console.WriteLine("El servidor enlazó el socket...");
            //LAnzando a correr los hilos por detras
            Thread hiloEscucha = new Thread(new ThreadStart(Escuchar));
            hiloEscucha.IsBackground = true;
            //Empezando la escucha del hilo para empezar
            hiloEscucha.Start();
        }

        public void Escuchar()
        {
            while (true)
            {
                //Condicion a habilitar el socket si estan trasmitiendo
                socketEscucha.Listen(-1);
                Console.WriteLine("El servidor entrá en espera de conexiones...");
                socketCliente = socketEscucha.Accept();
                Console.WriteLine("El servidor ha recibido a un cliente...");
                if (socketCliente.Connected)
                {
                    Thread hiloCliente = new Thread(new ThreadStart(Recibir));
                    hiloCliente.IsBackground = true; hiloCliente.Start();
                }
            }
        }

        public void Recibir()
        {
            //recibiendo los datos del cliente en el servidor
            Socket socketC = new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp);

            lock (this)
            {
                socketC = socketCliente;
            }

            Console.WriteLine("Recibiendo datos...");

            while (true)
            {
                //Impresion de los datos del tamaño de 2 bytes
                int cantidadBytesRecibidos = 0;
                byte[] bytesRecibidos = new byte[2];

                //La cantidad de bytes recibidos tiene que ser mayor a cero para que no haya excepciones.
                try
                {
                    cantidadBytesRecibidos = socketC.Receive(bytesRecibidos);

                    if (cantidadBytesRecibidos != 0)
                    {
                        Console.WriteLine(Encoding.ASCII.GetString(bytesRecibidos));
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex);
                }

                if (!socketC.Connected)
                {
                    break;
                }
            }
        }
    }
}