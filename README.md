# PathPeer

Platformă digitală hibridă de cursuri P2P cu recomandări personalizate.

Utilizatorii pot crea și distribui cursuri local prin Bluetooth/WiFi Direct, iar profesorii verificați pot depune cursuri în cloud printr-un sistem de review comunitar săptămânal.

---

## Cuprins

- [Descriere](#descriere)
- [Stack Tehnologic](#stack-tehnologic)
- [Structura Proiectului](#structura-proiectului)
- [Cerințe](#cerințe)
- [Instalare și Rulare](#instalare-și-rulare)
- [Variabile de Mediu](#variabile-de-mediu)
- [API Documentation](#api-documentation)

---

## Descriere

PathPeer rezolvă problema accesului dificil la educație de calitate prin:

- **P2P local** — cursuri distribuite prin Bluetooth/WiFi Direct, fără internet
- **Cloud review** — profesori verificați depun cursuri, comunitatea votează 7 zile, minim 10 upvotes pentru aprobare
- **Recomandări inteligente** — Content-Based (60%) + Collaborative Filtering (40%)
- **Securitate** — AES-256 pentru cursuri aprobate, semnături digitale RSA pentru cursuri locale

### Roluri utilizatori

| Rol | Permisiuni |
|-----|-----------|
| User | Creare cursuri locale, P2P sharing, votare review |
| Teacher Pending | Cerere trimisă, în așteptare aprobare admin |
| Teacher Approved | Depune 1 curs/săptămână în cloud pentru review |
| Admin | Aprobare profesori, monitorizare review, moderare |

---

## Stack Tehnologic

| Componentă | Tehnologie |
|------------|-----------|
| Backend | .NET 10, ASP.NET Core Web API |
| Bază de date | PostgreSQL 16 |
| Cache | Redis 7 |
| Realtime | SignalR |
| Background jobs | Hangfire |
| Frontend Web | React + Vite + TypeScript |
| State management web | TanStack Query + Zustand |
| Frontend Mobile | Flutter (Dart) |
| State management mobile | Riverpod |
| HTTP client mobile | Dio |
| Stocare locală | Isar |
| Autentificare | JWT |
| Plăți | Stripe |

---

## Structura Proiectului

```
pathpeer/
├── pathpeer-api/                    # Backend .NET
│   ├── PathPeer.API/       # Controllers, Middleware
│   ├── PathPeer.Application/  # Services, DTOs, Interfaces
│   ├── PathPeer.Domain/    # Entities, Enums, Constants
│   └── PathPeer.Infrastructure/  # EF Core, Redis, Hangfire
│
├── pathpeer-web/                    # Frontend React + Vite
│   └── src/
│       ├── features/       # Auth, Courses, Reviews, Chat...
│       ├── pages/          # Pagini per rută
│       └── shared/         # Componente, hooks, utils comune
│
└── pathpeer-mobile/                 # Flutter
│    └── lib/
│        ├── core/           # Network, storage, theme
│        └── features/       # Auth, Courses, P2P, Reviews...
│
├── docker-compose.yml
├── .env.example
└── README.md
```

---

## Cerințe

Asigură-te că ai instalat:

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Node.js 20+](https://nodejs.org/)
- [Flutter SDK 3.x](https://flutter.dev/docs/get-started/install)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/)
- [Git](https://git-scm.com/)

---

## Instalare și Rulare

### 1. Clonează repository-ul

```bash
git clone https://github.com/username/pathpeer.git
cd pathpeer
```

### 2. Configurează variabilele de mediu

```bash
cp .env.example .env
# Editează .env cu valorile tale
```

### 3. Pornește baza de date

```bash
docker-compose up -d
# PostgreSQL va fi disponibil pe localhost:5432
# Redis va fi disponibil pe localhost:6379
```

### 4. Rulează backend-ul

```bash
cd pathpeer/pathpeer-api
dotnet restore
dotnet ef database update --project PathPeer.Infrastructure --startup-project PathPeer.API
dotnet run --project PathPeer.API
# API disponibil pe https://localhost:5001
# Swagger UI: https://localhost:5001/swagger
```

### 5. Rulează frontend-ul web

```bash
cd pathpeer/pathpeer-web
npm install
npm run dev
# Disponibil pe http://localhost:5173
```

### 6. Rulează aplicația mobile

```bash
cd pathpeer/pathpeer-mobile
flutter pub get
flutter run
# Asigură-te că ai un emulator pornit sau un device conectat
```

---

## Variabile de Mediu

Copiază `.env.example` în `.env` și completează valorile:

```env
# PostgreSQL
POSTGRES_DB=pathpeer
POSTGRES_USER=admin
POSTGRES_PASSWORD=your_password_here

# Redis
REDIS_URL=localhost:6379

# JWT
JWT_SECRET=your_secret_key_minimum_32_characters
JWT_EXPIRY_HOURS=24

# Stripe
STRIPE_SECRET_KEY=sk_test_...
STRIPE_WEBHOOK_SECRET=whsec_...

# Email (pentru notificări profesori)
SMTP_HOST=smtp.gmail.com
SMTP_PORT=587
SMTP_USER=your_email@gmail.com
SMTP_PASSWORD=your_app_password
```

> ⚠️ **Nu comite niciodată fișierul `.env` sau `appsettings.Development.json` pe Git.**

---

## API Documentation

După ce pornești backend-ul, documentația completă a API-ului e disponibilă la:

```
https://localhost:5001/swagger
```

Endpoint-uri principale:

```
POST   /pathpeer-api/auth/register
POST   /pathpeer-api/auth/login
GET    /pathpeer-api/courses
POST   /pathpeer-api/courses
POST   /pathpeer-api/courses/{id}/submit-review
POST   /pathpeer-api/courses/{id}/vote
GET    /pathpeer-api/recommendations/{userId}
POST   /pathpeer-api/payments/create-checkout
```