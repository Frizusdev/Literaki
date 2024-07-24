using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Runtime.CompilerServices;
using znajdzslowa;
/*
//Wyciagniecie słów

Console.WriteLine("Znajdz słowa o długosci 5 liter");


string path = @"C:\Users\Szefu\Desktop\slowa.txt";
var result = new List<string>();

using (StreamReader sr = File.OpenText(path))
{
    string s = String.Empty;
    while ((s = sr.ReadLine()) != null)
    {
        if(s.Length == 5)
        {
            result.Add(s);
        }
    }
}

using (TextWriter tw = new StreamWriter(@"C:\Users\Szefu\Desktop\slowawybrane.txt"))
{
    foreach (String temp in result)
        tw.WriteLine(temp);
}

Console.WriteLine("Koniec");

*/
/*
// Update sql słowami
Console.WriteLine("Wstawianie słów do sql");



string path = @"C:\Users\Szefu\Desktop\slowawybrane.txt";
var result = new List<string>();
var entity = new List<Slowa>();

using (StreamReader sr = File.OpenText(path))
{
    string s = String.Empty;
    while ((s = sr.ReadLine()) != null)
    {
        using (ApplicationDBContext _context = new ApplicationDBContext())
        {
            _context.Slowa.Add(new Slowa { Slowo = s});
            _context.SaveChanges();
        }
    }
}*/
// path do miejsca gdzie ma utworzyć plik z wynikami
string path = @"C:\Users\Szefu\Desktop\wyniki.txt";

JsonSerializer serializer = new JsonSerializer();

var wyniki = new Wyniki();

if (!File.Exists(path))
{
        string json = JsonConvert.SerializeObject(new Wyniki { Wygranych = 0, Przegranych = 0 });
        File.WriteAllText(path, json);
}
else if (File.Exists(path))
{
    /* using (StreamReader sr = File.OpenText(path))
     {
         JsonReader jsonReader = new JsonTextReader(sr);
         Wyniki wyniki = serializer.Deserialize<Wyniki>(jsonReader);
         Console.WriteLine("Wygranych: " + wyniki.Wygranych.ToString() + " Przegranych: " + wyniki.Przegranych.ToString());
     }*/

    string file = File.ReadAllText(path);
    wyniki = JsonConvert.DeserializeObject<Wyniki>(file);
    Console.WriteLine("Wygranych: " + wyniki.Wygranych.ToString() + " Przegranych: " + wyniki.Przegranych.ToString());
}

var Slowa = new List<Slowa>();
using (ApplicationDBContext _context = new ApplicationDBContext())
{
    Slowa = _context.Slowa.ToList();
}

Console.WriteLine("Masz 5 prób");
Console.WriteLine("1- Start");
Console.WriteLine("2- Restart");
Console.WriteLine("0- Wyjdz");

var odpowiedz = Console.ReadLine();
var counter = 0;
while (odpowiedz != "0")
{
    if (Int32.Parse(odpowiedz) == 1 || Int32.Parse(odpowiedz) == 2)
    {
        gra(random());
        break;
    }
    if (odpowiedz == "0")
    {
        Console.WriteLine("Koniec");
        Environment.Exit(0);
    }
}


string random ()
{
    Random rnd = new Random();
    int IDslowa = rnd.Next(1, Slowa.Count);
    var temp = Slowa.Where(x => x.ID == IDslowa).FirstOrDefault().Slowo;
    //zakomentować by nie znać słowa
    Console.WriteLine(temp);
    return temp;
}
void gra(string temp)
{
    while (counter < 5)
    {
        odpowiedz = Console.ReadLine();
        while (!czyistnieje(odpowiedz))
        {
            Console.WriteLine("Nie istnieje");
            odpowiedz = Console.ReadLine();
        }
        if (czyistnieje(odpowiedz))
        {
            var s = sprawdz(odpowiedz, temp);
            if (s == true)
            {
                Console.WriteLine("Wygrałeś");
                break;
            }
        }
        counter++;
        Console.WriteLine(counter);
        if (counter == 5)
        {
            string json = JsonConvert.SerializeObject(new Wyniki { Wygranych = wyniki.Wygranych, Przegranych = wyniki.Przegranych+1 });
            File.WriteAllText(path, json);
        }
    }
}

bool sprawdz(string odpowiedz, string slowo)
{
    char[] odp = odpowiedz.ToCharArray(0,5);
    char[] slo = slowo.ToCharArray(0,5);
    string result = "";

    if(odpowiedz.Equals(slowo))
    {
        string json = JsonConvert.SerializeObject(new Wyniki { Wygranych = wyniki.Wygranych+1, Przegranych = wyniki.Przegranych });
        File.WriteAllText(path, json);

        return true;
    }

    foreach (char c in odp)
    {
        foreach (char d in slo)
        {
            if(d == c)
            {
                result += c.ToString();
            }
        }
    }

    string temp = "";
    string trim = "";
    for (int i = 0; i < odpowiedz.Length; i++)
    {
        if (odp[i] == slo[i])
        {
            temp += odp[i].ToString();
            trim += odp[i].ToString();
        }
        else
        {
            temp += "_ ";
        }
    }

    Console.WriteLine(result.TrimEnd(trim.ToArray()) + ":   Red");
    Console.WriteLine(temp + ":   Green");

    return false;
}

bool czyistnieje(string slowo)
{
    var x = Slowa.Find(x => x.Slowo.Equals(slowo)); 
    if(x != null) return true; else return false;
}