using System;

namespace Events
{
    public interface Id<T>
    {
        T Id { get; set;}
    }

    public interface IEntity<T> : Id<T> { }

    public interface IExternalEntity : IEntity<string>
    {
    }
}