# Как перейти от схемы базы данных к объектной моделе

## Таблицы -> классы

К данному этапу считается, что вы хотя бы в общих чертах прочитали и более-менее осознали все темы из [введения](https://gitlab.com/StriderAJR/dummy-database/-/tree/main#%D0%B2%D0%B2%D0%B5%D0%B4%D0%B5%D0%BD%D0%B8%D0%B5).

Итак, вернемся к примеру с базой для студентов. В C# все является объектами - экземплярами классов, которые в свою очередь являются просто описанием структуры из чего будут состоять будущие объекты. Поэтому из схемы базы данных мы можем с ходу выделить несколько классов: `Student`, `Subject` и `Group`.

> Название класса - это всегда существительное в единственном числе. Поэтомум название классы должно быть не `Students`, как называются таблица в БД, а `Student`.

Встает вопрос, почему у нас нет класса `StudentSubject`, ведь такая таблица есть, и по идее, если переносить все таблицы БД в классы бездумно, нам нужно было бы создать. Но как говорилось про [связь многие-ко-многим](https://gitlab.com/StriderAJR/dummy-database/-/blob/main/what-is-database.md#%D1%81%D0%B2%D1%8F%D0%B7%D1%8C-%D0%BC%D0%BD%D0%BE%D0%B3%D0%B8%D0%B5-%D0%BA%D0%BE-%D0%BC%D0%BD%D0%BE%D0%B3%D0%B8%D0%BC) мы создали эту таблицу, чтобы решить проблему ограниченности функционала таблиц. В языке программирования такой проблемы нет. Мы вполне можем внутри класса `Student` хранить список дисциплин и оценок для них, потому что языки программирования не ограничены только базовыми типами данных, как таблицы. Там есть тип данных список (List), словарь (Dictionary) и многие другие. Даже если каких-то типов данных нам не хватит, мы можем создать их сами.

> Для хранения перечня дисциплин с оценками неплохо может подойти, например, тип данных [Dictionary](https://metanit.com/sharp/tutorial/4.9.php). 

Вот грубый набросок классов (естественно, там есть ошибки, но всему свое время):
```cs
public class Subject
{
    public int Id;
    public string Name;
    public bool IsArchive;
}

public class Group
{
    public int Id;
    public string Name;
}

public class Student
{
    public int Id;
    public string FullName;
    public DateTime BirthDate;
    public int EntranceYear;
    public int GroupId;
    public bool Graduated;
    // вместо таблицы StudentSubjects заводим словарь с оценками
    public Dictionary<int, int> SubjectGrades; 
}
```

Несколько комментариев по коду:
- публичные поля класса именуются с большой буквы, приватные поля либо с символа '_', либо с маленькой буквы. Методы класса всегда идут с большой буквы вне зависимости от модификатора видимости
- везде используется тип данных `int` для полей Id - лучше, конечно же, использовать тип данных uint - unsigned int, потому что идентификатор не может быть отрицательным
- в словаре SubjectGrades ключом будет являться идентификатор дисциплины, а значением оценка за дисциплину, которая, кстати, по любой из известным шкал оценивания (5, 10 или 100 балльной) вполне себе поместится в тип данных byte, а int сильно избыточен.

## Объекты

Если классы - это отображение схемы нашей БД, то объекты - это собственно данные, которые хранятся в нашей БД. Опишем те данные, что были в наших таблицах на языке программирования:
```cs
Subject math = new Subject
{
    Id = 1,
    Name = "Мат. анализ",
    IsArchive = false
};

Subject english = new Subject
{
    Id = 2,
    Name = "Англ. язык",
    IsArchive = false
};

Subject programming = new Subject
{
    Id = 3,
    Name = "Программирование",
    IsArchive = false
};

// для небольших классов можно записывать в одну строку
Group group101 = new Group { Id = 1, Name = "ПрИ-101" };
Group group102 = new Group { Id = 2, Name = "ПрИ-102" };
```

Напомню, что созданные выше переменныt и являются объектами. Только вот если дисциплин или групп будет больше, а нам нужно будет, например, найти дисциплину по каким-то параметрам, нам придется проверять каждый объект подходит ли он под эти параметры. Например, поиск архивной дисциплины будет выглядеть так:
```cs
if (math.IsArchive)
{
    Console.WriteLine($"{math.Name} является архивной дисциплиной")
}

if (english.IsArchive)
{
    Console.WriteLine($"{english.Name} является архивной дисциплиной")
}

if (programming.IsArchive)
{
    Console.WriteLine($"{programming.Name} является архивной дисциплиной")
}
```

Выглядит ну очень не очень, как говорится. А если дисциплины будет не 3, а 33? Так можно докатиться и вот до [такого кода](https://github.com/AceLewis/my_first_calculator.py/blob/master/my_first_calculator.py). Не надо так, пожалуйста.

> Повторяемость кода - это признак плохого кода. Если потом нужно будет поменять что-то в повторяющемся коде, то это придется менять несколько раз. Есть риск в каком-то месте забыть это поменять.

Так что логичнее будет хранить все данные не в отдельных объектах, а все объекты складировать в список. Тогда получится что-то такое:
```cs
List<Subject> students = new List<Subject> { math, english, programming };
List<Group> groups = new List<Group> { group101, group102 };

// на самом деле создавать отдельные объекты, а потом добавлять их в список - такое себе,
// но сценарий, когда мы вручную в коде заполняем данные, сам по себе хреновый,
// поэтому сделаем допущение в учебных целях

var cultureInfo = new CultureInfo("ru-RU", false);
List<Student> students = new List<Student>
{
    new Student
    {
        Id = 1,
        FullName = "Иванов Иван Иванович",
        BirthDate = DateTime.Parse("12.06.2002", cultureInfo),
        EntranceYear = 2022,
        GroupId = 1,
        Graduated = false,
        SubjectGrades = new Dictionary<int,int>
        {
            {1, 5}, // мат.анализ (id = 1) с оценкой 5
            {2, 4}, // англ (id = 2), с оценкой 4
            {3, 4}  // программирование (id = 3) с оценкой 4
        }
    },
    new Student
    {
        Id = 2,
        FullName = "Петров Петр Петрович",
        BirthDate = DateTime.Parse("01.12.2001", cultureInfo),
        EntranceYear = 2020,
        GroupId = 2,
        Graduated = false,
        SubjectGrades = new Dictionary<int,int>
        {
            {1, 3}, // мат.анализ (id = 1) с оценкой 3
            {2, 5}, // англ (id = 2), с оценкой 5
            {3, 5}  // программирование (id = 3) с оценкой 5
        }
    }
}
```
И поиск архивной дисциплины станет более простым и не зависящим от кол-ва дисциплин:
```cs
foreach(var subject in subjects)
{
    if (subject.IsArchive)
    {
        Console.WriteLine($"{subject.Name} является архивной дисциплиной")
    }
}
```
Ну вот что-то такое у нас получилось, но есть проблемка. Допустим, мы хотим вывести на экран список всех студентов с названием группы, в которой они учатся.
```cs
foreach(var student in students)
{
    string groupName;
    foreach(var group in groups)
    {
        if (group.Id == student.GroupId)
        {
            groupName = group.Name;
            break;
        }
    }

    Console.WriteLine($"{student.FullName} учится в группе {groupName}");
}
```

И все выглядит просто ужасно. По стилю оформления кода все нормально, а вот производительность у такого кода будет ужасающей. Для каждого студента мы запускаем поиск его группы в списке групп по айдишнику. Что максимально неэффективно. Можно, конечно, попробовать оптимизировать, например, завести словарь с ключами GroupId и названием группы, но это тогда усложнит читаемость кода.

Другой пример, если этот пример вам показался еще недостаточно ужасным. Для каждого студента нужно вывести список дисциплин с оценками.
```cs
foreach(var student in students)
{
    Console.WriteLine($"оценки студента {student.FullName}:");
    foreach(var pair in student.SubjectGrades)
    {
        var subjectId = pair.Key;
        var grade = pair.Value;

        string subjectName;
        foreach(var subject in subjects)
        {
            if (subject.Id == subjectId)
            {
                subjectName = subject.Name;
                break;
            }
        }

        Console.WriteLine("{subjectName} - {grade}");
    }
}
```

Замечательно. Уже 2 вложеныых foreach. А это мы самые простые вещи пытаемся на экран вывести. В отличие от движка базы данных, которые оптимизированы именно на такую работу (вспомните join из языка запросов) в языках программирования есть другой инструмент, чтобы не страдать.

## Классы - это ссылочный тип данных

Объекты - это [ссылочный тип данных](https://metanit.com/sharp/tutorial/2.16.php?sa=X&ved=2ahUKEwj-092qsLXoAhWjAJ0JHY7iCv0Q9QF6BAgCEAI), а это значит, что в переменной хранится не сам объект со всеми полями и начинками, а адрес этого объекта в памяти и создавать таких переменнных можно много и они не будут занимать много места в памяти.

Сначала поменяем класс `Student` следующим образом:
```cs
public class Student
{
    public int Id;
    public string FullName;
    public DateTime BirthDate;
    public int EntranceYear;
    public Group Group;
    public bool Graduated;
    // вместо таблицы StudentSubjects заводим словарь с оценками
    public Dictionary<Subject, int> SubjectGrades; 
}
```
Мы заменили хранение айдишников объектов `Group` и `Subject` на ссылки на эти объекты. Поэтому создание объектов тоже поменяется. 
```cs
List<Student> students = new List<Student>
{
    new Student
    {
        Id = 1,
        FullName = "Иванов Иван Иванович",
        BirthDate = DateTime.Parse("12.06.2002", cultureInfo),
        EntranceYear = 2022,
        GroupId = group101,
        Graduated = false,
        SubjectGrades = new Dictionary<Subject,int>
        {
            {math, 5},
            {english, 4},
            {programming, 4}
        }
    },
    new Student
    {
        Id = 2,
        FullName = "Петров Петр Петрович",
        BirthDate = DateTime.Parse("01.12.2001", cultureInfo),
        EntranceYear = 2020,
        GroupId = group102,
        Graduated = false,
        SubjectGrades = new Dictionary<Subject,int>
        {
            {math, 3},
            {english, 5},
            {programming, 5}
        }
    }
}
```
И теперь можно переписать примеры вывода студентов, их групп и оценок:
```cs
foreach(var student in students)
{
    Console.WriteLine($"{student.FullName} учится в группе {student.Group.Name}");

    Console.WriteLine($"Оценки:");
    foreach(var pair in student.SubjectGrades)
    {
        var subject = pair.Key;
        var grade = pair.Value;

        Console.WriteLine("{subject.Name} - {grade}");
    }    
}
```

Код стал компактнее и читаемее. Не забывайте, что в C# есть ссылочные типы данных и другие особенности, которые нужно учитывать при создании объектной модели.

> Нельзя переносить таблицы базы данных в объектно-ориентированный язык "в лоб". Каждая технология имеет свои особенности, которые нужно учитывать при проектировании.
