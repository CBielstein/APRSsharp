namespace AprsSharp.Shared;

using System.Reflection;

/// <summary>
/// Utilities and helpers methods used across AprsSharp.
/// </summary>
public static class Utilities
{
        /// <summary>
        /// Returns the version of the assembly from the perspective of the caller.
        ///
        /// Note: Inclusion/exclusion of the source link part of the version depends on the
        /// IncludeSourceRevisionInInformationalVersion setting in the csproj of the
        /// calling class/project.
        /// </summary>
        /// <returns>The assembly version as a string.</returns>
        public static string GetAssemblyVersion()
        {
            var assemblyInfo = Assembly.GetCallingAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>();
            return assemblyInfo.InformationalVersion;
        }
}
