using System;

namespace Erabikata.Backend.CollectionManagers;

internal class AnkiConnectException : Exception
{
    public AnkiConnectException(string error) : base(error) { }
}
