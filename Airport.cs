using System.ComponentModel;

#pragma warning disable CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
namespace Lab3;
/**
Name: Gerald Vinyeta, Michael Meisenburg
Description: Class defining fields contained by each airport
Date: 9/29/2023
Bugs: None known
Reflection: Nothing new here
**/
#nullable enable
public class Airport : IEquatable<Airport>, INotifyPropertyChanged
{
    private string id;
    private string city;
    private DateTime dateVisited;
    private int rating;
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Initialize new airport with the given fields
    /// </summary>
    /// <param name="id">id of airport</param>
    /// <param name="city">city of airport</param>
    /// <param name="dateVisited">date airport was visited</param>
    /// <param name="rating">a personal rating given to the airport</param>
    public Airport(string id, string city, DateTime dateVisited, int rating)
    {
        this.id = id;
        this.city = city;
        this.dateVisited = dateVisited;
        this.rating = rating;
    }

    /// <summary>
    /// Default constructor for an airport containing fields for the
    /// KATW, Appleton airport
    /// </summary>
    public Airport()
    {
        id = "KATW";
        city = "Appleton";
        dateVisited = DateTime.Now;
        rating = 5;
    }

    /// <summary>
    /// Property pertaining to unique airport id
    /// </summary>
    public string Id 
    { 
        get { return id; } 
        set 
        { 
            id = value;
            OnPropertyChanged(nameof(Id));
        } 
    }

    /// <summary>
    /// Property pertaining to the airport's city
    /// </summary>
    public string City
    {
        get { return city; }
        set 
        { 
            city = value;
            OnPropertyChanged(nameof(City));
        }
    }

    /// <summary>
    /// Property pertaining to date airport was visited
    /// </summary>
    public DateTime DateVisited
    {
        get { return dateVisited; }
        set 
        { 
            dateVisited = value;
            OnPropertyChanged(nameof(DateVisited));
        }
    }

    /// <summary>
    /// Property pertaining to user's personal airport rating
    /// </summary>
    public int Rating
    {
        get { return rating; }
        set 
        { 
            rating = value;
            OnPropertyChanged(nameof(Rating));
        }
    }

    /// <summary>
    /// Notifies the UI that the given property was changed so that it can be updated
    /// </summary>
    /// <param name="propertyName">The property which was updated</param>
    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    /// <summary>
    /// Checks if this airport is the same as the given airport.
    /// Airports are considered the same if their ids are identical
    /// </summary>
    /// <param name="other">The airport to compare to this one</param>
    /// <returns>Whether this airport is equal to the other airport</returns>
    //
    public bool Equals(Airport? other)
    {
        if (other is null)
        {
            return false;
        }
        return id == other.id;
    }

    /// <summary>
    /// Checks if the given object is equal to this airport
    /// </summary>
    /// <param name="obj">The object to be compared</param>
    /// <returns>Whether the given object is equal to this airport</returns>
    public override bool Equals(object? obj)
    {
        return Equals(obj as Airport);
    }
}

