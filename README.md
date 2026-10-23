# Product Review Sentiment (.NET)

[TR](#tr) | [ENG](#eng)

---

## TR

### Yorum Duygu Analizi

Türkçe ürün yorumlarını cümle parçalarına ayırıp her parçanın pozitif/negatif olduğunu BERT tabanlı bir ONNX modeliyle tahmin eden .NET 8 konsol uygulaması.

#### Nasıl çalışır

1. Yorum; noktalama işaretleri (`. ! ? , ; :`) ve bağlaçlar (`ama`, `fakat`, `ancak`, `çünkü`, `lakin`, `ve`) üzerinden parçalara bölünür.
2. Her parça BERT tokenizer ile token'lara çevrilir (en fazla 32 token) ve ONNX modeline verilir.
3. Pozitiflik olasılığı 0.5 ve üzerindeyse parça **pozitif** sayılır.
4. Sonuçlar:
   - Pozitif parçalar masaüstündeki `DuyguAnalizi_Spire.xls` dosyasına eklenir (tarih, parça, pozitiflik oranı).
   - Negatif parçalar masaüstündeki `Olumsuz.log` dosyasına yazılır.

#### Gereksinimler

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- NuGet paketleri `.csproj` içinde tanımlıdır ve `dotnet restore` ile otomatik yüklenir:
  - Microsoft.ML.OnnxRuntime 1.22.1
  - Microsoft.ML.Tokenizers 1.0.2
  - Spire.XLS 15.7.8

> Spire.XLS ticari bir kütüphanedir; lisanssız kullanımda oluşturulan Excel dosyasına "Evaluation Warning" sayfası eklenir.

#### Model dosyası

`model.onnx` (~440 MB) GitHub'ın dosya boyutu sınırını aştığı için repoda yer almaz; Hugging Face'te barındırılır:
**https://huggingface.co/Mustafassrc/ProductReviewSentiment**

Dosyayı sayfadan indirip `YorumDuyguAnalizi_1/model.onnx` konumuna koyun ya da repo kök dizininde şu komutu çalıştırın:

```bash
pip install -U huggingface_hub
hf download Mustafassrc/ProductReviewSentiment model.onnx --local-dir YorumDuyguAnalizi_1
```

Tokenizer dosyaları (`tokenizer/vocab.txt` vb.) repoda mevcuttur.

#### Çalıştırma

```bash
cd YorumDuyguAnalizi_1
dotnet run
```

Analiz edilecek yorumlar `Program.cs` içindeki `yorumlar` listesinden okunur.

---

## ENG

### Review Sentiment Analysis

A .NET 8 console application that splits Turkish product reviews into clauses and predicts whether each clause is positive or negative using a BERT-based ONNX model.

#### How it works

1. The review is split on punctuation (`. ! ? , ; :`) and Turkish conjunctions (`ama`, `fakat`, `ancak`, `çünkü`, `lakin`, `ve`).
2. Each clause is tokenized with a BERT tokenizer (max 32 tokens) and fed to the ONNX model.
3. A clause is considered **positive** if its positive probability is 0.5 or higher.
4. Output:
   - Positive clauses are appended to `DuyguAnalizi_Spire.xls` on the desktop (date, clause, positive score).
   - Negative clauses are written to `Olumsuz.log` on the desktop.

#### Requirements

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- NuGet packages are declared in the `.csproj` and restored automatically with `dotnet restore`:
  - Microsoft.ML.OnnxRuntime 1.22.1
  - Microsoft.ML.Tokenizers 1.0.2
  - Spire.XLS 15.7.8

> Spire.XLS is a commercial library; without a license, an "Evaluation Warning" sheet is added to the generated Excel file.

#### Model file

`model.onnx` (~440 MB) is not included in the repository because it exceeds GitHub's file size limit; it is hosted on Hugging Face:
**https://huggingface.co/Mustafassrc/ProductReviewSentiment**

Download it from the page and place it at `YorumDuyguAnalizi_1/model.onnx`, or run this from the repository root:

```bash
pip install -U huggingface_hub
hf download Mustafassrc/ProductReviewSentiment model.onnx --local-dir YorumDuyguAnalizi_1
```

Tokenizer files (`tokenizer/vocab.txt`, etc.) are included in the repository.

#### Running

```bash
cd YorumDuyguAnalizi_1
dotnet run
```

Reviews to analyze are read from the `yorumlar` list in `Program.cs`.
