using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MySql.Data.MySqlClient;
using TMPro;

public class DBConnect
{
    private MySqlConnection conn;

    public DBConnect(string host, string dbname, string ui, string pw)
    {
        conn = new MySqlConnection($"Database = {dbname}; Data Source = {host}; User Id = {ui}; Password = {pw};");
    }

    private bool Connect_Open() 
    {
        try
        {
            conn.Open();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    private bool Connect_Close() 
    {
        try
        {
            conn.Close();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public void InsertInto(Data temp)
    {
        if (Connect_Open())
        {
            string query = "INSERT INTO data(save_Name, points, time) VALUES(@save_Name, @points, @time);";
            MySqlCommand cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@save_Name", temp.saveName);
            cmd.Parameters.AddWithValue("@points", temp.point);
            cmd.Parameters.AddWithValue("@time", temp.time);
            cmd.ExecuteNonQuery();
            Connect_Close();
        }
    }
}
