﻿using Autofac;
using ManagementServer.Infrastructure;
using RtspServer.Abstract;

namespace ManagementServer.Configuration.AutofacModules;

public class DefaultModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<StaticDataSource>()
            .As<IDataSource>()
            .SingleInstance();
        
        // if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        // {
        //     builder.RegisterType<MacosWebcamDataSource>()
        //         .As<IDataSource>()
        //         .AutoActivate()
        //         .SingleInstance();
        // }
    }
}