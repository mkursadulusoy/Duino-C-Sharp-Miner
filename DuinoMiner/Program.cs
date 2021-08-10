using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Diagnostics;
using System.Security.Cryptography;

namespace DuinoMiner
{
    // 
    public class poollar
    {
        public string name { get; set; }
        public string ip { get; set; }
        public string port { get; set; }
        public int connections { get; set; }
    }



    class Program
    {
        static void Main(string[] args)
        {
            Stopwatch stopWatch = new Stopwatch();
            Console.WriteLine("Insert your Duco username");
            string username = Console.ReadLine();
            string serverip = "server.duinocoin.com";
            int serverport = 2814;
            

            string job_message = "JOB,";
            string req_diff = ",LOW";
            string svversion;
            string svreply;
            int difficulty = 0;

            int rejected = 0;
            int accepted = 0;
            int hashrate = 0;


            var json = new WebClient().DownloadString("https://server.duinocoin.com/getPool");

            Console.WriteLine(json);

            /*
            poollar poollistesi = JsonSerializer.Deserialize<poollar>(json);
            
            serverip = poollistesi.ip;
            serverport = Convert.ToInt32(poollistesi.port);
            */
            serverip = "51.15.127.80";
            serverport = 2813;

            Socket s = new Socket(AddressFamily.InterNetwork,
            SocketType.Stream,
            ProtocolType.Tcp);


            while (true)
            {
                Console.WriteLine("Establishing Connection to {0}", serverip);
                s.Connect(serverip, serverport);
                Console.WriteLine("Connection established");


                while (s.Connected)
                {
                    byte[] b = new byte[90];
                    Console.WriteLine( s.Connected);
                    int k = s.Receive(b);
                    string szReceived = Encoding.ASCII.GetString(b, 0, k);
                    Console.Write("The answer from server:");
                    Console.WriteLine(Convert.ToString(szReceived));

                    if (szReceived.Length > 0)
                    {
                        if (Convert.ToString(szReceived[0]) == "2")
                        {
                            Console.Write("Current Server Version:");
                            Console.WriteLine(szReceived);
                            byte[] byData = System.Text.Encoding.ASCII.GetBytes("JOB," + username + ",LOW");
                            Console.WriteLine(byData);
                            s.Send(byData);


                    }
                    else if (szReceived.Substring(0, 4) == "GOOD")
                    {
                        Console.WriteLine("Job Success");
                        Console.WriteLine("Wanting new job");
                        byte[] byData = System.Text.Encoding.ASCII.GetBytes("JOB," + username + ",LOW");
                        s.Send(byData);

                    }
                    else if (szReceived.Substring(0, 3) == "BAD")
                    {
                        Console.WriteLine("Job cannot succesfully delivered");
                        Console.WriteLine("Wanting new job");
                            byte[] byData = System.Text.Encoding.ASCII.GetBytes("JOB," + username + ",LOW");
                        s.Send(byData);

                    }
                        else if (szReceived.Substring(0, 3) == "You")
                        {
                            Console.WriteLine("Another difficulty tier selected");
                            Console.WriteLine("Wanting new job");
                            byte[] byData = System.Text.Encoding.ASCII.GetBytes("JOB," + username + ",LOW");
                            s.Send(byData);

                        }
                        else
                    {
                        Console.WriteLine("new job accepted");
                        Console.WriteLine(szReceived);
                        //splitting the job
                        string[] is_parcalari = szReceived.Split(',');
                        difficulty = (int)Convert.ToInt64(is_parcalari[2]);
                        stopWatch.Start();
                        for (int result = 0; result < 100 * difficulty + 1; result++)
                        {
                            var data = Encoding.ASCII.GetBytes(is_parcalari[0] + result);
                            var hash = new SHA1Managed().ComputeHash(data);
                            var shash = string.Empty;
                            foreach (var ba in hash)
                            {
                                shash += ba.ToString("x2");
                            }


                            if (is_parcalari[1] == shash)
                            {
                                Console.WriteLine("Hash Calculated");
                                stopWatch.Stop();
                                decimal zaman = stopWatch.ElapsedMilliseconds / 1000;
                                if (zaman == 0) zaman = 0.00000000000000000001M;
                                var calchashrate = decimal.Round((result / zaman), 2, MidpointRounding.AwayFromZero);
                                Console.Write("Last Hash Value");
                                Console.WriteLine(calchashrate);
                                Console.WriteLine("The answer sending to server");
                                byte[] byData = System.Text.Encoding.ASCII.GetBytes(result + "," + calchashrate + ",C# Duino Miner by mkursadulusoy," + "C# Miner");
                                Console.WriteLine(byData);
                                s.Send(byData);

                                    break;
                                }



                            }


                        }

                }
                    
                else { Console.WriteLine("Trying to reconnect"); }


                }
                s.Close();
            }







            //Normally program should never enter this case
            Console.WriteLine("The End");



            Console.Read();





        }

    }
}
