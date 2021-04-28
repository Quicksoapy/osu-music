using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Xml;
using TagLib.Flac;
using TagLib.Mpeg;
using File = System.IO.File;

namespace osumusic
{
    class Program
    {
        static void Main(string[] args)
        {
            string addressSongs;
            string addressResult;
            string artworkBool;
            string unicodeBool;
            
            while (true)
            {
                Console.WriteLine("Paste the address of your osu songs folder (by default in %appdata% local):");
                addressSongs = Console.ReadLine();
                if (!Directory.Exists(addressSongs))
                {
                    Console.WriteLine("That address doesn't exist on this system. Try again.");
                    return;
                }
                break;
            }

            while (true)
            {
                Console.WriteLine("Paste the folder where you want to put the songs:");
                addressResult = Console.ReadLine();
                if (!Directory.Exists(addressResult))
                {
                    Console.WriteLine("That address doesn't exist on this system. Try again.");
                    return;
                }
                break;
            }

            while (true)
            {
                Console.WriteLine("Enter 0 for no artwork, press 1 if you want the artwork:");
                artworkBool = Console.ReadLine()?.Trim();
                if (artworkBool != "0" && artworkBool != "1")
                {
                    Console.WriteLine("Please type 0 or 1.");
                    return;
                }
                break;
            }
            while (true)
            {
                Console.WriteLine("Do you want the title and artist in original text, enter 0. Do you want it in romaji/english text, enter 1.");
                unicodeBool = Console.ReadLine()?.Trim();
                if (unicodeBool != "0" && unicodeBool != "1")
                {
                    Console.WriteLine("Please type 0 or 1.");
                    return;
                }
                break;
            }
            
            IEnumerable<string> songsList = Directory.EnumerateDirectories(addressSongs);

            foreach (var song in songsList)
            {
                var mp3path = Directory.GetFiles(song, "*.mp3");
                var filename = Path.GetFileName(song);
                if (!File.Exists(addressResult + "/" + filename + ".mp3"))
                {
                    File.Copy(mp3path[0], addressResult + "/" + filename + ".mp3");
                    var mp3File = new AudioFile(addressResult + "/" + filename + ".mp3");
                    var metaDataFiles = Directory.GetFiles(song, "*.osu");
                    
                    if (metaDataFiles.Length != 0)
                    {
                        var metaDataFile = metaDataFiles[0];
                        var metaDataArray = File.ReadAllLines(metaDataFile);
                        if (unicodeBool == "0")
                        {
                            string[] performers = new string[]{MetaDataSeeker(metaDataArray, "ArtistUnicode")};
                            mp3File.Tag.Title = MetaDataSeeker(metaDataArray, "TitleUnicode");
                            mp3File.Tag.Performers = performers;
                        }
                        else if (unicodeBool == "1")
                        {
                            string[] performers = new string[]{MetaDataSeeker(metaDataArray, "Artist:")};
                            mp3File.Tag.Title = MetaDataSeeker(metaDataArray, "Title:");
                            mp3File.Tag.Performers = performers;
                        }
                    }
                    if (artworkBool == "0")
                    {
                        if (mp3File.Tag.Pictures.Length > 0)
                        {
                            var pictures = new Picture[0];
                            mp3File.Tag.Pictures = pictures;
                            mp3File.Save();
                        }
                    }
                    else if (artworkBool == "1")
                    {
                        TagLib.Picture pic = null; 
                        var imagePath = Array.Empty<string>();
                        try
                        {
                            imagePath = Directory.GetFiles(song, "*.jpg");
                            if (imagePath.Length == 0)
                            {
                                imagePath = Directory.GetFiles(song, "*.png");
                            }
                        }
                        catch (Exception)
                        {
                            Console.WriteLine("Finding image went wrong");
                        }
                        try
                        {
                            pic = new TagLib.Picture(imagePath[0]);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Applying image went wrong");
                            Console.WriteLine(e);
                        }

                        if (pic != null)
                        {
                            pic.Type = TagLib.PictureType.FrontCover;
                            pic.Description = "Cover";
                            mp3File.Tag.Pictures = new TagLib.IPicture[] { pic };
                            mp3File.Save();
                        }
                    }
                    else
                    {
                        Console.WriteLine("Please enter 0 for no artwork, or 1 with artwork.");
                    }
                }
                else
                {
                    Console.WriteLine("File not found");
                }
            }
            
            Console.WriteLine("Files successfully exported, enjoy! :D");
            Console.WriteLine("Press enter to close...");
            Console.ReadLine();
        }
        public static string MetaDataSeeker(string[] fileText, string thing)
        {
            string[] output = null;
            foreach (var line in fileText)
            {
                if (line.Contains(thing))
                {
                    string lineThing = line;
                    output = lineThing.Split(":");
                }
            }
            return output[1];
        }
    }
}