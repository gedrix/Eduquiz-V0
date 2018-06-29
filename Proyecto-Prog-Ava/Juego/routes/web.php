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
$router->post('/usuario/actualizarinfo/{external_id}', 'PreguntaController@ActualizarNivelUsuario');

#preguntaController

$router->post('/usuario/registroPregunta', 'PreguntaController@Registrar');
