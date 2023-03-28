using Microsoft.VisualBasic.FileIO;
using SharpCompress.Archives;
using SharpCompress.Archives.Rar;
using SharpCompress.Common;
using System.IO.Compression;
using System.Net;
using System.Runtime;
using System.Security.AccessControl;

namespace CheckPrepper
{
    internal class Program
    {
        static void Main()
        {
            Console.WriteLine("Checkprepper!");
            
            // give the base folder containing unzipped Moodle archives
            string baseFolder = @"D:\Correctiewerk\";
            // start processing
            RecurseFolders(baseFolder);
        }

        /// <summary>
        /// Recursively process all folders
        /// </summary>
        /// <param name="_path">The starting point</param>
        /// <param name="_inStudentFolder">Marks if this is a folder containing a students work</param>
        private static void RecurseFolders(string _path, bool _inStudentFolder = false, List<string>? _currentLog = null)
        {
            bool inStudentFolder = _inStudentFolder;
            bool studentRootFolder;
            string? currentPath = _path;
            if (!Directory.Exists(currentPath) || currentPath == null)
            {
                Console.WriteLine($"{currentPath} doesn't exist!");
            }
            else
            {
                // check student folder            
                inStudentFolder = inStudentFolder || currentPath.Contains("assignsubmission_file_") || File.Exists($"{currentPath}\\_isStudent.txt") ;
                studentRootFolder = inStudentFolder && !_inStudentFolder;
                var logFileName = $"{currentPath}\\_isStudent.txt";

                if (studentRootFolder)
                {
                    // start new log
                    _currentLog = new List<string>
                    {
                        $"Timestamp: {DateTime.Now}"
                    };
                    if (!File.Exists(logFileName))
                    {
                        // first time in this student's folder, place marker
                        using (File.Create(logFileName)) { }
                    }

                    _currentLog.Add($"Processing student: {currentPath}");
                    Console.WriteLine($"Processing student: {currentPath}");
                    // rename folder
                    currentPath = RenameFolder(currentPath, _currentLog);
                    logFileName = $"{currentPath}\\_isStudent.txt";
                }

                if (inStudentFolder && currentPath != null)
                {
                    // delete obj / bin and stop processing
                    if (!TryDeleteDirectoryIfNeeded(currentPath, _currentLog))
                    {
                        // evaluate all files
                        foreach (string file in Directory.GetFiles(currentPath))
                        {
                            // unzip all rar/zip/gzip
                            if (UnZip(file, _currentLog))
                            {
                                File.Delete(file);
                            }
                        }
                    }
                }

                // proceed to subdirectories if paths still exists
                if (Directory.Exists(currentPath))
                {
                    foreach (var dir in Directory.GetDirectories(currentPath))
                    {
                        RecurseFolders(dir, inStudentFolder, _currentLog);
                    }
                }

                if (studentRootFolder)
                {
                    // all dirs processed for student, store logging
                    if (_currentLog != null)
                    {
                        File.AppendAllLines(logFileName, _currentLog);
                    }
                }
            }
        }

        /// <summary>
        /// Delete the directory if it is in the list op directories to delete
        /// </summary>
        /// <param name="_currentPath">The current directory</param>
        /// <param name="_currentLog">The current log to add stuff to</param>
        /// <returns>True if the directory was deleted</returns>
        private static bool TryDeleteDirectoryIfNeeded(string _currentPath, List<string>? _currentLog = null)
        {
            var dirInfo = new DirectoryInfo(_currentPath);
            if (dirInfo.Name.Equals("obj") || dirInfo.Name.Equals("bin") || dirInfo.Name.Equals("__MACOSX"))
            {
                if (_currentLog != null)
                {
                    _currentLog.Add($"-> Delete {_currentPath} because it is a temp folder.");
                    Console.WriteLine($"-> Delete {_currentPath} because it is a temp folder.");
                }
                Directory.Delete(_currentPath, true);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Rename the path and return the new name
        /// </summary>
        /// <param name="path">The original path</param>
        /// <param name="_currentLog">The current log to add stuff to</param>
        /// <returns>The new path</returns>
        private static string? RenameFolder(string? path, List<string>? _currentLog = null)
        {
            // voornaam achternaam_3122769_assignsubmission_file_
            if (path != null)
            {
                var dI = new DirectoryInfo(path);
                if (dI != null)
                {
                    var dirName = dI.Name;
                    var parts = dirName.Split('_');
                    if (parts.Length > 0 && !dirName.Equals(parts[0]))
                    {
                        try
                        {
                            var dest = $"{dI.Parent}\\{parts[0]}";
                            Directory.Move(path, dest);
                            return dest;
                        }
                        catch (Exception ex)
                        {
                            _currentLog?.Add($"Error renaming folder: {ex.Message}");
                            Console.WriteLine($"Error renaming folder");
                        } 
                    }
                }
            }
            return path;            
        }

        /// <summary>
        /// Unzip the file
        /// </summary>
        /// <param name="_file">The file to unzip</param>
        /// <param name="_currentLog">The current log to add stuff to</param>
        /// <returns>true if it was a zipfile and it was unzipped</returns>
        private static bool UnZip(string _file, List<string>? _currentLog = null)
        {
            if (_file != null)
            {
                var destDir = Path.GetDirectoryName(_file);
                var destFile = Path.GetFileNameWithoutExtension(_file).Replace(' ', '_');
                var dest = $"{destDir}\\{destFile}_unzip";

                try
                {
                    switch (Path.GetExtension(_file))
                    {
                        case ".zip":
                            if (destDir != null && destFile != null)
                            {
                                ZipFile.ExtractToDirectory(_file, dest, true);
                                _currentLog?.Add($"-> {_file} Unzipped!");
                                Console.WriteLine($"-> {_file} Unzipped!");
                                return true;
                            }
                            break;
                        case ".rar":
                            if (destDir != null && destFile != null)
                            {
                                Directory.CreateDirectory(dest);
                                using var archive = RarArchive.Open(_file);
                                foreach (var entry in archive.Entries.Where(entry => !entry.IsDirectory))
                                {
                                    entry.WriteToDirectory(dest, new ExtractionOptions()
                                    {
                                        ExtractFullPath = true,
                                        Overwrite = true
                                    });
                                }
                                return true;
                            }
                            break;
                        case ".gz":
                            // todo implement
                            Console.WriteLine($"-> {_file} is an GZ archive!");
                            break;
                        default:
                            break;
                    }
                }
                catch (Exception ex)
                {
                    // delete dest if unzip failed
                    Directory.Delete(dest, true);
                    _currentLog?.Add($"-> {_file} not unzipped: {ex.Message}");
                    Console.WriteLine($"-> {_file} not unzipped: {ex.Message}");
                }
            }
            return false;        
        }
    }
}