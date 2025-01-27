using System.Collections.Generic;

using CommonDLL.Dto;

namespace BlackTemple.EpicMine
{
    public class VersionHelper
    {
        /// <summary>
        /// Check app version is valid and need up to date
        /// </summary>
        /// <param name="currentVersion"></param>
        /// <param name="validVersions"></param>
        /// <param name="isValid"></param>
        /// <param name="isNeedUpToDate"></param>
        public static void CheckAppVersion(string currentVersion, List<AppVersionInfo> validVersions, out bool isValid, out bool isNeedUpToDate)
        {
            isValid = false;
            isNeedUpToDate = false;

            foreach (var versionInfo in validVersions)
            {
                if (versionInfo.IsOutdated)
                    continue;

                if (string.Equals(versionInfo.AppVersion, currentVersion))
                {
                    isValid = true;
                    continue;
                }

                var currentVersionNumbers = currentVersion.Split('.');
                var verifiableVersionNumbers = versionInfo.AppVersion.Split('.');

                var currentMajorVersion = int.Parse(currentVersionNumbers[0]);
                var currentMinorVersion = int.Parse(currentVersionNumbers[1]);
                var currentFixVersion = int.Parse(currentVersionNumbers[2]);

                var verifiableMajorVersion = int.Parse(verifiableVersionNumbers[0]);
                var verifiableMinorVersion = int.Parse(verifiableVersionNumbers[1]);
                var verifiableFixVersion = int.Parse(verifiableVersionNumbers[2]);

                
                if (verifiableMajorVersion > currentMajorVersion)
                {
                    if (versionInfo.IsPreferUpdate)
                    {
                        isNeedUpToDate = true;
                        break;
                    }
                }

                if (verifiableMajorVersion == currentMajorVersion && verifiableMinorVersion > currentMinorVersion)
                {
                    if (versionInfo.IsPreferUpdate)
                    {
                        isNeedUpToDate = true;
                        break;
                    }
                }

                if (verifiableMajorVersion == currentMajorVersion && verifiableMinorVersion == currentMinorVersion && verifiableFixVersion > currentFixVersion)
                {
                    if (versionInfo.IsPreferUpdate)
                    {
                        isNeedUpToDate = true;
                        break;
                    }
                }
            }
        }
    }
}