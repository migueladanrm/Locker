using System;

namespace Locker
{
    /// <summary>
    /// Excepción que se produce cuando ocurre un error al intentar descifrar los datos de una instancia de <see cref="LockerFile"/>.
    /// </summary>
    public class LockerPayloadDecryptException : Exception
    {

    }
}