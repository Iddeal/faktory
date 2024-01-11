using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Faktory.Core.InternalUtilities
{
    public static class FileUsage
    {

        #region "PInvoke Stuff"

        [DllImport("rstrtmgr.dll", CharSet = CharSet.Unicode)]
        [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
        static extern int RmStartSession(out uint pSessionHandle, int dwSessionFlags, string strSessionKey);

        [DllImport("rstrtmgr.dll")]
        [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
        static extern int RmEndSession(uint pSessionHandle);

        [DllImport("rstrtmgr.dll", CharSet = CharSet.Unicode)]
        [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
        static extern int RmRegisterResources(uint pSessionHandle, uint nFiles, string[] rgsFilenames, uint nApplications, [In] RM_UNIQUE_PROCESS[] rgApplications, uint nServices, string[] rgsServiceNames);

        [DllImport("rstrtmgr.dll")]
        static extern int RmGetList(uint dwSessionHandle, out uint pnProcInfoNeeded, ref uint pnProcInfo, [In, Out] RM_PROCESS_INFO[] rgAffectedApps, ref uint lpdwRebootReasons);

        [StructLayout(LayoutKind.Sequential)]
        struct RM_UNIQUE_PROCESS
        {
            public int dwProcessId;
            public FILETIME ProcessStartTime;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        struct RM_PROCESS_INFO
        {
            const int CCH_RM_MAX_APP_NAME = 255;
            const int CCH_RM_MAX_SVC_NAME = 63;

            public RM_UNIQUE_PROCESS Process;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = CCH_RM_MAX_APP_NAME + 1)]
            public string strAppName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = CCH_RM_MAX_SVC_NAME + 1)]
            public string strServiceShortName;
            public RM_APP_TYPE ApplicationType;
            public uint AppStatus;
            public uint TSSessionId;
            [MarshalAs(UnmanagedType.Bool)]
            public bool bRestartable;
        }

        enum RM_APP_TYPE
        {
            RmUnknownApp = 0,
            RmMainWindow = 1,
            RmOtherWindow = 2,
            RmService = 3,
            RmExplorer = 4,
            RmConsole = 5,
            RmCritical = 1000
        }
        #endregion

        /// <summary>
        /// Utility for determining if another process has locked a file.
        /// </summary>
        /// <param name="filePath">List of files to check.</param>
        /// <returns>Bool indicating in use. If True, it returns the name of the process using the file.</returns>
        public static (bool inUse, string processName) GetFileUsage(string filePath)
        {
            var processes = GetProcessesUsingFiles(filePath);
            return processes.Count > 0 ? (true, processes[0].ProcessName) : (false, null);
        }

        static IList<Process> GetProcessesUsingFiles(params string[] filePaths)
        {
            var processes = new List<Process>();

            // Create a session
            var result = RmStartSession(out var sessionHandle, 0, Guid.NewGuid().ToString("N"));
            if (result != 0) throw new Win32Exception();

            try
            {
                // Register files we're checking
                result = RmRegisterResources(sessionHandle, (uint)filePaths.LongLength, filePaths, 0, null, 0, null);
                if (result != 0) throw new Win32Exception();

                // Get list of processes using these files
                uint pnProcInfo = 0, lpdwRebootReasons = 0;
                result = RmGetList(sessionHandle, out var pnProcInfoNeeded, ref pnProcInfo, null, ref lpdwRebootReasons);
                if (result == 234) //ERROR_MORE_DATA
                {
                    var processInfo = new RM_PROCESS_INFO[pnProcInfoNeeded];
                    pnProcInfo = (uint)processInfo.Length;

                    result = RmGetList(sessionHandle, out pnProcInfoNeeded, ref pnProcInfo, processInfo, ref lpdwRebootReasons);
                    if (result != 0) throw new Win32Exception();

                    for (var i = 0; i < pnProcInfo; i++)
                    {
                        try
                        {
                            processes.Add(Process.GetProcessById(processInfo[i].Process.dwProcessId));
                        }
                        catch (ArgumentException)
                        {
                            //process likely exited }
                        }
                    }
                }
            }
            finally
            {
                RmEndSession(sessionHandle);
            }

            return processes;
        }
    }
}