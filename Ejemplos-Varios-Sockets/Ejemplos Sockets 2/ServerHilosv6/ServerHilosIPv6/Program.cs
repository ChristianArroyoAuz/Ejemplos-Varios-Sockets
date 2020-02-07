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

namespace ServerHilosIPv6
{
    class Program
    {
        //Creacion de los sockets definiendo tipo de protocolo y el tipo de puerto
        Socket socketEscucha = new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp);
        Socket socketCliente;

        static void Main(string[] args)
        {
            //Lanzamiento e inicializacion del programa
            new Program();
            Console.Read();
        }

        Program()
        {
            //Consiguiendo las direcciones IP de la red por las cuales puede enviar los datos al puerdo designado
            IPAddress[] direccionesIP = Dns.GetHostAddresses(Dns.GetHostName());
            IPAddress direccionServidor = direccionesIP[0];
            Console.WriteLine("Direcciones IP: ");

            //Recorriendo la lista de direccione e informando al usuario cual de ella es la que el socket esta usando
            foreach (IPAddress ip in direccionesIP)
            {
                Console.WriteLine(" * {0}", ip);
                if (ip.AddressFamily == AddressFamily.InterNetworkV6)
                {
                    if (ip.IsIPv6LinkLocal == false)
                    {
                        direccionServidor = ip; Console.WriteLine("El servidor está escuchando en la dirección: {0} puerto: 8080", ip);
                    }
                }
            }

            //Proceso de enlace del servidor, el hilo comienza a funcionar por detras
            IPEndPoint ipServidor = new IPEndPoint(direccionServidor, 8080);
            socketEscucha.Bind(ipServidor);
            Console.WriteLine("El servidor enlazó el socket...");
            Thread hiloEscucha = new Thread(new ThreadStart(Escuchar));
            hiloEscucha.IsBackground = true;
            hiloEscucha.Start();
        }

        //Proceso mediante el cual el socket se queda en estado de escucha hasta que un cliente se conecte al servidor con
        //una direccion y por el puerto adecuado
        //El socket pasa a recibir datos
        public void Escuchar()
        {
            while (true)
            {
                socketEscucha.Listen(-1);
                Console.WriteLine("El servidor entra en espera de conexiones...");
                socketCliente = socketEscucha.Accept();
                Console.WriteLine("El servidor ha recibido a un cliente...");
                if (socketCliente.Connected)
                {
                    Thread hiloCliente = new Thread(new ThreadStart(Recibir));
                    hiloCliente.IsBackground = true;
                    hiloCliente.Start();
                }
            }
        }

        //El socket comienza a recibir datos y los presenta en la consola
        public void Recibir()
        {
            Socket socketC = new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp);

            //El servidor se bloquea hasta que termina de recibir los datos de un usuario para que no
            //lleguen los datos menclados
            lock (this)
            {
                socketC = socketCliente;
            }

            Console.WriteLine("Recibiendo datos...");

            while (true)
            {
                int cantidadBytesRecibidos = 0;
                byte[] bytesRecibidos = new byte[2];

                try
                {
                    cantidadBytesRecibidos = socketC.Receive(bytesRecibidos);
                    if (cantidadBytesRecibidos != 0)
                    {
                        Console.WriteLine(Encoding.ASCII.GetString(bytesRecibidos));
                    }
                }
                //Mensaje de error si se produce una excepcion
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