using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace FtpFileCommunication.Service
{
    public class DalClass
    {
        // Connection string from web.config
        private static string sqlConnString = ConfigurationManager
            .ConnectionStrings["DefaultConnection"].ConnectionString;

        public static DataTable GetOrders(DataTable dtHeader)
        {
            if (dtHeader == null)
                dtHeader = new DataTable();
            using (SqlDataAdapter da = new SqlDataAdapter(
                "dbo.IMOrdersSelectFtp_sp", sqlConnString))
            {
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                da.Fill(dtHeader);
            }
            return dtHeader;
        }

        public static DataTable GetOrdersLines(DataTable dtLines, long documentId)
        {
            if (dtLines == null)
                dtLines = new DataTable();
            using (SqlDataAdapter da = new SqlDataAdapter(
                "dbo.IMOrderLinesSelectFtp_sp", sqlConnString))
            {
                da.SelectCommand.Parameters.AddWithValue("@OrderId", documentId);
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                da.Fill(dtLines);
            }
            return dtLines;
        }

        public static void UpdateOrderFtpStatus(long id, bool status)
        {
            using var sqlConnection = new SqlConnection(sqlConnString);
            try
            {
                sqlConnection.Open();
                using var sqlCommand = new SqlCommand(
                    "dbo.PorosiaUpdateStatusiFtp_sp", sqlConnection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                sqlCommand.Parameters.AddWithValue("@Id", id);
                sqlCommand.Parameters.AddWithValue("@StatusiFtp", status);
                sqlCommand.ExecuteScalar();
            }
            catch (SqlException ex)
            {
                throw ex;
            }
        }
    }
}