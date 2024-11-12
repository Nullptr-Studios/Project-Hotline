using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

/// <summary>
/// Processes the build version before the build starts.
/// </summary>
public class BuildVersionProcessor : IPreprocessBuildWithReport
{
    /// <summary>
    /// The order in which the callback is invoked.
    /// </summary>
    public int callbackOrder => 0;

    /// <summary>
    /// Called before the build process starts.
    /// </summary>
    /// <param name="report">The build report.</param>
    public void OnPreprocessBuild(BuildReport report)
    {
        // Split the bundle version string into components based on 'v' and '.' characters
        string[] version = PlayerSettings.bundleVersion.Split('v', '.');
        
        // Check if the version string has the expected format
        if (version.Length == 3)
        {
            // Parse the major and minor version numbers
            int major = int.Parse(version[1]);
            int minor = int.Parse(version[2]) + 1;
            
            // Construct the new version string
            string newVersion = version[0] + "v" + major + "." + minor;
            
            // Update the bundle version in PlayerSettings
            PlayerSettings.bundleVersion = newVersion;
        }
    }
}