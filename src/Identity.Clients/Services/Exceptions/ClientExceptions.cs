// Copyright 2020 Carnegie Mellon University. 
// Released under a MIT (SEI) license. See LICENSE.md in the project root. 

using System;

namespace Identity.Clients
{
    public class ClientException: Exception {}
    public class ClientUpdateException: ClientException { }
    public class ClientNameNotUniqueException: ClientException { }
    public class ResourceUpdateException: ClientException { }
    public class ResourceNameNotUniqueException: ClientException { }
}
