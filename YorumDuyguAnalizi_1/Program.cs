using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using Microsoft.ML.Tokenizers;
using Spire.Xls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

class DuyguAnaliziModel
{
    private InferenceSession session;
    private BertTokenizer tokenizer;
    private const int MaxLen = 32;

    public DuyguAnaliziModel(string modelPath, string vocabPath)
    {
        session = new InferenceSession(modelPath);
        tokenizer = BertTokenizer.Create(vocabPath);
    }

    public long[] Tokenize(string text)
    {
        string normalized;
        int chars;
        var ids = tokenizer.EncodeToIds(text, MaxLen, addSpecialTokens: true, out normalized, out chars);

        var padded = ids.Select(id => (long)id).ToList();
        while (padded.Count < MaxLen) padded.Add(0L);
        return padded.Take(MaxLen).ToArray();
    }

    public long[] GetAttentionMask(long[] inputIds) =>
        inputIds.Select(id => id == 0 ? 0L : 1L).ToArray();

    public long[] GetTokenTypeIds(int length) =>
        Enumerable.Repeat(0L, length).ToArray();

    public float[] Predict(string text)
    {
        var inputIds = Tokenize(text);
        var attentionMask = GetAttentionMask(inputIds);
        var tokenTypeIds = GetTokenTypeIds(inputIds.Length);

        var inputs = new List<NamedOnnxValue>
        {
            NamedOnnxValue.CreateFromTensor("input_ids", new DenseTensor<long>(inputIds, new[]{1,inputIds.Length})),
            NamedOnnxValue.CreateFromTensor("attention_mask", new DenseTensor<long>(attentionMask, new[]{1,attentionMask.Length})),
            NamedOnnxValue.CreateFromTensor("token_type_ids", new DenseTensor<long>(tokenTypeIds, new[]{1,tokenTypeIds.Length}))
        };

        using var res = session.Run(inputs);
        return res.First().AsEnumerable<float>().ToArray();
    }

    public static float[] Softmax(float[] logits)
    {
        var max = logits.Max();
        var exps = logits.Select(l => Math.Exp(l - max)).ToArray();
        var sum = exps.Sum();
        return exps.Select(e => (float)(e / sum)).ToArray();
    }
}

class Program
{

    public static List<string> CumleParcala(string metin)
    {
        string[] baglaclar = { "ama", "fakat", "ancak", "çünkü", "lakin", "ve" };


        var parcalar = metin
            .Replace(".", ".|")
            .Replace("!", "!|")
            .Replace("?", "?|")
            .Replace(",", ",|")
            .Replace(";", ";|")
            .Replace(":", ":|")
            .Split('|', StringSplitOptions.RemoveEmptyEntries)
            .Select(p => p.Trim())
            .ToList();

        var sonParcalar = new List<string>();
        foreach (var parca in parcalar)
        {

            string temp = parca;

            foreach (var baglac in baglaclar)
            {

                var splitParts = temp.Split(new string[] { $" {baglac} " }, StringSplitOptions.RemoveEmptyEntries);
                temp = string.Join("|", splitParts);
            }

            var subParts = temp.Split('|', StringSplitOptions.RemoveEmptyEntries)
                               .Select(s => s.Trim())
                               .ToList();

            sonParcalar.AddRange(subParts);
        }


        return sonParcalar.Where(p => p.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length >= 2).ToList();
    }

    static void Main()
    {
        string modelPath = Path.Combine(AppContext.BaseDirectory, "model.onnx");
        string vocabPath = Path.Combine(AppContext.BaseDirectory, "tokenizer", "vocab.txt");

        string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        string excelPath = Path.Combine(desktopPath, "DuyguAnalizi_Spire.xls");
        string logPath = Path.Combine(desktopPath, "Olumsuz.log");

        var analiz = new DuyguAnaliziModel(modelPath, vocabPath);
        var yorumlar = new List<string>
        {
          "Ayakkabının şekli güzel ama tabanı yırtık geldi"
        };

        Spire.Xls.Workbook workbook;
        Spire.Xls.Worksheet sheet;
        if (File.Exists(excelPath))
        {
            workbook = new Spire.Xls.Workbook();
            workbook.LoadFromFile(excelPath);
            sheet = workbook.Worksheets[0];
        }
        else
        {
            workbook = new Spire.Xls.Workbook();
            sheet = workbook.Worksheets[0];
            sheet.Name = "Analiz";
            sheet.Range["A1"].Text = "Tarih";
            sheet.Range["B1"].Text = "Pozitif Parça";
            sheet.Range["C1"].Text = "Pozitiflik Oranı";
        }

        int currentRow = sheet.LastRow + 1;

        foreach (var yorum in yorumlar)
        {
            Console.WriteLine($"\n--- Yeni Yorum ---");
            var parcaliYorumlar = CumleParcala(yorum);

            int altNo = 1;

            foreach (var parca in parcaliYorumlar)
            {
                var logits = analiz.Predict(parca);
                var probs = DuyguAnaliziModel.Softmax(logits);
                bool pozitif = probs[1] >= 0.5f;
                float pozitiflikOrani = probs[1];

                if (pozitif)
                {

                    sheet.Range[$"A{currentRow}"].Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    sheet.Range[$"B{currentRow}"].Text = parca;
                    sheet.Range[$"C{currentRow}"].NumberValue = pozitiflikOrani;
                    currentRow++;
                }
                else
                {

                    File.AppendAllText(logPath, $"Yorum | {DateTime.Now:yyyy-MM-dd HH:mm:ss} | {parca}{Environment.NewLine}");
                }

                Console.ForegroundColor = pozitif ? ConsoleColor.Green : ConsoleColor.Red;
                Console.WriteLine($"  {altNo}. Parça: {parca} → {(pozitif ? "Pozitif" : "Negatif")} (Pozitiflik: {pozitiflikOrani:F4})");
                Console.ResetColor();

                altNo++;
            }
        }

        workbook.SaveToFile(excelPath);
        Console.WriteLine($"\nExcel kaydedildi: {excelPath}");
        Console.WriteLine($"Olumsuz log kaydedildi: {logPath}");
    }
}