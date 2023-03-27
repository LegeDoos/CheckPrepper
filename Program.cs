using System.IO.Compression;
using System.Security.AccessControl;

namespace CheckPrepper
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Checkprepper!");

            string baseFolder = @"D:\Correctiewerk";
            RecurseFolders(baseFolder);
        }

        private static void RecurseFolders(string _path, bool _inStudentFolder = false)
        {
            bool inStudentFolder = _inStudentFolder;
            bool studentRootFolder = false;
            string currentPath = _path;
            if (!Directory.Exists(currentPath))
            {
                Console.WriteLine($"{currentPath} doesn't exist!");
            }
            else
            {
                // check student folder            
                inStudentFolder = inStudentFolder || currentPath.Contains("assignsubmission_file_") || File.Exists($"{currentPath}\\_isStudent.txt") ;

                if (inStudentFolder && !_inStudentFolder)
                {
                    studentRootFolder = true;
                    if (!File.Exists($"{currentPath}\\_isStudent.txt"))
                    {
                        // first time in this student's folder, place marker
                        using (File.Create($"{currentPath}\\_isStudent.txt")) { }
                    }
                }

                Console.WriteLine($"Processing {currentPath} (Studentfolder: {inStudentFolder}, Rootfolder: {studentRootFolder})");

                if (inStudentFolder)
                {
                    // delete obj / bin and stop processing
                    var dirInfo = new DirectoryInfo(_path);
                    if (dirInfo.Name.Equals("obj") || dirInfo.Name.Equals("bin"))
                    {
                        Console.WriteLine($"-> Delete {currentPath} because it is a temp folder.");
                        Directory.Delete(currentPath, true);
                    }
                    else
                    {
                        // evaluate all files
                        foreach (string file in Directory.GetFiles(currentPath))
                        {
                            // unzip all rar/zip/gzip
                            if (UnZip(file))
                            {
                                File.Delete(file);
                            }
                        }
                    }

                    // rename folder
                    currentPath = RenameFolder(currentPath);
                }

                // proceed to subdirectories if paths still exists
                if (Directory.Exists(currentPath))
                {
                    foreach (var dir in Directory.GetDirectories(currentPath))
                    {
                        RecurseFolders(dir, inStudentFolder);
                    }
                }

                if (studentRootFolder)
                {
                    // all dirs processed for student, store logging
                    // todo
                }
            }
        }

        /// <summary>
        /// Rename the path and return the new name
        /// </summary>
        /// <param name="path">The original path</param>
        /// <returns>The new path</returns>
        private static string RenameFolder(string path)
        {
            // todo implement
            return path;            
        }

        /// <summary>
        /// Unzip the file
        /// </summary>
        /// <param name="_file">The file to unzip</param>
        /// <returns>true if it was a zipfile and it was unzipped</returns>
        private static bool UnZip(string _file)
        {
            if (_file != null)
            {
                try
                {
                    switch (Path.GetExtension(_file))
                    {
                        case ".zip":
                            Console.WriteLine($"-> {_file} is an ZIP archive!");
                            var destDir = Path.GetDirectoryName(_file);
                            var destFile = Path.GetFileNameWithoutExtension(_file);
                            if (destDir != null && destFile != null)
                            {
                                var dest = $"{destDir}\\{destFile}_unzip";
                                ZipFile.ExtractToDirectory(_file, dest);
                                return true;
                            }
                            break;
                        case ".rar":
                            // todo implement
                            Console.WriteLine($"-> {_file} is an RAR archive!");
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
                    Console.WriteLine(ex.Message);
                }
            }
            return false;        
        }
    }
}