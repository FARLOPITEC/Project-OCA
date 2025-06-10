using System;
using System.Runtime.InteropServices;

public static class SQLiteLoader
{
    [DllImport("e_sqlite3")]
    public static extern int sqlite3_libversion_number();

    static SQLiteLoader()
    {
        try
        {
            int version = sqlite3_libversion_number();
            UnityEngine.Debug.Log("SQLite library loaded successfully! Version: " + version);
        }
        catch (Exception ex)
        {
            UnityEngine.Debug.LogError("Error loading SQLite library: " + ex.Message);
        }
    }
}

