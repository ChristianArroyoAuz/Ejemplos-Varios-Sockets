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
    public partial class FrmLogin : Form
    {
        public FrmLogin()
        {
            InitializeComponent();
            txtNombreUsuario.Focus();
        }


        private void FrmLogin_Load(object sender, EventArgs e)
        {
            //Evento para darle el foco al textbox al iniciar la aoplicacion
            txtNombreUsuario.Focus();
        }

        private void btnConectar_Click(object sender, EventArgs e)
        {
            //Ejecucion de la siguientes lineas de codigo si el textbox del login no esta vacio
            if (!string.IsNullOrEmpty(txtNombreUsuario.Text))
            {
                //Creando el cliente en el protocolo UDP
                UdpClient cliente = new UdpClient();
                ////Creando el arreglo de bytes para el bufer
                Byte[] buferTx = Encoding.ASCII.GetBytes(txtNombreUsuario.Text + " ha entrado a la sala...");
                //Asignando la direccion IP de broadcast y el puerto a usarse
                IPEndPoint sitioRemoto = new IPEndPoint(IPAddress.Broadcast, 1800);
                //Enviando los datos de nombre del cliente al sitio remoto
                cliente.Send(buferTx, buferTx.Length, sitioRemoto);
                //Cerrando el puerto que estaba usando el cliente
                cliente.Close();
                this.Hide();
                //Abriendo el nuevo formulario
                FrmMensaje formMensaje = new FrmMensaje();
                formMensaje.nombre = txtNombreUsuario.Text;
                formMensaje.Show();
            }
        }

        private void txtNombreUsuario_KeyDown(object sender, KeyEventArgs e)
        {
            //Metodo a realizar cuando se presiona la tecla enter
            if (e.KeyCode == Keys.Enter)
            {
                //LLamado al Boton conectar, es decir al btnConectar_Click
                btnConectar_Click(sender, e);
            }
        }
    }
}