using BertScout2022.Data;
using BertScout2022.Env;
using System.IO;
using Xamarin.Forms;

namespace BertScout2022
{
    public partial class App : Application
    {
        // app database
        private static BertScout2022Database database;

        public App()
        {
            InitializeComponent();

            MainPage = new MainPage();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }

        public static BertScout2022Database Database
        {
            get
            {
                if (database == null)
                {
                    string myDocumentsPath = DeviceEnvironment.GetMyDocumentsPath();
                    database = new BertScout2022Database(
                        Path.Combine(myDocumentsPath, BertScout2022Database.dbFilename)
                        );
                }
                return database;
            }
        }

    }
}
