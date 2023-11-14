using MySql.Data.MySqlClient;

namespace ServerValutaManager
{
    internal class CalculateCourse
    {
        public static string[] ReturnCurse(string firstValuta, string secondValuta, DateTime dateRequest)
        {
            string[] valueValuta = new string[2];
            string connStr = "server=localhost;user=root;database=valutamanager;password=**;";
            MySqlConnection conn = new MySqlConnection(connStr);
            conn.Open();
            string sql = $"SELECT  {firstValuta}, {secondValuta} FROM valutaexchange WHERE date = '{dateRequest.ToString("yyyy.MM.dd")}'";
            MySqlCommand command = new MySqlCommand(sql, conn);
            MySqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                valueValuta[0] = reader[0].ToString();
                valueValuta[1] = reader[1].ToString();
            }
            reader.Close();
            conn.Close();
            return valueValuta;
        }

        public static string GetAllCurseNowDay(int countValuta, DateTime dateRequest)
        {
            string actualValuta = "";
            string connStr = "server=localhost;user=root;database=valutamanager;password=**;";
            MySqlConnection conn = new MySqlConnection(connStr);
            conn.Open();
            string sql = $"SELECT * FROM valutaexchange WHERE date = '{dateRequest.ToString("yyyy.MM.dd")}';";
            MySqlCommand command = new MySqlCommand(sql, conn);
            MySqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                for (int i = 3;  i < countValuta + 3; i++)
                {
                    actualValuta += $"{reader[i]}/";
                }
            }
            actualValuta = actualValuta.Remove(actualValuta.Length - 1);
            reader.Close();
            conn.Close();
            return actualValuta;
        }
    }
}
