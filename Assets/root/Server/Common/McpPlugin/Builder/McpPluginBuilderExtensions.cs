#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace com.IvanMurzak.Unity.MCP.Common
{
    public static partial class McpPluginBuilderExtensions
    {
        public static IServiceCollection AddMcpPlugin(this IServiceCollection services, Action<IMcpPluginBuilder>? configure = null)
        {
            // Create an instance of McpAppBuilder
            var mcpPluginBuilder = new McpPluginBuilder(services);

            // Allow additional configuration of McpAppBuilder
            configure?.Invoke(mcpPluginBuilder);

            return services;
        }
        public static IMcpPluginBuilder WithAppFeatures(this IMcpPluginBuilder builder)
        {
            builder.AddMcpRunner();

            builder.Services.AddSingleton<RpcRouter>();
            builder.Services.AddSingleton<IRpcRouter>(sp => sp.GetRequiredService<RpcRouter>());
            builder.Services.AddSingleton<IRemoteServer>(sp => sp.GetRequiredService<RpcRouter>());

            // // TODO: Uncomment if any tools or prompts are needed from this assembly
            // // var assembly = typeof(McpAppBuilderExtensions).Assembly;

            // // builder.WithToolsFromAssembly(assembly);
            // // builder.WithPromptsFromAssembly(assembly);
            // // builder.WithResourcesFromAssembly(assembly);

            return builder;
        }

        public static IMcpPluginBuilder AddMcpRunner(this IMcpPluginBuilder builder)
        {
            builder.Services.TryAddSingleton<IMcpRunner, McpRunner>();
            return builder;
        }
    }
}