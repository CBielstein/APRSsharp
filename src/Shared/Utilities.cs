namespace AprsSharp.Shared;

using System.Reflection;

/// <summary>
/// Extension methods used across the project.
/// </summary>
public static class Utilities
{
        /// <summary>
        /// Returns the version of the assembly from the perspective of the caller.
        /// </summary>
        /// <returns>The assembly version as a string, without source link.</returns>
        public static string GetAssemblyVersion()
        {
            var assemblyInfo = Assembly.GetCallingAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>();
            return assemblyInfo.InformationalVersion;
        }
}