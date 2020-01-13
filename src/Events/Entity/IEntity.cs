using System;

namespace Events
{
    public interface Id<T>
    {
        T Id { get; set;}
    }

    public interface IEntity<T> : Id<T> {
        Network Network { get; set; }
    }

    public interface IExternalEntity : IEntity<string>
    {
    }
}