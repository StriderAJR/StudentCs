# Лабораторная работа 4. Анализ журнала событий сервера

## Имя

sem-1-lab-04-event-log

## Сложность

2 / 5

## Темы

Циклы, операторы перехода, методы, рефакторинг и улучшение кода.

## Задание

Во время работы игровой сервер ведёт журнал событий. Используйте тот же [event_server.log](lab-1-sem-03/event_server.log), что и в третьей лабораторной. Файл содержит 1000 записей. Формат строки: `yyyy-MM-dd HH:mm:ss.fff [level][category] message`.

Сначала прочитайте файл в массив строк `string[]`, затем преобразуйте каждую строку в запись `LogEntry`. Создайте `LogEntry` самостоятельно как **класс или структуру** и объясните выбор на защите. Тип должен только хранить данные, без собственных методов:

```csharp
public DateTime Timestamp;
public string Level;
public string Category;
public string Message;
```

Для результатов фильтрации можно использовать массив `LogEntry[]` или готовый `List<LogEntry>`.

Реализуйте следующие статические методы в классе `Program`:

- `ParseLog(string[] lines)` — возвращает массив `LogEntry[]`;
- `FilterByDate(LogEntry[] entries, DateTime date)` — возвращает записи указанной даты;
- `FilterByLevel(LogEntry[] entries, string level)` — возвращает записи уровня `Info`, `Warning`, `Error` или `Fatal`;
- `FilterByCategory(LogEntry[] entries, string category)` — возвращает записи категории, например `Server`, `Combat` или `Database`;
- `Search(LogEntry[] entries, string text)` — возвращает записи, в сообщении которых есть текст без учёта регистра;
- `CountByLevel(LogEntry[] entries, string level)` — возвращает количество записей переданного уровня;
- `GetServerStatus(LogEntry[] entries)` — возвращает `Сервер работает штатно`, если нет ошибок и `Fatal`; `Есть ошибки: требуется проверка`, если есть уровень `Error`; `КРИТИЧЕСКАЯ ОШИБКА: сервер остановлен`, если есть запись уровня `Fatal` категории `Server`.

Для предоставленного файла результат статуса должен быть `КРИТИЧЕСКАЯ ОШИБКА: сервер остановлен`: 2 сентября сервер остановился из-за недоступной базы данных.

Журнал может содержать произвольное количество записей.

После получения рабочего решения проанализируйте свой код и постарайтесь сделать его более понятным и удобным для сопровождения, не изменяя поведение программы.

## Подсказки

<details>
<summary>Показать подсказки</summary>

- Количество строк в журнале заранее неизвестно. Массив для результата фильтрации можно создать той же длины, что и исходный, а после заполнения скопировать нужную часть в новый массив.
- Для разбора одной строки найдите позиции скобок с помощью `IndexOf`, а затем получите части строки через `Substring`.
- После разбора фильтр по уровню выглядит так: `entries[i].Level == "Warning"`, а фильтр по категории — так: `entries[i].Category == "Combat"`.
- Чтобы найти записи за дату, сравните даты: `entries[i].Timestamp.Date == date.Date`.
- Если в программе появляются большие повторяющиеся фрагменты кода, подумайте, как сделать решение проще и понятнее.

### Разбор одной строки

```csharp
LogEntry entry = new LogEntry();

entry.Timestamp = DateTime.Parse(line.Substring(0, 23));

int firstOpenBracket = line.IndexOf('[');
int firstCloseBracket = line.IndexOf(']', firstOpenBracket);
int secondOpenBracket = line.IndexOf('[', firstCloseBracket);
int secondCloseBracket = line.IndexOf(']', secondOpenBracket);

entry.Level = line.Substring(firstOpenBracket + 1, firstCloseBracket - firstOpenBracket - 1);
entry.Category = line.Substring(secondOpenBracket + 1, secondCloseBracket - secondOpenBracket - 1);
entry.Message = line.Substring(secondCloseBracket + 2);
```

### Обход массива

```csharp
for (int i = 0; i < entries.Length; i++)
{
    LogEntry entry = entries[i];
    // Обработка одной записи.
}
```

### Фильтрация в новый массив

Сначала создайте временный массив максимального размера. Переменная `resultCount` показывает, сколько его ячеек уже заполнено.

```csharp
LogEntry[] temporaryResult = new LogEntry[entries.Length];
int resultCount = 0;

for (int i = 0; i < entries.Length; i++)
{
    if (entries[i].Level == "Warning")
    {
        temporaryResult[resultCount] = entries[i];
        resultCount++;
    }
}
```

После цикла скопируйте только заполненную часть в итоговый массив:

```csharp
LogEntry[] result = new LogEntry[resultCount];
Array.Copy(temporaryResult, result, resultCount);
return result;
```

### `List<LogEntry>` вместо массива фиксированного размера

Если заранее неизвестно, сколько записей пройдёт фильтр, можно использовать готовый список `List<LogEntry>`. Для этого в начале файла добавьте `using System.Collections.Generic;`. Список сам увеличивает размер при вызове `Add`.

```csharp
var result = new List<LogEntry>();

for (int i = 0; i < entries.Length; i++)
{
    if (entries[i].Level == "Warning")
    {
        result.Add(entries[i]);
    }
}

return result.ToArray();
```

Используйте либо этот вариант, либо временный массив с `Array.Copy` из предыдущего примера.

### Подсчёт записей

```csharp
int count = 0;

for (int i = 0; i < entries.Length; i++)
{
    if (entries[i].Level == "Error")
    {
        count++;
    }
}

return count;
```

</details>

## Дополнительное задание

Добавьте метод `ExportFiltered(string path, LogEntry[] entries)`, который сохраняет отфильтрованные записи в исходном формате лога.

## Пример юнит-тестов

Создайте отдельный проект `sem-1-lab-04-tests`, подключите NUnit и ссылку на основной проект. Добавьте в него [ExampleTests.cs](lab-1-sem-04/ExampleTests.cs). Для тестов скопируйте в выходной каталог тестового проекта [event_server.log](lab-1-sem-03/event_server.log).
