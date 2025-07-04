## Что такое база данных

Если совсем просто, то база данных - это один из способов хранения информации (еще говорят "данные"). Самый простой пример другого способа хранения данных - это файл, например, текстовый. Но в отличие от файла в базе данных информация структурирована по определенным правилам (это назвают **схемой базы данных**), поэтому не получится ошибиться при изменении информации.

Вообще базы данных бывают разных типов. Реляционные (от слова relation - взаимоотношение) - на данный момент все еще самый распространенный тип БД, основывается на таблицах, которые связаны друг с другом какими-то связями (отношениями). Документоориентированные БД, где нет таблиц, а есть "документы" - кусок произвольной информации, сгруппированный по логическому принципу. Еще есть столбчатые, графовые и т.д. У вас будет отдельная дисцлина на изучение всего этого зоопарка. В нашем курсе мы будем затрагивать только самый простой тип - реляционный, и тот в максимально упрощенном виде.

Пример. Деканату нужно хранить список всех студентов, которые у них обучались и обучаются. У студентов есть ФИО, номер группы, учатся они или выпустились, а также список оценок по дисциплинам. Допустим, у нас есть 2 студента Иванов и Петров. Создадим файлик, в котором будем хранить информацию о них.

```
Иванов Иван Иванович. Родился 12 июня 2002 года. Был зачислен в группу ПрИ-101 в 2020 году, не отчислен. 
Его оценки: по матану 5, по онглискому 4, по программированию 4

Петров Петр Петрович. 01.12.2001. ПИ-102 (2020), учится. МатАнализ - удовл, иняз - отл, прога - 5
```

Обратите внимание, что информация про Иванова и Петрова либо заполнялась разными людьми, либо у человека биполярка. У Иванова все расписано подробно (но есть ошибка в слове "онглискому"). Петрова заполнял какой-то ленивый методист, который сократил текст как только можно. Причем почти везде есть несоответствия. В одном случае группа записана как "ПрИ-101", в другом "ПИ-102". Названия дисциплин в обоих записях разные. "Матан" и "МатАнализ" - одна и та же дисциплина, но записана по-разному. То же касается "онглийского" и "иняза". "Программирование" и "прога". Такие ошибки называю "человеческим фактором". Если что-то делает человек, он обязательно ошибется.

Информацию в таком виде тяжело читать даже человеку, а для машины (исключая машинное обучение) такой вид хранения вообще нечитаем.

## Структурирование данных

Чтобы сделать информацию читаемой для машины, она должна быть строго структурирована (мы сейчас не рассматриваем любые практики, направленные на работу именно с неструктурированными данными - это машинное обучение, или другие виды баз данных без строгой структуры). 

Первый структурированный тип хранение информации, который приходит в голову, - это табличное представляение. Есть табличка, в ней есть столбцы - характеристики объекта и строки - собственно данные. Для простоты любую таблицу можно представить в виде формата csv (comma separated values) - данные разделенные точкой с запятой (разделитель на самом деле может быть любым, но обычно используют точку с запятой).

Запишем наших студентов в виде таблички:


| ФИО | Год рождения | Группа | Год зачисления | Мат.анализ | Англ | Програм | Учится
| ------ | ------ | ------ | ------ | ------ | ------ | ------ | ------ |
| Иванов Иван Иванович | 12.06.2002 | ПрИ-101 | 2022 | 5 | 4 | 4 | да
| Петров Петр Петрович | 01.12.2001 | ПИ-102 | 2020 | удовл | отл | 5 | да

или в виде текстового файла в формате csv это запишется так:
```
ФИО;Год рождения;Группа;Год зачисления;Мат.анализ;Англ;Програм;Учится
Иванов Иван Иванович;12.06.2002;ПрИ-101;2022;5;4;4;да
Петров Петр Петрович;01.12.2001;ПИ-102;2020;удовл;отл;5;да
```

## Схема базы данных

