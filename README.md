# Repro Steps
- `cd ./my-app` and run `npm i` and then `npm run build`
- `cd ../blazorpilet` and run `dotnet build`
- `cd ../piral~/blazorpilet` and run `pilet debug`

#  Problem
Open `http://localhost:1234/example`.
IBrowserViewportService is injected on this Page. 
When resizing the window from small to big the box changes because IBrowserViewportService.SubscribeAsync() callback is called correctly.
So everything is working.

But in the Browsers console we have the following error: 
```
_bound_js_globalThis_console_error:7 fail: Piral.Blazor.Core.ModuleContainerService[0]
      Could not establish local dependency injection. Error: System.Reflection.TargetInvocationException: Exception has been thrown by the target of an invocation.
       ---> System.InvalidOperationException: Unable to resolve service for type 'Microsoft.Extensions.Logging.ILogger`1[MudBlazor.BrowserViewportService]' while attempting to activate 'MudBlazor.BrowserViewportService'.
         at Microsoft.Extensions.DependencyInjection.ServiceLookup.CallSiteFactory.CreateArgumentCallSites(Type implementationType, CallSiteChain callSiteChain, ParameterInfo[] parameters, Boolean throwIfCallSiteNotFound)
         at Microsoft.Extensions.DependencyInjection.ServiceLookup.CallSiteFactory.CreateConstructorCallSite(ResultCache lifetime, Type serviceType, Type implementationType, CallSiteChain callSiteChain)
         at Microsoft.Extensions.DependencyInjection.ServiceLookup.CallSiteFactory.TryCreateExact(ServiceDescriptor descriptor, Type serviceType, CallSiteChain callSiteChain, Int32 slot)
         at Microsoft.Extensions.DependencyInjection.ServiceLookup.CallSiteFactory.TryCreateExact(Type serviceType, CallSiteChain callSiteChain)
         at Microsoft.Extensions.DependencyInjection.ServiceLookup.CallSiteFactory.CreateCallSite(Type serviceType, CallSiteChain callSiteChain)
         at Microsoft.Extensions.DependencyInjection.ServiceLookup.CallSiteFactory.GetCallSite(Type serviceType, CallSiteChain callSiteChain)
         at Microsoft.Extensions.DependencyInjection.ServiceProvider.CreateServiceAccessor(Type serviceType)
         at System.Collections.Concurrent.ConcurrentDictionary`2[[System.Type, System.Private.CoreLib, Version=7.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e],[System.Func`2[[Microsoft.Extensions.DependencyInjection.ServiceLookup.ServiceProviderEngineScope, Microsoft.Extensions.DependencyInjection, Version=7.0.0.0, Culture=neutral, PublicKeyToken=adb9793829ddae60],[System.Object, System.Private.CoreLib, Version=7.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e]], System.Private.CoreLib, Version=7.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e]].GetOrAdd(Type key, Func`2 valueFactory)
         at Microsoft.Extensions.DependencyInjection.ServiceProvider.GetService(Type serviceType, ServiceProviderEngineScope serviceProviderEngineScope)
         at Microsoft.Extensions.DependencyInjection.ServiceProvider.GetService(Type serviceType)
         at Dazinator.Extensions.DependencyInjection.ReRoutingServiceProvider.GetService(Type serviceType)
         at Dazinator.Extensions.DependencyInjection.DisposableServiceProvider.GetService(Type serviceType)
         at Piral.Blazor.Core.PiletServiceProvider.GetService(Type serviceType) in /home/vsts/work/1/s/src/Piral.Blazor.Core/PiletServiceProvider.cs:line 40
         at Microsoft.AspNetCore.Components.ComponentFactory.<>c__DisplayClass7_0.<CreateInitializer>g__Initialize|1(IServiceProvider serviceProvider, IComponent component)
         at Microsoft.AspNetCore.Components.ComponentFactory.PerformPropertyInjection(IServiceProvider serviceProvider, IComponent instance)
         at Microsoft.AspNetCore.Components.ComponentFactory.InstantiateComponent(IServiceProvider serviceProvider, Type componentType)
         at System.Reflection.MethodInvoker.InterpretedInvoke(Object obj, Span`1 args, BindingFlags invokeAttr)
         --- End of inner exception stack trace ---
         at System.Reflection.RuntimeMethodInfo.Invoke(Object obj, BindingFlags invokeAttr, Binder binder, Object[] parameters, CultureInfo culture)
         at System.Reflection.MethodBase.Invoke(Object obj, Object[] parameters)
         at Piral.Blazor.Core.Manipulator`1.<>c__DisplayClass6_0[[Piral.Blazor.Core.ModuleContainerService, Piral.Blazor.Core, Version=7.0.8.0, Culture=neutral, PublicKeyToken=null]].<InitializeRenderer>b__0(IServiceProvider provider, Type componentType) in /home/vsts/work/1/s/src/Piral.Blazor.Core/Manipulator.cs:line 48
         at Piral.Blazor.Core.Manipulator`1[[Piral.Blazor.Core.ModuleContainerService, Piral.Blazor.Core, Version=7.0.8.0, Culture=neutral, PublicKeyToken=null]].OverrideComponentInitializer(Type componentType, IServiceProvider provider) in /home/vsts/work/1/s/src/Piral.Blazor.Core/Manipulator.cs:line 69
```

The error is thrown here `https://github.com/MudBlazor/MudBlazor/blob/cabe5488b7116ad37c656d576e824a0429dd32ee/src/MudBlazor/Services/Browser/BrowserViewportService.cs#L51`  
because ILogger<BrowserViewportService> can not be injected. But it should be found because we added `services.AddLogging();` in `Module.cs`


I think this might be fixable via:
``` c#
services.TryAddScoped<ILogger<BrowserViewportService>>(sp => {
    var factory = sp.GetRequiredService<ILoggerFactory>();
    return factory.CreateLogger<ILogger<BrowserViewportService>>();
});
```
But we cant do this becuase BrowserViewportService is defined as `internal` inside MudBlazor.
