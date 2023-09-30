using Lab3.Enums;
using System.Collections.ObjectModel;

namespace Lab3.Interfaces;
/**
Name: Gerald Vinyeta, Michael Meisenburg
Description: Interface defining the operations that the business 
             logic must support for the user interface
Date: 9/29/2023
Bugs: None known
Reflection: Nothing new here
**/
#nullable enable
public interface IBusinessLogic
{
    AirportError AddAirport(string id, string city, DateTime dateVisited, int rating);
    AirportError DeleteAirport(string id);
    AirportError EditAirport(string id, string city, DateTime dateVisited, int rating);
    Airport? FindAirport(string id);
    string CalculateStatistics();
    ObservableCollection<Airport> GetAirports();

}
