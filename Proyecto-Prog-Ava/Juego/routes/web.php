<?php

/*
|--------------------------------------------------------------------------
| Application Routes
|--------------------------------------------------------------------------
|
| Here is where you can register all of the routes for an application.
| It is a breeze. Simply tell Lumen the URIs it should respond to
| and give it the Closure to call when that URI is requested.
|
*/

$router->get('/', function () use ($router) {
    return $router->app->version();
});
$router->get('/key', function () use ($router) {
    return str_random(32);
});

//Test
$router->get('/test/admin', 'Testing@test');

//Persona e info del juego del Usuario
$router->post('/usuario/editar/{external_id}', 'PersonaController@Modificar');
$router->post('/login', 'PersonaController@Login');
$router->post('/usuario/registro', 'PersonaController@Registrar');
$router->post('/usuario/actualizarinfo/{external_id}', 'PersonaController@ActualizarNivelUsuario');
$router->post('/usuario/datos/{external_id}', 'PersonaController@DatosPerfil');
//preguntas
$router->post('/preguntas/ramdon', 'PreguntaController@obtenerPreguntasRamdon');
$router->post('/preguntas/registro/{external_id}', 'PreguntaController@registrar');
$router->post('/preguntas/obtenerPreguntaId', 'PreguntaController@obtenerOpcionesPorIdPregunta');
$router->post('/preguntas/listarPorUser/{external_id}', 'PreguntaController@ListaPreguntasSugueridas');
$router->get('/preguntas/ultimas','PreguntaController@UltimasPreguntasIngresadas');
$router->post('/preguntas/listarPreguntas/{dificultad}/{categoria}','PreguntaController@listarPreguntasCatyDifi');
$router->get('/preguntas/SugueridasAceptar','PreguntaController@ListaPreguntasSugueridasAceptar');

//categoriaController
$router->post('/categoria/registroCategoria', 'CategoriaController@Registrar');
$router->post('/categoria/editarCategoria/{external_id}', 'CategoriaController@EditarCategoria');
$router->get('/categoria/listarCategoria', 'CategoriaController@ListarCategoria');

//nivelUsuarioController
$router->get('/usuario/listar', 'nivelUsuarioController@listar');