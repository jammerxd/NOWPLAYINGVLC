using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;
using System.Globalization;
using System.Threading;
using MahApps;
using MahApps.Metro;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System.Diagnostics;

namespace NOWPLAYINGVLC
{
    public class Program
    {

        [STAThread]
        public static void Main(string[] args)
        {

            Assembly myAssemblyList = Assembly.GetExecutingAssembly();
            string[] myResources = myAssemblyList.GetManifestResourceNames();
            foreach (string resource in myResources)
            {
                if (resource.EndsWith(".dll"))
                {
                    string[] temp_name = resource.Split('.');
                    List<String> temp_lst = new List<string>();
                    temp_lst = temp_name.ToList();
                    temp_lst.RemoveAt(0);
                    string filename = "";
                    foreach (string part in temp_lst)
                    {
                        filename += part + ".";
                    }
                    EmbeddedAssembly.Load(resource, filename);
                }
            }


            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);



            var app = new App();
            app.InitializeComponent();

            app.Run();



        }
        static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            return EmbeddedAssembly.Get(args.Name);
        }

    }
}
