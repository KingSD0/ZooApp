# ZooApp – Virtuele Dierentuinbeheerapplicatie

Dit project is ontwikkeld als eindopdracht voor het leertraject C# aan WFHBO ICT.  
De applicatie is een ASP.NET Core MVC-webapplicatie waarmee gebruikers een virtuele dierentuin kunnen beheren via zowel een webinterface als een REST API.

---

## 🎯 Functionaliteiten (volgens opdrachtomschrijving)

- ✅ CRUD-operaties op **dieren**, **categorieën** en **verblijven**
- ✅ Sunrise/Sunset logica op dier-, verblijf- en dierentuin-niveau
- ✅ Feeding Time logica met prioriteit op prooidieren
- ✅ Constraint-checks (zoals ruimte en beveiliging)
- ✅ Auto-indeling van dieren op basis van ruimte + veiligheid
- ✅ API en webinterface gebruiken dezelfde services
- ✅ Filtering en zoeken op eigenschappen
- ✅ Gebruik van enums voor gedrag, grootte, voeding, enz.

---

## 🛠️ Gebruikte technologieën

- .NET 8 (ASP.NET Core MVC)
- Entity Framework Core
- SQL Server LocalDB
- Razor Views
- Faker (Bogus) voor seeding
- GitHub voor versiebeheer
- Visual Studio 2022
- Postman (API-testing)
- xUnit (logica testen)

---

## ⚙️ Setup & installatie

1. Clone deze repository:
   
bash
   git clone https://github.com/KingSD0/ZooApp.git

2. Open de oplossing in **Visual Studio 2022**
3. Voer de volgende EF Core commando’s uit in de **Package Manager Console**:
powershell
   Update-Database
4. Start de applicatie (F5). Seeddata wordt automatisch toegevoegd bij eerste run.

---
## 🧪 Testen
- Het project bevat een apart testplan waarin alle uitgevoerde tests zijn opgenomen.
- Testtools: Postman (API-endpoints) en xUnit (logica zoals Sunrise)
- Handmatige tests zijn uitgevoerd via de webinterface
- Alle testgevallen zijn opgenomen in het document Testplan-VirtueleDierentuin.docx

---
## 📐 Wireframes
Voor het ontwerp van de gebruikersinterface zijn wireframes opgesteld.
Deze zijn opgenomen in het document Wireframes-VirtueleDierentuin.pdf en geven een visuele weergave van de pagina's voor:
-Dierenbeheer
-Categoriebeheer
-Verblijvenbeheer

---
## 📁 Ingeleverde documenten

De volgende documenten zijn toegevoegd aan de repository in de map `Docs/` (of root):

- ✅ **Dierentuin-DesignDocument-ShailoDouglas.docx** - bevat het technisch en functioneel ontwerp.
- ✅ **Testplan-VirtueleDierentuin_S1157233.docx** - beschrijft de teststrategie, testgevallen en testresultaten.
- ✅ **Reflectie-VirtueleDierentuin_S1157233.docx** - persoonlijke reflectie op het ontwikkelproces.
- ✅ **Wireframes-VirtueleDierentuin.pdf** - visueel ontwerp van belangrijke schermen.
- ✅ **ERD.png** - entiteit-relatie diagram van het datamodel.
- ✅ **README.md** - dit document, met uitleg over de opzet en werking van het project.

---



## 👤 Auteur - **Shailo Douglas**
**Studentnummer**: S1157233