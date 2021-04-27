using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace osumusic
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Paste the address of your osu songs folder (by default in %appdata% local):");
            string addressSongs = Console.ReadLine();
            if (!Directory.Exists(addressSongs))
            {
                Console.WriteLine("That address doesn't exist on this system.");
                return;
            }
            Console.WriteLine("Paste the folder where you want to put the songs:");
            string addressResult = Console.ReadLine();
            if (!Directory.Exists(addressResult))
            {
                Console.WriteLine("That address doesn't exist on this system.");
                return;
            }
            IEnumerable<string> songsList = Directory.EnumerateDirectories(addressSongs);
            foreach (var song in songsList)
            {
                var mp3path = Directory.GetFiles(song, "*.mp3");
                var filename = Path.GetFileName(song);
                File.Copy(mp3path[0], addressResult + "/" + filename + ".mp3");
            }
            Console.WriteLine("Files successfully exported, enjoy! :D");
            Console.WriteLine("Press enter to close...");
            Console.ReadLine();
        }
    }
}