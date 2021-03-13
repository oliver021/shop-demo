# Shop-demo

#### Una simple demostracion de tienda electronica con sencillas entidades

## Empezar

Para probar esta simple demo basta con compilar el
codigo fuente y corriendo la applicacion mediante el comando:

```sh
	dotnet run
```

El sistema contiene un conjunto servicios y funciones que se ncarga de ejecutar
las tareas de instaalcion del esuqema de la base de datos, no hay necesidad
de ejecutar ningun script manualmente, aunque se ha provisto
en este repo el fichero de *mysql.sql* con el esquema de base de datos.
Adicionalmente el servicio de migracion y gestion de esquema sql, ejecuta *seed* para
preparar los datos de la demo, que son usuarios y roles predetermiandos.

Hay tres usuarios con los tres roles predetermiando, una misma contraseña y nombre de usuarios
con los nombre de los roles:
- client: secret
- admin: secret
- seller: secret

La contraseña 'secret' es la misma para todos, debera autenticarse contra 'api/auth/token',
enviando `username` y `password`, este endpoint de devolvera el token necesario de autenticidad.

Para acceder a la documentacion del API acceda desde un navegador a `GET /api-doc`,
y se le mostrara una UI de Swagger herramienta utilizada para la documentacion.

Los endpoint estan protegidos con autenticacion basada en la portabilidad de token *JWT*
donde necesitara proveer un encabezado en la solicitud http, para acceder a los recursos
y operaciones descritos en el API. Con Swagger se especificado este metodo de autenticacion,
debera ejecutar dicho endpoint.