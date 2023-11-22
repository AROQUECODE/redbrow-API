# Web API - Redbrow 🚀
###### Descripción
## Web API que nos permite realizar un CRUD (Create, Read, Update, Delete)
<br>

Para poder utilizar este proyecto se utiliza Database firts, por ende primero crearemos nuestra base de datos y las tablas que utilizaremos crearemos un usuario admin ya que la API utiliza JWT y para poder hacer uso de los end point se requiere la autentificación.
````sql
create database redbrow
go

use redbrow
go

create table usuario(
	idUsuario int not null identity primary key,
	nombre varchar(50) not null,
	correo varchar(150) not null,
	edad int not null,
	clave varchar(20) not null,
	creadoPor varchar(100),
	FechaCreacion datetime
);

create table tokenHistory(
	idToken int not null primary key identity,
	idUsuario int references usuario(idUsuario),
	Token varchar(500) not null,
	RefreshToken varchar(200),
	FechaCreacion datetime,
	FechaExpiracion datetime,
	Activo as (iif(FechaExpiracion < getdate(), convert(bit,0),convert(bit,1)))
);

insert into usuario values ('Admin','alberto@aroquecode.com',29,'admin123','admin',GETDATE())
````
Una vez que tenemos nuestra base de datos ejecutamos el siguiente comando de scaffold, como el proyecto ya ejecuto el scaffolding se utiliza -force para ejecutar nuevamente: 
```csharp
Scaffold-DbContext "Server=(local); DataBase=redbrow; Trusted_Connection=True; TrustServerCertificate=True;" Microsoft.EntityFrameworkCore.SqlServer -OutPutDir Models -force
```
Una vez actualizado el DbContext cortamos la cadena de conexion y la remplazamos en el archivo appsettings.json en el elemento stringSQL
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "stringSQL": "Server=(local); DataBase=redbrow; Trusted_Connection=True; TrustServerCertificate=True;"
  },
  "JwtSettings": {
    "key": "=this is my custom Secret key for authentication="
  }
}
```
Dentro del proyecto se incluye el DockFile para la dokerizacion de la API.
