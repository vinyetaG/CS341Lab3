using Lab3.Enums;
using Lab3.Interfaces;
using System.Collections.ObjectModel;

namespace Lab3;
/**
Name: Gerald Vinyeta, Michael Meisenburg
Description: Class enforcing airport list rules which communicates with the Database
Date: 9/29/2023
Bugs: None known
Reflection: Just fixed a grammar error in calculate statistics when there is only one airport
**/
#nullable enable
public class BusinessLogic : IBusinessLogic
{
	private readonly IDatabase database;
	private const int BronzeCount = 42;
	private const int SilverCount = 84;
	private const int GoldCount = 125;
	public const int MinIdLength = 3;
	public const int MaxIdLength = 4;
	public const int MaxCityLength = 25;
	public const int MinRating = 1;
	public const int MaxRating = 5;

	/// <summary>
	/// Property pertaining to the observable collecton of airports retrieved from
	/// the database
	/// </summary>
	public ObservableCollection<Airport> Airports
	{
		get { return GetAirports(); }
	}

	/// <summary>
	/// Initialize new database which will perform the operations requested by the Business Logic
	/// </summary>
	public BusinessLogic()
	{
		database = new Database();
	}

	/// <summary>
	/// Requests that the database add a new airport with the provided fields
	/// </summary>
	/// <param name="id"></param>
	/// <param name="city"></param>
	/// <param name="dateVisited"></param>
	/// <param name="rating"></param>
	/// <returns>
	/// An error with the DB backend, an error with one of the fields, 
	/// an error if an airport with the id already exists, or no error
	/// </returns>
	public AirportError AddAirport(string id, string city, DateTime dateVisited, int rating)
	{
		//Ensure fields are valid
		AirportError possibleError = VerifyFields(id, city, rating);
		if (possibleError != AirportError.None)
		{
			return possibleError;
		}
		//Ensure airport is not already in list
		var airports = GetAirports();
		Airport airport = new(id, city, dateVisited, rating);
		if (airports.Contains(airport))
		{
			return AirportError.Duplicate;
		}
		//Request database to add, returning IO error if one occurs
		return database.InsertAirport(airport);
	}

	/// <summary>
	/// Requests that the database delete the airport with the given Id
	/// </summary>
	/// <param name="id">The id of the airport to be deleted</param>
	/// <returns>
	/// An error if the id is invalid or if the airport does not exist,
	/// no error otherwise
	/// </returns>
	public AirportError DeleteAirport(string id)
	{
		//Check id is valid
		AirportError possibleError = VerifyFields(id: id);
		if (possibleError != AirportError.None)
		{
			return possibleError;
		}
		//Check that airport exists in list
		Airport? airport = FindAirport(id);
		if (airport is null)
		{
			return AirportError.DoesNotExist;
		}
		//Request database for deletion, returning IO error if one occurs
		database.DeleteAirport(id);
		return AirportError.None;
	}

	/// <summary>
	/// Requests that database edit airport with given id
	/// </summary>
	/// <param name="id">id of the airport to be updated</param>
	/// <param name="city">new city</param>
	/// <param name="dateVisited">new date visited</param>
	/// <param name="rating">new rating</param>
	/// <returns>
	/// An error if the city or rating are invalid, an error with the DB
	/// backend, or no error
	/// </returns>
	public AirportError EditAirport(string id, string city, DateTime dateVisited, int rating)
	{
		//Ensure new fields are valid
		AirportError possibleError = VerifyFields(city: city, rating: rating);
		if (possibleError != AirportError.None)
		{
			return possibleError;
		}

		//Request database to update, returning DB error if one occurs
		return database.UpdateAirport(id, city, dateVisited, rating);
	}

	/// <summary>
	/// Fetches airport information from the database for the airport with the given id
	/// </summary>
	/// <param name="id">id of airport to be located</param>
	/// <returns>The airport with the given id, or null</returns>
	public Airport? FindAirport(string id)
	{
		Airport? airport = database.SelectAirport(id);
		return airport;
	}

	/// <summary>
	/// Gets the list of airports from the database as an observable collection
	/// </summary>
	/// <returns>The observable collection of airports</returns>
	public ObservableCollection<Airport> GetAirports()
	{
		return database.SelectAllAirports();
	}

    /// <summary>
	/// Returns a string indicating how many more airports are needed for the user
	/// to attain the next rank, or a commendation if the user has achieved max rank
	/// </summary>
	/// <returns>The string containing user rank progress</returns>
    public string CalculateStatistics()
    {
        int airportsVisited = GetAirports().Count;
        string stats;
		string plurality = airportsVisited != 1 ? "s" : string.Empty;

        if (airportsVisited < BronzeCount)
        {
            stats = string.Format("{0} airport{1} visited; {2} airports remaining until achieving Bronze",
                               airportsVisited, plurality, BronzeCount - airportsVisited);
        }
        else if (airportsVisited < SilverCount)
        {

            stats = string.Format("{0} airport{1} visited; {2} airports remaining until achieving Silver",
                               airportsVisited, plurality, SilverCount - airportsVisited);

        }
        else if (airportsVisited < GoldCount)
        {
            stats = string.Format("{0} airport{1} visited; {2} airports remaining until achieving Gold",
                               airportsVisited, plurality, GoldCount - airportsVisited);
        }
        else
        {
            stats = "You have visited all airports and achieved Gold! Nice!";
        }
        return stats;
    }

    /// <summary>
    /// Verifies the fields that the business logic knows the rules for, and returns the first error
    /// that was found or no error. Each field is optional so the caller only needs to provide what
    /// they need verified
    /// </summary>
	/// <param name="id">possibly invalid airport id</param>
	/// <param name="city">possibly invalid airport city</param>
	/// <param name="rating">possibly invalid airport rating</param>
	/// <returns>Error denoting the first field that was found to be invalid, or no error</returns>
    private static AirportError VerifyFields(string id = "KATW", string city = "Appleton", int rating = 5)
	{
        if (id.Length < MinIdLength || id.Length > MaxIdLength)
        {
            return AirportError.Id;
        }
        if (city.Length > MaxCityLength)
        {
            return AirportError.City;
        }
        if (rating < MinRating || rating > MaxRating)
        {
            return AirportError.Rating;
        }
		return AirportError.None;
    }
}
