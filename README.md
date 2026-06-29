# Whaledevelop.Module

Набор базовых скриптов для построения модульной архитектуры Unity-проекта поверх VContainer. Модуль задаёт общий жизненный цикл моделей, сервисов и систем, связывает их с игровыми состояниями, централизует Unity Update-циклы и содержит вспомогательные решения для UI, пулов компонентов, UniTask и editor-инструментов.

Модуль не является готовым игровым фреймворком с фиксированным набором состояний. Он предоставляет строительные блоки: проект определяет собственные модели, сервисы, системы, entry points и способы создания UI, а общая инфраструктура отвечает за регистрацию, инициализацию и освобождение этих объектов.

## Основная схема работы

1. `GameLifetimeScopeBase<TEntryPoint>` создаёт корневой VContainer scope, регистрирует Unity callbacks, стандартные dispatchers и игровой entry point.
2. Проект помечает реализации атрибутами `GameModel`, `GameService` и `GameSystem`.
3. `ScriptsGenerator` находит эти реализации и создаёт composition-классы для их регистрации и разрешения через VContainer.
4. При входе в состояние `StateEntryPointBase` сначала инициализирует модели, выполняет пользовательскую логику входа, затем инициализирует системы и подключает их к Update-циклам.
5. При выходе `StateExitPointBase` выполняет обратный порядок: отключает и освобождает системы, выполняет пользовательскую логику выхода и освобождает модели.

Сервисы генерируются отдельно от state-scoped моделей и систем. Их создание, инициализацию и освобождение должен организовать composition root конкретного проекта.

## Основные части модуля

### Container

- `GameLifetimeScopeBase<TEntryPoint>` — базовый корневой `LifetimeScope`. Автоматически регистрирует `ActionsDispatcher`, `EventsDispatcher`, `UpdatesDispatcher`, `MonoBehaviourCallbacks` и указанный `EntryPointBase`.
- `EntryPointBase` — базовый асинхронный entry point приложения. Проект реализует `StartAsync` и запускает из него нужный сценарий загрузки.

### Lifetime

- `ILifetime` — общий маркер объектов с управляемым жизненным циклом.
- `SyncLifetime` — синхронная реализация с методами расширения через `IInitializable` и `IReleasable`. Наследники переопределяют `OnInitialize` и `OnRelease`.
- `AsyncLifetime` — асинхронный вариант на UniTask. Наследники переопределяют `OnInitializeAsync` и `OnReleaseAsync`.
- `LifetimeExtensions` — единая точка вызова `InitializeAsync` и `ReleaseAsync` для синхронных и асинхронных объектов.

Повторная инициализация уже активного объекта или освобождение неинициализированного объекта не запускает жизненный цикл второй раз.

### Models, Services и Systems

- `IGameModel` — состояние и данные игрового домена.
- `IService` — долгоживущие сервисы, не привязанные к Update-циклу состояния.
- `IGameSystem` — логика, работающая с моделями и участвующая в жизненном цикле состояния.
- `GameModel<TClass, TInterface>` — базовый класс синхронной модели с поддержкой регистрации в VContainer.
- `RuntimeModelsContainer`, `RuntimeServicesContainer`, `RuntimeSystemsContainer` — runtime-списки объектов, используемые orchestration-слоем и editor-отладкой.
- `SharedGameModelsContainer` — возвращает один экземпляр модели для нескольких scopes, если модель объявлена общей.

Атрибуты для code generation:

```csharp
[GameModel("Game")]
public sealed class PlayerModel : GameModel<PlayerModel, IPlayerModel>, IPlayerModel
{
}

[GameService]
public sealed class SaveService : SyncLifetime, ISaveService
{
}

[GameSystem("Battle")]
public sealed class CombatSystem : SyncLifetime, IGameSystem, IUpdate
{
    public void OnUpdate()
    {
    }
}
```

Если scope не указан у модели или системы, используется `Game`. Модель, указанная для нескольких scopes, создаётся через `SharedGameModelsContainer` и переиспользуется между ними. Для сервиса генератор ожидает ровно один конкретный интерфейс, наследующий `IService`.

### States

- `StateEntryPointBase` — инициализирует модели и системы текущего scope. Системы с интерфейсами `IUpdate`, `IFixedUpdate` или `ILateUpdate` автоматически передаются в `UpdatesDispatcher`.
- `StateExitPointBase` — освобождает только успешно инициализированные объекты в обратном порядке и сохраняет первую ошибку освобождения до завершения cleanup.
- `StateLifecycleContext` — хранит модели и системы, реально запущенные текущим состоянием.

Для специфичной логики состояния нужно переопределить `OnEnterAsync` или `OnExitAsync`.

