using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace DotNetCommon.SystemFunctions
{
    public class SystemFunctions
    {
        private const string _defaultSystemProcessFile = "cmd.exe";
        private static readonly char _pathChar = Path.DirectorySeparatorChar;

        public static void RunSystemProcess(string commandString, string workingDirectory = null, string senstiveCommandInfoToReplace = null)
        {
            RunCustomProcess(_defaultSystemProcessFile, commandString, workingDirectory, senstiveCommandInfoToReplace);
        }

        public static void RunCustomProcess(string processFile, string parameters, string workingDirectory = null, string senstiveParameterInfoToReplace = null)
        {
            Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.WindowStyle = ProcessWindowStyle.Normal;
            startInfo.FileName = processFile;
            startInfo.Arguments = processFile.StartsWith(_defaultSystemProcessFile) ? "/C " + parameters : parameters;
            if (!String.IsNullOrWhiteSpace(workingDirectory)) startInfo.WorkingDirectory = workingDirectory;
            process.StartInfo = startInfo;
            process.Start();
            process.WaitForExit();
            if (process.ExitCode != 0)
            {
                if (!String.IsNullOrWhiteSpace(senstiveParameterInfoToReplace)) parameters = parameters.Replace(senstiveParameterInfoToReplace, "***");
                throw new Exception("An error occured while executing the following process: " + processFile + " " + parameters + ". The exit code was " + process.ExitCode);
            }
        }

        public static void CreateFreshDirectory(string directory)
        {
            DeleteDirectory(directory);
            Directory.CreateDirectory(directory);
        }

        public static void DeleteDirectory(string directory)
        {
            if (Directory.Exists(directory))
            {
                Directory.Delete(directory, true);
            }
        }

        public static void CreateDirectory(string directory)
        {
            Directory.CreateDirectory(directory);
        }

        public static string GetCurrentDirectory()
        {
            return Directory.GetCurrentDirectory();
        }

        public static void DeleteFile(string file)
        {
            if (File.Exists(file))
            {
                File.Delete(file);
            }
        }

        public static string RenameFile(string file, string newFileName)
        {
            FileInfo fileInfo = new FileInfo(file);
            string newFile = $"{fileInfo.Directory}{_pathChar}{newFileName}{fileInfo.Extension}";
            File.Move(file, newFile);
            return newFile;
        }

        public static string GetDateTimeAsFileNameSafeString()
        {
            string fileNameSafeDateTime = DateTime.Now.ToString("MM/dd/yyyy hh:mm tt").Replace("/", ".").Replace(":", "-");
            return fileNameSafeDateTime;
        }

        public static void CreateFile(string file, byte[] bytes)
        {
            File.WriteAllBytes(file, bytes);
        }

        public static void CreateFile(string file, string text)
        {
            File.WriteAllText(file, text);
        }

        public static void OpenFile(string file)
        {
            Process.Start($"{_defaultSystemProcessFile} ", @"/c " + "\"" + file + "\"");
        }

        public static string ReadAllText(string file)
        {
            return File.ReadAllText(file);
        }

        public static void CopyDirectory(string sourceDirectory, string targetDirectory)
        {
            Directory.CreateDirectory(targetDirectory);
            string[] files = Directory.GetFiles(sourceDirectory);
            foreach (string file in files)
            {
                FileInfo fileInfo = new FileInfo(file);
                string newFile = $"{targetDirectory}{_pathChar}{fileInfo.Name}";
                File.Copy(file, newFile);
            }

            string[] directories = Directory.GetDirectories(sourceDirectory);

            foreach (string directory in directories)
            {
                string subDirectory = directory.Split(_pathChar).Last();
                string targetSubDirectory = $"{targetDirectory}{_pathChar}{subDirectory}";
                CopyDirectory(directory, targetSubDirectory);
            }
        }

        public static string CombineDirectoryComponents(string directory, string directoryOrFile)
        {
            return directory.TrimEnd(_pathChar) + _pathChar + directoryOrFile.TrimStart(_pathChar);
        }

        public static string CombineDirectoryComponents(string directory1, string directory2, string directoryOrFile)
        {
            return CombineDirectoryComponents(CombineDirectoryComponents(directory1, directory2), directoryOrFile);
        }

        public static string CombineDirectoryComponents(string directory1, string directory2, string directory3, string directoryOrFile)
        {
            return CombineDirectoryComponents(CombineDirectoryComponents(directory1, directory2, directory3), directoryOrFile);
        }
    }
}
