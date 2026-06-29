using System;
using System.Collections.Generic;
using System.Text;

namespace Game
{
    public partial class ScriptsGenerator
    {
        private static string GenerateServicesCode(IReadOnlyList<ServiceRegistrationData> serviceTypes)
        {
            var builder = new StringBuilder();
            builder.AppendLine("using System.Collections.Generic;");
            builder.AppendLine("using VContainer;");
            builder.AppendLine("using Whaledevelop.Services;");
            builder.AppendLine();
            builder.AppendLine("namespace Game");
            builder.AppendLine("{");
            builder.AppendLine("    public static class GameServicesComposition");
            builder.AppendLine("    {");
            builder.AppendLine("        public static void RegisterServices(IContainerBuilder builder)");
            builder.AppendLine("        {");

            foreach (var serviceType in serviceTypes)
            {
                builder.AppendLine($"            builder.Register<{GetTypeName(serviceType.ConcreteType)}>(Lifetime.Singleton).AsSelf().As<{GetTypeName(serviceType.InterfaceType)}>().As<IService>();");
            }

            builder.AppendLine();
            builder.AppendLine("            return;");
            builder.AppendLine("        }");
            builder.AppendLine();
            builder.AppendLine("        public static IReadOnlyList<IService> ResolveServices(IObjectResolver resolver)");
            builder.AppendLine("        {");
            builder.AppendLine("            var services = new IService[]");
            builder.AppendLine("            {");

            foreach (var serviceType in serviceTypes)
            {
                builder.AppendLine($"                resolver.Resolve<{GetTypeName(serviceType.ConcreteType)}>(),");
            }

            builder.AppendLine("            };");
            builder.AppendLine();
            builder.AppendLine("            return services;");
            builder.AppendLine("        }");
            builder.AppendLine("    }");
            builder.AppendLine("}");

            return builder.ToString();
        }

        private static string GenerateRuntimeDataCode(
            string scopeName,
            IReadOnlyList<ModelRegistrationData> modelTypes,
            IReadOnlyList<Type> systemTypes)
        {
            var builder = new StringBuilder();
            builder.AppendLine("using System.Collections.Generic;");
            builder.AppendLine("using VContainer;");
            builder.AppendLine("using Whaledevelop.Systems;");
            builder.AppendLine();
            builder.AppendLine("namespace Game");
            builder.AppendLine("{");
            builder.AppendLine($"    public static class {scopeName}RuntimeDataComposition");
            builder.AppendLine("    {");
            builder.AppendLine("        public static void RegisterModels(IContainerBuilder builder)");
            builder.AppendLine("        {");

            foreach (var modelType in modelTypes)
            {
                if (modelType.IsShared)
                {
                    var concreteTypeName = GetTypeName(modelType.ConcreteType);
                    builder.AppendLine($"            builder.Register<{concreteTypeName}>(Lifetime.Transient).Keyed(typeof({concreteTypeName}));");
                    builder.AppendLine($"            builder.Register<{concreteTypeName}>(resolver =>");
                    builder.AppendLine($"                    resolver.Resolve<global::Game.ISharedGameModelsContainer>().GetOrCreate(");
                    builder.AppendLine($"                        () => resolver.Resolve<{concreteTypeName}>(typeof({concreteTypeName}))),");
                    builder.AppendLine($"                    Lifetime.Singleton)");
                    builder.AppendLine($"                .AsSelf()");
                    builder.AppendLine($"                .As<{GetTypeName(modelType.InterfaceType)}>()");
                    builder.AppendLine($"                .As<IGameModel>();");
                }
                else
                {
                    builder.AppendLine($"            builder.Register<{GetTypeName(modelType.ConcreteType)}>(Lifetime.Singleton).AsSelf().As<{GetTypeName(modelType.InterfaceType)}>().As<IGameModel>();");
                }
            }

            builder.AppendLine();
            builder.AppendLine("            return;");
            builder.AppendLine("        }");
            builder.AppendLine();
            builder.AppendLine("        public static IReadOnlyList<IGameModel> ResolveModels(IObjectResolver resolver)");
            builder.AppendLine("        {");
            builder.AppendLine("            var models = new IGameModel[]");
            builder.AppendLine("            {");

            foreach (var modelType in modelTypes)
            {
                builder.AppendLine($"                resolver.Resolve<{GetTypeName(modelType.ConcreteType)}>(),");
            }

            builder.AppendLine("            };");
            builder.AppendLine();
            builder.AppendLine("            return models;");
            builder.AppendLine("        }");
            builder.AppendLine();
            builder.AppendLine("        public static void RegisterSystems(IContainerBuilder builder)");
            builder.AppendLine("        {");

            foreach (var systemType in systemTypes)
            {
                builder.AppendLine($"            builder.Register<{GetTypeName(systemType)}>(Lifetime.Singleton).AsSelf().As<IGameSystem>();");
            }

            builder.AppendLine();
            builder.AppendLine("            return;");
            builder.AppendLine("        }");
            builder.AppendLine();
            builder.AppendLine("        public static IReadOnlyList<IGameSystem> ResolveSystems(IObjectResolver resolver)");
            builder.AppendLine("        {");
            builder.AppendLine("            var systems = new IGameSystem[]");
            builder.AppendLine("            {");

            foreach (var systemType in systemTypes)
            {
                builder.AppendLine($"                resolver.Resolve<{GetTypeName(systemType)}>(),");
            }

            builder.AppendLine("            };");
            builder.AppendLine();
            builder.AppendLine("            return systems;");
            builder.AppendLine("        }");
            builder.AppendLine("    }");
            builder.AppendLine("}");

            return builder.ToString();
        }

        private static string GetTypeName(Type type)
        {
            var typeName = type.FullName.Replace("+", ".");

            return $"global::{typeName}";
        }
    }
}
