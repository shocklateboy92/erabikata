using System;

namespace Erabikata.Backend.CollectionManagers;

public class AnkiConnectException : Exception
{
    public AnkiConnectException(string error) : base(error) { }
}
