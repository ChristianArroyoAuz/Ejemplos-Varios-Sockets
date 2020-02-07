// ******************************************************************************************
// Arroyo Auz Christian Xavier.                                                             *
// 01/06/2016.                                                                              *
// ******************************************************************************************


using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System;

namespace Codificador
{
    public class Codificar
    {
        //Asignando el tamaño maximo de la cadena y el codigo con el cual se codificara texto
        public class ConstantesCodificadorTexto
        {
            public static readonly String CODIFICACION_POR_DEFECTO = "ascii";
            public static readonly int LONG_MAX_FLUJO = 1024;
        }

        //Asignando el tamaño maximo de la cadena y el codigo con el cual se codificara en binario
        //Asignacion del tamaño aleatorio de las banderas
        public class ConstantesCodificadorBinario
        {
            public static readonly String CODIFICACION_POR_DEFECTO = "ascii";
            public static readonly byte BANDERA_DESCUENTO = 1 << 7;
            public static readonly byte BANDERA_EN_STOCK = 1 << 0;
            public static readonly int LONG_MAX_DESCRIPCION = 255;
            public static readonly int LONG_MAX_FLUJO = 1024;
        }

        //Proceso mediante el cual se va a codificar, heredando de la clase Elemento del COdificar
        //se podra codificar el elemento desea.
        public class CodificadorTexto : Elemento.CodificadorElemento
        {
            public Encoding codificador;

            public CodificadorTexto() : this(ConstantesCodificadorTexto.CODIFICACION_POR_DEFECTO)
            {
            }

            //Reciviendo la cadena de texto
            public CodificadorTexto(string datos)
            {
                codificador = Encoding.GetEncoding(datos);
            }

            public byte[] Codificar(Elemento elemento)
            {
                String cadenaCodificada = elemento.numeroElemento + " ";

                //Excepcion cuando dentro del texto se envia un salto de linea
                if (elemento.descripcion.IndexOf('\n') != -1)
                {
                    throw new IOException("Descripcion no valida (contiene un salto de linea)");
                }

                cadenaCodificada = cadenaCodificada + elemento.descripcion + "\n";
                cadenaCodificada = cadenaCodificada + elemento.cantidad + " ";
                cadenaCodificada = cadenaCodificada + elemento.precio + " ";

                //Revisando los parametros recividos segun esta definido en el el constructor
                if (elemento.tieneDescuento)
                {
                    cadenaCodificada = cadenaCodificada + "d";
                }

                if (elemento.enStock)
                {
                    cadenaCodificada = cadenaCodificada + "s";
                }

                cadenaCodificada = cadenaCodificada + "\n";

                //Informe de error cuando la cadena excede el tamaño establecido
                if (cadenaCodificada.Length > ConstantesCodificadorTexto.LONG_MAX_FLUJO)
                {
                    throw new IOException("Longitud codificada demasiado grande");
                }

                //Retornando el bufer
                byte[] bufer = codificador.GetBytes(cadenaCodificada);
                return bufer;
            }
        }

        //Proceso mediante el cual se hace la decodificacion de la cadena recibida
        //Esta hereda de la clase elemento del Codificador
        public class DecodificadorTexto : Elemento.DecodificadorElemento
        {
            public Encoding decodificador;

            public DecodificadorTexto() : this(ConstantesCodificadorTexto.CODIFICACION_POR_DEFECTO)
            {
            }

            public DecodificadorTexto(String datoCodificado)
            {
                decodificador = Encoding.GetEncoding(datoCodificado);
            }

            //Retornado la informacion decodificada en el destino segun los parametros dados en el constructor
            public Elemento Decodificar(Stream flujo)
            {
                String noElemento, descripcion, cant, precio, banderas;
                byte[] espacios = decodificador.GetBytes(" ");
                byte[] saltoLinea = decodificador.GetBytes("\n");
                noElemento = decodificador.GetString(Entramar.SiguienteToken(flujo, espacios));
                descripcion = decodificador.GetString(Entramar.SiguienteToken(flujo, saltoLinea));
                cant = decodificador.GetString(Entramar.SiguienteToken(flujo, espacios));
                precio = decodificador.GetString(Entramar.SiguienteToken(flujo, espacios));
                banderas = decodificador.GetString(Entramar.SiguienteToken(flujo, saltoLinea));
                return new Elemento(Int64.Parse(noElemento),
                                    descripcion, Int32.Parse(cant),
                                    Int32.Parse(precio),
                                    (banderas.IndexOf('d') != -1),
                                    (banderas.IndexOf('s') != -1));
            }

