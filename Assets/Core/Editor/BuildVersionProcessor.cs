using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

public class BuildVersionProcessor : IPreprocessBuildWithReport
{
    public int callbackOrder => 0;

    public void OnPreprocessBuild(BuildReport report)
    {
        string[] version = PlayerSettings.bundleVersion.Split('v', '.');
        if (version.Length == 3)
        {
            int major = int.Parse(version[1]);
            int minor = int.Parse(version[2]) + 1;
            string newVersion = version[0] + "v" + major + "." + minor;
            PlayerSettings.bundleVersion = newVersion;
        }
    }
}
