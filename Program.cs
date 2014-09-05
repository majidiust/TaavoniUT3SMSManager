using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Globalization;
using TaavoniUT3SMSManager.Model;
using System.Runtime.InteropServices;

namespace TaavoniUT3SMSManager
{
    class Program
    {
        static Thread mRecievrThread;
        static TaavoniUT3SMSManager.Model.DatabaseDataContext m_model = new Model.DatabaseDataContext();
        static String signature = "تعاونی شماره 3 کارکنان دانشگاه تهران";
        static void Main(string[] args)
        {
          //  Console.WriteLine(GetUserPayment(Guid.Parse("2756e071-03c5-47cd-ad93-f51bff32b3d1")).Item2);
             mRecievrThread = new Thread(() => { ThreadHandler(); });
            mRecievrThread.Start();
            mRecievrThread.Join();
            //GetRankList();
        }

        public static void GetRankList()
        {
            try
            {
                {
                    var unsortRankList = (from p in m_model.MembersProfiles
                                   select new
                                   {
                                       FirstName = p.FirstName,
                                       LastName = p.LastName,
                                       userId = p.MemberID,
                                       NationalityCode = p.InternationalCode,
                                       Point = CalculateUserPoint((Guid)p.MemberID)
                                   }).ToList();
                    var rankList = unsortRankList.OrderByDescending(P=>P.Point);
                    List<RankModel> Result = new List<RankModel>();
                    for (int i = 0; i < rankList.Count(); i++)
                    {
                        var x = rankList.ElementAt(i);
                        Result.Add(new RankModel
                        {
                            FirstName = x.FirstName,
                            LastName = x.LastName,
                            UserId = (Guid)x.userId,
                            UserName = x.NationalityCode,
                            Point = x.Point,
                            Rank = i+1
                        });
                    }


                    foreach (var x in Result)
                        Console.WriteLine(x.UserName + " : " + x.Point + " : " + x.Rank);

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        static void ThreadHandler()
        {
            while (true)
            {
                try
                {
                    var messages = Utility.SMS.GetMessage(SMSProfile.UserName, SMSProfile.Password, SMSProfile.Number);
                    if (messages != null)
                    {
                        foreach (var msg in messages)
                        {
                            Console.WriteLine("From : " + msg.SenderNumber);
                            Console.WriteLine("Msg : " + msg.Body);
                            Console.WriteLine("Date : " + msg.ReceiveDate);
                            if (m_model.MemberContacts.Count(P => P.MobilePhone.Contains(msg.SenderNumber)) > 0)
                            {
                                if (msg.Body.Contains("1"))
                                {
                                    var users = m_model.MemberContacts.Where(P => P.MobilePhone.Contains(msg.SenderNumber));
                                    foreach (Model.MemberContact u in users)
                                    {

                                        var user = m_model.MembersProfiles.Single(P => P.MemberID.Equals(u.MemberID));
                                        var point = CalculateUserPoint((Guid)u.MemberID);
                                        var rank = GetRankForUser((Guid)u.MemberID);
                                        var result = "عضو محترم " + user.FirstName + " " + user.LastName + " " + "امتیاز شما تا به امروز " + point + "  و رتبه شما  " + rank +  " می باشد.";
                                        result += signature;
                                        TaavoniUT3SMSManager.Utility.SMS.SendSMS(msg.SenderNumber, SMSProfile.UserName, SMSProfile.Password, result, SMSProfile.Number);
                                    }
                                }
                                else if (msg.Body.Contains("2"))
                                {
                                    var users = m_model.MemberContacts.Where(P => P.MobilePhone.Contains(msg.SenderNumber));
                                    foreach (Model.MemberContact u in users)
                                    {

                                        var user = m_model.MembersProfiles.Single(P => P.MemberID.Equals(u.MemberID));
                                        var point = GetUserPayment((Guid)u.MemberID).Item2;
                                        var count = GetUserPayment((Guid)u.MemberID).Item1;
                                        var result = "عضو محترم " + user.FirstName + " " + user.LastName + " " + "پرداخت شما تا به امروز " + point + " و تعداد دفعات پرداخت " + count + " می باشد.";
                                        result += signature;
                                        TaavoniUT3SMSManager.Utility.SMS.SendSMS(msg.SenderNumber, SMSProfile.UserName, SMSProfile.Password, result, SMSProfile.Number);
                                    }
                                }
                                else if (msg.Body.Contains("3"))
                                {
                                    var Result = "تلفن 88966770 و 88966849 " + "\r\n";
                                    Result += "ایمیل: taavoniut3@ut.ac.ir  \r\n";
                                    Result += "نشانی وبلاگ: taavoniut3.blogfa.ir \r\n";
                                    Result += "پایگاه اطلاع رسانی: taavoniut3.ir \r\n";

                                    Result += signature;
                                    TaavoniUT3SMSManager.Utility.SMS.SendSMS(msg.SenderNumber, SMSProfile.UserName, SMSProfile.Password, Result, SMSProfile.Number);
                                }
                                else if (msg.Body.Contains("4"))
                                {
                                    var result = "";
                                    result += "نشانی تعاونی: بلوار کشاورز، خیابان وصال، کوچه شاهد، پلاک 8، طبقه دوم. \r\n";
                                    result += "نشانی پروژه: اتوبان تهران- کرج، اتوبان آزادگان شمال، کوهک، نسیم شانزدهم، جنب پروژه آفتاب 22 مجلس شورای اسلامی \r\n";

                                    result += signature;
                                    TaavoniUT3SMSManager.Utility.SMS.SendSMS(msg.SenderNumber, SMSProfile.UserName, SMSProfile.Password, result, SMSProfile.Number);
                                }
                                else if (msg.Body.Contains("5"))
                                {
                                    var result = "(شماره حساب 135718548 و حساب شبای 520180000000000135718548 نزد بانک تجارت شعبه اردیبهشت، کد 187";
                                    result += signature;
                                    TaavoniUT3SMSManager.Utility.SMS.SendSMS(msg.SenderNumber, SMSProfile.UserName, SMSProfile.Password, result, SMSProfile.Number);
                                }
                                else if (msg.Body.Contains("6"))
                                {
                                    var result = "مشخصات مدیرعامل تعاونی: محمود حدادیان، به شماره همر ه 09194458649" + "و ایمیل mhaddadi@ut.ac.ir";
                                    result += signature;
                                    TaavoniUT3SMSManager.Utility.SMS.SendSMS(msg.SenderNumber, SMSProfile.UserName, SMSProfile.Password, result, SMSProfile.Number);

                                }
                                else
                                {
                                    var result = "راهنمای پیامک : \r\n";
                                    result += "1.آگاهی از امتیاز و رتبه عضو\r\n";
                                    result += "2.جمع پرداخت و دفعات واریز.\r\n";
                                    result += "3.اطلاعات تماس با تعاونی\r\n";
                                    result += "4.نشانی پروژه و نشانی تعاونی \r\n";
                                    result += "5.شماره حساب تعاونی و شبای آن \r\n";
                                    result += "6. نام مدیرعامل و شماره همراه آن \r\n";
                                    result += "برای استفاده از هر منو شماره ردیف آن را به همین شماره پیامک کنید";
                                    TaavoniUT3SMSManager.Utility.SMS.SendSMS(msg.SenderNumber, SMSProfile.UserName, SMSProfile.Password, result, SMSProfile.Number);

                                }
                            }
                            else
                            {
                                var result = "شما از اعضای تعاونی نمیباشید. برای عضویت با شماره 88966770 تماس حاصل فرمایید. تعاونی شماره 3 کارکنان دانشگاه تهران";
                                Utility.SMS.SendSMS(msg.SenderNumber, SMSProfile.UserName, SMSProfile.Password, result, SMSProfile.Number);
                            }
                        }

                        System.Threading.Thread.Sleep(1000);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        public static int GetRankForUser(Guid userId)
        {
            System.Globalization.PersianCalendar jc = new System.Globalization.PersianCalendar();
            String tempdate = jc.GetYear((DateTime)DateTime.Now) + ":" + jc.GetMonth((DateTime)DateTime.Now) + ":" + jc.GetDayOfMonth((DateTime)DateTime.Now);
            var unsortRankList = (from p in m_model.MembersProfiles
                                  select new
                                  {
                                      FirstName = p.FirstName,
                                      LastName = p.LastName,
                                      userId = p.MemberID,
                                      NationalityCode = p.InternationalCode,
                                      Point = CalculateUserPoint((Guid)p.MemberID),
                                      Date = p.CreateDate != null ? p.CreateDate : tempdate,
                                      IsApproved = p.aspnet_User.aspnet_Membership.IsApproved
                                  }).ToList();
            var rankList = unsortRankList.OrderByDescending(P => P.Point);
            List<RankModel> Result = new List<RankModel>();
            for (int i = 0; i < rankList.Count(); i++)
            {
                var x = rankList.ElementAt(i);
                Result.Add(new RankModel
                {
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    UserId = (Guid)x.userId,
                    UserName = x.NationalityCode,
                    Point = x.Point,
                    Rank = i + 1,
                    IsApproved = x.IsApproved,
                    Date = x.Date
                });
            }

            return Result.Single(P => P.UserId.Equals(userId)).Rank;

        }

        private static double CalculateUserPoint(Guid userId)
        {
            TaavoniUT3SMSManager.Model.Payment p = new Model.Payment();
            try
            {
                double result = 0;

                if (m_model.Payments.Count(P => P.MemberID.Equals(userId)) <= 0)
                {
                    result = 0;
                    return result;
                }
                else
                {
                    var payments = m_model.Payments.Where(P => P.MemberID.Equals(userId));
                    PersianCalendar persian = new PersianCalendar();
                    foreach (var x in payments)
                    {
                        p = x;
                        String[] dates = x.DateOfPayment.Split(new char[] { '/' });
                        DateTime tempDateTime = new DateTime(int.Parse(dates[0]), int.Parse(dates[1]), int.Parse(dates[2]), persian);
                        DateTime tempNowDate = GetPersianDateInstance(DateTime.Now);
                        double days = (tempNowDate - tempDateTime).TotalDays;
                        double moneyWeight = double.Parse(x.Fee) / 100000.0;
                        result += days * moneyWeight;
                    }
                    return Math.Round(result / 100.00, 2); ;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(p.ID);
                return 0;
            }
        }


        private static Tuple<int, double> GetUserPayment(Guid userId)
        {
            try
            {
                Tuple<int, double> result = Tuple.Create(0, 0.0);
               
                if (m_model.Payments.Count(P => P.MemberID.Equals(userId)) <= 0)
                {
                    return result;
                }
                else
                {
                    var payments = m_model.Payments.Where(P => P.MemberID.Equals(userId));
                    double tmpResult = 0;
                    foreach (var x in payments)
                    {
                        String[] dates = x.DateOfPayment.Split(new char[] { '/' });
                        DateTime tempDateTime = new DateTime(int.Parse(dates[0]), int.Parse(dates[1]), int.Parse(dates[2]));
                        DateTime tempNowDate = GetPersianDateInstance(DateTime.Now);
                        tmpResult += double.Parse(x.Fee);
                    }

                    return new Tuple<int, double>(payments.Count(), tmpResult);
                }
            }
            catch (Exception ex)
            {
                return Tuple.Create(0, 0.0);
            }
        }

        public static String GetPersianDate(DateTime now)
        {
            System.Globalization.PersianCalendar jc = new System.Globalization.PersianCalendar();
            String tempdate = jc.GetYear(now) + ":" + jc.GetMonth(now) + ":" + jc.GetDayOfMonth(now);
            return tempdate;
        }

        public static DateTime GetPersianDateInstance(DateTime now)
        {
            System.Globalization.PersianCalendar jc = new System.Globalization.PersianCalendar();
            String tempdate = jc.GetYear(now) + ":" + jc.GetMonth(now) + ":" + jc.GetDayOfMonth(now);
            return new DateTime(jc.GetYear(now), jc.GetMonth(now), jc.GetDayOfMonth(now), jc);
        }
    }
}
