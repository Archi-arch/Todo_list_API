using My_todo_API.Models;
using My_todo_API.dto;
using My_todo_API.Data;

namespace My_todo_API.Endpoints;


public static class Endpoints
{   

     // тут я пишу що це шось тільки для читання мій List але з _ бо він створений взагалі в іншому фалі а потім новйи такий самий List ?
    public static void MapTaskEndpoints(this WebApplication app) // щось типу в стандартні методи Мап я добавляю свій ?
    {   




        app.MapPost("/task", (CreateTaskDto dto, AppDbContext db) => // тут створюю новий таск і передаю тільки дто щоб було зрозуміло які поля  я маю заповнити
        {   
             // тут автоматично рахую ід тобто беру елементи в списку їх кількість і добавляю 1 

           var  newTask = new TodoTask // тут створюю змінну в яку запишуться всі данні які треба додати до списку 
           {
             
             Title = dto.Title,
             IsComplete = false

           };

            db.Tasks.Add(newTask); // тут добавляю цмінну з данними в список
            db.SaveChanges();
            return $"New task {newTask.Title} added succesful"; // повертаю команду щоб було зрозуміло що в се додано


        });



        app.MapPut("/task/{id}", (int id, UpdateTaskDto dto, AppDbContext db) => // тут я вже в шлях передаю змінну ід і тому добавляю її в джуки а чому треба ще дто добавляти ?
        {
            var existingTask = db.Tasks.FirstOrDefault(t => t.Id == id); // тут перевіряється чи взагалі існує такий таск але недуже зрозуміло звідки взялось t ?


            if(existingTask == null)
            {
                return $"Task with {id} not fount"; // тут все зрозуміло замість стандартної помилки користувач побачить що нема такого завдання
            }

            existingTask.Title = dto.Title; // тут типу ми створили новий екземпляр туда записали данні і кажемо що ці данні це поля з дто чи як ?
            existingTask.IsComplete = dto.IsComplete;

            db.SaveChanges();

            return $"Task {id} update succesful"; // відповідь замість стандартного 200

        });


        app.MapGet("/task", (AppDbContext db) => // тут я так розумію шлях ? але що можна в дужки вставити ?
        {
            var response = db.Tasks.Select(task => new GetTaskDto( // тут що створюється новйи екземпляр класу getTaskdto ?
                task.Id,
                task.Title,
                task.IsComplete
            )).ToList();//недуже зрозумів навіщо тут tolist ?


            return response; // я так зрозумів що в змінну responese я вкладаю данні в вигляді словника? і потім повертаю їх але куда ?
        });




        app.MapDelete("/task/{id}", (int id, AppDbContext db) => // передав шлях і id
        {
           var existingTask = db.Tasks.FirstOrDefault(e => e.Id == id ); // перевіряю чи є таск ще первірив чи справді можна замість t написати що завгодно

            if(existingTask == null)
            {
                return $"Task with id - {id} not found"; // якщо таски немає то повертаю помилку
            }


            db.Tasks.Remove(existingTask); // якщо таск є то видаляю 
            db.SaveChanges();
            return $"Task with id - {id} deleted succesful";

        });
    }
}