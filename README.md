TR
Duygu Analizi Projesi
======================

- Proje .NET 8.0 ile geliştirilmiştir.
- Gerekli NuGet paketleri proje dosyasında (.csproj) belirtilememiştir.
- Bu yüzden NuGet paketleri elle yüklenmelidir:
    * Microsoft.ML.OnnxRuntime (v1.17.0)
    * Microsoft.ML.Tokenizers (v0.20.0)
    * Spire.XLS (v13.6.3)

- Model dosyaları:
  * model.onnx
  * tokenizer/vocab.txt
  Proje klasöründe uygun şekilde yer almalıdır.

- Excel çıktısı ve log dosyası masaüstüne kaydedilir.

---

Not: Kütüphaneler elle yüklendikten sonra proje çalışacaktır.

İyi çalışmalar!
*****************************************************************************************************************************

ENG
Sentiment Analysis Project
======================
The project is developed with .NET 8.0.

Required NuGet packages are not listed in the project file (.csproj).

Therefore, NuGet packages must be installed manually:

Microsoft.ML.OnnxRuntime (v1.17.0)

Microsoft.ML.Tokenizers (v0.20.0)

Spire.XLS (v13.6.3)

Model files:

model.onnx

tokenizer/vocab.txt
Must be placed properly in the project folder.

Excel output and log files are saved to the desktop.

Note: The project will run after manually installing the required libraries.

Best regards!