В табличном виде данные выглядят уже гораздо лучше (для машины). Для человека они тоже все еще хорошо читаемы. При помощи таблиц удалось избавиться от части человеческих ошибок. Во-первых, не нужно писать никакого "дополнительного текста", только сырая информация. Не нужно вводить названия дисциплин, а значит в них не ошибешься. 

Но часть информации все еще не приведена к единому виду. Названия групп, "ПрИ" и "ПИ", а также оценки. Где-то числами, а где названиями. Разработчику придется писать много проверок и костылей, чтобы обработать такую информацию и объяснить машине, что с этим делать. Нужно как-то запретить пользователю вводить неверные данные.

Для этого и используют правила описания таблиц баз данных. Схема строго регламентирует какие есть таблицы, какие в ней есть столбцы, какой тип данных можно хранить в этих столбцах (текст, число, логический тип и т.д.)

### Описание схемы для примера

Если мы хотим перевести информацию о студентах в базу данных, нужно сначала описать таблицы. В данный момент таблица у нас одна - Студенты. Опишем схему таблицы - создадим файл с названием, например, `student.schema`. Формат csv нам уже не очень подойдет, описание схемы таблицы нельзя структурировать в виде строк и столбоц, там будет много разной информации. Поэтому воспользуемся форматом [json](https://ru.wikipedia.org/wiki/JSON). Он более универсален, чем csv, но более компактен, чем [xml](https://ru.wikipedia.org/wiki/XML), поэтому имеет очень широкое распространение.

В целях обучения, мы будем работать с обоими этими форматами. Для схем будем использовать json, для хранения самих данных - csv.

```json
{
    "name": "Student",
    "columns": [
        {
            "name": "fullName",
            "type": "string"
        },
        {
            "name": "birthDate",
            "type": "datetime"
        },
        {
            "name": "entranceYear",
            "type": "int"
        },
        {
            "name": "groupName",
            "type": "string"
        },
        {
            "name": "mathematicsGrade",
            "type": "int"
        },
        {
            "name": "englishGrade",
            "type": "int"
        },
        {
            "name": "programGrade",
            "type": "int"
        },
        {
            "name": "graduated",
            "type": "bool"
        }
    ]
}
```

Имея на руках схему БД, при добавлении в таблицу Student данных вводимая информация будем проверяться на соответствие заявленному типу данных. Теперь не получится в графу "оценка" ввести текст "отл" или "удовл", потому что это не числа. Хорошо, еще одну потенциальную проблему мы решили.

### Нормализация БД

#### Имеюшиеся проблемы

Осталась проблема записи групп "ПИ" и "ПрИ". Столбец `groupName` хранит данные типа `string` - обычная строка, а значит мы не можем контролировать, что введет пользователь.

Еще скажу наперед, что для каждой отдельной дисциплины заводить отдельный столбец и хранить в нем оценку - плохая идея. Проблема здесь заключается в сложностях внесения любых измений в схему БД. При любых изменения движок базы будет проверять не сломают ли эти изменения уже имеющиеся данные в базе. Чем больше база, тем выше цена таких изменений. В случай с дисциплинами это особенно актуально.

Ситуация: до 2020 года была дисцпилина "Экономика", которую должны были изучать все студенты. Для нее в таблице Student завели отдельный столбец. Но в 2020 году дисциплину решили убрать из учебной программы и больше не преподают, т.е. у студентов поступивших в 2020 году этот столбец будет пустым. Но в таблице Student мы храним как выпустившихся студентов, которые учились до 2020 года, и у них эта оценка есть. Поэтому мы не можем просто взять и удалеть столбец с экономикой - будет потеря данных и мы не сможем у выпускников узнать, какая была оценка за эту дисциплину.

#### Методы исправления

Кто-то может подумать: "давайте тогда заведем 2 таблицы - выпустившиеся студенты и учащиеся студенты", это ведь решит проблему? На первый взгляд, да, но на самом деле это повлечет ряд других проблем. Если нам нужно будет узнать статистику и по выпускникам, и по учащимся, то нам придется просматривать данные сразу в двух таблицах, а это дорогая операция. Да, вы сейчас еще мне не верите, поэтому у вас будет отдельная дисциплина, где вы будете изучать все правила построения баз данных. Если кому-то интересно вот прям щас, то на Хабре я при беглом поиске нашел [краткий, но достаточно понятный мануал по проектированию БД](https://habr.com/ru/post/193136/), там есть раздел [Нормализация баз данных](https://habr.com/ru/post/193756/), в котором рассказываются правила, как сделать правильно, чтобы не наступить на грабли.

Так вот. Если кратко, в нашей текущей базе данных есть проблемы. Как минимум:
- если у нас будут зачислены полные тезки по ФИО, рожденные в один день, мы не сможем понять, кто есть кто. Для этого в каждой таблице БД должен быть **первичный ключ** (PK - primary key)
- мы можем хранить оценки только для 3 дисциплин. Если дисциплины больше не преподаются, то у новых студентов этот столбец будет пустым - это пустая трата памяти.
- данные могут повторяться или в них можно ошибиться. Здесь речь идет про название группы. "ПрИ" и "ПИ". Даже если мы будем писать название группы правильно везде "ПрИ", то понятно, что в одной группе учится не один студент и у каждого в столбце будет одно и то же значение. Если группу нужно будет переименовать, например, при переходе с курса на курс ПрИ-101 -> ПрИ-102, придется вручную переименовыввать у всех студентов название группы.

Все эти проблемы нужно исправить. Для этого сделаем следующие шаги:
- в каждой таблице всегда должен PK (primary key) с уникальным значением, которое не должно повторяться. 
- в базах данных есть понятие таблицы-справочника. Это таблица, которая хранит список уникальных значений чего-либо, а остальные таблицы содержат внешний ключ (FK, foreign key), который говорит из какой таблицы, какое значение брать

#### Новые таблицы и связи между ними

Итак, заведем новые таблицы: Groups для групп и Subjects для дисциплин. В таблицу Students нужно добавить PK с уникальным значением студента и FK на таблицу с группой. Про оценки поговорим чуть позже. Запишем схемы этих таблиц.

Таблица Subjects:
```json
{
    "name": "Subjects",
    "columns": [
        {
            "name": "id",
            "type": "int",
            "isPrimary": true
        },
        {
            "name": "name",
            "type": "string",
            "isPrimary": false
        },
        {
            "name": "isArchive",
            "type": "bool",
            "isPrimary": false
        }
    ]
}
```
Для PK в таблицах часто берут автоинкрементируемый числовой столбец, который увеличивается сам при добавлении новых данных. Называю его чаще всего id (identificator - идентификатор).

Еще добавили дисциплинам флажок является ли дисциплина архивной или ее преподают в данный момент.

Таблица Groups:
```json
{
    "name": "Groups",
    "columns": [
        {
            "name": "id",
            "type": "int",
            "isPrimary": true
        },
        {
            "name": "name",
            "type": "string",
            "isPrimary": false
        }
    ]
}
```
Да, схемы таблиц с дисциплинами и группами очень похожи - это нормально, ведь это таблицы-справочники, часто в них вообще всего 2 поля: идентификатор и название.

Теперь таблица Students (для удобства из описания таблицы студентов убраны оценки):
```json
{
    "name": "Student",
    "columns": [
        {
            "name": "id",
            "type": "int",
            "isPrimary": true
        },
        {
            "name": "fullName",
            "type": "string",
            "isPrimary": false
        },
        {
            "name": "birthDate",
            "type": "datetime",
            "isPrimary": false
        },
        {
            "name": "entranceYear",
            "type": "int",
            "isPrimary": false
        },
        {
            "name": "groupId",
            "type": "int",
            "referencedTable": "Groups",
            "isPrimary": false
        },
        {
            "name": "graduated",
            "type": "bool",
            "isPrimary": false
        }
    ]
}
```
Обратите внимание, что раньше у нас в таблице был столбец `groupName` - текстовое название группы. Этот столбец был заменен на столбец с FK - числовой уникальный номер записи в таблице и название таблицы, где искать запись с таким номером.

Теперь при переходе с курса на курс, если нам нужно будет переименовать группу с ПрИ-101 на ПрИ-201, мы пойдем в таблицу Groups, переименуем группу там, а в таблице Students никаких действий делать не нужно будет, потому что у студентов хранится не само название группы, а номер записи из таблицы с группами.

##### Связь "многие-ко-многим"

Решить проблему с группами было легко. Завели отдельную таблицу и добавили студентам внешний ключ на эту таблицу. А вот с оценками встает вопрос: у каждого студента есть оценки по нескольким дисциплинам. Для каждого студента нужно знать список дисциплин и оценку для каждой из них. Как, блин, это все запихнуть в таблицу Students? Ок, названия дисциплин мы уже решили, что будем хранить отдельно. Но все равно нужно хранить для студента несколько айдишников дисциплин и оценку для каждого. Как?!

Попробуйте сами посидеть 10-15 минут и придумать, какие есть способы решить эту проблему. Кстати, это штука называется связь "многие-ко-многим". У одного студента - много дисцплин, и одна дисциплина может читаться у множества студентов.

(прошло 15 минут)

...

...

...

Вопрос: как запихнуть дисциплины и оценки в таблицу Students.

Вы готовы, дети?

Ответ: никак. Пам-пам.

Нам нужна отдельная таблица. Связь "многие-ко-многим" реализуется только с помощью отдельной таблицы. Создадим еще одну таблицу. Называют таблицу со связями многие-ко-многие по имени двух таблиц, которые она связывает. В нашем случае это будет таблица StudentSubjects:
```json
{
    "name": "StudentSubjects",
    "columns": [
        {
            "name": "studentId",
            "type": "int",
            "referencedTable": "Students",
            "isPrimary": true
        },
        {
            "name": "subjectId",
            "type": "int",
            "referencedTable": "Subjects",
            "isPrimary": true
        },
        {
            "name": "grade",
            "type": "int",
            "isPrimary": false
        }
    ]
}
```
В таблице мы храним id студента, дисциплины и оценку для данной комбинации. Первичный ключ в данной случае составной: состоит не из одного столбца, как было в предыдущих таблицах, а из комбинации двух столбцов `studentId` и `subjectId`. Считается, что студент может прослушать дисциплину только один раз, поэтому комибацния `studentId + subjectId` не будет повторяться, только тогда не будет ошибки уникальности PK.

Но некоторые дисциплины могут читать несколько семестров. И в каждом семестре может быть своя оценка. Как тогда быть? В нашем примере мы не рассматриваем этот вариант для простоты, но решить проблему с семестрами легко. Добавляем еще один столбец с номером семестра - `semesterNumber`, и делаем его тоже частью первичного ключа. Тогда уникальной должна будет быть комбинация `studentId + subjectId + semesterNumber`.

Таблицу Students менять никак не пришлось.

## Графическое изображение схемы БД

Для удобства схему БД часто рисуют в виде графика. Есть различные стандарты для этого UML, ERP, поставляемые с движком базы данных - много разных. Можно рисовать что-то среднее между ними, используя графические редакторы типа drawio.io, dbdiagram.io - их тоже много разных. Возьмем для примера такой рисунок:

![image](https://user-images.githubusercontent.com/2069875/184515671-fdeb3f3a-3c79-4307-b0b7-fc49ffc24072.png)

- Строки выделенные жирным - это первичные ключи
- Линии между строками - внешние ключи
- Цифры и символ * у линий - это обозначение кол-ва связей. 1 и * на концах линии - связь один ко многим.
- varchar - это аналог типа данных string в движках баз данных (variable character data - данные с переменным кол-вом символов)

## Пример хранимых данных

Теперь приведем пример данных, которые будут храниться в таблицах. Как уже говорилось, хранить данные будем в csv формате. Названия столбцов в файле с данными уже записываться не будут. Считается, что файл с данными полностью соответствует схеме базы данных. Полная читаемость для человека также не обязательна - мы привели данные к виду оптимальному для работы машины, и сохранили читаемость для человека в определенной степени.

Таблица Groups:
```
1;ПрИ-101
2;ПрИ-102
```

Таблица Subjects:
```
1;Мат. анализ;false
2;Англ. яз.;false
3;Программирование;false
```

Таблица Students:
```
1;Иванов Иван Иванович;12.06.2002;2022;1;false
2;Петров Петр Петрович;01.12.2001;2020;2;false
```
Напоминаю, что первый столбец - это уникальный номер записи студента. А предпоследний столбец в этой таблице - это FK на таблицу Groups.

У Иванова groupId равен 1 - ищем в таблице Groups запись с `id = 1` это группа ПрИ-101. Для Петрова соответственно `groupId = 2` это ПрИ-102. Надеюсь, никому не нужно объяснять, что цифры 1, 2 и другие, а также то, что у Иванова PK = 1 и groupId = 1 - простые совпадения. Эти значения не должны быть равны, это все числа для примера.

Таблица StudentSubjects:
```
1;1;5
1;2;4
1;3;4
2;1;3
2;2;5
2;2;5
```
Данная таблица для человека выглядит слабо читаемой, потому что состоит из одних чисел: двух FK и одной числовой оценки. Для примера расшифруем строчку "2;2;5".

Первый столбец - это studentId, FK на таблицу Students. В столбце стоит значение 2. Ищем в таблице Students запись с айдишников 2. Это Петров.

Второй столбец - это subjectId, FK на таблицу Subjects. В столбце стоит значение 2. Ищем в таблице Subjects запись с айдишником 2. Это английский.

Итого: у Петрова за английский стоит оценка 5.

## Язык запросов

Последняя таблица показала, что сырые данные в таблице могут быть нечитаемы. Справочники хороши для машины, но нам мешают понимать, что написано в таблице. Чем больше БД, тем больше справочников и внешних ключей. Чтобы их расшифровать приходится бегать по таблицам и смотреть, что в них лежит. Сомнительное удовольствие.

Еще нужно уметь вычислять какую-то статистику. Например, средний балл студента по всем дисциплинам. Искать информацию по каким-то критериям и т.п.

Для таких задач все движки БД имеют встроенный язык запросов. Опять очень распространенный, но не единственный - это [SQL](https://en.wikipedia.org/wiki/SQL#:~:text=SQL%20(%2F%CB%8C%C9%9Bs%CB%8C,stream%20management%20system%20(RDSMS).) (structured query language - структурированный язык запросов). Причем даже у него есть несколько диалектов (у Microsoft, Apache и MySQL языки максимально похожи, но все же немного отличаются).

Если все пойдет хорошо, то во втором семестре мы напишем свой простенький язык запросов. Но это не точно и далеко не факт. Основной целью практик будет реализовать возможность создавать базы данных с табличками с любой структурой и хранить в них данные. Использовать будем json и csv файлы соответственно.

Но для общей эрудиции приведу пример запроса, как привести таблицу StudentSubjects к читаемому виду.
```sql
select st.fullName, sub.name, ss.grade
from StudentSubjects ss
join Students st on ss.studentId = s.id
join Subjects sub on ss.subjectId = sub.id
```
Названия таблиц, думаю, можно не комментировать. `ss`, `st` и `ss` - это краткие названия для таблиц ради удоства (вам кажется, что это нифига не удоство, но это только вам там кажется по неопытности).

Команда `select` говорит, какие столбцы из каких таблиц нужно взять в итоговую выборку. Из таблицы Student мы возьмем `fullName` - ФИО, из таблицы Subjects нам нужен `name` - название дисциплины, а из таблицы StudentSubjects `grade` - оценка.

Команда `from` говорит, из какой таблицы мы возьмем данные.

Команда `join` "объединяет" таблицы по какому-то условию. В данном случае к таблице StudentSubjects приклеиваем данные из таблицы Students, так чтобы совпали айдишники studentId и id. А затем приклеиваем таблицу Subjects по совпадению айдишников дисциплин.

По шагам. Сначала была голая таблица StudentSubjects:
| ss.studentId | ss.subjectId | ss.grade |
| ------ | ------ | ------ |
| 1 | 1 | 5
| 1 | 2 | 4
| 1 | 3 | 4
| 2 | 1 | 3
| 2 | 2 | 5
| 2 | 2 | 5

Затем выполняется первый `join` с приклеиванием таблицы Students
| ss.studentId | ss.subjectId | ss.grade | st.id | st.fullName | st.birthDate | st.entranceYear | st.groupId | st.graduated |
| ------ | ------ | ------ | ------ | ------ | ------ | ------ | ------ | ------ |
| 1 | 1 | 5 | 1 | Иванов Иван Иванович | 12.06.2002 | 2022 | 1 | false
| 1 | 2 | 4 | 1 | Иванов Иван Иванович | 12.06.2002 | 2022 | 1 | false
| 1 | 3 | 4 | 1 | Иванов Иван Иванович | 12.06.2002 | 2022 | 1 | false
| 2 | 1 | 3 | 2 | Петров Петр Петрович | 01.12.2001 | 2020 | 2 | false
| 2 | 2 | 5 | 2 | Петров Петр Петрович | 01.12.2001 | 2020 | 2 | false
| 2 | 2 | 5 | 2 | Петров Петр Петрович | 01.12.2001 | 2020 | 2 | false

Склеивание прозошло так, чтобы совпадали ss.studentId и st.id. Везде, где ss.student = 1 приклеилась строчка с Ивановым, а у ss.studentId = 2 приклеился Петров.

Вторым `join` к уже немаленькой таблице приклеиваем таблицу Subjects по условию ss.subjectId = sub.id.
| ss.studentId | ss.subjectId | ss.grade | st.id | st.fullName | st.birthDate | st.entranceYear | st.groupId | st.graduated | sub.id | sub.name | sub.isArchive |
| ------ | ------ | ------ | ------ | ------ | ------ | ------ | ------ | ------ | ------ | ------ | ------ |
| 1 | 1 | 5 | 1 | Иванов Иван Иванович | 12.06.2002 | 2022 | 1 | false | 1 | Мат. анализ | false
| 1 | 2 | 4 | 1 | Иванов Иван Иванович | 12.06.2002 | 2022 | 1 | false | 2 | Англ. яз. | false
| 1 | 3 | 4 | 1 | Иванов Иван Иванович | 12.06.2002 | 2022 | 1 | false | 3 | Программирование | false
| 2 | 1 | 3 | 2 | Петров Петр Петрович | 01.12.2001 | 2020 | 2 | false | 1 | Мат. анализ | false
| 2 | 2 | 5 | 2 | Петров Петр Петрович | 01.12.2001 | 2020 | 2 | false | 2 | Англ. яз. | false
| 2 | 2 | 5 | 2 | Петров Петр Петрович | 01.12.2001 | 2020 | 2 | false | 3 | Программирование | false

Монструозно, правда? И все это лежит в памяти. А таблички-то маленькие. И теперь представьте, что обычно в БД в одной таблице сотни тысяч или миллионы записей. И одновременно запросы к БД могут делать десятки или сотни пользователей. Поэтому операция `join` считается самой медленной и ресурсоемкой, ее стараются по возможности избегать.

Теперь нам нужно отфильтровать только те столбцы, которые нам нужно. Это сделано командой `select` и перечнем столбцов, что нам интересны. Получится так:
| st.fullName | sub.name | ss.grade |
| ------ | ------ | ------ |
| Иванов Иван Иванович | Мат. анализ | 5
| Иванов Иван Иванович | Англ. яз. | 4
| Иванов Иван Иванович | Программирование | 4
| Петров Петр Петрович | Мат. анализ | 3
| Петров Петр Петрович | Англ. яз. | 5
| Петров Петр Петрович | Программирование | 5

## Реляционная модель и объектно-ориентированная

Мы рассмотрели структуру как выглядят данные при хранении в реляционном (табличном) виде. Однако, это не значит, что при переносе в объектно-ориентированный язык программирования классы и объекты будут иметь такую же один-в-один структуру. Но это совсем другая история.
