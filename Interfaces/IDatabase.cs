using Lab3.Enums;
using System.Collections.ObjectModel;

namespace Lab3.Interfaces;
/**
Name: Gerald Vinyeta, Michael Meisenburg
Description: Interface defining the basic operations and fields that 
             a class handling an airports database must support
Date: 9/29/2023
Bugs: None known
Reflection: Nothing new here
**/
#nullable enable
public interface IDatabase
{
    ObservableCollection<Airport> SelectAllAirports();
    Airport? SelectAirport(string id);
    AirportError InsertAirport(Airport airport);
    void DeleteAirport(string id);
    AirportError UpdateAirport(string id, string city, DateTime date, int rate);
}
