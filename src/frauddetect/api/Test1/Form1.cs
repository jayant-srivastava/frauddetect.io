using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Test1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string fileName = @"C:\SplunkRecords\test.csv";

            using (Stream fd = File.Create(fileName))
            using (Stream fs = File.OpenRead(@"C:\SplunkRecordsZipped\tmp_0.csv.gz"))
            using (Stream csStream = new GZipStream(fs, CompressionMode.Decompress))
            {
                byte[] buffer = new byte[10240];
                int nRead;
                while ((nRead = csStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    fd.Write(buffer, 0, nRead);
                }
            }

            var reader = new StreamReader(File.OpenRead(fileName));
            List<string> listA = new List<string>();
            List<string> listB = new List<string>();
            while (!reader.EndOfStream)
            {
                var line1 = reader.ReadLine();
                var values1 = line1.Split(',');

                listA.AddRange(values1);

                var line2 = reader.ReadLine();
                var values2 = line2.Split(',');

                listB.AddRange(values2);

            }

            string searchTerm = listB[7];

            string userAccountNumber = string.Empty;
            List<string> contents = new List<string>(searchTerm.Split(' '));
            userAccountNumber = contents.Find(s => (s.Length == 16) && (s.StartsWith("1111")));

        }
    }
}
