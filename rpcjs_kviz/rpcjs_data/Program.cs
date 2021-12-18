using System;
using System.IO;
using System.Text;

namespace rpcjs_data
{
    class Program
    {
        static void Main()
        {
            //Kérdések folyammá
            byte tmp;
            uint count = 1;
            BinaryReader br = new BinaryReader(new FileStream("nyers.dat", FileMode.Open, FileAccess.Read), Encoding.Default);
            BinaryWriter bw = new BinaryWriter(new FileStream("kerdesek.dat", FileMode.Create, FileAccess.Write), Encoding.Default);
            while (br.PeekChar() > -1)
                if (br.ReadByte() == 0x0A)
                    count++;
            br.BaseStream.Position = 0;
            bw.Write((byte)count);
            count *= sizeof(uint);
            bw.Write(++count);
            while (br.PeekChar() > -1)
                if (br.ReadByte() == 0x0A)
                    bw.Write((uint)(br.BaseStream.Position + count));
            br.BaseStream.Position = 0;
            while (br.PeekChar() > -1)
            {
                tmp = br.ReadByte();
                if (tmp == 0x0A)
                    bw.Write((byte)0x00);
                else
                    bw.Write(tmp);
            }
            bw.Close();
            br.Close();
            //Képek folyammá
            string[] files = Directory.GetFiles("pics", "*.png");
            FileInfo finfo;
            uint flen;
            bw = new BinaryWriter(new FileStream("kepek.dat", FileMode.Create, FileAccess.Write), Encoding.Default);
            bw.Write((byte)files.Length);
            flen = (uint)files.Length * sizeof(uint);
            for (int i = 0; i < files.Length; i++)
            {
                finfo = new FileInfo(files[i]);
                bw.Write(flen + 1);
                flen += (uint)finfo.Length;
            }
            for (int i = 0; i < files.Length; i++)
                bw.Write(File.ReadAllBytes(files[i]));
        }
    }
}