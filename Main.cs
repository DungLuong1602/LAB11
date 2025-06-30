using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Firebase.Database;
using Firebase.Database.Query;

namespace LAB11
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            HttpClient client = new HttpClient();
            string url = "https://raw.githubusercontent.com/NTH-VTC/OnlineDemoC-/main/simple_players.json";
            string json = await client.GetStringAsync(url);

            List<Player> players = JsonConvert.DeserializeObject<List<Player>>(json);



            //Bài 1: Tìm kiếm người chơi có Gold lớn hơn 1000 và Coins lớn hơn 1000. Hiển thị Tên, Gold,Coins 
            //và sắp theo Gold giảm dần và thêm vào firebase database

            var PlayersList = players.Where(p => p.Gold > 1000 && p.Coins > 100).OrderByDescending(p => p.Gold)
                .Select(p => new { p.Name, p.Gold, p.Coins })
                .ToList();

            Console.WriteLine("Danh sách nguời chơi có Gold lớn hơn 1000 và Coins lớn hơn 1000:");
            foreach (var player in PlayersList)
            {
                Console.WriteLine($"Tên: {player.Name}, Gold: {player.Gold}, Coins: {player.Coins}");
            }
            var firebase = new FirebaseClient("https://lab11-c02c0-default-rtdb.asia-southeast1.firebasedatabase.app/");
            foreach (var p in PlayersList)
            {
                await firebase
                    .Child("quiz_bai1_richPlayers")
                    .PostAsync(p);
            }
            Console.WriteLine("Dữ liệu đã được thêm vào Firebase Database.");
            Console.ReadLine();


            //Bài 2: Tính số lượng người chơi vip theo từng khu vực (region) và hiển thị kết quả theo định dạng:
            var richPlayersByRegion = players.Where(p => p.Viplevel > 0)
                .GroupBy(p => p.region)
                .Select(g => new { Region = g.Key, Count = g.Count() })
                .ToList();
            Console.WriteLine("\nSố lượng người chơi VIP theo từng khu vực:");
            foreach (var region in richPlayersByRegion)
            {
                Console.WriteLine($"Khu vực: {region.Region}, Số lượng người chơi VIP: {region.Count}");
            }
            //Tìm thêm người chơi có Viplevel > 0 và mới đăng nhập 2 ngày , hiển thị Tên, vip level , LastLogin
            DateTime now = new DateTime(2025, 06, 30, 0, 0, 0);
            var recentVipPlayers = players.Where(p => p.Viplevel > 0 && (now - p.LastLogin).TotalDays <= 2)
                .Select(p => new { p.Name, p.Viplevel, p.LastLogin })
                .ToList();
            Console.WriteLine("\nNgười chơi VIP mới đăng nhập trong 2 ngày qua:");
            foreach (var player in recentVipPlayers)
            {
                Console.WriteLine($"Tên: {player.Name}, Vip Level: {player.Viplevel}, Last Login: {player.LastLogin}");
            }

            foreach (var p in recentVipPlayers)
            {
                await firebase
                    .Child("quiz_bai2_recentVipPlayers")
                    .PostAsync(p);
            }
            Console.ReadLine();
        }



        public class Player
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public int Gold { get; set; }
            public int Coins { get; set; }

            public Boolean IsActive { get; set; }
            public int Viplevel { get; set; }
            public string region { get; set; }
            public DateTime LastLogin { get; set; }

        }
    }



    }
}
