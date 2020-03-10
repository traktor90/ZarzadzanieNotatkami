# ZarzadzanieNotatkami
Aplikacja do zarzadzania notatkami

This application manages notes and segregates them by users who created them.
It uses ASP.NET Core MVC.

You have to run **update-database** im Package Manager Console to generate Database for this program to use.

If you want to create first note you have to **create User first** and change application to use it.

### How application works:

If you only want to update "importants" of notes you can click "Update importants"

You can sort notes by clicking sort ascending and sort descending.

Application uses cookies to preserve state of chosen user.

You can manage Users by clicking responsible for it button.

You can create notes and users. For both of them there are implemented CRUD operations.

##### If you want to delete user, you have to delete all of his notes first.

Main screen of application:

![Image of main screen](https://github.com/wiktortyt/ZarzadzanieNotatkami/blob/master/Main%20Screen.png)
