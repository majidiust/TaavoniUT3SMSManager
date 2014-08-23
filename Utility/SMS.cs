using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaavoniUT3SMSManager.GhasedakSMSService;

namespace TaavoniUT3SMSManager.Utility
{
    public class SMS
    {
        public static GhasedakSMSService.v2SoapClient smsClient = new v2SoapClient();
        public static void SendSMS(String to, String username, String password, String message, String from)
        {
            try
            {
          
                ArrayOfString senders = new ArrayOfString();
                senders.Add(from);
                ArrayOfString recv = new ArrayOfString();
                recv.Add(ClearifyCellNumber(to));
                ArrayOfString body = new ArrayOfString();
                body.Add(message);
                var s = smsClient.SendSMS(username, password, senders, recv, body, null, null, null);
                foreach(var ss in s)
                Console.WriteLine("MSG:" + ss);

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error  : " + ex.Message);
                smsClient = new v2SoapClient();
            }
        }

        public static Messages[] GetMessage(String userName, String pwd, String number)
        {
            try
            {
               
                var result = smsClient.GetReceiveMessages(userName, pwd, number, 0);
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                smsClient = new v2SoapClient();
                return null;
            }
        }

        public static string ClearifyCellNumber(String number)
        {
            if (number != "")
            {
                if (number[0] == '+')
                {
                    number = number.Substring(1, number.Length);
                    return "0" + number;
                }
                else if (number.Length == "9197343303".Length)
                {
                        return "0" + number;
                }
                else return number;

            }
            else
                return null;
        }
    }
}
