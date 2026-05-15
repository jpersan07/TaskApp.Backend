# TaskApp Backend — Documentación del Proyecto

## ¿Qué es el proyecto?

Una **API REST** para una aplicación de lista de tareas (TODO List).
Construida con **.NET 10** siguiendo **Clean Architecture**.
El backend expone endpoints HTTP que una futura app frontend consumirá.

---

## Progreso

| Área | Estado |
|---|---|
| Entidades de dominio (`AppTask`, `Category`, `Tag`, `SubTask`, `AppUser`) | ✅ |
| Base de datos con EF Core + SQL Server | ✅ |
| Patrón Repositorio genérico + 4 repositorios específicos | ✅ |
| Servicios de aplicación (`AppTaskService`, `CategoryService`, `TagService`, `SubTaskService`) | ✅ |
| DTOs (Create / Update / Read por entidad) | ✅ |
| Controllers REST con todos los endpoints CRUD | ✅ |
| DTOs de autenticación + interfaz `IAuthService` | ✅ |
| `AuthService` con Identity (UserManager, SignInManager) | ✅ |
| Migraciones y base de datos creada en SQL Server | ✅ |
| Implementación de `Register` y `Login` con JWT | 🔄 En progreso |
| `AuthController` | ⏳ Pendiente |
| Middleware de excepciones personalizado | ⏳ Pendiente |
| Swagger con soporte JWT | ⏳ Pendiente |
| Comentarios XML en todos los archivos | ⏳ Pendiente |

---

## Arquitectura — 4 capas (Clean Architecture)

```
TaskApp.Domain          → Entidades y contratos (sin dependencias externas)
TaskApp.Application     → Lógica de negocio, DTOs, interfaces de servicios
TaskApp.Infrastructure  → Base de datos, repositorios, servicios externos
TaskApp.API             → Controllers HTTP, configuración, punto de entrada
```

Cada capa solo conoce a la de abajo — la API conoce Application, Application conoce Domain.
Infrastructure implementa los contratos definidos en Domain.

---

## Tipos de archivo y por qué hay 4 de cada uno

El proyecto trabaja con 4 entidades principales: `AppTask`, `Category`, `Tag`, `SubTask`.
Por eso, por cada "tipo" de archivo hay exactamente 4 copias — una por entidad.

---

### 1. Interfaces de repositorio (`Domain/Interfaces/`)

**¿Qué es?** Un contrato que dice "quien implemente esto debe tener estos métodos".

Primero existe una interfaz **genérica** con las operaciones CRUD básicas:

```csharp
// IRepository<T> — válido para cualquier entidad
public interface IRepository<T>
{
    Task<IEnumerable<T>> GetAll();
    Task<T?> GetById(int id);
    Task Add(T entity);
    Task Delete(int id);
    Task Update(T entity);
}
```

Luego hay 4 interfaces **específicas**, una por entidad, que heredan de la genérica y añaden métodos extra:

```csharp
// IAppTaskRepository — hereda IRepository<AppTask> y añade filtro por usuario
public interface IAppTaskRepository : IRepository<AppTask>
{
    Task<IEnumerable<AppTask>> GetAllByUserId(string userId);
}
```

> **Por qué 4:** cada entidad necesita métodos distintos. `AppTask` filtra por `userId`,
> `SubTask` filtra por `taskId`. La genérica no puede saber eso, así que cada
> interfaz específica añade lo que le falta.

---

### 2. Implementaciones de repositorio (`Infrastructure/Repositories/`)

**¿Qué es?** El código real que habla con la base de datos usando Entity Framework.

Primero existe una clase **genérica** `Repository<T>` que implementa `IRepository<T>`:

```csharp
// Repository<T> — Add, Delete, GetAll, GetById, Update para cualquier entidad
public class Repository<T> : IRepository<T> where T : class
{
    protected AppDbContext _context;
    
    public async Task Add(T entity)
    {
        await _context.Set<T>().AddAsync(entity);
        await _context.SaveChangesAsync();
    }
    // ... resto de métodos CRUD
}
```

Luego hay 4 clases **específicas** que heredan `Repository<T>` y añaden sus consultas extra:

