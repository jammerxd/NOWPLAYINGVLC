using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Timers;
using PropertyChanged;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Xml;
namespace NOWPLAYINGVLC
{
    [ImplementPropertyChanged]
    public class ViewModel
    {
        IMainWindowCallbacks mainWindowCallbacks = null;
        public string windowTitle { get; set; }
        public string btnStartStopText { get; set; }
        public Boolean isRunning { get; set; }
        public BackgroundWorker getXML { get; set; }
        public Timer runXMLWorker { get; set; }
        public int updateInterval { get; set; }
        public string lblText { get; set; }
        public string fileText { get; set; }
        public string password { get; set; }
        public string url { get; set; }
        public string saveFileFullName { get; set; }
        string oldFileText { get; set; }
        public Boolean foundAlbum { get; set; }
        public Boolean foundTitle { get; set; }
        public string album { get; set; }
        public string title { get; set; }
        public ViewModel(IMainWindowCallbacks mainCallbacks)
        {
            if(mainCallbacks ==null)
            {
                throw new ArgumentException("mainwindow callbacks can't be null");
            }
            mainWindowCallbacks = mainCallbacks;
            windowTitle = "VLC Media Player Now Playing App";
            btnStartStopText = "Start";
            isRunning = false;
            updateInterval = 2;
            fileText = "Not Active or Started";
            lblText = fileText;
            password = "test";
            url = "http://localhost:8080/requests/status.xml";
            saveFileFullName = Directory.GetCurrentDirectory() + "\\song.txt";
            album = "Unknown";
            title = "Unknown";

            getXML = new BackgroundWorker();
            getXML.DoWork += GetXML_DoWork;
            getXML.RunWorkerCompleted += GetXML_RunWorkerCompleted;

            runXMLWorker = new Timer(updateInterval * 1000);
            runXMLWorker.Elapsed += RunXMLWorker_Elapsed;
        }

        private void RunXMLWorker_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (!getXML.IsBusy)
            {
                getXML.RunWorkerAsync();
            }
            else
            {
                isRunning = false;
                runXMLWorker.Enabled = false;
                btnStartStopText = "Start";
                mainWindowCallbacks.ShowMessage("TOO MANY BACKGROUND WORKERS", "There already is a background worker downloading the information locally.", false);
                
            }
        }

        private void GetXML_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            lblText = "File Contents: " + fileText;
        }

        private void GetXML_DoWork(object sender, DoWorkEventArgs e)
        {
            try {
                XmlDocument doc = new XmlDocument();
                WebClient wc = new WebClient();
                string creds = Convert.ToBase64String(Encoding.ASCII.GetBytes(":" + password));
                wc.Headers[HttpRequestHeader.Authorization] = "Basic " + creds;
                doc.LoadXml(wc.DownloadString(url));
                wc.Dispose();
                XmlNode rootNode = doc.SelectSingleNode("root");
                XmlNode infoNode = rootNode.SelectSingleNode("information");
                XmlNode categoryNode = infoNode.SelectNodes("category")[0];

                foundAlbum = false;
                foundTitle = false;

                foreach(XmlNode temp in categoryNode.SelectNodes("info"))
                {
                    if(temp.Attributes["name"].Value == "album")
                    {
                        album = temp.InnerText;
                        foundAlbum = true;
                    }
                    if(temp.Attributes["name"].Value == "title")
                    {
                        title = temp.InnerText;
                        foundTitle = true;
                    }
                }
                
                if(!foundAlbum)
                {
                    album = "Unknown";
                }
                if(!foundTitle)
                {
                    title = "Unknown";
                }


                fileText = album + " - " + title;

                if (oldFileText != fileText)
                {
                    string spaces = "";
                    for (int i = fileText.Length; i< 83; i++)
                        spaces += " ";
                    

                    File.WriteAllText(saveFileFullName, fileText + spaces);
                }
                oldFileText = fileText;
                doc = null;
                rootNode = null;
                categoryNode = null;
                
            }
            catch(Exception ex)
            {
                mainWindowCallbacks.ShowMessage("ERROR IN GETTING INFORMATION", "The following error has ocurred: " + ex.Message.ToString(),false);
                isRunning = false;
                runXMLWorker.Enabled = false;
                btnStartStopText = "Start";
            }
            
            

        }

        private void StartStop(object currentobj)
        {
            isRunning = !runXMLWorker.Enabled;
            runXMLWorker = new Timer(updateInterval * 1000);
            runXMLWorker.Elapsed += RunXMLWorker_Elapsed;
            runXMLWorker.Enabled = isRunning;
            if (isRunning)
            {
                btnStartStopText = "Stop";
            }
            else
            {
                btnStartStopText = "Start";
                lblText = "Timer Active - Current File Contents: " + fileText;
            }
        }



        private RelayCommand _StartStopCommand;
        public RelayCommand StartStopCommand
        {
            get { 
                if(_StartStopCommand == null)
                {
                    _StartStopCommand = new RelayCommand(StartStop);
                }
                return _StartStopCommand;
            }
        }
    }
}
