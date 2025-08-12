using Microsoft.Data.SqlClient;
using Nancy.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using ECampusDiscussion.Utility_Function;

namespace ECampusDiscussion.JwtAuth
{
    public class CheckPermission
    {
        private readonly string connString = Common_Func.ConnectionString();

        public bool Role_Granted(int login_id, string? pages, string? permissions)
        {
            SqlConnection con = null;
            try
            {
                con = new SqlConnection(connString);
                var query = @"
                 SELECT p.Permissions
                 FROM User_Roles r
                 JOIN Permission_Table p ON r.Role_ID = p.Roles_ID
                 WHERE r.Login_ID =" + login_id;
                SqlCommand cm = new SqlCommand(query, con);
                con.Open();
                SqlDataReader sdr = cm.ExecuteReader();
                JObject roles = new JObject();
                while (sdr.Read())
                {
                    JObject role = JObject.Parse(sdr.GetString(0));
                    roles.Merge(role, new JsonMergeSettings
                    {
                        // union array values together to avoid duplicates
                        MergeArrayHandling = MergeArrayHandling.Union
                    });

                }
                string json = roles.ToString(Formatting.None);
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                dynamic item = serializer.Deserialize<object>(json);
                if (pages != null && permissions != null)
                {
                    return (item[pages][permissions]);
                }
                else
                {
                    return false;
                }
                // return (item[pages][permissions]);
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public object Permission_Granted(int login_id)
        {
            SqlConnection con = null;
            try
            {
                con = new SqlConnection(connString);
                var query = @"
           SELECT p.Permissions
           FROM User_Roles r
           JOIN Permission_Table p ON r.Role_ID = p.Roles_ID
           WHERE r.Login_ID = " + login_id;

                SqlCommand cm = new SqlCommand(query, con);
                con.Open();
                SqlDataReader sdr = cm.ExecuteReader();
                JObject roles = new JObject();
                while (sdr.Read())
                {
                    JObject role = JObject.Parse(sdr.GetString(0));
                    roles.Merge(role, new JsonMergeSettings
                    {
                        MergeArrayHandling = MergeArrayHandling.Union
                    });
                }
                string json = roles.ToString(Formatting.None);
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                dynamic item = serializer.Deserialize<object>(json);
                return item;

            }
            catch (Exception e)
            {
                return null;
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                }
            }
        }
    }
}


