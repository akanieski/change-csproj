using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Build.BuildEngine;
using System.IO;

namespace UpdateCSPROJ
{
    class Program
    {
        static void Main(string[] args)
        {
            List<String> parms = Environment.GetCommandLineArgs().ToList();
            string dir = parms[parms.IndexOf("-d") + 1];
            foreach (string path in Directory.GetFiles(dir, "*.csproj", SearchOption.AllDirectories))
            {
                try
                {
                    Project project = new Project();
                    project.Load(path);
                    bool modified = false;
                    foreach (BuildPropertyGroup grp in project.PropertyGroups)
                    {
                        if (grp.Condition.Contains("x86") || grp.Condition.Contains("Debug|"))
                        {
                            bool bTemp1 = false, bTemp2 = false, bTemp3 = false;
                            foreach (BuildProperty prop in grp)
                            {
                                if (prop.Name == "Optimize")
                                {
                                    prop.Value = "false";
                                    bTemp1 = true;
                                    modified = true;
                                }
                                if (prop.Name == "DebugSymbols")
                                {
                                    prop.Value = grp.Condition.Contains("Release") ? "false" : "true";
                                    bTemp2 = true;
                                    modified = true;
                                }
                                if (prop.Name == "DebugType")
                                {
                                    prop.Value = "full";
                                    bTemp3 = true;
                                    modified = true;
                                }
                            }
                            if (!bTemp1)
                            {
                                grp.AddNewProperty("Optimize", "false");
                                modified = true;
                            }
                            if (!bTemp2)
                            {
                                grp.AddNewProperty("DebugSymbols", grp.Condition.Contains("Release") ? "false" : "true");
                                modified = true;
                            }
                            if (!bTemp3)
                            {
                                grp.AddNewProperty("DebugType", "full");
                                modified = true;
                            }
                        }
                    }
                    if (modified)
                    {
                        File.SetAttributes(path, File.GetAttributes(path) & ~FileAttributes.ReadOnly);
                        project.Save(path);
                        Console.WriteLine("Updated " + path);
                    }

                }
                catch (Exception ex)
                {

                    Console.WriteLine(ex.ToString());
                }
            }
        }
    }
}
