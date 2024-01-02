using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

public abstract class Enumeration<T> : IComparable where T : Enumeration<T> 
{
    public int Id { get; private set; }
    public string Name { get; private set; }

    protected Enumeration(string name)
    {
        Name = name;
        Id = Name.GetHashCode();
    }

    public static List<T> GetAll()
    {
        List<T> result = new();
        var fields = typeof(T).GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);
        foreach (var info in fields)
        {
            var locatedValue = info.GetValue(null);

            if (locatedValue != null && locatedValue is T)
            {
                result.Add((T)locatedValue);
            }
        }
        return result;
    }

    public static T GetByName(string name)
    {
        foreach(var e in GetAll())
        {
            if (e.Name.Equals(name)) return e;
        }
        return null;
    }
    public override bool Equals(object obj)
    {
        if (obj is not T otherValue)
        {
            return false;
        }

        var typeMatches = GetType().Equals(obj.GetType());
        var valueMatches = Id.Equals(otherValue.Id);

        return typeMatches && valueMatches;
    }

    public static bool operator == (Enumeration<T> e1, Enumeration<T> e2) => e1.Equals(e2);
    public static bool operator != (Enumeration<T> e1, Enumeration<T> e2) => !(e1 == e2);

    public int CompareTo(object other) => other is T t ? Id.CompareTo(t.Id) : 0;

    public override int GetHashCode()
    {
        return Id;
    }
}