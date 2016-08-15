using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Security.Cryptography;
using System.Configuration;
using System.Data.SqlClient;
//using Senparc.Weixin.MP.Helpers;
//using Senparc.Weixin.MP.CommonAPIs;

public partial class weixin_yuxi_zhuangyuan : System.Web.UI.Page
{
    public string nonceStr = "";
    public string timestamp = "";
    public string signature = "";
    public string ticket = "";
    public string url = "";
    public string imgurl = "";
    private string appId = "wx049bc0184f6796d2";
    private string secret = "5f4fb92a1fd5b80759a62ef84d8d6502";
    Database database = new Database();
    protected void Page_Load(object sender, EventArgs e)
    {
        read_data();
        //Label1.Text = nonceStr + " " + timestamp + " " + signature;
    }
    /// <summary>
    /// use sha1 to encrypt string
    /// </summary>
    public string SHA1_Encrypt(string Source_String)
    {
        byte[] StrRes = Encoding.Default.GetBytes(Source_String);
        HashAlgorithm iSHA = new SHA1CryptoServiceProvider();
        StrRes = iSHA.ComputeHash(StrRes);
        StringBuilder EnText = new StringBuilder();
        foreach (byte iByte in StrRes)
        {
            EnText.AppendFormat("{0:x2}", iByte);
        }
        return EnText.ToString();
    }
    public string HttpGet(string Url, string postDataStr)
    {
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url + (postDataStr == "" ? "" : "?") + postDataStr);
        request.Method = "GET";
        request.ContentType = "text/html;charset=UTF-8";

        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        Stream myResponseStream = response.GetResponseStream();
        StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
        string retString = myStreamReader.ReadToEnd();
        myStreamReader.Close();
        myResponseStream.Close();

        return retString;
    }

    public void get_data()
    {
        string token = HttpGet("https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid=wx049bc0184f6796d2&secret=5f4fb92a1fd5b80759a62ef84d8d6502", "");

        token = token.Replace("{\"access_token\":\"", "");
        token = token.Replace("\",\"expires_in\":7200}", "");

        string ticket = HttpGet("https://api.weixin.qq.com/cgi-bin/ticket/getticket?access_token=" + token + "&type=jsapi", "");

        ticket = ticket.Substring(ticket.IndexOf("ticket") + 9);
        ticket = ticket.Replace("\",\"expires_in\":7200}", "");

        Guid guid = Guid.NewGuid();
        nonceStr = guid.ToString();
        timestamp = ((long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds / 1000).ToString();
        string url = Request.Url.ToString();
        string str = "jsapi_ticket=" + ticket +
                "&noncestr=" + nonceStr +
                "&timestamp=" + timestamp +
                "&url=" + url;
        //sha1加密
        signature = SHA1_Encrypt(str);
    }

    public void get_data2()
    {
        //timestamp = JSSDKHelper.GetTimestamp();
        //nonceStr = JSSDKHelper.GetNoncestr();
        //JSSDKHelper jssdkhelper = new JSSDKHelper();
        //Senparc.Weixin.Exceptions.UnRegisterAppIdException”类型的异常在 Senparc.Weixin.MP.dll 中发生，但未在用户代码中进行处理

        //其他信息: 此appId尚未注册，请先使用JsApiTicketContainer.Register完成注册（全局执行一次即可）！
        // JsApiTicketContainer.Register(appId, secret);

        // ticket = JsApiTicketContainer.GetJsApiTicket(appId, true);

        //Senparc.Weixin.Exceptions.ErrorJsonResultException”类型的异常在 Senparc.Weixin.dll 中发生，但未在用户代码中进行处理

        //其他信息: 微信请求发生错误！错误代码：40125，说明：invalid appsecret, view more at http://t.cn/RAEkdVq hint: [bBHuLa0495re59]

        // string url = Request.Url.AbsoluteUri.ToString();

        // signature = JSSDKHelper.GetSignature(ticket, nonceStr, timestamp, Request.Url.AbsoluteUri.ToString());
    }

    public void read_data()
    {
        url = Request.Url.AbsoluteUri.ToString();
        imgurl = url.Substring(0, url.LastIndexOf("/") + 1) + "main.png";

        string strSql = "select * from share_weixin where url='" + url + "' order by id desc";
        if (database.IsHave(strSql))
        {
            strSql = "select top 1 * from share_weixin where url='" + url + "' order by id desc";
            string settings = Convert.ToString(ConfigurationManager.ConnectionStrings["sqlservices"]);
            SqlConnection myconn = new SqlConnection(settings);
            myconn.Open();
            SqlCommand cmd = new SqlCommand(strSql, myconn);

            SqlDataReader myReader = cmd.ExecuteReader();

            while (myReader.Read())
            {
                string time = myReader["time"].ToString();
                DateTime lasttime = DateTime.Parse(time);
                TimeSpan res = DateTime.Now - lasttime;
                if (res.TotalHours > 1.9)
                {
                    get_data();
                    strSql = "insert into share_weixin values('" + ticket + "','" + nonceStr + "','" + timestamp + "','" + signature + "','" + url + "','" + DateTime.Now + "')";
                    database.carryout(strSql);
                }
                else
                {
                    nonceStr = myReader["nonceStr"].ToString();
                    timestamp = myReader["timestamp"].ToString();
                    signature = myReader["signature"].ToString();
                }
            }
            myconn.Close();
        }
        else
        {
            get_data();
            strSql = "insert into share_weixin values('" + ticket + "','" + nonceStr + "','" + timestamp + "','" + signature + "','" + url + "','" + DateTime.Now + "')";
            database.carryout(strSql);
        }
    }
}