```csharp
// CategoryRepository — hereda todo de Repository<Category> y añade GetAllByUserId
public class CategoryRepository : Repository<Category>, ICategoryRepository
{
    public async Task<IEnumerable<Category>> GetAllByUserId(string userId)
        => await _context.Categories.Where(c => c.UserId == userId).ToListAsync();
}
```

> **Resumen del patrón:**
> `IRepository<T>` → contrato genérico  
> `IAppTaskRepository` → contrato específico (hereda + añade)  
> `Repository<T>` → implementación genérica  
> `AppTaskRepository` → implementación específica (hereda + añade)

---

### 3. DTOs (`Application/DTOs/`)

**¿Qué es?** Objetos que viajan entre la API y el cliente. No son entidades de base de datos — son lo que el usuario envía o recibe.

Por cada entidad hay 3 DTOs:

| DTO | Dirección | Para qué |
|---|---|---|
| `CategoryDto` | API → Cliente | Respuesta al hacer GET |
| `CreateCategoryDto` | Cliente → API | Cuerpo del POST |
| `UpdateCategoryDto` | Cliente → API | Cuerpo del PUT (campos opcionales) |

> **Por qué no usar la entidad directamente:** la entidad `Category` puede tener campos
> internos (como claves foráneas o propiedades de navegación) que no deben exponerse.
> El DTO es una "vista" controlada de los datos.

---

### 4. Interfaces de servicio (`Application/Services/`)

**¿Qué es?** El contrato de la lógica de negocio. Define qué operaciones existen para cada entidad.

```csharp
public interface ICategoryService
{
    Task<IEnumerable<CategoryDto>> GetAllCat(string userId);
    Task<CategoryDto?> GetCatById(int id);
    Task<CategoryDto> CreateCategory(CreateCategoryDto newCat, string userId);
    Task UpdateCat(UpdateCategoryDto uptCat, int id);
    Task DeleteCat(int id);
    Task BulkDelete(string userId);
}
```

> A diferencia de los repositorios, los servicios trabajan con **DTOs**, no con entidades.
> El controller no sabe que existe una base de datos — solo sabe que existe un servicio.

---

### 5. Implementaciones de servicio (`Application/Services/`)

**¿Qué es?** La lógica real. Recibe DTOs, llama al repositorio, devuelve DTOs.

```csharp
public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _repository;

    public async Task<CategoryDto> CreateCategory(CreateCategoryDto newCat, string userId)
    {
        // 1. Convierte el DTO en entidad
        Category _newCat = new Category { Name = newCat.Name };
        // 2. Guarda en base de datos a través del repositorio
        await _repository.Add(_newCat);
        // 3. Devuelve un DTO con los datos creados
        return new CategoryDto { Id = _newCat.Id, Name = _newCat.Name, UserId = userId };
    }
}
```

> El servicio **nunca** devuelve entidades de dominio hacia la API.
> Siempre convierte entidad → DTO antes de devolver.

---

### 6. Controllers (`API/Controllers/`)

**¿Qué es?** Los endpoints HTTP. Reciben la petición, llaman al servicio, devuelven la respuesta HTTP.

```
GET    /api/category?userId=xxx   → 200 OK  + lista de categorías
GET    /api/category/{id}         → 200 OK  + categoría | 404 NotFound
POST   /api/category              → 201 Created + categoría nueva
PUT    /api/category/{id}         → 204 NoContent
DELETE /api/category/{id}         → 204 NoContent
DELETE /api/category/bulk/{userId}→ 204 NoContent
```

> El controller **no tiene lógica**. Solo traduce HTTP ↔ servicio.

---

## Flujo de una petición (de punta a punta)

```
Cliente HTTP
    ↓  POST /api/category  { "name": "Trabajo" }
CategoryController
    ↓  CreateCategory(dto, userId)
CategoryService
    ↓  _repository.Add(entity)
CategoryRepository
    ↓  _context.Categories.AddAsync(entity)
SQL Server
    ↑  Id generado
CategoryRepository → CategoryService → CategoryController
    ↑  201 Created  { "id": 1, "name": "Trabajo", "userId": "..." }
Cliente HTTP
```

---

## Git Workflow

```
main        ← código estable (GitHub)
develop     ← integración (GitHub)
feature/xxx ← una rama por tarea (local, se pushea al terminar)
```

Commits con Conventional Commits: `feat:`, `fix:`, `chore:`
