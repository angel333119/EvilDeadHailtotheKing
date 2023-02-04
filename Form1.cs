using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace EvilDeadHailtotheKing
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "Arquivo Evil Dead Hail to the King PS1|*.P1|Todos os arquivos (*.*)|*.*";
            openFileDialog1.Title = "Escolha um arquivo do jogo Evil Dead Hail to the King de PlaySation 1...";
            openFileDialog1.Multiselect = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                foreach (String file in openFileDialog1.FileNames)
                {
                    using (FileStream stream = File.Open(file, FileMode.Open))
                    {
                        BinaryReader br = new BinaryReader(stream);
                        BinaryWriter bw = new BinaryWriter(stream);

                        int magic = br.ReadInt32();

                        br.BaseStream.Seek(0x07A0, SeekOrigin.Begin);

                        int quantidadetexto = 0x127;

                        int[] tamanhotexto = new int[quantidadetexto];
                        int[] ponteiro = new int[quantidadetexto];

                        for (int i = 0; i < quantidadetexto - 1; i++)
                        {
                            ponteiro[i] = br.ReadInt32();
                            tamanhotexto[i] = br.ReadInt32();
                            long unknow = br.ReadInt64();
                        }

                        br.BaseStream.Seek(ponteiro[0], SeekOrigin.Begin);

                        string todosOsTextos = "";

                        for (int i = 0; i < quantidadetexto - 1; i++)
                        {
                            byte[] texto = new byte[tamanhotexto[i]];

                            for (int j = 0; j < tamanhotexto[i]; j++)
                            {
                                texto[j] = br.ReadByte();
                            }

                            string ascii = System.Text.Encoding.ASCII.GetString(texto);

                            ascii = ascii.Replace("\0", string.Empty);

                            todosOsTextos += ascii + "\r\n";
                        }

                        File.WriteAllText(Path.Combine(Path.GetDirectoryName(file), Path.GetFileName(file)) + ".txt", todosOsTextos);
                    }
                }
                MessageBox.Show("Texto extraido com sucesso!", "AVISO!");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "Arquivo Evil Dead Hail to the King PS1|*.P1|Todos os arquivos (*.*)|*.*";
            openFileDialog1.Title = "Escolha um arquivo do jogo Evil Dead Hail to the King de PlaySation 1...";
            openFileDialog1.Multiselect = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                foreach (String file in openFileDialog1.FileNames)
                {
                    FileInfo dump = new FileInfo(Path.Combine(Path.GetDirectoryName(file), Path.GetFileName(file)) + ".txt");

                    if (dump.Exists)
                    {
                        using (FileStream stream = File.Open(file, FileMode.Open))
                        {
                            BinaryReader br = new BinaryReader(stream);
                            BinaryWriter bw = new BinaryWriter(stream);

                            var texto = File.ReadLines(Path.Combine(Path.GetDirectoryName(file), Path.GetFileName(file)) + ".txt");

                            uint offsetatual = 0x07A0;
                            stream.SetLength(0x0DF2A4);
                            int novoponteiro = 0x0DF2A4;
                            int numeroLinha = 0;
                            int tamanhotexto = 0;

                            foreach (var linha in texto)
                            {
                                stream.Seek(offsetatual + numeroLinha * 16, SeekOrigin.Begin);

                                bw.Write(novoponteiro);

                                byte[] bytes = Encoding.ASCII.GetBytes(linha);

                                tamanhotexto = bytes.Length + 1;

                                long offsetDados = novoponteiro;
                                
                                bw.Write(tamanhotexto);

                                novoponteiro += bytes.Length + 1;

                                stream.Seek(offsetDados, SeekOrigin.Begin);

                                bw.Write(bytes);

                                stream.Seek(novoponteiro - bytes.Length, SeekOrigin.Current);

                                numeroLinha++;
                            }

                            MessageBox.Show("Texto inserido com sucesso!", "AVISO!");





                        }
                    }
                }
            }
        }
    }
}