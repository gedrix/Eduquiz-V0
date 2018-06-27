<?php
namespace App\Http\Controllers;
use App\Models\Persona;
use App\Models\Pregunta;
use App\Models\Categoria;
use App\Models\Opcion;
use App\Models\Preg_cate;

class Testing extends Controller 
{
    public function test()
    {
        /*
        $admin = new Persona();
        $admin->nombre = 'Gerardo Ramirez';
        $admin->correo = 'gedoram@gmail.com';
        $admin->clave = '55555';
        $admin->rol = 0;//0 admin - 1 user
        //$admin->estado = 1;//1 activo - 0 inactivo
        $admin->external_id = Utilidades\UUID::v4();
        $admin->save();
        */
        $cate = new Categoria();
        $cate->nombre = "Ciencia";
        $cate->external_id = Utilidades\UUID::v4();
        $cate->save();
        /*
        $noticia = new Noticia();
        $noticia->titulo = "super titulo";
        $noticia->descripcion = "texto responsive, aguamarina, aguacate, perlas y coyotes";
        $noticia->external_id = Utilidades\UUID::v4();
        $noticia->Administrador()->associate($admin);//asocio las tablas relacionadas
        $noticia->save();
        
        $imagen = new Imagen();
        $imagen->ruta = "images/aguacate.png";
        $imagen->portada = true;
        $imagen->Noticia()->associate($noticia);//asocio las tablas relacionadas
        $imagen->save();
        
        $comentario = new Comentario();
        $comentario->comentario = "1234 salamanqueja";
        $comentario->usuario = "rik";
        $comentario->estado = true;
        $comentario->Noticia()->associate($noticia);//asocio las tablas relacionadas
        $comentario->save(); */
    }
}