### Dispatchers и Unity callbacks

- `UpdatesDispatcher` — вызывает обычный, физический и поздний Update для зарегистрированных объектов.
- `ActionsDispatcher` — создаёт action, внедряет его зависимости через VContainer и вызывает `Invoke`. Поддерживает action без параметров и с `IActionParams`.
- `EventsDispatcher` — базовая singleton-точка для проектного event dispatcher API.
- `MonoBehaviourCallbacks` — переводит `Update`, `FixedUpdate`, `LateUpdate`, `OnDestroy`, `OnApplicationQuit` и `OnDrawGizmos` в C# events.
- `SingletonDispatcher<TSelf, TInterface>` — общая VContainer-регистрация и статический доступ к экземпляру dispatcher.

Предпочтительный доступ к dispatchers — через интерфейсы и DI. Статическое свойство `Instance` доступно только после создания dispatcher в scope.

### UI

- `UIService<TEnum>` — хранит открытые окна по enum-коду, внедряет зависимости во view model и управляет жизненным циклом model/view.
- `UIView` и `UIView<TModel>` — базовые MonoBehaviour-представления.
- `IUIViewModel` — синхронная view model с управляемым жизненным циклом.
- `UIViewRuntimeData` — связка открытого view и его model.

Конкретный UI-сервис должен реализовать `CreateView` и `DestroyView`. Сам модуль не определяет, откуда загружаются окна: prefab, addressables или заранее сериализованный registry выбирает проект.

### Pool

- `ComponentsPool<T>` — обёртка над `UnityEngine.Pool.ObjectPool<T>` для компонентов prefab. Ведёт список активных экземпляров и поддерживает prewarm, `Get`, `Return`, `ReturnAll` и `Clear`.
- `ComponentsPoolExtensions` — поиск активного элемента по predicate.

### UniTask utilities

- `UniTaskUtility` — ожидание `UniTaskCompletionSource`, запуск отменяемой задачи и отложенный callback по кадрам или секундам.
- `DOTweenUniTaskUtility` — ожидание завершения DOTween tween через UniTask.
- `CancellationTokenSourceExtensions` — совместные отмена и освобождение `CancellationTokenSource`.

### SerializableDictionary

`SerializableDictionary<TKey, TValue>` сериализует пары ключ-значение через Unity serialization callbacks, сохраняя обычный dictionary API во время выполнения.

## Editor-инструменты

### Scripts Generator

Открывается через `Whaledevelop/ScriptsGenerator`.

Генератор сканирует загруженные assemblies и создаёт:

- `GameServicesComposition.Generated.cs` с регистрацией сервисов;
- `<Scope>RuntimeDataComposition.Generated.cs` с регистрацией и разрешением моделей и систем каждого scope.

Папка вывода по умолчанию: `Assets/_game/Scripts/Generated`. Её можно изменить в окне генератора. Сгенерированные методы необходимо вызвать из LifetimeScope конкретного проекта — генератор не изменяет scope автоматически.

### Runtime Helper

Открывается через `Whaledevelop/Runtime Helper`. Во время Play Mode показывает найденные game/scene scopes и зарегистрированные в них сервисы, модели и системы. Для наполнения окна composition root должен передать разрешённые объекты в runtime/debug containers.

### Scripts Unifier

Открывается через `Tools/Whaledevelop/Scripts Unifier`. Собирает выбранные `.cs`, `.uxml` и `.uss` файлы в один текстовый файл с опциональными заголовками и ограничением общего числа символов.

## Зависимости

- Unity `6000.0.34f` или новее;
- VContainer — через manifest и scoped dependency;
- UniTask — из unitypackage;
- Odin Inspector — через manifest/package;
- R3 — через NuGet;
- DOTween — из Asset Store.

## Минимальная интеграция

1. Установить зависимости и добавить скрипты модуля в Unity-проект.
2. Создать интерфейсы и реализации моделей, сервисов и систем.
3. Добавить нужные `GameModel`, `GameService` и `GameSystem` атрибуты.
4. Запустить `Whaledevelop/ScriptsGenerator`.
5. В проектных LifetimeScope вызвать сгенерированные методы регистрации и заполнить runtime containers разрешёнными объектами.
6. Создать наследников `StateEntryPointBase` и `StateExitPointBase` для входа и выхода из игровых состояний.
7. Создать корневой scope от `GameLifetimeScopeBase<TEntryPoint>` и реализовать запуск приложения в наследнике `EntryPointBase`.

Модуль не создаёт assets, prefabs или конкретную структуру сцен. Эти части остаются ответственностью проекта, который использует инфраструктуру.
