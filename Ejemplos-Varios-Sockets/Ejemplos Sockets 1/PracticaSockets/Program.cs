using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Text;
using System.Net;
using System;

namespace PracticaSockets
{
    class Program
    {
        static void Main(string[] args)
        {
            //Presentacion de datos automatico
            //Console.WriteLine("Bienvenido! Estas trabajando en: " + Dns.GetHostName());
            //IPAddress[] direccionesIP = Dns.GetHostAddresses("www.epn.edu.ec");
            //foreach (IPAddress ip in direccionesIP)
            //{
            //    Console.WriteLine(ip.ToString());
            //    Console.ReadLine();
            //}

            //Ingreso manual de los datos
            string nombre;
            string direccion;
            string ingresarDireccion;
            Console.Write("Ingrese su nombre: ");
            nombre = Console.ReadLine();
            Console.Write("Ingrese la direccion DNS: ");
            direccion = Console.ReadLine();
            Console.WriteLine("Bienvenido!: " + nombre + "  " + "Estas trabajando en: " + direccion);
            //Inicializamos una nueva instacia con un arreglo de bytes
            IPAddress[] direccionesIP = Dns.GetHostAddresses(direccion);
            //Hacemos un recorrido al arreglo de direcciones IP, con referencia a la clase IPAddress
            foreach (IPAddress ip in direccionesIP)
            {
                Console.WriteLine("Tu direccion IP es: " + ip.ToString());
            }
            Console.WriteLine("\n");
            Console.Write("Ingrese la direccion IP: ");
            ingresarDireccion = Console.ReadLine();
            //Inicializamos una nueva instancia con un numero entero de la direccion IP
            IPAddress direccionIP = IPAddress.Parse(ingresarDireccion);
            //Inicializa una nueva instancia para conseguir el DNS de una red conectada a internet
            IPHostEntry hostName = Dns.GetHostEntry(direccionIP);
            //.HostName me imprime el nombre del DNS o del Host buscado
            Console.WriteLine("El nombre del DNS del Host es: " + hostName.HostName);
            Console.Write("Presiona un tecla para finalizar... ");
            Console.Read();
        }
    }
}