            //Almacenado la cadena de informacion en el bufer de memoria mientras esta se esta decodificando
            public Elemento Decodificar(byte[] paquete)
            {
                Stream cargaUtil = new MemoryStream(paquete, 0, paquete.Length, false);
                return Decodificar(cargaUtil);
            }
        }

        //Proceso mediante el cual se da la codificacion en binario
        //hereda de la clase Elemento
        public class CodificadorBinario : Elemento.CodificadorElemento
        {
            public Encoding codificador;

            public CodificadorBinario()
                : this(ConstantesCodificadorBinario.CODIFICACION_POR_DEFECTO)
            {
            }

            //Recibiendo la cadena de caracteres para codificarlos
            public CodificadorBinario(String datos)
            {
                codificador = Encoding.GetEncoding(datos);
            }

            public byte[] Codificar(Elemento elemento)
            {
                //Creacion de un bufer con memoria mentras se va codificando los datos puedan ir haciendo
                //cola en espera de ser codificados
                MemoryStream flujoMemoria = new MemoryStream();
                BinaryWriter escritorBinario = new BinaryWriter(new BufferedStream(flujoMemoria));
                escritorBinario.Write(IPAddress.HostToNetworkOrder(elemento.numeroElemento));
                escritorBinario.Write(IPAddress.HostToNetworkOrder(elemento.cantidad));
                escritorBinario.Write(IPAddress.HostToNetworkOrder(elemento.precio));
                byte banderas = 0;

                //Agregando las banderas de descuento y/o stock a la cadena codificada
                if (elemento.tieneDescuento)
                {
                    banderas |= ConstantesCodificadorBinario.BANDERA_DESCUENTO;
                }

                if (elemento.enStock)
                {
                    banderas |= ConstantesCodificadorBinario.BANDERA_EN_STOCK;
                }

                escritorBinario.Write(banderas);

                byte[] bytesDescripcion = codificador.GetBytes(elemento.descripcion);

                //Informacion de error si la cadena excede el tamaño establecido
                if (bytesDescripcion.Length > ConstantesCodificadorBinario.LONG_MAX_DESCRIPCION)
                {
                    throw new IOException("La descripcion del elemento excede el límite establecido");
                }

                escritorBinario.Write((byte)bytesDescripcion.Length);
                escritorBinario.Write(bytesDescripcion);
                escritorBinario.Flush();
                return flujoMemoria.ToArray();
            }
        }

        //Proceso mediante el cual se hace la decodificacion en binario, hereda de la clase elemento
        public class DecodificadorBinario : Elemento.DecodificadorElemento
        {
            public Encoding decodificador;

            public DecodificadorBinario()
                : this(ConstantesCodificadorBinario.CODIFICACION_POR_DEFECTO)
            {
            }

            public DecodificadorBinario(String datos)
            {
                decodificador = Encoding.GetEncoding(datos);
            }

            public Elemento Decodificar(Stream flujo)
            {
                //Comienzo de lectura de los bits que van llegando en el bufer
                BinaryReader lectorBinario = new BinaryReader(new BufferedStream(flujo));
                long noElemento = IPAddress.NetworkToHostOrder(lectorBinario.ReadInt64());
                int cant = IPAddress.NetworkToHostOrder(lectorBinario.ReadInt32());
                int precio = IPAddress.NetworkToHostOrder(lectorBinario.ReadInt32());
                byte banderas = lectorBinario.ReadByte();
                int longCadena = lectorBinario.Read();

                //Si no hay cadena de texto se produce una excepcion
                if (longCadena == -1)
                {
                    throw new EndOfStreamException();
                }

                byte[] buferDescripcion = new byte[longCadena];
                lectorBinario.Read(buferDescripcion, 0, longCadena);
                String descripcion = decodificador.GetString(buferDescripcion);
                return new Elemento(noElemento,
                                    descripcion,
                                    cant,
                                    precio,
                                    ((banderas & ConstantesCodificadorBinario.BANDERA_DESCUENTO) == ConstantesCodificadorBinario.BANDERA_DESCUENTO),
                                    ((banderas & ConstantesCodificadorBinario.BANDERA_EN_STOCK) == ConstantesCodificadorBinario.BANDERA_EN_STOCK));
            }

            public Elemento Decodificar(byte[] paquete)
            {
                Stream cargaUtil = new MemoryStream(paquete, 0, paquete.Length, false);
                return Decodificar(cargaUtil);
            }
        }
    }
}