using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace FormatXmlFiles
{
    class Program
    {
        static void Main(string[] args)
        {
            //var testfile = @"C:\Users\pc-yuminaka\Desktop\law.xml";
            //var testpath = $"{ Path.GetDirectoryName(testfile)}\\orig_{Path.GetFileName(testfile)}";
            //File.Copy(testfile, testpath, true);
            //FormatData(testfile);
            //var testfolder = @"C:\Users\pc-yuminaka\Desktop\NTT個別対応";
            //CopyDirectory(testfolder, @"C:\Users\pc-yuminaka\Desktop\orig_NTT個別対応");
            //foreach (var file in Directory.EnumerateFiles(testfolder, "*.xml", SearchOption.AllDirectories))
            //{
            //    FormatData(file);
            //}

            var target = System.Environment.GetCommandLineArgs();
            var filenames = new List<string>();
            var folders = new List<string>();

            if (target.Length > 1)
            {
                var targets = target.Skip(1).ToList();

                filenames = targets.Where(x => !File.GetAttributes(x).HasFlag(FileAttributes.Directory)).ToList();
                folders = targets.Where(x => File.GetAttributes(x).HasFlag(FileAttributes.Directory)).ToList();
            }
            else
            {
                Console.WriteLine("XMLファイルの整形を行います。\r\n使用方法\r\n・xmlファイル(※複数可)\r\n・xmlが含まれたフォルダ(※複数可)\r\n" +
                    "上記をまとめてexeにドラックアンドドロップしてください。\r\n空白タグを整形します。");
                Console.ReadKey();
                return;
            }
            Console.WriteLine("対象ファイル一覧：");
            filenames.ForEach(x => Console.WriteLine(x));

            Console.WriteLine("\r\n対象フォルダ一覧：");
            folders.ForEach(x => Console.WriteLine(x));


            var cnt = 0;
            try
            {
                foreach (var file in filenames)
                {
                    var splited = file.Split('.');
                    var path = splited[0] + "_resaved." + splited[1];
                    File.Copy(file, path, true);
                    FormatData(path);
                    cnt++;
                }
                foreach (var folder in folders)
                {
                    var newfolder = $"{folder}_resaved";
                    CopyDirectory(folder, newfolder);
                    foreach (var file in Directory.EnumerateFiles(newfolder, "*.xml", SearchOption.AllDirectories))
                    {
                        FormatData(file);
                        cnt++;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            Console.WriteLine($"\r\n\r\n{cnt}件のファイルを複製、整形しました。\r\nPressAnyKeyToFinish...");
            Console.ReadKey();
        }

        static void FormatData(string sourceFileName)
        {
            var doc = new XmlDocument();
            doc.Load(sourceFileName);
            var result = (XmlDocument)doc.CloneNode(true);
            result.Save(sourceFileName);
        }
        public static void CopyDirectory(string sourceDirName, string destDirName)
        {
            //コピー先のディレクトリがないときは作る
            if (!System.IO.Directory.Exists(destDirName))
            {
                System.IO.Directory.CreateDirectory(destDirName);
                //属性もコピー
                System.IO.File.SetAttributes(destDirName,
                    System.IO.File.GetAttributes(sourceDirName));
            }

            //コピー先のディレクトリ名の末尾に"\"をつける
            if (destDirName[destDirName.Length - 1] !=
                    System.IO.Path.DirectorySeparatorChar)
                destDirName = destDirName + System.IO.Path.DirectorySeparatorChar;

            //コピー元のディレクトリにあるファイルをコピー
            string[] files = System.IO.Directory.GetFiles(sourceDirName);
            foreach (string file in files)
                System.IO.File.Copy(file,
                    destDirName + System.IO.Path.GetFileName(file), true);

            //コピー元のディレクトリにあるディレクトリについて、再帰的に呼び出す
            string[] dirs = System.IO.Directory.GetDirectories(sourceDirName);
            foreach (string dir in dirs)
                CopyDirectory(dir, destDirName + System.IO.Path.GetFileName(dir));
        }
    }
}
