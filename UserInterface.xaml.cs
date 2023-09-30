namespace Lab3;

using Lab3.Enums;
using System;
/**
Name: Gerald Vinyeta, Michael Meisenburg
Date: 9/29/23
Description: Class backing the user interface xaml, providing methods 
             for the user to add, edit, or delete airports and print
             statistics for their medal progress
Bugs: None known
Reflection: Only change was adding event handler for calculate
            statistics button which was trivial
**/
#nullable enable
public partial class UserInterface : ContentPage
{
    public UserInterface()
    {
        InitializeComponent();
        BindingContext = MauiProgram.bl;
    }

    /// <summary>
    /// Adds airport using contents of text fields
    /// </summary>
    void AddAirport_Clicked(object sender, EventArgs e)
    {
        AirportError possibleError = AddAirport(IdENT.Text, CityENT.Text, DateENT.Text, RatingENT.Text);
        if (possibleError != AirportError.None)
        {
            DisplayAlert("Error", GetErrorMsg(possibleError, AirportOperation.Add), "OK");
        }
    }

    /// <summary>
    /// Deletes currently selected airport
    /// </summary>
    void DeleteMovie_Clicked(object sender, EventArgs e)
    {
        Airport? airport = CV.SelectedItem as Airport;
        if (airport == null) return;
        AirportError possibleError = DeleteAirport(airport.Id);
        if (possibleError != AirportError.None)
        {
            DisplayAlert("Error", GetErrorMsg(possibleError, AirportOperation.Delete), "OK");
        }
    }

    /// <summary>
    /// Attempts to edit currently selected airport with fields contained in text boxes. 
    /// Id cannot be changed.
    /// </summary>
    void EditMovie_Clicked(object sender, EventArgs e)
    {
        Airport? airport = CV.SelectedItem as Airport;
        if (airport == null) return;
        Console.WriteLine(airport.Id);
        AirportError possibleError = EditAirport(airport.Id, CityENT.Text, DateENT.Text, RatingENT.Text);
        if (possibleError != AirportError.None)
        {
            DisplayAlert("Error", GetErrorMsg(possibleError, AirportOperation.Edit), "OK");
        }
    }

    /// <summary>
    /// Displays an alert containing progress to the next rank
    /// </summary>
    void PrintStatistics_Clicked(object sender, EventArgs e)
    {
        DisplayAlert(string.Empty, MauiProgram.bl.CalculateStatistics(), "OK");
    }

    /// <summary>
    /// Attempts to add an airport to the list
    /// </summary>
    /// <param name="id">airport id</param>
    /// <param name="city">airport city</param>
    /// <param name="dateInput">date airport was visited</param>
    /// <param name="rateInput">a personal rating given to the airport</param>
    /// <returns>
    /// Any error pertaining to invalid fields, a possible DB backend error, or no error
    /// </returns>
    private static AirportError AddAirport(string id, string city, string dateInput, string rateInput)
    {
        //Check for rudimentary errors
        AirportError possibleError = VerifyFields(id, city, dateInput, rateInput);
        if (possibleError != AirportError.None)
        {
            return possibleError;
        }
        DateTime date = DateTime.Parse(dateInput);
        int rate = Int32.Parse(rateInput);
        //Prompt business logic to add airport, returning error if one occurs
        possibleError = MauiProgram.bl.AddAirport(id, city, date, rate);
        return possibleError;
    }

    /// <summary>
    /// Attempts to delete airport with given id
    /// </summary>
    /// <param name="id">Id of airport to be deleted</param>
    /// <returns>Error if airport with given id does not exist, or no error</returns>
    private static AirportError DeleteAirport(string id)
    {
        //Prompt business logic to delete airport, returning error if one occurs
        AirportError possibleError = MauiProgram.bl.DeleteAirport(id);
        return possibleError;
    }

    /// <summary>
    /// Attempts to edit airport with the given id with the remaining fields
    /// </summary>
    /// <param name="id">id of airport to be edited</param>
    /// <param name="city">new airport city</param>
    /// <param name="dateInput">new airport date</param>
    /// <param name="rateInput">new airport rating</param>
    /// <returns>An error pertaining to airport fields, a DB backend error, or no error</returns>
    private static AirportError EditAirport(string id, string city, string dateInput, string rateInput)
    {
        //Check for rudimentary errors
        AirportError possibleError = VerifyFields(id, city, dateInput, rateInput);
        if (possibleError != AirportError.None)
        {
            return possibleError;
        }
        DateTime date = DateTime.Parse(dateInput);
        int rate = Int32.Parse(rateInput);
        //Prompt business logic layer to edit airport, returning error if one occurs
        possibleError = MauiProgram.bl.EditAirport(id, city, date, rate);
        return possibleError;
    }

    /// <summary>
    /// Ensures the given fields are not empty or formatted incorrectly
    /// </summary>
    /// <param name="id">possibly invalid airport id</param>
    /// <param name="city">possibly invalid airport city</param>
    /// <param name="dateInput">possibly invalid date visited</param>
    /// <param name="rateInput">possibly invalid airport rating</param>
    /// <returns>The first AirportError that is found, or AirportError.None if none were found</returns>
    private static AirportError VerifyFields(string? id, string? city, string? dateInput, string? rateInput)
    {
        if (id is null || id.Length == 0)
        {
            return AirportError.Id;
        }
        if (city is null || city.Length == 0)
        {
            return AirportError.City;
        }
        bool dateValid = DateTime.TryParse(dateInput, out DateTime date);
        if (!dateValid)
        {
            return AirportError.Date;
        }
        bool rateValid = Int32.TryParse(rateInput, out int rate);
        if (!rateValid)
        {
            return AirportError.Rating;
        }
        return AirportError.None;
    }

    /// <summary>
    /// If an error was returned by a method performing an operation on the airports list, return the
    /// appropriate error message for the operation that was attempted
    /// </summary>
    /// <param name="e">The airport error</param>
    /// <param name="op">The operation that invoked the error</param>
    /// <returns>The appropriate error message for the operation that was attempted</returns>
    private static string GetErrorMsg(AirportError e, AirportOperation op)
    {
        string opStr = "adding";
        if (op == AirportOperation.Edit)
        {
            opStr = "editing";
        }
        else if (op == AirportOperation.Delete)
        {
            opStr = "deleting";
        }
        switch (e)
        {
            case AirportError.Id:
                return string.Format("Error while {0} Airport: Id must be between {1} and {2} characters long",
                                  opStr, BusinessLogic.MinIdLength, BusinessLogic.MaxIdLength);
            case AirportError.City:
                return string.Format("Error while {0} Airport: City must not be empty and within {1} characters",
                                  opStr, BusinessLogic.MaxCityLength);
            case AirportError.Rating:
                return string.Format("Error while {0} Airport: Rating must be between {1} and {2}",
                                  opStr, BusinessLogic.MinRating, BusinessLogic.MaxRating);
            case AirportError.Date:
                return string.Format("Error while {0} Airport: Date must be entered in mm/dd/yyyy format",
                                  opStr);
            case AirportError.Duplicate:
                return string.Format("Error while {0} Airport: Duplicate airport Id", opStr);
            case AirportError.DoesNotExist:
                return string.Format("Error while {0} Airport: Airport with given Id does not exist", opStr);
            case AirportError.DB:
                return "Error occurred while communicating with the database. Your changes could not be saved.";
            default:
                return string.Empty;
        }
    }
}

