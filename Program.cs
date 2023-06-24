using System;
using System.Collections.Generic;
using System.IO;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

class Program
{
    static void Main()
    {
        string inputFilePath = "makineler.txt"; // Giriş dosyası yolu
        string outputFilePath = "sonuc.txt"; // Çıkış dosyası yolu

        List<List<string>> machineNamesList = ReadMachineNames(inputFilePath);

        using (StreamWriter writer = new StreamWriter(outputFilePath))
        {
            Parallel.ForEach(machineNamesList, machineNames =>
            {
                foreach (string machineName in machineNames)
                {
                    PingReply reply = PingHost(machineName);
                    if (reply != null)
                    {
                        if (reply.Status == IPStatus.Success)
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine($"{machineName}  :Bağlantı var.");
                            writer.WriteLine($"{machineName}");
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine($"{machineName}   : Bağlantı başarısız.");

                        }
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"{machineName}   : Böyle bir hostname bulunmuyor");

                    }
                }
            });
        }

        Console.ResetColor();

        Console.WriteLine("Ping sonuçları çıkış dosyasına yazıldı.");
        Console.ReadLine();
    }

    static List<List<string>> ReadMachineNames(string filePath)
    {
        List<List<string>> machineNamesList = new List<List<string>>();

        try
        {
            string[] lines = File.ReadAllLines(filePath);
            foreach (string line in lines)
            {
                string[] names = line.Split(',');
                List<string> machineNames = new List<string>();
                foreach (string name in names)
                {
                    string machineName = name.Trim();
                    machineNames.Add(machineName);
                }
                machineNamesList.Add(machineNames);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Dosya okunurken bir hata oluştu: {ex.Message}");
        }

        return machineNamesList;
    }

    static PingReply PingHost(string machineName)
    {
        using (Ping ping = new Ping())
        {
            try
            {
                return ping.Send(machineName);
            }
            catch (PingException)
            {
                return null;
            }
        }
    }
}
