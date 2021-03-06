using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using System.IO;


namespace Alpheus_0._6
{
    public partial class computadoras : System.Web.UI.Page
    {
        Sesion usuario = new Sesion();
        protected void Page_Load(object sender, EventArgs e)
        {
            string fecha_backup = DateTime.Now.ToString("dd_MM_yyyy"), query = "", direccion = @"C:\BACKUP\";
            Directory.CreateDirectory(direccion);
            string UsuarioLogeado = Session["UsuarioLogeado"].ToString();

            string conectar = ConfigurationManager.ConnectionStrings["MatiasConnection"].ConnectionString;
            SqlConnection con = new SqlConnection(conectar);

            con.Open();

            if (!Directory.Exists(direccion))
            {
                query = "BACKUP DATABASE AlpheusPlus TO DISK ='" + direccion + fecha_backup + ".bak'";

                SqlCommand cmd = new SqlCommand(query, con);
                SqlDataReader dr = cmd.ExecuteReader();
                dr.Close();

                string mensaje;

                cmd = new SqlCommand("SELECT Nombre, Apellido_Paterno FROM Usuario WHERE Usuario = '" + UsuarioLogeado + "'", con);
                dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    mensaje = dr["Nombre"].ToString() + " " + dr["Apellido_Paterno"].ToString();
                    SesionLbl.Text = mensaje;
                    dr.Close();
                }

            }
            else
            {
                query = "BACKUP DATABASE AlpheusPlus TO DISK ='" + direccion + fecha_backup + ".bak'";
                SqlCommand cmd = new SqlCommand(query, con);
                SqlDataReader dr = cmd.ExecuteReader();
                dr.Close();

                string mensaje;

                cmd = new SqlCommand("SELECT Nombre, Apellido_Paterno FROM Usuario WHERE Usuario = '" + UsuarioLogeado + "'", con);
                dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    mensaje = dr["Nombre"].ToString() + " " + dr["Apellido_Paterno"].ToString();
                    SesionLbl.Text = mensaje;
                    dr.Close();
                }

                
            }

            //SesionLbl.Text = Sesion.Nombre(UsuarioLogeado);
            

