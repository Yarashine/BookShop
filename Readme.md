API for Book Marketplace  
This API is developed in C# using ASP.NET Core and includes all the features needed for selling e-books. It supports user authorization with JWT tokens, email verification using MailKit, CRUD operations for all entities (Entity Framework, PostgreSQL), payment integration with Stripe, multiple user roles, a system to block users, comments, and a rating system for users.  

список реализованных запросов:
<pre>
пользователь:  
     GET /api/users                                     получить всех пользователей
     GET /api/user/{id}                                 получить пользователя по идентификатору
     GET /api/user/favorites/{id}                       получить избранное пользователя по идентификатору
     GET /api/user/library/{id}                         получить библиотеку пользователя по идентификатору
     GET /api/user/bookstosell/{id}                     получить книги пользователя для продажи по идентификатору
     GET /api/user/purchasedbooks/{id}                  получить книги, купленные пользователем, по идентификатору
     POST /api/user/{id}/book/{id}/addcomment           добавить комментарий пользователя
     PATCH /api/user/{id}/like/comment/{id}             добавить лайк к комментарию
     PATCH /api/user/{id}/dislike/comment/{id}          добавить неприязнь к комментарию
     PATCH /api/user/{id}/deletelike/comment/{id}       удалить лайк для комментария
     PATCH /api/user/{id}/deletedislike/comment/{id}    удалить неприязнь к комментарию
     PUT /api/user/update                               обновить информацию о пользователе
     DELETE /api/user/{id}/comment{id}                  удалить комментарий пользователя по идентификатору
     PATCH /api/user/{id}/like/book{id}                 добавить лайк к книге
     PATCH /api/user/{id}/like/book{id}                 удалить книгу из избранного по пользователю по идентификатору
     PATCH /api/user/{id}/bookstosell{id}               удалить книгу из книг для продажи пользователем по идентификатору
     PATCH /api/user/{id}/addedbook/{id}                купить книгу
     GET /api/user/{id}/purchasedbook/{id}              скачать купленную книгу
     POST /api/user/{id}/unbanrequest                   запрос на разбан
авторизация:
    GET /api/auth/login                                 пользователь входа в систему
    POST /api/auth/register                             зарегистрировать нового пользователя
    GET /api/auth/confirmemail                          подтверждает адрес электронной почты пользователя
    POST /api/auth/forgotpassword                       отправить электронное письмо пользователю для сброса пароля
    GET /api/auth/resetpassword                         получить модель для отправки запроса по электронной почте
    PATCH /api/auth/resetpassword                       сброс пароля пользователя
книга:
     GET /api/books                                     получить все книги
администратор:
     GET /api/admins                                    получить всех администраторов
     PUT /api/admin/block/{id}/user/{id}                заблокировать пользователя по идентификатору
     PUT /api/admin/update                              обновление администратора
     PUT /api/admin/block/{id}/user/{id}                разблокировать пользователя по идентификатору
     DELETE /api/admin/user/{id}                        удалить пользователя по идентификатору
</pre>
