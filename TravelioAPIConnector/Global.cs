using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;

namespace TravelioAPIConnector;

public static class Global
{
    public const bool IsREST = false;

    public static Binding GetBinding(string uri)
    {
        return uri.StartsWith("https", StringComparison.OrdinalIgnoreCase)
            ? new BasicHttpsBinding()
            : new BasicHttpBinding();
    }
}
