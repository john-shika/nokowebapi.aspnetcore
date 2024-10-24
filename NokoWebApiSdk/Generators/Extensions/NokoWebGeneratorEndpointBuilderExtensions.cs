﻿using NokoWebApiSdk.Cores;
using NokoWebApiSdk.Generators.Helper;

namespace NokoWebApiSdk.Generators.Extensions;

public static class NokoWebGeneratorEndpointBuilderExtensions
{
    public static void EntryPoint<TEntryPoint>(this NokoWebApplication nokoWebApplication) 
        where TEntryPoint : class, INokoWebGeneratorHelperEntryPoint, new()
    {
        var entryPoint = new TEntryPoint();
        var action = (NokoWebApplication application) =>
        { 
            entryPoint.OnInitialized(application);
        };
        
        nokoWebApplication.Listen(action.Listener);
    }
}