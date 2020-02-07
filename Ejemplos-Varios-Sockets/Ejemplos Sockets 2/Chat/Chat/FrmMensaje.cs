// ******************************************************************************************
// Arroyo Auz Christian Xavier.                                                             *
// 01/06/2016.                                                                              *
// ******************************************************************************************


using System.Collections.Generic;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Threading;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Net;
using System;

namespace Chat
{
    public partial class FrmMensaje : Form
    {
        public string nombre;
        //Asignacion de los parametros de la direccion IP y el puerto
        UdpClient cliente = new UdpClient(1800);
        IPEndPoint sitioRemoto = new IPEndPoint(IPAddress.Any, 0);

        public FrmMensaje()
        {
            InitializeComponent();
        }

        private void FrmMensaje_Load(object sender, EventArgs e)
        {
            txtMensaje.Text = nombre + " se ha unido a la sala...";
            //Creando el hilo del cliente que se ha unido
            Thread hiloTrabajador = new Thread(RecepcionMensajes);
            //Iniciando el hilo del cliente
            hiloTrabajador.Start();
            txtMensajeParaEnviar.Focus();
        }

        private void btnEnviar_Click(object sender, EventArgs e)
        {
            //Si el textbox no esta vacio se envia la cadena de texto ingresada en el textbox a los demas usuarios
            if (!string.IsNullOrEmpty(txtMensajeParaEnviar.Text))
            {
                string datos = nombre + " dice >> " + txtMensajeParaEnviar.Text; EnviarMensaje(datos);
            }
        }

        private void RecepcionMensajes()
        {
            try
            {
                while (true)
                {
                    //Creando el arreglo de bytes para el bufer
                    Byte[] buferRx = cliente.Receive(ref sitioRemoto);
                    //Codificando la cadena de caracteres
                    string mensaje = Encoding.ASCII.GetString(buferRx);
                    //Presentacion de los mensajes en pantalla locales y usuarios que han salido
                    if (mensaje == nombre + " ha salido...")
                    {
                        break;
                    }
                    else
                    {
                        if (mensaje.Contains(nombre + " dice >> "))
                        {
                            mensaje = mensaje.Replace(nombre + " dice >> ", "Yo digo >> ");
                        }
                        //Presentacion del mensaje
                        PresentarMensaje(mensaje);
                    }
                }
                //Cierre del hilo y de la aplicacion
                Thread.CurrentThread.Abort();
                Application.Exit();
            }
            catch (ThreadAbortException)
            {
                //Si ocurre una excepcion la aplicacion se cerrara
                Application.Exit();
            }
        }

        private void EnviarMensaje(string datos)
        {
            //Envio del mensaje en un arreglo de bytes, por medio de la direccion de broadcast y por el puertp 1800
            UdpClient envio = new UdpClient();
            Byte[] mensaje = Encoding.ASCII.GetBytes(datos);
            IPEndPoint remoto = new IPEndPoint(IPAddress.Broadcast, 1800);
            //Proceso de envio del mensaje
            envio.Send(mensaje, mensaje.Length, remoto);
            //Cerrando el proceso y limpiando los campos
            envio.Close();
            txtMensajeParaEnviar.Clear();
            txtMensajeParaEnviar.Focus();
        }

        private void PresentarMensaje(string mensaje)
        {
            //Presentacion del mensaje de forma global para todos los usuarios, activacion del scrollbar del textbox
            //y limpiar el textbox para que no haya mensajes repetitivos cada que llega un mensaje
            if (this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(delegate () { PresentarMensaje(mensaje); }));
            }
            else
            {
                txtMensaje.Text = txtMensaje.Text + Environment.NewLine + mensaje;
                txtMensaje.SelectionStart = txtMensaje.TextLength;
                txtMensaje.ScrollToCaret();
                txtMensaje.Refresh();
            }
        }

        private void txtMensaje_KeyDown(object sender, KeyEventArgs e)
        {
            //Evita que sender ingresen letras
            e.SuppressKeyPress = true;
        }

        private void txtMensajeParaEnviar_KeyDown(object sender, KeyEventArgs e)
        {
            //Para poder enviar mensajes presionando la tecla enter
            if (e.KeyCode == Keys.Enter)
            {
                if (!string.IsNullOrEmpty(txtMensajeParaEnviar.Text))
                {
                    string datos = nombre + " dice >> " + txtMensajeParaEnviar.Text;
                    EnviarMensaje(datos);
                }
            }
        }

        private void FrmMensaje_FormClosing(object sender, FormClosingEventArgs e)
        {
            //Informacion a los demas usuarios cuando un miembro se ha salido al cerrar el formulario
            string datos = nombre + " ha salido...";
            EnviarMensaje(datos);
        }
    }
}