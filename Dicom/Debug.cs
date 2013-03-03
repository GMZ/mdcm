// mDCM: A C# DICOM library
//
// Copyright (c) 2006-2008  Colby Dillion
//
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
//
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
//
// Author:
//    Colby Dillion (colby.dillion@gmail.com)

using System;
using System.Diagnostics;
using System.IO;
using Dicom.Utility;

using NLog;
using NLog.Config;
using NLog.Targets;

namespace Dicom
{
    public delegate void DebugOutputHandler(String msg);

    public static class Debug
    {
        static Debug()
        {
            GetStartDirectory();
        }

        private static Logger _log;

        public static Logger Log
        {
            get { return _log ?? (_log = LogManager.GetLogger("DICOM")); }
            set
            {
                _log = value;
            }
        }

        public static void InitializeSyslogLogger(bool console)
        {
            InitializeSyslogLogger(514, console);
        }
        public static void InitializeSyslogLogger(int port, bool console)
        {
            var config = new LoggingConfiguration();

            var st = new SyslogTarget {Port = port};
            config.AddTarget("Syslog", st);

            config.LoggingRules.Add(new LoggingRule("*", LogLevel.Debug, st));

            if (console)
            {
                var ct = new ColoredConsoleTarget {Layout = "${message}"};
                config.AddTarget("Console", ct);

                config.LoggingRules.Add(new LoggingRule("*", LogLevel.Debug, ct));
            }

            LogManager.Configuration = config;
        }

        public static void InitializeConsoleDebugLogger()
        {
            var config = new LoggingConfiguration();

            var ct = new ColoredConsoleTarget {Layout = "${message}"};
            config.AddTarget("Console", ct);

            config.LoggingRules.Add(new LoggingRule("*", LogLevel.Debug, ct));

            LogManager.Configuration = config;
        }

        private static string _startdir;
        public static string GetStartDirectory()
        {
            if (_startdir == null)
            {
                _startdir = Process.GetCurrentProcess().StartInfo.WorkingDirectory;
                if (String.IsNullOrEmpty(_startdir))
                {
                    _startdir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
                    if (_startdir != null) _startdir = _startdir.Substring(6);
                }
            }
            return _startdir;
        }

        public static string GetCallingFunction()
        {
            try
            {
                var trace = new StackTrace(true);
// ReSharper disable ForControlVariableIsNeverModified
                for (var i = 2; i < trace.FrameCount; )
// ReSharper restore ForControlVariableIsNeverModified
                {
                    var frame = trace.GetFrame(i);
                    return String.Format("{0}() at {1}:{2}", frame.GetMethod().Name, frame.GetFileName(), frame.GetFileLineNumber());
                }
            }
// ReSharper disable EmptyGeneralCatchClause
            catch (Exception)
// ReSharper restore EmptyGeneralCatchClause
            {
            }
            return "[unable to trace stack]";
        }
    }
}
