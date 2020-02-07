using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Linq;
using System.Text;
using System.Net;
using System;

namespace ServidorUDP
{
    class Program
    {
        //Asignacion del numero de puerto a usarse, por donde recibira la informacion
        private const int puertoEscucha = 11000;

        static void Main(string[] args)
        {
            bool terminado = false;
            //Creando los servicios de escucha por el puerto asigado
            UdpClient servidor = new UdpClient(puertoEscucha);
            //recibiendo de culquier direccion ip, que se conecte al puertoescucha
            IPEndPoint sitioRemoto = new IPEndPoint(IPAddress.Any, puertoEscucha);
            string datosRx;
            byte[] bufferRx;
            Console.WriteLine("Empezando...");
            while (!terminado)
            {
                Console.WriteLine("\n");
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write("Esperando por mensajes...");
                //Recibiendo los datos por el puerto designado
                bufferRx = servidor.Receive(ref sitioRemoto);
                Console.WriteLine("\n");
                Console.WriteLine("Se recibió un mensaje de {0} ...", sitioRemoto);
                //decodificando los ytes recibidos
                datosRx = Encoding.ASCII.GetString(bufferRx, 0, bufferRx.Length);
                // Console.Write("El contenido del mensaje es: {0}", datosRx);
                Console.Write("El contenido del mensaje es: ", datosRx);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write(datosRx);
            }
        }
    }
}