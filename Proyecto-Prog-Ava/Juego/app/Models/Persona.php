<?php

namespace App\Models;

use Illuminate\Database\Eloquent\Model;
class persona extends Model{
    /*referencia a la tabla de la base de datos*/
    protected $table = 'persona';
    //para saber si en la tabla usamos created_at y updated_at
    public $timestamps = true;
    //lista blancas campos publicos
    protected $fillable = ['nombre', 'correo', 'clave', 'rol', 'estado', 'external_id', 'imagen', 'created_at', 'updated_at'];
    //lista negra campos que no quieren que se encuentren facilmente
    protected $guarded = ['id'];
    
    //Relacion UNO a MUCHOS
    public function Preguntas(){
        return $this->hasMany('App\Models\pregunta', 'id_persona');
    }

    //Relacion UNO a MUCHOS
    public function Pregunta_Reporte(){
        return $this->hasMany('App\Models\pregunta_reporte', 'id_persona');
    }
    
    //Relacion UNO a UNO
    public function Nivel_Usuario(){
        return $this->hasOne('App\Models\nivel_usuario', 'id_persona');
    }
    //relacion uno a muchos
    public function Per_Logro(){
        return $this->hasMany('App\Models\per_logro', 'id_persona');
    }

}