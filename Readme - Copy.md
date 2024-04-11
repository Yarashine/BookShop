user:
	GET /api/users			                            get all users
    GET /api/user/{id}                                  get user by id
    GET /api/user/favorites/{id}                        get user favorites by id
    GET /api/user/library/{id}                          get user library by id
    GET /api/user/bookstosell/{id}                      get user books to sell by id
    GET /api/user/purchasedbooks/{id}                   get user purchased books by id
    POST /api/user/{id}/book/{id}/addcomment            add comment by user
    PATCH /api/user/{id}/like/comment/{id}              add like to comment
    PATCH /api/user/{id}/dislike/comment/{id}           add dislike to comment
    PATCH /api/user/{id}/deletelike/comment/{id}        delete like for comment
    PATCH /api/user/{id}/deletedislike/comment/{id}     delete dislike for comment
    PUT /api/user/update                                update user info
    DELETE /api/user/{id}/comment{id}                   delete comment by user by id
    PATCH /api/user/{id}/like/book{id}                  add like for book
    PATCH /api/user/{id}/like/book{id}                  delete book from favorites by user by id
    PATCH /api/user/{id}/bookstosell{id}                delete book from books to sell by user by id
    PATCH /api/user/{id}/addedbook/{id}                 buy book
    GET /api/user/{id}/purchasedbook/{id}               download purchased book
    POST /api/user/{id}/unbanrequest                    request for unban
auth:
	GET /api/auth/login         		                login user
	POST /api/auth/register     		                register new user
	GET /api/auth/confirmemail                          confirm user email
	POST /api/auth/forgotpassword                       send email to user to reset password
	GET /api/auth/resetpassword                         get model to post request from email
	PATCH /api/auth/resetpassword                       reset user password
book:
    GET /api/books                                      get all books
administrator:
    GET /api/admins                                     get all admins
    PUT /api/admin/block/{id}/user/{id}                 block user by id
    PUT /api/admin/update                               update admin
    PUT /api/admin/block/{id}/user/{id}                 unblock user by id    
    DELETE /api/admin/user/{id}                               delete user by id

пользователь:
    GET /api/users                                      получить всех пользователей
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
