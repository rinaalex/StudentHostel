# StudentHostel
## Наименование задачи: учет проживания студентов в общежитии
### Учебные группы
В базе данных хранится информация о наименованиях учебных групп. 
Реализован просмотр, добавление, редактирование и удаление групп.
### Студенты
О каждом студенте хранится следующая информация: ФИО, учебная группа, телефон и дополнительная информация. Поле Дополнительная информация не является обязательным и заполняется при необходимости. 
Реализован просмотр, добавление, редактирование и удаление студентов.
### Комнаты
В общежитии расположены комнаты различной вместимости, каждая из которых имеет номер. 
Реализован просмотр, добавление и редактирование информации о комнатах, а также просмотр проживающих в комнате студентов.
### Размещение
При размещении студента в комнате необходимо указать дату заселения. Дата выселения заполняется при выселении студента. Размещение студентов, уже проживающих в комнате, не допускается. Также не допускается превышение максимального количества проживающих в комнате.
Реализован просмотр, добавление и редактирование информации о размещениях студентов.
## Краткое описание использованных технологий
Создано приложение WPF, в котором управление данными реализовано посредством Entity Framework 6 (метод Code First). Использован паттерн MVVM, в котором выделены VM для отдельных сущностей и коллекций сущностей с CRUD-функционалом. 
