using Lab3.Enums;
using Lab3.Interfaces;
using Npgsql;
using System.Collections.ObjectModel;
namespace Lab3;
/**
Name: Gerald Vinyeta, Michael Meisenburg
Date: 9/29/23
Description: Class working directly with the database of airports.
             Retrieves list from and saves changes to the airports table in our database
Bugs: None known
Reflection: Fairly straightforward using the example code
**/

#nullable enable
public class Database : IDatabase
{
    private readonly ObservableCollection<Airport> airports;
    private readonly string connString;

    /// <summary>
    /// Initialize this database object with connection string to connect to database
    /// and retrieve the list of airports from the database
    /// </summary>
	public Database()
	{
        connString = GetConnectionString();
        airports = new();
        airports = SelectAllAirports();
	}

    /// <summary>
    /// Get connection string used for database connection
    /// </summary>
    /// <returns>Connection string used for database connection</returns>
    static string GetConnectionString()
    {
        var connStringBuilder = new NpgsqlConnectionStringBuilder
        {
            Host = "angry-bleater-13037.5xj.cockroachlabs.cloud",
            Port = 26257,
            SslMode = SslMode.VerifyFull,
            Username = "vinyetagerald",  //hard coded for now
            Password = "eT_4Qv-dM_oZ76gWFeZYgg", //hard coded for now
            Database = "defaultdb",
            ApplicationName = string.Empty,
            IncludeErrorDetail = true
        };
        return connStringBuilder.ConnectionString;
    }

    /// <summary>
    /// Delete the airport containing the given id from the database
    /// </summary>
    /// <param name="id">The id of the airport to be deleted</param>
    public void DeleteAirport(string id)
    {
        //Business Logic will assure that airport with given id exists
        var conn = new NpgsqlConnection(connString);
        conn.Open();
        using var cmd = new NpgsqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = "DELETE FROM airports WHERE id = @id";
        cmd.Parameters.AddWithValue("id", id);
        int numDeleted = cmd.ExecuteNonQuery();
        SelectAllAirports();
    }

    /// <summary>
    /// Insert the given airport into the database
    /// </summary>
    /// <param name="airport">The airport to be inserted into the database</param>
    /// <returns>A DB error if a backend error occurs with database, no error otherwise</returns>
    public AirportError InsertAirport(Airport airport)
    {
        airports.Add(airport);
        try
        {
            using var conn = new NpgsqlConnection(connString);
            conn.Open(); ///Open database connection
            var cmd = new NpgsqlCommand //Create parameters for insertion
            {
                Connection = conn,
                CommandText = "INSERT INTO airports VALUES (@id, @city, @dateVisited, @rating)"
            };
            //Set parameters for insertion
            cmd.Parameters.AddWithValue("id", airport.Id);
            cmd.Parameters.AddWithValue("city", airport.City);
            cmd.Parameters.AddWithValue("dateVisited", airport.DateVisited);
            cmd.Parameters.AddWithValue("rating", airport.Rating);
            cmd.ExecuteNonQuery(); //Execute insertion
            SelectAllAirports(); //Update airports list
        }
        catch (PostgresException)
        {
            return AirportError.DB;
        }
        return AirportError.None;
    }

    /// <summary>
    /// Attempt to locate and return the airport with the given id, else return null
    /// </summary>
    /// <param name="id">id of airport to be located</param>
    /// <returns>The airport if it is found, null otherwise</returns>
    public Airport? SelectAirport(string id)
    {
        for (int i = 0; i < airports.Count; i++)
        {
            if (id == airports[i].Id)
            {
                return airports[i];
            }
        }
        return null;
    }

    /// <summary>
    /// Retrieve the list of airports as an observable collection from the 
    /// airports table in the database
    /// </summary>
    /// <returns>The list of airports as an observable collection</returns>
    public ObservableCollection<Airport> SelectAllAirports()
    {
        airports.Clear();
        var conn = new NpgsqlConnection(connString);
        conn.Open(); //Open database connection
        using var cmd = new NpgsqlCommand("SELECT id, city, date_visited, rating FROM airports", conn);
        using var reader = cmd.ExecuteReader();
        //Iterate through reader and add each airport from table to local observable collection
        while (reader.Read())
        {
            //Get data from each column for current row
            string id = reader.GetString(0);
            string city = reader.GetString(1);
            DateTime date = reader.GetDateTime(2);
            int rate = reader.GetInt32(3);
            Airport newAirport = new(id, city, date, rate);
            airports.Add(newAirport);
        }
        return airports;
    }

    /// <summary>
    /// Update the airport in the database with the given id with the remaining fields
    /// </summary>
    /// <param name="id">id of airport that will be updated</param>
    /// <param name="city">new city</param>
    /// <param name="date">new date</param>
    /// <param name="rate">new rating</param>
    /// <returns>A DB error if a backend error occurs with database, no error otherwise</returns>
    public AirportError UpdateAirport(string id, string city, DateTime date, int rate)
    {
        try
        {
            using var conn = new NpgsqlConnection(connString);
            conn.Open(); //Open database connection
            var cmd = new NpgsqlCommand //Create parameters for update
            {
                Connection = conn,
                CommandText = "UPDATE airports SET city = @city, date_visited = @dateVisited, " +
                              "rating = @rating WHERE id = @id;"
            };
            //Set parameters for update
            cmd.Parameters.AddWithValue("id", id);
            cmd.Parameters.AddWithValue("city", city);
            cmd.Parameters.AddWithValue("dateVisited", date);
            cmd.Parameters.AddWithValue("rating", rate);
            cmd.ExecuteNonQuery(); //Execute update
            SelectAllAirports(); //Update airports list
        }
        catch (PostgresException)
        {
            return AirportError.DB;
        }
        return AirportError.None;
    } 
}
