using MySql.Data.MySqlClient;
using System.Text.RegularExpressions;

namespace ServerValutaManager
{
    internal class EveryDayAddDB
    {
        async static public void FillDBNewData()
        {
            await Task.Run(() =>
            {
                while (true)
                {
                    Restart:;
                    var date = DateTime.Now.ToString("yyyy.MM.dd");
                    string connStr = "server=localhost;user=root;database=valutamanager;password=**;";
                    MySqlConnection conn = new MySqlConnection(connStr);
                    conn.Open();
                    string id = new MySqlCommand("SELECT MAX(id) FROM valutaexchange;", conn).ExecuteScalar().ToString();
                    string lastDate = new MySqlCommand($"SELECT date FROM valutaexchange WHERE id = {id}", conn).ExecuteScalar().ToString();
                    lastDate = $"{lastDate.Split(' ')[0].Split('.')[2]}.{lastDate.Split(' ')[0].Split('.')[1]}.{lastDate.Split(' ')[0].Split('.')[0]}";
                    if (lastDate == date)
                    {
                        Thread.Sleep(86400000);
                        goto Restart;
                    }
                    lastDate = Convert.ToDateTime(lastDate).AddDays(1).ToString("yyyy.MM.dd").Replace(".", "/");
                    string[] lastValutaRate = new string[0];
                    string[] lastValutaCharName = new string[0];
                    int count = 0;
                    string sql = $"SELECT * FROM valutaexchange WHERE id = {id};";
                    MySqlCommand command = new MySqlCommand(sql, conn);
                    MySqlDataReader readerRate = command.ExecuteReader();
                    while (readerRate.Read())
                    {
                        for (int i = 0; i < readerRate.FieldCount - 3; i++)
                        {
                            if (readerRate[i + 3].ToString() == "")
                            {
                                continue;
                            }
                            Array.Resize(ref lastValutaRate, lastValutaRate.Length + 1);
                            lastValutaRate[i] = readerRate[i + 3].ToString().Replace(",", ".");
                        }
                    }
                    readerRate.Close();
                    sql = $"SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE table_name = 'valutaexchange';";
                    command = new MySqlCommand(sql, conn);
                    MySqlDataReader readerCharName = command.ExecuteReader();
                    while (readerCharName.Read())
                    {
                        for (int i = 0; i < readerCharName.FieldCount; i++)
                        {
                            if (readerCharName[i].ToString() == "id" || readerCharName[i].ToString() == "date" || readerCharName[i].ToString() == "RUB" || readerCharName[i].ToString() == "ATS" || readerCharName[i].ToString() == "AZM" || readerCharName[i].ToString() == "BEF" || readerCharName[i].ToString() == "BYB" || readerCharName[i].ToString() == "BYR" || readerCharName[i].ToString() == "DEM" || readerCharName[i].ToString() == "EEK" || readerCharName[i].ToString() == "ESP" || readerCharName[i].ToString() == "FIM" || readerCharName[i].ToString() == "FRF" || readerCharName[i].ToString() == "GRD" || readerCharName[i].ToString() == "IEP" || readerCharName[i].ToString() == "ISK" || readerCharName[i].ToString() == "ITL" || readerCharName[i].ToString() == "LTL" || readerCharName[i].ToString() == "LVL" || readerCharName[i].ToString() == "NLG" || readerCharName[i].ToString() == "NUL" || readerCharName[i].ToString() == "PTE" || readerCharName[i].ToString() == "TRL" || readerCharName[i].ToString() == "XEU")
                            {
                                continue;
                            }
                            Array.Resize(ref lastValutaCharName, lastValutaCharName.Length + 1);
                            lastValutaCharName[count] = readerCharName[i].ToString();
                            count += 1;
                        }
                    }
                    readerCharName.Close();
                    var httpClient = new HttpClient();
                    var response = httpClient.GetAsync($"https://www.cbr-xml-daily.ru/archive/{lastDate}/daily_json.js").Result;
                    var content = response.Content.ReadAsStringAsync().Result;
                    var regex = new Regex(@"(?<=\u0022CharCode\u0022\:\s\u0022)[a-zA-Z]{3}|(?<=\u0022Value\u0022\:\s)[0-9.]{1,15}(?=\,)").Matches(content);
                    string[] charCodeValuta = new string[regex.Count / 2];
                    string[] valueValuta = new string[regex.Count / 2];
                    int countName = 0;
                    int countValue = 0;
                    for (int i = 0; i < regex.Count; i++)
                    {
                        if (i % 2 == 0)
                        {
                            charCodeValuta[countName] = regex[i].Value;
                            countName++;
                        }
                        else
                        {
                            valueValuta[countValue] = regex[i].Value;
                            countValue++;
                        }
                    }
                    string valutaName = "";
                    string valutaMoney = "";
                    foreach (var match in charCodeValuta)
                    {
                        valutaName += $"{match}, ";
                    }
                    foreach (var match in valueValuta)
                    {
                        valutaMoney += $"{match}, ";
                    }
                    if (valutaName == "" && valutaMoney == "")
                    {
                        foreach (var match in lastValutaRate)
                        {
                            valutaMoney += $"{match}, ";
                        }
                        foreach (var match in lastValutaCharName)
                        {
                            valutaName += $"{match}, ";
                        }
                    }
                    valutaName = valutaName.Remove(valutaName.Length - 2, 2);
                    valutaMoney = valutaMoney.Remove(valutaMoney.Length - 2, 2);
                    sql = $"INSERT INTO `valutaexchange` (date, {valutaName}) VALUES ('{lastDate}', {valutaMoney});";
                    command = new MySqlCommand(sql, conn);
                    command.ExecuteNonQuery();
                }
            });
        } 
    }
}
