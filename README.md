# 🧣 Vossence E-Ticaret Yönetim Sistemi (E-Commerce Management System)

[![Lisans](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)
[![Yapım Durumu](https://img.shields.io/badge/Status-Geliştirme%20Aşamasında-orange.svg)]()
[![Backend](https://img.shields.io/badge/Backend-C%23%2F.NET%20Core-512BD4.svg)]()
[![ORM](https://img.shields.io/badge/Micro--ORM-Dapper-red.svg)]()

## 📝 Proje Özeti

**Vossence E-Ticaret Yönetim Sistemi**, yeni kurulan ve şal/eşarp satışı yapan **Vossence** firmasının tüm çevrimiçi operasyonlarını yönetmek üzere geliştirilmiş, yüksek performanslı bir e-ticaret çözümüdür.

Bu proje, bir e-ticaret platformu için gerekli olan temel işlevleri (Katalog, Sepet, Sipariş) sağlarken, arka uçta performansı maksimize etmek amacıyla **Dapper Micro-ORM** kullanılarak **Katmanlı Mimari (Layered Architecture)** prensipleriyle inşa edilmiştir.

## ✨ Öne Çıkan E-Ticaret Özellikleri

* **Dinamik Ürün Katalog Yönetimi:** Yönetici paneli (Vossence.ADMIN) üzerinden şal ve eşarp kategorilerini, stokları ve ürün detaylarını kolayca yönetme yeteneği.
* **Kullanıcı Merkezli Web Arayüzü (Vossence.WEB):** Müşterilerin ürünleri inceleyebileceği, filtreleyebileceği ve güvenli alışveriş yapabileceği ön yüz.
* **Sepet ve Sipariş Akışı:** Kullanıcı kaydı, alışveriş sepeti işlevselliği ve sipariş takibi modülleri.
* **Ayrık Yönetici Paneli (Vossence.ADMIN):** Siparişleri, kullanıcıları ve stok durumunu yönetmek için geliştirilmiş özel arayüz.

## ⚙️ Teknolojiler ve Mimari Derinlik

### 🌐 Arka Uç (Backend) & Veri Katmanı

| Teknoloji | Önemi ve Açıklama |
| :--- | :--- |
| **C# ve ASP.NET Core MVC** | Modern, ölçeklenebilir ve platformlar arası bir e-ticaret altyapısı sunar. |
| **Dapper (Micro-ORM)** | Veri erişiminde hızı önceliklendiren temel teknik karar. Stok sorgulama ve sipariş oluşturma gibi kritik işlemlerde maksimum performans sağlar. |
| **Repository & Unit of Work Pattern** | Veri erişim katmanını izole ederek kodun test edilebilirliğini ve veritabanı işlemlerinin tutarlılığını sağlar. |
| **Dependency Injection (DI)** | Bileşenler arası gevşek bağlantıyı ve esnekliği destekler. |

### 🎨 Ön Uç (Frontend)

| Teknoloji | Açıklama |
| :--- | :--- |
| **HTML5, CSS3, JavaScript** | Temel web standartları. |
| **[Kullanıldıysa Framework/Kütüphane - Örn: Bootstrap, jQuery]** | Duyarlı (responsive) tasarım ve hızlı kullanıcı deneyimi. |

---

### 🧱 Mimari Tasarım: Katmanlı Mimari (N-Tier)

Proje, yazılımın sürdürülebilirliğini ve yönetilebilirliğini artırmak için net bir şekilde ayrılmış üç katman kullanır:

1.  **Sunum (Vossence.WEB / .ADMIN):** Müşteri ve yöneticilere yönelik kullanıcı arayüzleri.
2.  **İş (Olası Vossence.CORE/SERVICE):** Ürün fiyatlandırması, stok kontrolü ve sipariş onaylama gibi temel e-ticaret iş kuralları.
3.  **Veri Erişim (Vossence.DATA):** **Dapper** kullanarak SQL sorgularını çalıştırır ve veri nesnelerini iş katmanına iletir.

Bu yapı, teknik yeterliliğinizin ve mimari düşünce yapınızın en güçlü kanıtıdır. 

[Image of Layered Architecture Diagram]


## ⚙️ Kurulum ve Çalıştırma

### Ön Gereksinimler

* [.NET SDK 6.0 veya üzeri](https://dotnet.microsoft.com/download)
* [Veritabanı Sistemi - Örn: SQL Server]
* [Visual Studio 2022]

### Adımlar

1.  **Depoyu Klonlayın:**
    ```bash
    git clone [https://github.com/malikuzunca/Vossence.git](https://github.com/malikuzunca/Vossence.git)
    cd Vossence
    ```
2.  **Veritabanı Yapılandırması:**
    * Veritabanı bağlantı dizesini (`appsettings.json`) güncelleyin.
    * Veritabanı şemasını oluşturmak için [SQL Script Adı].sql dosyasını çalıştırın.
3.  **Projeyi Başlatın:**
    * Visual Studio'da `Vossence.sln` dosyasını açın.
    * Projeyi çalıştırın (F5).

## 🤝 İletişim ve Geliştirici

Bu proje **Malik Uzunca** tarafından geliştirilmiştir.

* **GitHub:** [@malikuzunca](https://github.com/malikuzunca)
* **LinkedIn:** [@malikuzunca](https://www.linkedin.com/in/malikuzunca/)
* **E-posta:** [gsmalikuzunca@outlook.com]

---

Bu README ile, bir işe alımcıya hem **iş hedefini (e-ticaret)** anladığınızı hem de **ileri düzey teknik kararlar (Dapper)** alabildiğinizi göstermiş olursunuz.

Bu taslak üzerinde son bir düzenleme yapmak ister misiniz, yoksa bir sonraki adıma (örneğin bu projenin CV'de nasıl vurgulanacağı) geçelim mi?
