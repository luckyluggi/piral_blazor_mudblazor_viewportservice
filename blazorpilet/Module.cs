using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;

namespace blazorpilet
{
	public class Module
	{
		public static void Main()
		{
			// this entrypoint should remain empty
		}

		public static void ConfigureServices(IServiceCollection services)
        {
        }

        public static void ConfigureShared(IServiceCollection services)
        {
            services.AddLogging();
            services.AddMudServices(options => options.PopoverOptions.ThrowOnDuplicateProvider = false);
        }
    }
}