            con.Close();
        }

        protected void Registrar_Click(object sender, EventArgs e)
        {
            string fecha = DateTime.Now.ToString("dd/MM/yyyy"), NoInventario = "NO";

            string conectar = ConfigurationManager.ConnectionStrings["MatiasConnection"].ConnectionString;
            SqlConnection con = new SqlConnection(conectar);

            try
            {
                if(DispCPURadio.SelectedItem.Text == "CPU")
                {
                    if(TipoList.Text == "PC" || TipoList.Text == "LAPTOP")
                    {
                        //VOLVER TODOS LOS REGISTROS EN UPPER CASE
                        SqlCommand cmd = new SqlCommand("REGISTRAR_CPU", con)
                        {
                            CommandType = CommandType.StoredProcedure
                        };
                        cmd.Connection.Open();

                        if (string.IsNullOrEmpty(NoInventarioTxt.Text))
                        {
                            NoInventario = "NO";
                        }
                        else
                        {
                            NoInventario = NoInventarioTxt.Text;
                        }

                        int RAM, DISC;
                        string RAM_CAD = RAMTxt.Text, DISC_CAD = DiscoTxt.Text;

                        RAM = int.Parse(RAM_CAD);
                        DISC = int.Parse(DISC_CAD);

                        if (RAM > 1 && DISC > 1)
                        {
                            //CONVERTIR TODOS LOS REGISTROS EN MAYUSCULAS
                            NombreTxt.Text = NombreTxt.Text.ToUpper();
                            MarcaTxt.Text = MarcaTxt.Text.ToUpper();
                            ModeloTxt.Text = ModeloTxt.Text.ToUpper();
                            SOText.Text = SOText.Text.ToUpper();
                            OfficeTxt.Text = OfficeTxt.Text.ToUpper();
                            ObservacionTxt.Text = ObservacionTxt.Text.ToUpper();

                            cmd.Parameters.Add("@No_Serie", SqlDbType.VarChar, 100).Value = NoSerieTxt.Text;
                            cmd.Parameters.Add("@Area", SqlDbType.VarChar, 100).Value = AreaList.Text;
                            cmd.Parameters.Add("@Tipo", SqlDbType.VarChar, 100).Value = TipoList.Text;
                            cmd.Parameters.Add("@Nombre", SqlDbType.VarChar, 100).Value = NombreTxt.Text;
                            cmd.Parameters.Add("@Marca", SqlDbType.VarChar, 100).Value = MarcaTxt.Text;
                            cmd.Parameters.Add("@Modelo", SqlDbType.VarChar, 100).Value = ModeloTxt.Text;
                            cmd.Parameters.Add("@RAM", SqlDbType.Int).Value = RAMTxt.Text;
                            cmd.Parameters.Add("@DISCODURO", SqlDbType.Int).Value = DiscoTxt.Text;
                            cmd.Parameters.Add("@SO", SqlDbType.VarChar, 100).Value = SOText.Text;
                            cmd.Parameters.Add("@Office", SqlDbType.VarChar, 100).Value = OfficeTxt.Text;
                            //COLOCAR MÁS TEXT BOX A PROCESADOR PARA LOS GHZ Y TIPO
                            cmd.Parameters.Add("@Procesador", SqlDbType.VarChar, 100).Value = ProcesadorTxt.Text;
                            cmd.Parameters.Add("@NoInventario", SqlDbType.VarChar, 100).Value = NoInventario;
                            cmd.Parameters.Add("@Estatus", SqlDbType.VarChar, 100).Value = EstatusList.Text;
                            cmd.Parameters.Add("@Fecha_Entrega", SqlDbType.Date).Value = fecha;
                            cmd.Parameters.Add("@Observacion", SqlDbType.Text).Value = ObservacionTxt.Text;

                            SqlDataReader dr = cmd.ExecuteReader();

                            if (dr.Read())
                            {
                                Error.Text = "No Registrado";
                            }
                            else
                            {
                                Error.Text = "Registrado";

                                NoSerieTxt.Text = "";
                                NombreTxt.Text = "";
                                MarcaTxt.Text = "";
                                ModeloTxt.Text = "";
                                SOText.Text = "";
                                OfficeTxt.Text = "";
                                ObservacionTxt.Text = "";
                                RAMTxt.Text = "";
                                DiscoTxt.Text = "";
                                NoInventario = "NO";
                            }
                        }
                        else
                        {
                            Error.Text = "Tamaño de Disco o RAM inválidos.";
                        }
                    }
                    else
                    {
                        Error.Text = "No puedes agregar " + TipoList.Text + " en CPU.";
                    }
                }
                else
                {
                    if(TipoList.Text == "LAPTOP" || TipoList.Text == "PC")
                    {
                        Error.Text = "No puedes agregar " + TipoList.Text + " en Dispositivos.";
                    }
                    else
                    {
                        //VOLVER TODOS LOS REGISTROS EN UPPER CASE
                        SqlCommand cmd = new SqlCommand("SP_REGISTRAR_DISPOSITIVOS", con)
                        {
                            CommandType = CommandType.StoredProcedure
                        };
                        cmd.Connection.Open();

                        //CONVERTIR TODOS LOS REGISTROS EN MAYUSCULAS
                        MarcaTxt.Text = MarcaTxt.Text.ToUpper();
                        ModeloTxt.Text = ModeloTxt.Text.ToUpper();
                        ObservacionTxt.Text = ObservacionTxt.Text.ToUpper();

                        if (string.IsNullOrEmpty(NoInventarioTxt.Text))
                        {
                            NoInventario = "NO";
                        }
                        else
                        {
                            NoInventario = NoInventarioTxt.Text;
                        }



                        cmd.Parameters.Add("@NoSerie", SqlDbType.VarChar, 100).Value = NoSerieTxt.Text;
                        cmd.Parameters.Add("@Area", SqlDbType.VarChar, 100).Value = AreaList.Text;
                        cmd.Parameters.Add("@Tipo", SqlDbType.VarChar, 100).Value = TipoList.Text;
                        cmd.Parameters.Add("@Marca", SqlDbType.VarChar, 100).Value = MarcaTxt.Text;
                        cmd.Parameters.Add("@Modelo", SqlDbType.VarChar, 100).Value = ModeloTxt.Text;
                        //COLOCAR MÁS TEXT BOX A PROCESADOR PARA LOS GHZ Y TIPO
                        cmd.Parameters.Add("@NoInventario", SqlDbType.VarChar, 100).Value = NoInventario;
                        cmd.Parameters.Add("@Estatus", SqlDbType.VarChar, 100).Value = EstatusList.Text;
                        cmd.Parameters.Add("@Fecha", SqlDbType.Date).Value = fecha;
                        cmd.Parameters.Add("@Observacion", SqlDbType.Text).Value = ObservacionTxt.Text;

                        SqlDataReader dr = cmd.ExecuteReader();
                        if (dr.Read())
                        {
                            Error.Text = "No Registrado";
                        }
                        else
                        {
                            Error.Text = "Registrado";

                            NoSerieTxt.Text = "";
                            NombreTxt.Text = "";
                            MarcaTxt.Text = "";
                            ModeloTxt.Text = "";
                            SOText.Text = "";
                            OfficeTxt.Text = "";
                            ObservacionTxt.Text = "";
                            RAMTxt.Text = "";
                            DiscoTxt.Text = "";
                        }
                    }
                }
            }
            catch (Exception er)
            {
                Error.Text = er.Message;
            }
            
        }

        protected void DispCPURadio_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (DispCPURadio.SelectedItem.Text == "CPU")
            {
                NombreTxt.Enabled = true;
                RAMTxt.Enabled = true;
                DiscoTxt.Enabled = true;
                SOText.Enabled = true;
                OfficeTxt.Enabled = true;
                ProcesadorTxt.Enabled = true;
            }
            else
            {
                NombreTxt.Enabled = false;
                RAMTxt.Enabled = false;
                DiscoDuro.Enabled = false;
                SOText.Enabled = false;
                OfficeTxt.Enabled = false;
                ProcesadorTxt.Enabled = false;
            }
        }
    }
}
