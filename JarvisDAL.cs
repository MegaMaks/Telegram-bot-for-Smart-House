using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;


namespace ConsoleTelegram
{
    class JarvisDAL
    {
        private SqlConnection sqlCn = null;
        private string sqlStr = null;

        public JarvisDAL()
        {            
            sqlStr = ConfigurationManager.ConnectionStrings["cnStrJarvis"].ConnectionString;
        }

        public void OpenConnection()
        {
            sqlCn = new SqlConnection();
            sqlCn.ConnectionString = sqlStr;
            sqlCn.Open();
        }

        public void CloseConnection()
        {
            sqlCn.Close();
        }

        public DataTable SelEquipment()
        {
            DataTable dTable = new DataTable();
            using (SqlCommand cmd = new SqlCommand())
            {
                SqlDataAdapter sqlAdapter = new SqlDataAdapter();
                cmd.Connection = this.sqlCn;
                cmd.CommandText = "select * from equipment where id_equipment=1";
                cmd.CommandType = CommandType.Text;
                sqlAdapter.SelectCommand = cmd;
                sqlAdapter.Fill(dTable);
            }
            return dTable;
        }

        public DataTable Sellamp(int idlamp)
        {
            DataTable dTable = new DataTable();
            using (SqlCommand cmd = new SqlCommand())
            {
                SqlDataAdapter sqlAdapter = new SqlDataAdapter();
                cmd.Connection = this.sqlCn;
                cmd.CommandText = "select * from lamp where id_lamp=@id_lamp";
                cmd.Parameters.Add("@id_lamp", SqlDbType.Int).Value = idlamp;
                cmd.CommandType = CommandType.Text;
                sqlAdapter.SelectCommand = cmd;
                sqlAdapter.Fill(dTable);
            }
            return dTable;
        }


        public void UpdateStatusLamp(int status,int idlamp)
        {

            using (SqlCommand cmd = new SqlCommand())
            {
                SqlDataAdapter sqlAdapter = new SqlDataAdapter();
                cmd.Connection = this.sqlCn;
                cmd.CommandText = "update lamp set status=@status where id_lamp=@id_lamp";
                cmd.Parameters.Add("@status", SqlDbType.Int).Value = status;
                cmd.Parameters.Add("@id_lamp", SqlDbType.Int).Value = idlamp;
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();
            }
        }

        public void UpdateDimmerLamp(int dimmer, int idlamp)
        {

            using (SqlCommand cmd = new SqlCommand())
            {
                SqlDataAdapter sqlAdapter = new SqlDataAdapter();
                cmd.Connection = this.sqlCn;
                cmd.CommandText = "update lamp set dimmer=@dimmer where id_lamp=@id_lamp";
                cmd.Parameters.Add("@dimmer", SqlDbType.Int).Value = dimmer;
                cmd.Parameters.Add("@id_lamp", SqlDbType.Int).Value = idlamp;
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();
            }
        }

        public void UpdateStatusEquipment(int status)
        {

            using (SqlCommand cmd = new SqlCommand())
            {
                SqlDataAdapter sqlAdapter = new SqlDataAdapter();
                cmd.Connection = this.sqlCn;
                cmd.CommandText = "update equipment set status=@status where id_equipment=1";
                cmd.Parameters.Add("@status", SqlDbType.Int).Value = status;
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();
            }
        }

        public void UpdateValueEquipment(int value)
        {

            using (SqlCommand cmd = new SqlCommand())
            {
                SqlDataAdapter sqlAdapter = new SqlDataAdapter();
                cmd.Connection = this.sqlCn;
                cmd.CommandText = "update equipment set current_value=@value where id_equipment=1";
                cmd.Parameters.Add("@value", SqlDbType.Int).Value = value;
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();
            }
        }
    }
}
