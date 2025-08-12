namespace ECampusDiscussion.Utility_Function
{
    public static class Common_Func
    {
        private readonly static string Env = "STAGE"; // STAGE, LIVE
        public static string ConnectionString()
        {
            if (Env == "STAGE")
                return "Data Source= 103.38.50.46,61450;Initial Catalog=EcampusDiss_Stage;User ID=TekInsDBAdmin;TrustServerCertificate=True;Password=Demo123!@#;Command Timeout=300; pooling=true;Max Pool Size=2000;";
            else if (Env == "LIVE")
                return "Data Source= 103.38.50.46,61450;Initial Catalog=Hrlense_Live;User ID=TekInsDBAdmin;TrustServerCertificate=True;Password=Demo123!@#;Command Timeout=300; pooling=true;Max Pool Size=2000;";
            else
                return "Data Source=.\\MSSQLLocalDB;Initial Catalog=AjaxSamples;Integrated Security=True;TrustServerCertificate=True;Encrypt=False;Server=DESKTOP-M0BKMAC;Database=QATesting";
        }

        public static string JobvrittaUrl()
        {
            if (Env == "STAGE")
                return "https://portal.jobvritta.com/api";
            else if (Env == "LIVE")
                return "https://auditapi.jobvritta.com/api";
            else
                return "https://portal.jobvritta.com/api";
        }
    }
}
